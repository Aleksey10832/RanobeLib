
using AngleSharp;
using AngleSharp.Dom;

var config = Configuration.Default.WithDefaultLoader();
var context = BrowsingContext.New(config);
string ranobeName = "ranobe174";
var document = await context.OpenAsync($"https://ranobe.me/{ranobeName}");
var pages = document.QuerySelectorAll(".FicContentsChapterName");
foreach(var page in pages)
{
    System.Console.WriteLine(page.QuerySelector("a").Flags);
}