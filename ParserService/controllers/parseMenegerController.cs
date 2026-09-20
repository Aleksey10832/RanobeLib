using Microsoft.AspNetCore.Mvc;
using App.Result;
using App.Parser;
using DbConnect;
using Microsoft.EntityFrameworkCore;

namespace App.Controller.ParseController;

[Route("/parse")]
public class ParseController: ControllerBase
{
    private readonly Database database;
    private readonly ParsersMeneger parser;
    public ParseController(Database _db, ParsersMeneger parser){
        this.database = _db;
        this.parser = parser;
    }
    [HttpGet("status/{userId}")]
    public async Task<Result<ParseModel>> GetParseStatus(string userId){
        System.Console.WriteLine(userId);
        ParseModel? parse = await this.database.Parsers.FirstOrDefaultAsync(p => p.UserId.ToString() == userId); 
        if(parse == null)
        {
            return Result<ParseModel>.Fail(404, "Not Found");
        }
        return Result<ParseModel>.Succesful(parse);
    }
    [HttpGet("{ranobeUrl}")]
    public async Task<Result<RanobeModel>> GetRanobe(string ranobeUrl){
        RanobeModel? ranobe = await database.Ranobes.FirstOrDefaultAsync(el => el.RanobeUrl == ranobeUrl); 
        if(ranobe == null)
        {
            return Result<RanobeModel>.Fail(404, "Not Found");
        }
        ranobe.Chapters = await database.Chapters.Where(el => el.ranobeId == ranobe.Id).ToListAsync();
        foreach(ChapterModel chapter in ranobe.Chapters)
        {
            chapter.paragrafs = await database.Paragrafs.Where(el => el.chapterId == chapter.Id).ToListAsync();
        }
        return Result<RanobeModel>.Succesful(ranobe);
    }
    [HttpPost]
    public Result<ParseStatus> Start([FromBody] ParseModel userDate)
    {
        if(userDate == null || !this.parser.Start(userDate.RanobeUrl, userDate.UserId))
        {
            return Result<ParseStatus>.Fail(400, "Чёто не чисто");
        }
        Parse? status = this.parser.GetParse(userDate.UserId.ToString());
        if(status == null)
        {
            return Result<ParseStatus>.Fail(500, "Потеряшка");
        }
        return Result<ParseStatus>.Succesful(status.status);
    }
}