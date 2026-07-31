
using AngleSharp;
using DbConnect;
using System.Collections.Concurrent;

namespace App.Parser;
public static class ParsersMeneger{
    public static ConcurrentDictionary<string, Parse> parsers {get;}= new();
    public static bool Start(string ranobeUrl, string userId)
    {
        Parse parser = new();
        if(!parsers.TryAdd(userId, parser)){
            return false;
        }
        parser.startParse(ranobeUrl, userId);
        return true;
    }
    public static Parse? GetParse(string userId)
    {
        parsers.TryGetValue(userId, out var parse);
        return parse;
    }
    public static void Remove(string userId)
    {
        parsers.Remove(userId, out _);
    }
}


public class Parse
{
    public string userId = "";
    public ParseStatus status = new();
    
    public async void startParse(string ranobeUrl, string userId){
        this.userId = userId;
        var config = Configuration.Default.WithDefaultLoader();
        var context = BrowsingContext.New(config);
        
        
        var document = await context.OpenAsync($"https://ranobe.me/{ranobeUrl}");
        var pages = document.QuerySelectorAll(".FicContentsChapterName");
        this.status.chaptersCount = pages.Count;

        Ranobe ranobe = new();
        ranobe.Id = Guid.NewGuid().ToString();
        ranobe.name = document.QuerySelector("h1").TextContent;
        ranobe.userId = this.userId;
        this.status.ranobeName = ranobe.name;
        List<Chapter> chapters = [];
        foreach(var page in pages)
        {
            var chapter = await context.OpenAsync($"https://ranobe.me/{page.QuerySelector("a").GetAttribute("href")}");
            string chapterName = chapter.QuerySelector("h1").TextContent;
            List<Paragraf> paragrafs = [];
            Chapter chapter1 = new();
            chapter1.Id = Guid.NewGuid().ToString();
            chapter1.name = chapterName;
            chapter1.ranobeId = ranobe.Id;
            chapter1.number = this.status.chaptersDone;
            int paragrafIterrator = 0;
            foreach(var paragraf in chapter.QuerySelectorAll(".fict"))
            {
                Paragraf newP = new ();
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
        }
        ranobe.chapters = chapters;
        using(var db = new Database()){
            db.Ranobes.Add(ranobe);
            await db.SaveChangesAsync();
        }
        ParsersMeneger.Remove(userId);
        
    }
}
public class ParseStatus{
    public double status {get; set;}
    public string ranobeName {get; set;}
    public int chaptersCount {get; set;}
    public int chaptersDone {get; set;}
    public double getStatus()
    {
        return Math.Round((double)chaptersDone / chaptersCount, 4) * 100;
    }
}