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
    public async Task<Result<Ranobe>> GetById(string id, int page)
    {
        Ranobe? ranobe = await database.Ranobes.FindAsync(id);
        if(ranobe != null){
            try{
                ranobe.chapters = await database.Chapters.Where(c => c.ranobeId == id).OrderBy(c => c.number).Skip(page * 100).Take(100).ToListAsync();
                return Result<Ranobe>.Succesful(ranobe);
            } catch{
                return Result<Ranobe>.Fail(400, "Ну блять, нету больше нихуя");
            }
        }
        return Result<Ranobe>.Fail(404, "Куда ты блять лезешь");
    }

    [HttpGet("{page}")]
    [HttpGet("")]
    public async Task<Result<RanobeP>> GetAllRanobe(int page){
        return Result<RanobeP>.Succesful(new RanobeP(await database.Ranobes.Where(r => r.name != null).Take(30).Skip(30 * page).ToListAsync(), await database.Ranobes.CountAsync()));
    }
    [HttpGet("{ranobeId}/chapter/{number}")]
    public async Task<Result<Chapter>> GetChapter(string ranobeId, int number){
        try{
            Chapter? chapter = await database.Chapters.Where(el => el.ranobeId == ranobeId).FirstAsync(el => el.number == number);
            chapter.paragrafs = await database.Paragrafs.Where(p => p.chapterId == chapter.Id).OrderBy(p => p.number).ToListAsync();
            return Result<Chapter>.Succesful(chapter);
        } catch{
            return Result<Chapter>.Fail(404, "Нет такой главы");
        }
        
    }
    



    public class RanobeP
    {
        public List<Ranobe> ranobe {get; set;}
        public double pages {get; set;}
        public RanobeP(List<Ranobe> ranobe, int count){
            this.ranobe = ranobe;
            this.pages = Math.Round((double)count / 20, 2);
        }
    }
}