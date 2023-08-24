using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.EntityFrameworkCore;
using SQLitePCL;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly DataContext _context;

    public UsersController(DataContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<AppUser>>> GetUsers()
    {
        var Users = await _context.Users.ToListAsync();
        return Users;
    }


    [HttpGet("{id}")]

    public async Task<ActionResult<AppUser>> GetUserById(int id)
    {
        var User = await _context.Users.FindAsync(id);
        return User;
    }






}
