using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AngleSharp.Dom;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.IdentityModel.Tokens;
public static class Functions
{
    public static string GenerateAccessToken (string login, string role)
    {
        JwtSecurityToken jwt = new (
            issuer: "MyAuthServer", 
            audience: "MyAPIClient", 
            claims: new List<Claim> () {
                new Claim(ClaimTypes.Role, role), 
                new Claim(ClaimTypes.Name, login)
            },
            expires: DateTime.UtcNow.Add(TimeSpan.FromMinutes(2)), 
            signingCredentials: new SigningCredentials (new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(
                    Environment.GetEnvironmentVariable("JWT_KEY")
                )),  
                SecurityAlgorithms.HmacSha256
            )
        );
        return new JwtSecurityTokenHandler().WriteToken(jwt);
    }
    public static string? GetLoginIsToken (string jwtToken) {
        var validateParams = new TokenValidationParameters{
            ValidateIssuer = true,
            ValidIssuer = "MyAuthServer",
            ValidateAudience = true,
            ValidAudience = "MyAPIClient",
            ValidateLifetime = false,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("JWT_KEY"))),
            ValidateIssuerSigningKey = true,
            
        };        
        try{
            var token = new JwtSecurityTokenHandler ()
            .ValidateToken(
                jwtToken[7..], 
                validateParams, out _
            );
            return token.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
        }
        catch{
            return null;
        }
    }
}