using App.Controller.Filter.AuthFillter;
using App.Models.TokensModel;
using App.Models.UserModel;
using App.Result;
using App.Services.SessionService;
using DbConnect;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace App.Controller.UserController;

[Route("user")]
public class UserController : ControllerBase
{
    private readonly Database database;
    private readonly ISessionService service;
    public UserController(Database _db, ISessionService service) {
        this.database = _db;
        this.service = service;
    }
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
            User? dbUser = await database.Users.SingleOrDefaultAsync(us => us.Login == user.Login);
            if ( dbUser != null && dbUser.checkPasword(user.Password)){
                return await service.RefershToken(null, user.Login, null, userAgent);
            }
            return Result<TokensM>.Fail(401, "Логин или пароль не верен");
        } catch (Exception error){
            return Result<TokensM>.Fail(500, "Server Error Message: " + error.Message);
        }
        
    }

    [HttpPost("register")]
    public async Task<Result<TokensM>> RegisterUser( [FromBody] IUserM user, [FromHeader(Name = "User-Agent")] string userAgent)
    {
        try{
            database.Users.Add(new User(user.Login, user.Password, "User", "name"));
            await database.SaveChangesAsync();
            return await service.RefershToken(null, user.Login, null, userAgent);
        } catch (DbUpdateException ex) {
            return Result<TokensM>.Fail(401, "Данный логин уже занят, попробуйте другой");
        } catch (Exception error){
            return Result<TokensM>.Fail(500, "Server Error Message: " + error.Message);
        }
    }

    

    [HttpGet("profile")]
    [TypeFilter(typeof(AuthFillter), Arguments = ["profile"])]
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
    [HttpGet("getAll")]
    [TypeFilter(typeof(AuthFilter), Arguments = ["usersMenegment"])]
    public async Task<Result<List<User>>> GetProfile([FromHeader(Name = "Authorization")] string jwtToken)
    {
        try{
            List<User> users = await database.Users.ToListAsync();
            return Result<List<User>>.Succesful(users);
        }
        catch{
            return Result<List<User>>.Fail(500, "Server Error");
        }
    }

    
}
