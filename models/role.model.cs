public class Role{
    public Guid Id {get; set;}
    public string Name {get; set;}
    public List<string> Rules {get; set;}
    public Role(string name, List<string> rules){
        this.Id  = Guid.NewGuid();
        this.Rules = rules;
        this.Name = name;
    }
}