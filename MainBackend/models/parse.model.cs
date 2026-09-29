public class ParseModel
{
    public Guid Id {get; set;}
    public Guid UserId {get; set;}
    public string RanobeUrl { get; set;} = "";
    public double? ProcessStatus { get; set;}
    public bool? IsDone { get; set;}
    public ParseModel(Guid UserId, string RanobeUrl) {
        this.RanobeUrl = RanobeUrl;
        this.UserId = UserId;
        this.Id = Guid.NewGuid();
    }
}