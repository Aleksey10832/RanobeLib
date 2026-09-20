using System.Security.Cryptography;
using Microsoft.AspNetCore.WebUtilities;

namespace App.Models.TokensModel;
public class TokensM{
    public TokensM (string accessToken, string refershToken){
        this.AccessToken = accessToken;
        this.RefershToken = refershToken;
        this.ConfirmToken = WebEncoders.Base64UrlEncode(RandomNumberGenerator.GetBytes(32));
    }
    public string RefershToken { get; set; }
    public string AccessToken { get; set; }
    public string ConfirmToken {get; set;}
}