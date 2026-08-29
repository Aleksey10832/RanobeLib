using App.Controller.Filter.AuthFillter;
using App.Models.TokensModel;
using App.Models.UserModel;
using App.Result;
using DbConnect;
using Microsoft.AspNetCore.Mvc;
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
                database.Users.Add(new User(user.Login, user.Password, user.Role, "name"));
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
    public async Task<Result<TokensM>> LoginUser( [FromBody] IUserM user, [FromHeader(Name = "User-Agent")] string userAgent)
    {
        try{
            User dbUser = await database.Users.Where(us => us.Login == user.Login).FirstAsync();
            if (dbUser.checkPasword(user.Password)){
                return await this.RefershToken(null, user.Login, null, userAgent);
            }
            return Result<TokensM>.Fail(401, "Логин или пароль не верен"); //password
        } catch{
            try{
                if(await database.Users.CountAsync() > 0)
                {
                    return Result<TokensM>.Fail(401, "Логин или пароль не верен"); //login
                }
                database.Users.Add(new User(user.Login, user.Password, "Admin", "name"));
                await database.SaveChangesAsync();
                return await this.RefershToken(null, user.Login, null, userAgent);
            } catch{
                return Result<TokensM>.Fail(500, "server error");
            }
            
        }
    }

    [HttpPost("register")]
    public async Task<Result<TokensM>> RegisterUser( [FromBody] IUserM user, [FromHeader(Name = "User-Agent")] string userAgent)
    {
        try{
            database.Users.Add(new User(user.Login, user.Password, "User", "name"));
            await database.SaveChangesAsync();
            return await this.RefershToken(null, user.Login, null, userAgent);
        } catch{
            return Result<TokensM>.Fail(401, "Данный логин уже занят, попробуйте другой"); //login
        }
    }

    [HttpPost("token/refersh/{refershToken}")]
    public async Task<Result<TokensM>> RefershToken(string refershToken, string? login, [FromHeader(Name = "Authorization")] string? jwtToken, string? userAgent)
    {
        if(refershToken != null && jwtToken != null){
            try {
                string? SessionIdIsToken = Functions.GetSessionIdIsToken(jwtToken);
                UserSession? session = await database.UserSessions.FindAsync(Guid.Parse(SessionIdIsToken));
                
                if(session != null){
                    User? dbUser = await database.Users.FindAsync(session.UserId);
                    if(dbUser != null){
                        string? newRefToken = session.UpdateRefToken(refershToken);
                        if(newRefToken != null) {
                            TokensM tokens = new (
                                Functions.GenerateAccessToken(dbUser.Login, dbUser.Role, session.Id),
                                newRefToken
                            );
                            session.RefershToken = tokens.RefershToken;
                            await database.SaveChangesAsync();
                            return Result<TokensM>.Succesful(tokens);
                        }
                    } else {
                        Result<TokensM>.Fail(401, "Нет, отказано0");
                    }
                } else{
                    Result<TokensM>.Fail(401, "Нет, отказано0");
                }
                
            } catch {
                Result<TokensM>.Fail(401, "Нет, отказано");
            }
        } else if (login != null){
            User? dbUser = await database.Users.SingleOrDefaultAsync(us => us.Login == login);
            
            if(dbUser != null && userAgent != null){
                UserSession session = new(dbUser.Id, dbUser.Login, dbUser.Role, userAgent);
                TokensM tokens = new (
                    Functions.GenerateAccessToken(dbUser.Login, dbUser.Role, session.Id),
                    session.SetRefToken()
                );
                await database.UserSessions.AddAsync(session);
                await database.SaveChangesAsync();
                return Result<TokensM>.Succesful(tokens);
            }
            return Result<TokensM>.Fail(401, "Нет, отказано");
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