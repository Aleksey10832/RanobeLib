using System.Security.Cryptography;
using App.Models.UserModel;
using App.Result;
using DbConnect;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;

namespace App.Controller.UserController;

[Route("user")]
public class UserController : ControllerBase
{
    private readonly Database database = new();
    [HttpPost("create")]
    [TypeFilter(typeof(AdminFillter))]
    public async Task<Result<User>> CreateUser( [FromBody] IUserM user)
    {
        database.Users.Add(new User(user.Login, user.Password));
        await database.SaveChangesAsync();
        return Result<User>.Succesful(await database.Users.Where(us => us.login == user.Login).FirstAsync());
    }

    [HttpPost("login")]
    public async Task<Result<string>> LoginUser( [FromBody] IUserM user)
    {
        try{
            User dbUser = await database.Users.Where(us => us.login == user.Login).FirstAsync();
            if (dbUser.checkPasword(user.Password))
            {   
                return Result<string>.Succesful(Functions.GenerateAccessToken(user.Login, dbUser.Role));
            }
        } catch{
            return Result<string>.Fail(401, "Логин или пароль не верен");
        }
        return Result<string>.Fail(401, "Логин или пароль не верен");
    }

    [HttpPost("token/refersh")]
    public async Task<Result<string>> RefershToken( [FromBody] IUserM user)
    {
        var refreshToken = WebEncoders.Base64UrlEncode(RandomNumberGenerator.GetBytes(32));
    }
}