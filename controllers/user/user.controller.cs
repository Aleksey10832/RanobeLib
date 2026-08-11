using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using App.controller.UserModel;
using App.Result;
using DbConnect;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.IdentityModel.Tokens.Experimental;

namespace App.controller.UserController;

[Route("user")]
public class UserController : ControllerBase
{
    private readonly Database database = new();
    [HttpPost("create")]
    [TypeFilter(typeof(AdminFillter))]
    public async Task<Result<User>> CreateUser( [FromBody] UserM user)
    {
        database.Users.Add(new User(user.login, user.password));
        await database.SaveChangesAsync();
        return Result<User>.Succesful(await database.Users.Where(us => us.login == user.login).FirstAsync());
    }
    [HttpPost("login")]
    public async Task<Result<string>> LoginUser( [FromBody] UserM user)
    {
       
        if((await database.Users.Where(us => us.login == user.login).FirstAsync()).checkPasword(user.password))
        {
            
            List<Claim> claims = new () {new Claim(ClaimTypes.Role, "User"), new Claim(ClaimTypes.Name, user.login)};
            JwtSecurityToken jwt = new (issuer: "MyAuthServer", audience: "MyAPIClient", claims: claims, expires: DateTime.UtcNow.Add(TimeSpan.FromMinutes(5)), signingCredentials: new SigningCredentials (new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("JWT_KEY"))), SecurityAlgorithms.HmacSha256));
            return Result<string>.Succesful(new JwtSecurityTokenHandler().WriteToken(jwt));
        }
        return Result<string>.Fail(401, "Логин или пароль не верен");
        
    }
}