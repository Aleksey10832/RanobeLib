using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Mvc.Filters;

public class AdminFillter : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync( ActionExecutingContext httpContext, ActionExecutionDelegate next)
    {
        // SecurityToken aboba;
        try{
            if(new JwtSecurityTokenHandler ().ValidateToken(httpContext.HttpContext.Request.Headers.Authorization.ToString()[7..], Functions.ValidateParams, out _).IsInRole("Admin"))
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