using System.Security.Cryptography;
using App.Controller.Filter.AuthFillter;
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
    private readonly Database database;
    public UserController(Database _db) => this.database = _db;
    [HttpPost("create")]
    [TypeFilter(typeof(AdminFillter))]
    public async Task<Result<User>> CreateUser( [FromBody] IUserM user)
    {
        if(user.Role != null){
            try{
                database.Users.Add(new User(user.Login, user.Password, user.Role));
                await database.SaveChangesAsync();
                return Result<User>.Succesful(await database.Users.Where(us => us.Login == user.Login).FirstAsync());
            } catch{
                return Result<User>.Fail(400, "Пользователь с таким именем уже существует");
            }
        }
        else{
            return Result<User>.Fail(400, "роль не указана");
        }
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
            try{
                if(await database.Users.CountAsync() > 0)
                {
                    return Result<TokensM>.Fail(401, "Логин или пароль не верен"); //login
                }
                database.Users.Add(new User(user.Login, user.Password, "Admin"));
                await database.SaveChangesAsync();
                return await this.RefershToken(null, user.Login, null);
            } catch{
                return Result<TokensM>.Fail(500, "server error"); //database problem
            }
            
        }
    }

    [HttpPost("register")]
    public async Task<Result<TokensM>> RegisterUser( [FromBody] IUserM user)
    {
        try{
            database.Users.Add(new User(user.Login, user.Password, "User"));
            await database.SaveChangesAsync();
            return await this.RefershToken(null, user.Login, null);
        } catch{
            return Result<TokensM>.Fail(401, "Данный логин уже занят, попробуйте другой"); //login
        }
    }

    [HttpPost("token/refersh/{refershToken}")]
    public async Task<Result<TokensM>> RefershToken(string refershToken, string? login, [FromHeader(Name = "Authorization")] string? jwtToken)
    {
        if(refershToken != null && jwtToken != null){
            try {
                string? loginIsToken = Functions.GetLoginIsToken(jwtToken);
                User dbUser = await database.Users.Where(us => us.Login == loginIsToken).FirstAsync();
                if(dbUser.RefershToken == refershToken) {
                    TokensM tokens = new (
                        Functions.GenerateAccessToken(dbUser.Login, dbUser.Role),
                        WebEncoders.Base64UrlEncode(RandomNumberGenerator.GetBytes(32))
                    );
                    dbUser.RefershToken = tokens.RefershToken;
                    await database.SaveChangesAsync();
                    return Result<TokensM>.Succesful(tokens);
                } else{
                    Result<TokensM>.Fail(401, "Нет, отказано0");
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

    [HttpGet("profile")]
    [TypeFilter(typeof(AuthFillter))]
    public async Task<Result<ProfileM>> GetProfile([FromHeader(Name = "Authorization")] string jwtToken)
    {
        try{
            User user = await database.Users.FirstAsync(user => user.Login == Functions.GetLoginIsToken(jwtToken));
            return Result<ProfileM>.Succesful(new ProfileM(user.Id, user.Role, user.Name));
        }
        catch{
            return Result<ProfileM>.Fail(401, "Аккаунт не найден");
        }
    }

    
}