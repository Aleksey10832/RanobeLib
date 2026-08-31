using DbConnect;
using dotenv.net;
// using Microsoft.EntityFrameworkCore;


DotEnv.Load();
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<Database>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("NextJSPolicy", policy =>
    {
        policy.WithOrigins("http://localhost:3000", "http://127.0.0.1:3000", "http://192.168.1.186:3000")
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});
builder.Services.AddControllers();
builder.Services.AddStackExchangeRedisCache(options => {
    options.Configuration = Environment.GetEnvironmentVariable("REDIS_CONNECTION");
    options.InstanceName = "RanobeLib";
});
var app = builder.Build();
app.UseRouting();
app.UseCors("NextJSPolicy");
app.UseAuthorization();
app.MapControllers();
app.Run();