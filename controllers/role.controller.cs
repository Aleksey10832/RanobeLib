using System.Text.Json;
using App.Result;
using DbConnect;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Npgsql;

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
        await cache.SetStringAsync("Role" + role.Name, JsonSerializer.Serialize(role));
        Role? dbRole = await database.Roles.SingleOrDefaultAsync(findRole => role.Name == findRole.Name);
        if(dbRole != null){
            dbRole.Rules = role.Rules;
        } else{
            database.Roles.Add(role);
        }
        await database.SaveChangesAsync();
        return Result<Role>.Succesful(JsonSerializer.Deserialize<Role>(await cache.GetStringAsync("Role" + role.Name))); 
    }
    [HttpGet("get")]
    [TypeFilter(typeof(AdminFillter))]
    public async Task<Result<List<Role>>> getRoles(){
        List<Role> roles = await database.Roles.ToListAsync();
        // foreach( Role roleName in roles){
        //     string? cacheRole = await cache.GetStringAsync("Role" + roleName.Name);
        //     if(cacheRole != null){
        //         Role? role = JsonSerializer.Deserialize<Role>(cacheRole);
        //         if( role != null){
        //             roleName.Rules = role.Rules;
        //         }
        //     }
        // }
        return Result<List<Role>>.Succesful(roles); 
    }
}