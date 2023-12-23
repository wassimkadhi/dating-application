
using API.Data;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using SQLitePCL;

namespace API;

public class MessageRepositoy : IMessageRepository
{


    private readonly DataContext _context ;
    private readonly IMapper _mapper ; 
    public MessageRepositoy(DataContext context ,IMapper mapper){
        _context=context ; 
        _mapper=mapper ; 
    }


    public void AddMessage(Message message)
    {
       _context.Messages.Add(message) ; 
    }

    public void DeleteMessage(Message message)
    {
       _context.Messages.Remove(message) ;
    }

    public async Task<Message> GetMessage(int id)

    {
    return await _context.Messages.FindAsync(id)   ; 
    }

    public async Task<PagedList<MessageDto>> GetMessageForUser(MessageParams messageParams)
    {
      var query =_context.Messages
      .OrderByDescending(x=>x.MessageSent).AsQueryable();

      query = messageParams.Container switch
      {
          "Inbox" =>query.Where(u=>u.RecipientUsername==messageParams.Username),
          "Outbox" => query.Where(u=>u.SenderUsername==messageParams.Username),
          _ =>query.Where(u=>u.RecipientUsername==messageParams.Username && u.DateRead==null)
      };


        var messages = query.ProjectTo<MessageDto>(_mapper.ConfigurationProvider);
        return await PagedList<MessageDto>.CreateAsync(messages,messageParams.PageNumber,messageParams.PageSize) ;
    }




    public async Task<IEnumerable<MessageDto>> GetMessageThread(string CurrenntUsername , string recepientUsername)
    {
       var messages=await _context.Messages
       .Include(u=> u.Sender).ThenInclude(p=>p.Photos)
       .Include(u=>u.Recipient).ThenInclude(p=>p.Photos)
       .Where(
        m=>m.RecipientUsername==CurrenntUsername && 
        m.SenderUsername==recepientUsername || 
        m.SenderUsername==CurrenntUsername && 
        m.RecipientUsername== recepientUsername
       )
       .OrderByDescending(m=>m.MessageSent)
       .ToListAsync();

       var unreadMessages = messages.Where(m=>m.DateRead==null && m.RecipientUsername==CurrenntUsername).ToList() ;

       if(unreadMessages.Any()) {
        foreach (var message in messages){
            message.DateRead=DateTime.UtcNow ; 
        }

        await _context.SaveChangesAsync() ; 


       }

       return _mapper.Map<IEnumerable<MessageDto>>(messages) ; 

    }

    public  async Task<bool> SaveAllAsync()
    {
        return await _context.SaveChangesAsync() > 0 ; 
    }
}
