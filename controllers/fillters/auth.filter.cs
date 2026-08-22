using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Mvc.Filters;
namespace App.Controller.Filter.AuthFillter;

public class AuthFillter : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync( ActionExecutingContext httpContext, ActionExecutionDelegate next)
    {
        // SecurityToken aboba;
        try {
            new JwtSecurityTokenHandler ().ValidateToken(httpContext.HttpContext.Request.Headers.Authorization.ToString()[7..], Functions.ValidateParams, out _);
            await next();
        } catch {
            httpContext.HttpContext.Response.StatusCode = 401;
        }
    }
}