using System.Text.Json;
using App.Result;
using DbConnect;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;

[Route("role")]
public class RanobeController: ControllerBase{
    private readonly Database database;
    private readonly IDistributedCache cache;
    public RanobeController(Database database, IDistributedCache cache) {
        this.database = database;
        this.cache = cache;
    }

    [HttpPost("set")]
    [TypeFilter(typeof(AdminFillter))]
    public async Task<Result<Role>> SetRole([FromBody] Role role){
        // try{
            await cache.SetStringAsync(role.Name, JsonSerializer.Serialize(role));
            database.Roles.Add(role);
            await database.SaveChangesAsync();
            return Result<Role>.Succesful(JsonSerializer.Deserialize<Role>(await cache.GetStringAsync(role.Name)));
        // } catch {
        //     return Result<Role>.Fail(400, "Роль уже существует");
        // }        
    }
}