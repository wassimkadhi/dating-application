namespace API;

public interface IMessageRepository
{

    void AddMessage(Message message );
    void DeleteMessage(Message message) ; 
    Task<Message> GetMessage(int id) ; 
    Task<PagedList<MessageDto>> GetMessageForUser(MessageParams messageParams) ; 
    Task<IEnumerable<MessageDto>>GetMessageThread(string CurrentUsername , string recepientUsername) ; 

    void AddGroup(Group group );
    void RemoveConnection(Connection connection);

    Task<Connection>GetConnection(string connectionId);
     
     Task<Group>GetMessageGroup(string groupName) ;



}
