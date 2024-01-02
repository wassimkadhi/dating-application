using API.Data;
using API.Interfaces;
using API.Services;
using Microsoft.EntityFrameworkCore;

namespace API;

public static class ApplicationServiceExtensions
{

  public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
  {

    // Add services to the container.


    services.AddDbContext<DataContext>(opt =>
{
  opt.UseSqlite(config.GetConnectionString("DefaultConnection"));
}
);
    services.AddCors();
    //  adding our own service  
    services.AddScoped<ITokenService, TokenService>();  
    services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies()) ;
    services.Configure<CloudinarySettings>(config.GetSection("CloudinarySettings")) ; 
    services.AddScoped<IPhotoService , PhotoService>() ; 
    services.AddScoped<LogUserActivity>() ;  
    services.AddSignalR() ;
    services.AddSingleton<presenceTracker>() ;
    services.AddScoped<IUnitOfWorck,UnitOfWork>() ;
    return services;
  }
}
