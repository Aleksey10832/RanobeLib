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
    public DbSet<Session> Sessions {get; set;}

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
public class User{
    public Guid Id {get; set;}
    public string login {get; set;}
    public byte[] password {get; set;}
    public string Role {get; set;}
    private User() { }
    public User( string login, string password ){
        Id = Guid.NewGuid();
        this.login = login;
        this.password = SHA256.HashData(ASCIIEncoding.ASCII.GetBytes(password));
    }
    public bool checkPasword(string password){
        byte[] sourcePassword = SHA256.HashData(ASCIIEncoding.ASCII.GetBytes(password));
        if(sourcePassword.Length == this.password.Length)
        {
            int i = 0;
            while(i < sourcePassword.Length && sourcePassword[i] == this.password[i])
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
public class Session{
    public Guid Id{get; set;}
    public Guid UserId {get; set;}
    public string AccesToken {get; set;}
    public string RefershToken {get; set;}
}
public class UserCheckChapter{
    
}