using System.IdentityModel.Tokens.Jwt;
using Grpc.Core;
using Highload.SocialNetwork.Contracts;

namespace Highload.SocialNetwork.GRPC;

public class LoginService(
    IUserRepository userRepository,
    IPasswordService passwordService,
    ITokenService loginService)
    : Login.LoginBase
{
    public override async Task<LoginResponse> Login(
        LoginRequest request,
        ServerCallContext context)
    {
        var password = await userRepository.GetPassword(new Guid(request.UserId), context.CancellationToken);

        var isPasswordCorrect = passwordService.VerifyHashedPassword(password, request.Password);
        if (isPasswordCorrect)
        {
            var jwt = loginService.GenerateJWToken(request.UserId);
            var token = new JwtSecurityTokenHandler().WriteToken(jwt);
            return new LoginResponse
            {
                Token = token
            };
        }

        throw new RpcException(new Status(StatusCode.InvalidArgument, "Password is not correct"));
    }
}
