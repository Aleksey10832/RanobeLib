using Microsoft.AspNetCore.Mvc;
using App.Result;
using App.Parser;
using DbConnect;
using Microsoft.EntityFrameworkCore;

namespace App.Controller.Parser;

[Route("/parse")]
public class ParseController: ControllerBase
{
    private readonly Database database = new();
    [HttpGet]
    public Result<ParseStatus> getParseStatus(){
        Parse parse = ParsersMeneger.GetParse("123"); 
        if(parse == null)
        {
            return Result<ParseStatus>.Fail(404, "Not Found");
        }
        parse.status.status = parse.status.getStatus();
        return Result<ParseStatus>.Succesful(parse.status);
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
    
    [HttpDelete("{RanobeId}")]
    public async Task<Result<string>> deleteRanobe( string RanobeId )
    {
        if(await database.Ranobes.FindAsync(RanobeId) == null)
        {
            return Result<string>.Fail(404, "Нет такой ранобе");
        }
        List<Chapter> chapters = await database.Chapters.Where(r => r.ranobeId == RanobeId).ToListAsync();
        chapters.ForEach(chapter =>{
            database.Paragrafs.RemoveRange(database.Paragrafs.Where(paragraf => paragraf.chapterId == chapter.Id));
        });
        database.Chapters.RemoveRange(chapters);
        database.Ranobes.Remove(await database.Ranobes.FindAsync(RanobeId));
        database.SaveChanges();
        return Result<string>.Succesful(RanobeId);
    }
}