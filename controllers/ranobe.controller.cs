using Microsoft.AspNetCore.Mvc;
using App.Result;
using DbConnect;
using Microsoft.EntityFrameworkCore;

namespace App.Controller.Main;

[Route("/ranobe")]
public class RanobeController: ControllerBase
{
    private readonly Database database = new();
    [HttpGet("{id}/{page}")]
    public async Task<Result<Ranobe>> getById(string id, int page)
    {
        Ranobe? ranobe = await database.Ranobes.FindAsync(id);
        if(ranobe != null)
        {
            ranobe.chapters = await database.Chapters.Where(c => c.ranobeId == id).OrderBy(c => c.number).Skip(page * 100).Take(100).ToListAsync();
            return Result<Ranobe>.Succesful(ranobe);
        }
        return Result<Ranobe>.Fail(404, "HUY");
    }

    [HttpGet("{page}")]
    [HttpGet("")]
    public async Task<Result<List<Ranobe>>> getAllRanobe(int page)
    {
        return Result<List<Ranobe>>.Succesful(await database.Ranobes.Where(r => r.name != null).Take(30).Skip(30 * page).ToListAsync());
    }
    [HttpGet("{ranobeId}/chapter/{id}")]
    public async Task<Result<Chapter>> getChapter(string id)
    {
        Chapter? chapter = await database.Chapters.FindAsync(id);
        if(chapter != null)
        {
            chapter.paragrafs = await database.Paragrafs.Where(p => p.chapterId == id).OrderBy(p => p.number).ToListAsync();
            return Result<Chapter>.Succesful(chapter);
        }
        return Result<Chapter>.Fail(404, "Нет такой главы");
    }
}