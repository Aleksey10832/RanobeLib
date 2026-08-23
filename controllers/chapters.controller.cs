using App.Controller.Filter.AuthFillter;
using App.Result;
using DbConnect;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace App.Controller.RanobeController;



[Route("/chapter")]
public class ChapterController: ControllerBase
{
    private readonly Database database;
    public ChapterController(Database _db) => this.database = _db;

    [HttpGet("check/{ranobeId}")]
    [TypeFilter(typeof(AuthFillter))]
    public async Task<Result<List<UserCheckChapter>>> userCheck([FromHeader(Name = "Authorization")] string jwtToken){
        Guid userId = (await database.Users.Where(user => user.Login == Functions.GetLoginIsToken(jwtToken)).FirstOrDefaultAsync()).Id;
        return Result<List<UserCheckChapter>>.Succesful(database.UserCheckChapters.Where(check => check.UserId == userId).ToList());
    }

    [HttpPost("check")]
    [TypeFilter(typeof(AuthFillter))]
    public async Task<Result<UserCheckChapter>> SetCheckChapterStatus([FromBody] UserCheckChapterP inStatus, [FromHeader(Name = "Authorization")] string jwtToken){
        try{
            Chapter? chapter = await database.Chapters.FindAsync(inStatus.chapterId);
            string? login = Functions.GetLoginIsToken(jwtToken);
            if(login != null){
                Guid userId = (await database.Users.FirstAsync(el => el.Login == login)).Id;
                try {
                    UserCheckChapter status = new (userId, inStatus.chapterId, inStatus.pNumber, (await database.Chapters.FindAsync(inStatus.chapterId)).ranobeId);
                    await database.UserCheckChapters.AddAsync(status);
                    await database.SaveChangesAsync();
                    return Result<UserCheckChapter>.Succesful(status);
                } catch {
                    return Result<UserCheckChapter>.Fail(400, "глава уже прочитана");
                }
            }
            return Result<UserCheckChapter>.Fail(403, "пользователя не существует");
        } catch{
            return Result<UserCheckChapter>.Fail(404, "Нет такой главы или пользователь неавторизирован");
        }
    }

    [HttpPut("check")]
    [TypeFilter(typeof(AuthFillter))]
    public async Task<Result<UserCheckChapter>> UpdateCheckChapterStatus([FromBody] UserCheckChapterP inStatus, [FromHeader(Name = "Authorization")] string jwtToken){
        try{
            Chapter? chapter = await database.Chapters.FindAsync(inStatus.chapterId);
            string? login = Functions.GetLoginIsToken(jwtToken);
            if(login != null){
                Guid userId = (await database.Users.FirstAsync(el => el.Login == login)).Id;
                try {
                    UserCheckChapter userCheck = await database.UserCheckChapters.SingleAsync(cChapter => cChapter.CidUsId == chapter.Id + userId);
                    userCheck.PNumber = inStatus.pNumber;
                    await database.SaveChangesAsync();
                    return Result<UserCheckChapter>.Succesful(userCheck);
                } catch {
                    return Result<UserCheckChapter>.Fail(400, "глава уже прочитана");
                }
            }
            return Result<UserCheckChapter>.Fail(403, "пользователя не существует");
        } catch{
            return Result<UserCheckChapter>.Fail(404, "Нет такой главы или пользователь неавторизирован");
        }
    }

    [HttpGet("info/{chapterId}")]
    public async Task<Result<Chapter>> getChapterInfo(string chapterId){
        try{
            return Result<Chapter>.Succesful(await database.Chapters.SingleAsync(chapter => chapter.Id == chapterId));
        }
        catch{
            return Result<Chapter>.Fail(404, "Глава не найдена");
        }
    }

    public class UserCheckChapterP{
        public string chapterId {get; set;} = "";
        public int pNumber {get; set;}
    }
}