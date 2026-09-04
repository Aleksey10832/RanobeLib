using App.Models.TokensModel;
using App.Result;
using App.Services.SessionService;
using DbConnect;
using Microsoft.AspNetCore.Mvc;

namespace App.Controller.SessionController;

[Route("session")]
public class SessionController : ControllerBase{
    private readonly Database database;
    private readonly ISessionService service;
    public SessionController(Database _db, ISessionService service) {
        this.database = _db;
        this.service = service;
    }

    [HttpPost("refersh/{refershToken}")]
    public async Task<Result<TokensM>> UpdateTokens(string refershToken, string? login, string? userAgent, [FromHeader(Name = "Authorization")] string? jwtToken){
        try {
            return await service.RefershToken(refershToken, login, jwtToken, userAgent);
        } catch {
            return Result<TokensM>.Fail(401, "Отказано");
        }
    }
    
    [HttpPost("confirm/{confirmToken}")]
    public async Task<Result<string>> ConfirmUserTokens(string confirmToken, [FromHeader(Name = "Authorization")] string accessToken){
        try {
            return await service.ConfirmToken(confirmToken, accessToken);
        } catch (Exception ex) {
            return Result<string>.Fail(500, "Server Error Message: " + ex);
        }
    }
}