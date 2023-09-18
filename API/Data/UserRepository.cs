using API.Data;
using API.Entities;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API;

public class UserRepository : IUserRepository
{
    private readonly DataContext _context;

    public UserRepository(DataContext context)
    {
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
}
