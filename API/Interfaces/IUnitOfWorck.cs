using API.Interfaces;

namespace API;

public interface IUnitOfWorck
{
    IUserRepository UserRepository {get;}
    IMessageRepository MessageRepository {get;} 
    ILikesRepository LikesRepository{get;}

    Task<bool> Complete() ;

    bool HasChanges() ;
}

