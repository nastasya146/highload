using Grpc.Core;
using Highload.SocialNetwork.Contracts;

namespace Highload.SocialNetwork.Services;

public class UserService : UserApi.UserApiBase
{
    private readonly IUserRepository _userRepository;
    public UserService(
        IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public override Task<LoginResponse> Login(
        LoginRequest request, 
        ServerCallContext context)
    {
        return Task.FromResult(new LoginResponse());
    }

    public override async Task<RegisterUserResponse> RegisterUser(
        RegisterUserRequest request, 
        ServerCallContext context)
    {
        var id = Guid.NewGuid();
        var user = request.MapToUser(id);
        await _userRepository.AddUser(user, request.Password, context.CancellationToken);
        return new RegisterUserResponse
        {
            UserId = id.ToString()
        };
    }
    
    public override async Task<UserResponse> User(
        UserRequest request, 
        ServerCallContext context)
    {
        var user = await _userRepository.GetUser(new Guid(request.UserId), context.CancellationToken);
        return user.MapToResponse();
    }
}
