namespace Result;
public record Result<T>
{
    public int StatusCode {get; set;}
    public string StatusMessage {get; set;} = "Ok";
    public T? Value {get; set;}

    public Result(int StatusCode, T Value){
        this.StatusCode = StatusCode;
        this.Value = Value;
    }
    public Result(int StatusCode, string StatusMessage){
        this.StatusCode = StatusCode;
        this.StatusMessage = StatusMessage;
    }
    public static Result<T> Succesful(T Value)
    {
        return new Result<T>(200, Value);
    }
    public static Result<T> Fail(int StatusCode, string StatusMessage){
        return new Result<T>(StatusCode, StatusMessage);
    }
}