using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace API.Controllers;

[Authorize]
public class UsersController : BaseApiController
{
    private readonly DataContext _context;

    public UsersController(DataContext context)
    {
        _context = context;
    }
  [AllowAnonymous]
    [HttpGet] //api/users
    public async Task<ActionResult<IEnumerable<AppUser>>> GetUsers()
    {
        var Users = await _context.Users.ToListAsync();
        return Users;
    }


    [HttpGet("{id}")]  //api/users/id

    public async Task<ActionResult<AppUser>> GetUserById(int id)
    {
        var User = await _context.Users.FindAsync(id);
        return User;
    }






}
