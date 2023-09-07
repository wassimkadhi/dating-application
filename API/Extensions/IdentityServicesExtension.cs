using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace API;

public static class IdentityServicesExtension
{
 public static IServiceCollection AddIdentityServices(this IServiceCollection services , IConfiguration config) 
 {

      services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(Options=>
{
        Options.TokenValidationParameters=new TokenValidationParameters{
        ValidateIssuerSigningKey = true ,
        IssuerSigningKey=new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["TokenKey"])) ,
        ValidateIssuer=false ,
        ValidateAudience=false 
    } ;
    
}


);

return services;



 }
}
