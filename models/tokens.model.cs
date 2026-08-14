namespace App.Models.TokensModel;
public class TokensM{
    public TokensM (string accessToken, string refershToken){
        this.AccessToken = accessToken;
        this.RefershToken = refershToken;
    }
    public string RefershToken { get; set; }
    public string? AccessToken { get; set; }
}