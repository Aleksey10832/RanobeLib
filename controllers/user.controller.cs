using System.Security.Cryptography;
using App.Models.TokensModel;
using App.Models.UserModel;
using App.Result;
using DbConnect;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;

namespace App.Controller.UserController;

[Route("user")]
public class UserController : ControllerBase
{
    private readonly Database database = new();
    [HttpPost("create")]
    // [TypeFilter(typeof(AdminFillter))]
    public async Task<Result<User>> CreateUser( [FromBody] IUserM user)
    {
        database.Users.Add(new User(user.Login, user.Password, user.Role));
        await database.SaveChangesAsync();
        return Result<User>.Succesful(await database.Users.Where(us => us.Login == user.Login).FirstAsync());
    }

    [HttpPost("login")]
    public async Task<Result<TokensM>> LoginUser( [FromBody] IUserM user)
    {
        try{
            User dbUser = await database.Users.Where(us => us.Login == user.Login).FirstAsync();
            if (dbUser.checkPasword(user.Password)){
                return await this.RefershToken(null, user.Login, null);
            }
            return Result<TokensM>.Fail(401, "Логин или пароль не верен"); //password
        } catch{
            return Result<TokensM>.Fail(401, "Логин или пароль не верен"); //login
        }
    }

    [HttpPost("token/refersh")]
    public async Task<Result<TokensM>> RefershToken([FromBody] TokensM? tokensM, string? login, [FromHeader(Name = "Authorization")] string? jwtToken)
    {
        if(tokensM != null && jwtToken != null){
            try {
                string? loginIsToken = Functions.GetLoginIsToken(jwtToken);
                User dbUser = await database.Users.Where(us => us.Login == loginIsToken).FirstAsync();
                if(dbUser.RefershToken == tokensM.RefershToken) {
                    TokensM tokens = new (
                        Functions.GenerateAccessToken(dbUser.Login, dbUser.Role),
                        WebEncoders.Base64UrlEncode(RandomNumberGenerator.GetBytes(32))
                    );
                    dbUser.RefershToken = tokens.RefershToken;
                    await database.SaveChangesAsync();
                    return Result<TokensM>.Succesful(tokens);
                } else{
                    Result<TokensM>.Fail(401, "Нет, отказано");
                }
                
            } catch {
                Result<TokensM>.Fail(401, "Нет, отказано");
            }
        } else if (login != null){
            User dbUser = await database.Users.Where(us => us.Login == login).FirstAsync();
            TokensM tokens = new (
                Functions.GenerateAccessToken(dbUser.Login, dbUser.Role),
                WebEncoders.Base64UrlEncode(RandomNumberGenerator.GetBytes(32))
            );
            dbUser.RefershToken = tokens.RefershToken;
            await database.SaveChangesAsync();
            return Result<TokensM>.Succesful(tokens);
        }
        return Result<TokensM>.Fail(401, "Нет, отказано");
    }
}