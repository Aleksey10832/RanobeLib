using App.Parser;
using DbConnect;
dotenv.net.DotEnv.Load();
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<Database>();
builder.Services.AddControllers();
builder.Services.AddSingleton<ParsersMeneger>();
builder.Services.AddDbContext<Database>();
var app = builder.Build();
app.MapControllers();
app.Use(async (context, next) =>
{
    string? key = Environment.GetEnvironmentVariable("CONNECT_KEY");
    string? bearer = context.Request.Headers["Authorization"];
    if(bearer != null)
    {
        if(bearer[7..] == key) {
            await next();
        }
    }
    
    else {
        context.Response.StatusCode = 401;
    }
});
app.Run();
