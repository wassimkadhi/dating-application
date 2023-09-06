using System.Text;
using API;
using API.Data;
using API.Interfaces;
using API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddDbContext<DataContext>(opt =>
{
opt.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"));
}
);
builder.Services.AddCors();
//  adding our own service  
builder.Services.AddScoped<ITokenService,TokenService>() ;
//add authetication  shema 
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(Options=>
{
        Options.TokenValidationParameters=new TokenValidationParameters{
        ValidateIssuerSigningKey = true ,
        IssuerSigningKey=new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["TokenKey"])) ,
        ValidateIssuer=false ,
        ValidateAudience=false 
    } ;
    
}


);


var app = builder.Build();
app.UseMiddleware<ExceptionMiddelware>() ; 



// Configure the HTTP request pipeline.

app.UseCors(builder=>builder.AllowAnyHeader().AllowAnyHeader().WithOrigins("https://localhost:4200"));
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

