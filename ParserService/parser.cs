
using AngleSharp;
using DbConnect;
using Microsoft.EntityFrameworkCore;


// using DbConnect;
using System.Collections.Concurrent;


namespace App.Parser;
public class ParsersMeneger{
    public static ConcurrentDictionary<string, Parse> parsers {get;}= new();
    IServiceScopeFactory scopeFactory;
    public ParsersMeneger(IServiceScopeFactory scopeFactory){
        this.scopeFactory = scopeFactory;
    }
    public bool Start(string ranobeUrl, Guid userId)
    {
        var scop = this.scopeFactory.CreateScope();
        Parse parser = new(scop.ServiceProvider.GetRequiredService<Database>(), this);
        if(!parsers.TryAdd(userId.ToString(), parser)){
            scop.Dispose();
            return false;
        }
        parser.startParse(ranobeUrl, userId);
        return true;
    }
    public Parse? GetParse(string userId)
    {
        System.Console.WriteLine(1);
        parsers.TryGetValue(userId, out var parse);
        return parse;
    }
    public void Remove(string userId)
    {
        parsers.Remove(userId, out _);
    }
}


public class Parse
{
    public Guid userId;
    public ParseStatus status = new();
    ParsersMeneger parser;
    Database database;
    public Parse(Database database, ParsersMeneger parser){
        this.database = database;
        this.parser = parser;
    }

    public async void startParse(string ranobeUrl, Guid userId){
        this.userId = userId;
        var config = Configuration.Default.WithDefaultLoader();
        var context = BrowsingContext.New(config);
        
        
        var document = await context.OpenAsync($"https://ranobe.me/{ranobeUrl}");
        var pages = document.QuerySelectorAll(".FicContentsChapterName");

        RanobeModel ranobe = new();
        ranobe.RanobeUrl = ranobeUrl;
        ranobe.Id = Guid.NewGuid().ToString();
        string? RanobeName = document.QuerySelector("h1")?.TextContent;
        if(RanobeName != null)
        {
            ranobe.Name = RanobeName;
        }
        
        ranobe.UserId = this.userId;
        this.status.ranobeName = ranobe.Name;
        this.status.chaptersCount = pages.Count;
        List<ChapterModel> chapters = [];
        ParseModel parseModel = new ();
        parseModel.RanobeUrl = ranobeUrl;
        parseModel.ProcessStatus = status.getStatus();
        parseModel.UserId = userId;
        database.Parsers.Add(parseModel);
        await database.SaveChangesAsync();
        ParseModel? dbParseStatus = await database.Parsers.FirstOrDefaultAsync(pars => pars.RanobeUrl == ranobeUrl);
        foreach(var page in pages)
        {
            var chapter = await context.OpenAsync($"https://ranobe.me/{page.QuerySelector("a").GetAttribute("href")}");
            string? chapterName = chapter.QuerySelector("h1")?.TextContent;
            if(chapterName == null)
            {
                break;
            }
            List<ParagrafModel> paragrafs = [];
            ChapterModel chapter1 = new();
            chapter1.Id = Guid.NewGuid().ToString();
            chapter1.name = chapterName;
            chapter1.ranobeId = ranobe.Id;
            chapter1.number = this.status.chaptersDone;
            int paragrafIterrator = 0;
            foreach(var paragraf in chapter.QuerySelectorAll(".fict"))
            {
                ParagrafModel newP = new ();
                newP.Id = Guid.NewGuid().ToString();
                newP.chapterId = chapter1.Id;
                newP.text = paragraf.TextContent;
                newP.number = paragrafIterrator;
                paragrafs.Add(newP);
                paragrafIterrator ++;
            }
            chapter1.paragrafs = paragrafs;
            chapters.Add(chapter1);
            
            this.status.chaptersDone++;
            if(dbParseStatus != null){
                dbParseStatus.ProcessStatus = this.status.getStatus();
                await database.SaveChangesAsync();
            }
            
        }
        ranobe.Chapters = chapters;
        database.Ranobes.Add(ranobe);
        await database.SaveChangesAsync();
        parser.Remove(userId.ToString());
        
    }
}
public class ParseStatus{
    public double status {get; set;}
    public string ranobeName {get; set;} = "";
    public int chaptersCount {get; set;}
    public int chaptersDone {get; set;}
    public double getStatus()
    {
        return Math.Round((double)chaptersDone / chaptersCount, 4) * 100;
    }
}