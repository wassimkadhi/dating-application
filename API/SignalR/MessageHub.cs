using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.SignalR;

namespace API;

public class MessageHub:Hub
{



private readonly IMapper _mapper ; 
private readonly IUnitOfWorck _uow ; 

private readonly IHubContext<PresenceHub> _presenceHub;
public MessageHub(IUnitOfWorck uow , IMapper mapper,
//injectinga presence hub 
IHubContext<PresenceHub> presenceHub
) 
{
    _uow=uow;
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
        var messages=await _uow.MessageRepository.
        GetMessageThread(Context.User.GetUsername(),otherUser) ;
        if(_uow.HasChanges()) await _uow.Complete() ; 


        
        //user coonect to the message hub they gonne receive message from signalr
        await Clients.Group(groupName).SendAsync("ReceiveMessageThread",messages) ;

    }

         public async Task sendMessage(CreateMessageDto createMessageDto){

             var username = Context.User.GetUsername();

        if (username == createMessageDto.RecipientUsername.ToLower())
            throw  new HubException("you cant send a message for yourself");

        var sender = await _uow.UserRepository.GetUserByUserNameAsync(username);
        var recipient = await _uow.UserRepository.GetUserByUserNameAsync(createMessageDto.RecipientUsername);
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
        var group=await _uow.MessageRepository.GetMessageGroup(groupName) ; 

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

        _uow.MessageRepository.AddMessage(message);
        if (await _uow.Complete()) 
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

        var group =await _uow.MessageRepository.GetMessageGroup(groupName) ;
        var connection = new Connection(Context.ConnectionId,Context.User.GetUsername()) ; 
        if(group==null) {
            group=new Group(groupName);
            _uow.MessageRepository.AddGroup(group) ; 

        }
        group.Connections.Add(connection) ; 

        return await _uow.Complete() ; 
    }


    private async Task RemoveFromMessageGroup() {

        var connection= await _uow.MessageRepository.GetConnection(Context.ConnectionId) ; 
        _uow.MessageRepository.RemoveConnection(connection) ; 
        await _uow.Complete() ;
    }




}
