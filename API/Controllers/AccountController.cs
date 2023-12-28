using System.Security.Cryptography;
using System.Text;
using API.Data;
using API.DtoS;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

public class AccountController : BaseApiController
{

   private readonly UserManager<AppUser> _usermanager ;
   private readonly ITokenService _tokenService;
   private readonly IMapper _mapper;
   public AccountController(UserManager<AppUser> usermanager, ITokenService tokenService, IMapper mapper)
   {
   _tokenService = tokenService;
   _usermanager= usermanager ;
   _mapper = mapper;

   }

   [HttpPost("register")] //api/account/register

   public async Task<ActionResult<UserDto>> Register(RegisterDto register)
   {
      if (await UserExist(register.Username)) return BadRequest("Username allready taken ");

      var user = _mapper.Map<AppUser>(register);
      user.UserName = register.Username.ToLower();
      
      var result=await _usermanager.CreateAsync(user,register.Password) ;
      if(!result.Succeeded) return BadRequest(result.Errors);
      var rolesResult=await _usermanager.AddToRoleAsync(user,"member") ; 
      if(!rolesResult.Succeeded)return BadRequest(result.Errors);
      return new UserDto
      {
         Username = user.UserName,
         Token = await _tokenService.CreateToken(user),
         KnownAs = user.KnownAs,
         Gender=user.Gender

      };
   }

   [HttpPost("login")]
   public async Task<ActionResult<UserDto>> login(LoginDto loginDto)
   {

      var user = await _usermanager.Users.
      Include(p => p.Photos)
      .SingleOrDefaultAsync(x => x.UserName == loginDto.Username);
      if (user == null) return Unauthorized("there is no Account registred with this username ");

      var result=await _usermanager.CheckPasswordAsync(user,loginDto.Password) ;
      if(!result)return Unauthorized("wrong password please verifey your password") ; 

      return new UserDto
      {
         Username = user.UserName,
         Token = await _tokenService.CreateToken(user),
         PhotoUrl = user.Photos.FirstOrDefault(x => x.ISMain)?.Url,
         KnownAs=user.KnownAs ,
         Gender=user.Gender
      };


   }


   private async Task<Boolean> UserExist(string username)
   {

      return await _usermanager.Users.AnyAsync(x => x.UserName == username.ToLower());
   }

}
