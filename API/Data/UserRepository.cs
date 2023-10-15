using API.Data;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace API;

public class UserRepository : IUserRepository
{
    private readonly DataContext _context;
    private readonly IMapper _mapper ;

    public UserRepository(DataContext context ,IMapper mapper)
    {
        _mapper=mapper ;
        _context = context;
    }
    public async Task<AppUser> GetUserByUserNameAsync(string username)
    {

        var User = await _context.Users
        .Include(p => p.Photos).
        SingleOrDefaultAsync(u => u.UserName == username);
        return User;
    }

    public async Task<IEnumerable<AppUser>> GetUsersAsync()
    {
        var Users = await _context.Users
        .Include(p =>p.Photos).
        ToListAsync();
        return Users;

    }

    public async Task<bool> SaveAllAsync()
    {
        return await _context.SaveChangesAsync()>0;
    }

    public void UpadteUser(AppUser user)
    {
        _context.Entry(user).State = EntityState.Modified;
    }

    public async Task<AppUser> GetUserByIdAsync(int id)
    {
        var User = await _context.Users.FindAsync(id);
        return User;
    }

    public async Task<PagedList<MemeberDto>> GetMembersAsync(UserParams userParams)
    {
        var query = _context.Users.AsQueryable(); 

        var minDob=DateOnly.FromDateTime(DateTime.Today.AddYears(-userParams.MaxAge-1)) ;
        var maxDob=DateOnly.FromDateTime(DateTime.Today.AddYears(-userParams.MinAge)) ; 




        query=query.Where(u=>u.UserName!=userParams.CurrentUsername) ; 
        query=query.Where(u=>u.Gender ==userParams.Gender);
        query=query.Where(u=>u.DateOfBirth>=minDob && u.DateOfBirth<=maxDob);
        query=userParams.OrderBy switch{
            "created"=> query.OrderByDescending(u=>u.Cretaed) , 
            _=>query.OrderByDescending(u=>u.LastActive) 
        } ;

      
        return await PagedList<MemeberDto>.CreateAsync(
        query.AsNoTracking().ProjectTo<MemeberDto>(_mapper.ConfigurationProvider),
         userParams.PageNumber,
         userParams.PageSize) ;


        
    }

    public async Task<MemeberDto> GetMembersAsync(string username)
    {
       return await _context.Users.Where(x=>x.UserName==username)
        .ProjectTo<MemeberDto>(_mapper.ConfigurationProvider).SingleOrDefaultAsync();
    }
}

   
