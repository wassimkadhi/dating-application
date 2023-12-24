using System.Reflection.Metadata.Ecma335;
using API.Controllers;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace API;

public class MessagesController : BaseApiController
{



    private readonly IUserRepository _userRepository;
    private readonly IMessageRepository _messageRepository;
    private readonly IMapper _mapper;
    public MessagesController(IUserRepository userRepository, IMessageRepository messageRepository, IMapper mapper)
    {
        _userRepository = userRepository;
        _messageRepository = messageRepository;
        _mapper = mapper;

    }

    [HttpPost]

    public async Task<ActionResult<MessageDto>> CreateMessage(CreateMessageDto createMessageDto)
    {

        var username = User.GetUsername();

        if (username == createMessageDto.RecipientUsername.ToLower())
            return BadRequest("you cant send a message for yourself");

        var sender = await _userRepository.GetUserByUserNameAsync(username);
        var recipient = await _userRepository.GetUserByUserNameAsync(createMessageDto.RecipientUsername);
        if (recipient == null) return NotFound();
        var message = new Message
        {
            Sender = sender,
            Recipient = recipient,
            SenderUsername = sender.UserName,
            RecipientUsername = recipient.UserName,
            Content = createMessageDto.Content,
        };

        _messageRepository.AddMessage(message);
        if (await _messageRepository.SaveAllAsync()) return Ok(_mapper.Map<MessageDto>(message));
        return BadRequest("send failed from some reason");

    }


    [HttpGet]

    public async Task<ActionResult<PagedList<MessageDto>>> GetMessagesForUser([FromQuery] MessageParams messageParam)
    {

        messageParam.Username = User.GetUsername();
        var messages = await _messageRepository.GetMessageForUser(messageParam);
        Response.AddPaginationHeader(new PaginationHeader(messages.CurrentPage, messages.PageSize, messages.TotalCount, messages.TotalePages));
        return messages;






    }

    [HttpGet("thread/{username}")]
    public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessagethread(string username)
    {

        var CurrentUsername = User.GetUsername();
        var recepientUsername = username;

        var messages = await _messageRepository.GetMessageThread(CurrentUsername, recepientUsername);
        return Ok(messages);
    }

    [HttpDelete("{id}")] 
    public async Task<ActionResult>DeleteMessage(int id){

        var username=User.GetUsername(); 
        var message=await _messageRepository.GetMessage(id) ;
        if(message.SenderUsername !=username && message.RecipientUsername!=username) return Unauthorized() ;
        if(message.SenderUsername==username) message.SenderDeleted=true; 
        if(message.RecipientUsername==username) message.RecipientDeleted=true;  
        if(message.SenderDeleted && message.RecipientDeleted) {
            _messageRepository.DeleteMessage(message) ; 
        }
        if(await _messageRepository.SaveAllAsync()) return Ok() ;
        return BadRequest("some thing went wrong") ;
    }

}
