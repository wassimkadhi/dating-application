using System.Reflection.Metadata.Ecma335;
using API.Controllers;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace API;

public class MessagesController : BaseApiController
{



 private readonly IUnitOfWorck _uow ; 
    private readonly IMapper _mapper;
    public MessagesController(IUnitOfWorck uow, IMapper mapper)
    {
       _uow=uow;
        _mapper = mapper;

    }

    [HttpPost]

    public async Task<ActionResult<MessageDto>> CreateMessage(CreateMessageDto createMessageDto)
    {

        var username = User.GetUsername();

        if (username == createMessageDto.RecipientUsername.ToLower())
            return BadRequest("you cant send a message for yourself");

        var sender = await _uow.UserRepository.GetUserByUserNameAsync(username);
        var recipient = await _uow.UserRepository.GetUserByUserNameAsync(createMessageDto.RecipientUsername);
        if (recipient == null) return NotFound();
        var message = new Message
        {
            Sender = sender,
            Recipient = recipient,
            SenderUsername = sender.UserName,
            RecipientUsername = recipient.UserName,
            Content = createMessageDto.Content,
        };

        _uow.MessageRepository.AddMessage(message);
        if (await _uow.Complete()) return Ok(_mapper.Map<MessageDto>(message));
        return BadRequest("send failed from some reason");

    }


    [HttpGet]

    public async Task<ActionResult<PagedList<MessageDto>>> GetMessagesForUser([FromQuery] MessageParams messageParam)
    {

        messageParam.Username = User.GetUsername();
        var messages = await _uow.MessageRepository.GetMessageForUser(messageParam);
        Response.AddPaginationHeader(new PaginationHeader(messages.CurrentPage, messages.PageSize, messages.TotalCount, messages.TotalePages));
        return messages;

    }

    

    [HttpDelete("{id}")] 
    public async Task<ActionResult>DeleteMessage(int id){

        var username=User.GetUsername(); 
        var message=await _uow.MessageRepository.GetMessage(id) ;
        if(message.SenderUsername !=username && message.RecipientUsername!=username) return Unauthorized() ;
        if(message.SenderUsername==username) message.SenderDeleted=true; 
        if(message.RecipientUsername==username) message.RecipientDeleted=true;  
        if(message.SenderDeleted && message.RecipientDeleted) {
            _uow.MessageRepository.DeleteMessage(message) ; 
        }
        if(await _uow.Complete()) return Ok() ;
        return BadRequest("some thing went wrong") ;
    }

}
