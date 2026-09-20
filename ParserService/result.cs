namespace App.Result;
public record Result<T> : IResult
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
    public async Task ExecuteAsync(HttpContext context)
    {
        context.Response.StatusCode = this.StatusCode;
        context.Response.ContentType = "application/json";
        if (StatusCode >= 200 && StatusCode < 300){
            await context.Response.WriteAsJsonAsync(Value);
        }
        else{
            await context.Response.WriteAsJsonAsync(new { Error = StatusMessage });
        }
    }
}