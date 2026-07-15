using App.Parse;
using Microsoft.AspNetCore.Mvc;
using Result;
using App.Parse;
using DbConnect;
using Microsoft.EntityFrameworkCore;

namespace App.Controller.Main;

[Route("/")]
public class MainController: ControllerBase
{
    [HttpGet]
    public Result<ParseStatus> getParseStatus(){
        if(ParsersMeneger.GetParse("123") == null)
        {
            return Result<ParseStatus>.Fail(404, "Not Found");
        }
        return Result<ParseStatus>.Succesful(ParsersMeneger.GetParse("123").status);
    }
    [HttpPost]
    public Result<ParseStatus> Start(string ranobeUrl)
    {
        if(!ParsersMeneger.Start(ranobeUrl, "123"))
        {
            return Result<ParseStatus>.Fail(400, "Чёто не чисто");
        }
        if(ParsersMeneger.GetParse("123") == null)
        {
            return Result<ParseStatus>.Fail(500, "Потеряшка");
        }
        return Result<ParseStatus>.Succesful(ParsersMeneger.GetParse("123").status);
    }
    [HttpGet("all")]
    public async Task<Result<List<Ranobe>>> getAllRanobe()
    {
        return Result<List<Ranobe>>.Succesful(await new Database().Ranobes.Where(r => r.name != null).ToListAsync());
    }
}