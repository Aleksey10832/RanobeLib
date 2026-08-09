using App.controller.UserModel;
using App.Result;
using DbConnect;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

namespace App.controller.UserController;

[Route("user")]
public class UserController : ControllerBase
{
    private readonly Database database = new();
    [HttpPost("create")]
    public async Task<Result<User>> createUser( [FromBody] UserM user)
    {
        database.Users.Add(new User(user.login, user.password));
        await database.SaveChangesAsync();
        return Result<User>.Succesful(await database.Users.Where(us => us.login == user.login).FirstAsync());
    }
    [HttpPost("login")]
    [TypeFilter(typeof(AdminFillter))]
    public async Task<Result<bool>> loginUser( [FromBody] UserM user)
    {
        return Result<bool>.Succesful((await database.Users.Where(us => us.login == user.login).FirstAsync()).checkPasword(user.password));
    }
}