using Grpc.Core;

namespace Highload.SocialNetwork.Services;

public class UserService : UserApi.UserApiBase
{
    private readonly ILogger<UserService> _logger;
    public UserService(ILogger<UserService> logger)
    {
        _logger = logger;
    }

    public override Task<LoginResponse> Login(LoginRequest request, ServerCallContext context)
    {
        return Task.FromResult(new LoginResponse());
    }
    
    public override Task<RegisterUserResponse> RegisterUser(RegisterUserRequest request, ServerCallContext context)
    {
        return Task.FromResult(new RegisterUserResponse());
    }
    
    public override Task<UserResponse> User(UserRequest request, ServerCallContext context)
    {
        return Task.FromResult(new UserResponse());
    }
}
