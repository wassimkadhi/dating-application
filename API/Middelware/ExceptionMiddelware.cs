using System.Net;
using System.Text.Json;
using API.Erros;

namespace API;

public class ExceptionMiddelware 
{

    private readonly RequestDelegate _next ; 
    private readonly ILogger<ExceptionMiddelware> _logger;
    private readonly IHostEnvironment _env  ; 
    
    public ExceptionMiddelware(RequestDelegate next,ILogger<ExceptionMiddelware>  logger , IHostEnvironment env)
    {
        _next = next;   
        _logger = logger;       
        _env = env; 
        
    }
        public async Task InvokeAsync(HttpContext context) {
        try
        {
            await _next(context) ; 
        }
        catch (Exception ex )
        {
            _logger.LogError(ex , ex.Message);
            context.Response.ContentType="application/json" ; 
            context.Response.StatusCode = (int) HttpStatusCode.InternalServerError ;
            var response =_env.IsDevelopment() 
            ? new ApiException(context.Response.StatusCode,ex.Message,ex.StackTrace?.ToString())
            :new ApiException(context.Response.StatusCode,ex.Message,"internal server") ;  
            var options =new JsonSerializerOptions{PropertyNamingPolicy=JsonNamingPolicy.CamelCase} ; 
            var json=JsonSerializer.Serialize(response,options) ;
            await   context.Response.WriteAsync(json) ;

        }
    }

}
