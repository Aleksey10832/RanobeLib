using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
public static class Functions
{
    public static string GenerateAccesToken (string login, string role)
    {
        JwtSecurityToken jwt = new (
            issuer: "MyAuthServer", 
            audience: "MyAPIClient", 
            claims: new List<Claim> () {
                new Claim(ClaimTypes.Role, role), 
                new Claim(ClaimTypes.Name, login)
            }, 
            expires: DateTime.UtcNow.Add(TimeSpan.FromMinutes(5)), 
            signingCredentials: new SigningCredentials (new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(
                    Environment.GetEnvironmentVariable("JWT_KEY")
                )), 
                SecurityAlgorithms.HmacSha256
            )
        );
        return new JwtSecurityTokenHandler().WriteToken(jwt);
    }
}