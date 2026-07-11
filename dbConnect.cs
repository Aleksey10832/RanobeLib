using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using dotenv.net;


namespace DbConnect;

public class Database: DbContext
{
    public DbSet<Ranobe> Ranobes {get; set;}
    public DbSet<Chapter> Chapters {get; set;}
    public DbSet<Paragraf> Paragrafs {get; set;}

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(
            Environment.GetEnvironmentVariable("DB_CONNECTION_STRING"));
    }
}

public class Ranobe
{
    public string Id {get; set;}
    public string name {get; set;}
    public List<Chapter> chapters {get; set;} = [];

}
public class Chapter
{
    public string Id {get; set;}
    public string ranobeId {get; set;}
    public string name {get; set;}
    public int number {get; set;}
    public List<Paragraf> paragrafs {get; set;} = [];
}
public class Paragraf
{
    public string Id {get; set;}
    public int number {get; set;}
    public string text {get; set;}
    public string chapterId {get; set;}
}