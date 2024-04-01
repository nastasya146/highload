using System.IdentityModel.Tokens.Jwt;

namespace Highload.SocialNetwork.Contracts;

public interface ITokenService
{
    JwtSecurityToken GenerateJWToken(string userId);
}