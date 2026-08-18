using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
namespace DbConnect;

public class Database: DbContext
{
    public DbSet<Ranobe> Ranobes {get; set;}
    public DbSet<Chapter> Chapters {get; set;}
    public DbSet<Paragraf> Paragrafs {get; set;}
    public DbSet<RanobeParser> RanobeParsers {get; set;}
    public DbSet<User> Users {get; set;}
    public DbSet<UserCheckChapter> UserCheckChapters {get; set;}

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(
            Environment.GetEnvironmentVariable("DB_CONNECTION_STRING"));
    }
}
public class RanobeParser
{
    public string Id {get; set;}
    public string userId{get; set;}
    public string name{get; set;}
    public string ranobeURL{get; set;}
}
public class Ranobe
{
    public string Id {get; set;}
    public string name {get; set;}
    public string userId {get; set;}
    public List<Chapter> chapters {get; set;}

}
public class Chapter
{
    public string Id {get; set;}
    public string ranobeId {get; set;}
    public string name {get; set;}
    public int number {get; set;}
    public List<Paragraf> paragrafs {get; set;}
}
public class Paragraf
{
    public string Id {get; set;}
    public int number {get; set;}
    public string text {get; set;}
    public string chapterId {get; set;}
}

[Index(nameof(Login), IsUnique = true)]
public class User{
    public Guid Id {get; set;}
    public string Login {get; set;}
    public byte[] Password {get; set;}
    public string Role {get; set;}
    public string? RefershToken {get; set;}
    private User() { }
    public User( string login, string password, string role){
        Id = Guid.NewGuid();
        this.Login = login;
        this.Password = SHA256.HashData(ASCIIEncoding.ASCII.GetBytes(password));
        this.Role = role;
    }
    public bool checkPasword(string password){
        byte[] sourcePassword = SHA256.HashData(ASCIIEncoding.ASCII.GetBytes(password));
        if(sourcePassword.Length == this.Password.Length)
        {
            int i = 0;
            while(i < sourcePassword.Length && sourcePassword[i] == this.Password[i])
            {
                i++;
            }
            if(i == sourcePassword.Length)
            {
                return true;
            }
        }
        return false;
    }
}
public class UserCheckChapter{
    public Guid Id {get; set;}
    public Guid UserId {get; set;}
    public string ChapterId {get; set;} = "";
    public int PNumber {get; set;}
    public UserCheckChapter(Guid userId, string chapterId, int pNumber){
        this.UserId = userId;
        this.ChapterId = chapterId;
        this.PNumber = pNumber;
    }
}