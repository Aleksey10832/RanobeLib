namespace App.Models.TokensModel;
public class ITokensM{
    public ITokensM (string accessToken, string refershToken){
        this.AccessToken = accessToken;
        this.RefershToken = refershToken;
    }
    public string RefershToken { get; set; }
    public string AccessToken { get; set; }
}