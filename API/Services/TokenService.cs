using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using API.Entities;
using API.Interfaces;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace API.Services;


public class TokenService : ITokenService
{
    private readonly SymmetricSecurityKey _key;
    private readonly UserManager<AppUser> _usermanager;
    public TokenService(IConfiguration config    , UserManager<AppUser> usermanager)
    {
        // creating the key from config file 
        _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["TokenKey"]));
        _usermanager=usermanager ;
    }

    public async Task<string> CreateToken(AppUser user)
    {
        var claims = new List<Claim>
       {
        new Claim(JwtRegisteredClaimNames.NameId,user.Id.ToString()),
        new Claim(JwtRegisteredClaimNames.UniqueName,user.UserName)
       };

       var roles= await _usermanager.GetRolesAsync(user);
       claims.AddRange(roles.Select(role=>new Claim(ClaimTypes.Role,role))) ;


               var creds = new SigningCredentials( _key ,SecurityAlgorithms.HmacSha512Signature) ; 
        var tokenDescriptor =  new SecurityTokenDescriptor{
            Subject=new ClaimsIdentity(claims),
            Expires=DateTime.Now.AddDays(7),
            SigningCredentials=creds
          //descriping what token should include
        } ; 

        //we need a handler now
        var tokenHandler = new JwtSecurityTokenHandler() ; //identity model package insatlled
          // creating token  
          var token =tokenHandler.CreateToken(tokenDescriptor); 
          //return the token 
          return tokenHandler.WriteToken(token);    




    }
}


