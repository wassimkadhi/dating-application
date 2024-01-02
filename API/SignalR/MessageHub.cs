using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.SignalR;

namespace API;

public class MessageHub:Hub
{


private readonly IMessageRepository _messagerepository;
private readonly IUserRepository _userrepository ;
private readonly IMapper _mapper ; 
private readonly IHubContext<PresenceHub> _presenceHub;
public MessageHub(IMessageRepository messageRepository , IUserRepository userRepository , IMapper mapper,
//injectinga presence hub 
IHubContext<PresenceHub> presenceHub
) 
{
    _messagerepository=messageRepository;
    _userrepository=userRepository ; 
    _mapper=mapper ; 
    _presenceHub=presenceHub ; 

}

    public override async Task OnConnectedAsync()
    {
       var httpContext=Context.GetHttpContext() ; 
       var otherUser=httpContext.Request.Query["user"] ; 
       var groupName=this.GetGroupName(Context.User.GetUsername(),otherUser) ;
// create a groupe discusstion
        await Groups.AddToGroupAsync(Context.ConnectionId,groupName) ; 
        await AddToGroup(groupName) ;
        //
        var messages=await _messagerepository.
        GetMessageThread(Context.User.GetUsername(),otherUser) ;
        
        //user coonect to the message hub they gonne receive message from signalr
        await Clients.Group(groupName).SendAsync("ReceiveMessageThread",messages) ;




    }

         public async Task sendMessage(CreateMessageDto createMessageDto){

             var username = Context.User.GetUsername();

        if (username == createMessageDto.RecipientUsername.ToLower())
            throw  new HubException("you cant send a message for yourself");

        var sender = await _userrepository.GetUserByUserNameAsync(username);
        var recipient = await _userrepository.GetUserByUserNameAsync(createMessageDto.RecipientUsername);
        if (recipient == null) throw new HubException("NotFound") ; 

        var message = new Message
        {
            Sender = sender,
            Recipient = recipient,
            SenderUsername = sender.UserName,
            RecipientUsername = recipient.UserName,
            Content = createMessageDto.Content,
        };

        var groupName = GetGroupName(sender.UserName,recipient.UserName) ;
        var group=await _messagerepository.GetMessageGroup(groupName) ; 

        if( group.Connections.Any(x=>x.Username==recipient.UserName)){
            message.DateRead=DateTime.UtcNow;        }
            else{
                var connections=await presenceTracker.GetConnectionsForUser(recipient.UserName) ; 
                if(connections!=null) {
                    await _presenceHub.Clients.Clients(connections).SendAsync("newMessageReceived" , 
                    new {
                        username=sender.UserName ,
                        knownAs=sender.KnownAs
                    }
                     ) ;
                }
            }

        _messagerepository.AddMessage(message);
        if (await _messagerepository.SaveAllAsync()) 
        {
    
            await Clients.Group(groupName).SendAsync("NewMessage", _mapper.Map<MessageDto>(message)) ;
        }
        
    
         }


    public override async Task OnDisconnectedAsync(Exception exception)
    {
        await RemoveFromMessageGroup() ; 
        await base.OnDisconnectedAsync(exception);
    }


    private string GetGroupName(string caller, string other) {
    var strinCompare=string.CompareOrdinal(caller ,other)>0 ; 
    return strinCompare? $"{caller}-{other}":$"{other}-{caller}" ;
}


    private async Task<bool>AddToGroup(string  groupName){

        var group =await _messagerepository.GetMessageGroup(groupName) ;
        var connection = new Connection(Context.ConnectionId,Context.User.GetUsername()) ; 
        if(group==null) {
            group=new Group(groupName);
            _messagerepository.AddGroup(group) ; 

        }
        group.Connections.Add(connection) ; 

        return await _messagerepository.SaveAllAsync() ; 
    }


    private async Task RemoveFromMessageGroup() {

        var connection= await _messagerepository.GetConnection(Context.ConnectionId) ; 
        _messagerepository.RemoveConnection(connection) ; 
        await _messagerepository.SaveAllAsync() ;
    }




}
