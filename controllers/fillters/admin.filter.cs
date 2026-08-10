using System.IdentityModel.Tokens.Jwt;
using System.Text;
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
        
        string token = httpContext.HttpContext.Request.Headers.Authorization.ToString()[7..];
        SecurityToken aboba;
        (new JwtSecurityTokenHandler ()).ValidateToken(token, validateParams, out aboba);

        await next();
        System.Console.WriteLine(aboba.SigningKey.);
    }
    
}