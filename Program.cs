
using AngleSharp;
using AngleSharp.Dom;
using dotenv.net;
using DbConnect;
DotEnv.Load();
// using(var db = new Database())
// {
//     Paragraf paragraf3 = new ();
//     paragraf3.chapterId = "123";
//     paragraf3.Id = Guid.NewGuid().ToString();
//     paragraf3.number = 1;
//     paragraf3.text = "123";
//     db.Paragrafs.Add(paragraf3);
//     await db.SaveChangesAsync();
// }
var config = Configuration.Default.WithDefaultLoader();
var context = BrowsingContext.New(config);
System.Console.Write("Укажите индификатор тайтла для парса: ");
string ranobeName = Console.ReadLine();
var document = await context.OpenAsync($"https://ranobe.me/{ranobeName}");
var pages = document.QuerySelectorAll(".FicContentsChapterName");

Ranobe ranobe = new();
ranobe.Id = Guid.NewGuid().ToString();
ranobe.name = document.QuerySelector("h1").TextContent;
List<Chapter> chapters = [];
int chapterIterrator = 0;
foreach(var page in pages)
{
    var chapter = await context.OpenAsync($"https://ranobe.me/{page.QuerySelector("a").GetAttribute("href")}");
    string chapterName = chapter.QuerySelector("h1").TextContent;
    List<Paragraf> paragrafs = [];
    Chapter chapter1 = new();
    chapter1.Id = Guid.NewGuid().ToString();
    chapter1.name = chapterName;
    chapter1.ranobeId = ranobe.Id;
    chapter1.number = chapterIterrator;
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
    System.Console.WriteLine(chapterName);
    chapterIterrator++;
}
ranobe.chapters = chapters;
using(var db = new Database())
{
    db.Ranobes.Add(ranobe);
    await db.SaveChangesAsync();
}