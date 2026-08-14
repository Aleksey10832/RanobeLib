using System.Security.Cryptography;
using App.Models.TokensModel;
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
        return Result<User>.Succesful(await database.Users.Where(us => us.Login == user.Login).FirstAsync());
    }

    [HttpPost("login")]
    public async Task<Result<ITokensM>> LoginUser( [FromBody] IUserM user)
    {
        try{
            User dbUser = await database.Users.Where(us => us.Login == user.Login).FirstAsync();
            if (dbUser.checkPasword(user.Password)){
                return await this.RefershToken(user);
            }
        } catch{
            return Result<ITokensM>.Fail(401, "Логин или пароль не верен");
        }
        return Result<ITokensM>.Fail(401, "Логин или пароль не верен");
    }

    [HttpPost("token/refersh")]
    public async Task<Result<ITokensM>> RefershToken( [FromBody] IUserM user)
    {
        try{
            User dbUser = await database.Users.Where(us => us.Login == user.Login).FirstAsync();
            ITokensM tokens = new (
                WebEncoders.Base64UrlEncode(RandomNumberGenerator.GetBytes(32)),
                Functions.GenerateAccessToken(user.Login, dbUser.Role)
            );
            return Result<ITokensM>.Succesful(tokens);
        }
        catch{
            return Result<ITokensM>.Fail(401, "ДИ нахуй");
        }
        
    }
}