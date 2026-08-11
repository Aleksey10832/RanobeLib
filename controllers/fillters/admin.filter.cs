using System.IdentityModel.Tokens.Jwt;
using System.Text;
using App.Result;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.IdentityModel.Tokens;

public class AdminFillter : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync( ActionExecutingContext httpContext, ActionExecutionDelegate next)
    {
        var validateParams = new TokenValidationParameters{
            ValidateIssuer = true,
            ValidIssuer = "MyAuthServer",
            ValidateAudience = true,
            ValidAudience = "MyAPIClient",
            ValidateLifetime = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("JWT_KEY"))),
            ValidateIssuerSigningKey = true,
            
        };
        
        
        SecurityToken aboba;
        try{
            if(new JwtSecurityTokenHandler ().ValidateToken(httpContext.HttpContext.Request.Headers.Authorization.ToString()[7..], validateParams, out aboba).IsInRole("Admin"))
            {
                await next();
                // тут потом метрику можно будет сделать
            }
            httpContext.HttpContext.Response.StatusCode = 403;
        }
        catch{
            httpContext.HttpContext.Response.StatusCode = 401;
        }
    }
    
}