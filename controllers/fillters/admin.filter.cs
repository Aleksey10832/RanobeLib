using Microsoft.AspNetCore.Mvc.Filters;

public class AdminFillter : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync( ActionExecutingContext httpContext, ActionExecutionDelegate next)
    {
        System.Console.WriteLine(httpContext.HttpContext.Request.Headers.Authorization.ToString()[7..]);
        await next();
        System.Console.WriteLine(1);
    }
    
}