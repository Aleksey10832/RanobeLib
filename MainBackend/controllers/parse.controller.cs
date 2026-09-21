using Microsoft.AspNetCore.Mvc;
using App.Result;
using DbConnect;
using Microsoft.EntityFrameworkCore;
using App.HttpClients.Parse;
using App.Controller.Filter.AuthFillter;

namespace App.Controller.ParseController;

[Route("/parse")]
public class ParseController: ControllerBase
{
    private readonly Database database = new();
    private readonly ParsersMeneger parser;
    public ParseController(Database _db, ParsersMeneger parser){
        this.database = _db;
        this.parser = parser;
    }
    [HttpGet("{ranobeUrl}")]
    [TypeFilter(typeof(AuthFillter), Arguments = ["parser"])]
    public async Task<Result<ParseModel>> GetParseStatus(string ranobeUrl, [FromHeader(Name = "Authorization")] string accessToken){
        string? userLogin = Functions.GetLoginIsToken(accessToken);
        Guid userId = Guid.NewGuid();
        if(userLogin != null)
        {
            User? user = await this.database.Users.FirstOrDefaultAsync(user => userLogin == user.Login);
            if(user != null) {userId = user.Id;}
        }
        return await parser.GetParseStatus(ranobeUrl, userId); 
    }
    [HttpPost("result/{ranobeUrl}")]
    [TypeFilter(typeof(AuthFillter), Arguments = ["parser"])]
    public async Task<Result<string>> GetParseResult(string ranobeUrl, [FromHeader(Name = "Authorization")] string accessToken){
        string? userLogin = Functions.GetLoginIsToken(accessToken);
        Guid userId = Guid.NewGuid();
        if(userLogin != null)
        {
            User? user = await this.database.Users.FirstOrDefaultAsync(user => userLogin == user.Login);
            if(user != null) {userId = user.Id;}
            Result<Ranobe> ranobe = await parser.GetParseResult(ranobeUrl);
            if (ranobe.Value != null){
                database.Add(ranobe.Value);
                database.SaveChanges();
                return Result<string>.Succesful("OK");
            }
        }
        return Result<string>.Fail(404, "NOT FOUND");
    }
    [HttpPost("{ranobeUrl}")]
    [TypeFilter(typeof(AuthFillter), Arguments = ["parser"])]
    public async Task<Result<ParseModel>> Start(string ranobeUrl, [FromHeader(Name = "Authorization")] string accessToken)
    {
        return await parser.SetParseStatus(ranobeUrl, accessToken);
    }
}