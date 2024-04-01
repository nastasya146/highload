using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Highload.SocialNetwork.Contracts;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Highload.SocialNetwork.Services;

public class TokenService(IOptions<JWTSettings> jwtSettings) : ITokenService
{    
    private readonly JWTSettings _jwtSettings = jwtSettings.Value;

    public JwtSecurityToken GenerateJWToken(string userId)
    {
        var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
        var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

        var jwtSecurityToken = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            notBefore: DateTime.UtcNow,
            expires: DateTime.UtcNow.AddMinutes(_jwtSettings.DurationInMinutes),
            signingCredentials: signingCredentials);
        
        return jwtSecurityToken;
    }
}