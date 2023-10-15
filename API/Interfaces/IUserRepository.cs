using API.Entities;

namespace API.Interfaces;

public interface IUserRepository
{
 void UpadteUser (AppUser  user)  ;
 Task<bool> SaveAllAsync() ; 
 Task <IEnumerable<AppUser>> GetUsersAsync() ;  
 Task<AppUser>GetUserByIdAsync(int id) ; 
 Task<AppUser>GetUserByUserNameAsync(string username) ; 

 Task<PagedList<MemeberDto>> GetMembersAsync(UserParams userParams) ;

 Task<MemeberDto> GetMembersAsync(String username ) ;
 
}
