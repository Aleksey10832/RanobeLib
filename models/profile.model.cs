public class ProfileM{
    public Guid Id {get; set;}
    public string Role {get; set;}
    public string Name {get; set;}
    public ProfileM(Guid id, string role, string name){
        this.Id = id;
        this.Role = role;
        this.Name = name;
    }
}