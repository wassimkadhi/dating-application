using System.Security.Cryptography;
using System.Text;
using API.Data;
using API.DtoS;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

public class AccountController : BaseApiController
{

   private readonly DataContext _context;
   private readonly ITokenService _tokenService;
   private readonly IMapper _mapper;
   public AccountController(DataContext context, ITokenService tokenService, IMapper mapper)
   {
      _tokenService = tokenService;
      _context = context;
      _mapper = mapper;

   }

   [HttpPost("register")] //api/account/register

   public async Task<ActionResult<UserDto>> Register(RegisterDto register)
   {
      if (await UserExist(register.Username)) return BadRequest("Username allready taken ");

      var user = _mapper.Map<AppUser>(register);

      using var hmac = new HMACSHA512();

      user.UserName = register.Username.ToLower();
      user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(register.Password));
      user.PasswordSalt = hmac.Key;

      _context.Users.Add(user);

      await _context.SaveChangesAsync();
      return new UserDto
      {
         Username = user.UserName,
         Token = _tokenService.CreateToken(user),
         KnownAs = user.KnownAs,
         Gender=user.Gender

      };
   }

   [HttpPost("login")]
   public async Task<ActionResult<UserDto>> login(LoginDto loginDto)
   {

      var user = await _context.Users.
      Include(p => p.Photos)
      .SingleOrDefaultAsync(x => x.UserName == loginDto.Username);
      if (user == null) return BadRequest("there is no Account registred with this username ");
      using var hmac = new HMACSHA512(user.PasswordSalt);
      var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));

      for (int i = 0; i < computedHash.Length; i++)
      {
         if (computedHash[i] != user.PasswordHash[i]) return Unauthorized("wrong password");

      }
      return new UserDto
      {
         Username = user.UserName,
         Token = _tokenService.CreateToken(user),
         PhotoUrl = user.Photos.FirstOrDefault(x => x.ISMain)?.Url,
         KnownAs=user.KnownAs ,
         Gender=user.Gender
      };


   }


   private async Task<Boolean> UserExist(string username)
   {

      return await _context.Users.AnyAsync(x => x.UserName == username.ToLower());
   }

}
