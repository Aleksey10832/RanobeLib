using System.IdentityModel.Tokens.Jwt;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Distributed;
namespace App.Controller.Filter.AuthFillter;

public class AuthFillter : IAsyncActionFilter
{
    private readonly IDistributedCache cache;
    private readonly string rule;
    public AuthFillter(string rule, IDistributedCache cache){
        this.cache = cache;
        this.rule = rule;
    }
    public async Task OnActionExecutionAsync( ActionExecutingContext httpContext, ActionExecutionDelegate next)
    {
        try {
            
            string token = httpContext.HttpContext.Request.Headers.Authorization.ToString();
            new JwtSecurityTokenHandler ().ValidateToken(token[7..], Functions.ValidateParams, out _);
            string? StringRole = await cache.GetStringAsync(Functions.GetRoleIsToken(token));
            Console.WriteLine(rule);
            if(StringRole != null) {
                Role? role = JsonSerializer.Deserialize<Role>(StringRole);
                if(role != null){
                    foreach(string roleRule in role.Rules){
                        if(roleRule == this.rule){
                            await next();
                        }
                    }
                }
            }
            httpContext.HttpContext.Response.StatusCode = 403;
        } catch {
            httpContext.HttpContext.Response.StatusCode = 401;
        }
    }
}