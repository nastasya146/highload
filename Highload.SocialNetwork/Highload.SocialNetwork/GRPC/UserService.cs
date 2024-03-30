using Grpc.Core;
using Highload.SocialNetwork.Contracts;
using Highload.SocialNetwork.Services;

namespace Highload.SocialNetwork.GRPC;

public class UserService : UserApi.UserApiBase
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordService _passwordService;
    public UserService(
        IUserRepository userRepository,
        IPasswordService passwordService)
    {
        _userRepository = userRepository;
        _passwordService = passwordService;
    }

    public override async Task<LoginResponse> Login(
        LoginRequest request, 
        ServerCallContext context)
    {
        var password = await _userRepository.GetPassword(new Guid(request.UserId), context.CancellationToken);
        
        var isPasswordCorrect = _passwordService.VerifyHashedPassword(password, request.Password);
        if (isPasswordCorrect)
        {
            return new LoginResponse
            {
                Token = "success"
            };
        }
        //auth error
        return new LoginResponse();
    }

    public override async Task<RegisterUserResponse> RegisterUser(
        RegisterUserRequest request, 
        ServerCallContext context)
    {
        var id = Guid.NewGuid();
        var user = request.MapToUser(id);

        var hashedPassword = _passwordService.HashPassword(request.Password);
        await _userRepository.AddUser(user, hashedPassword, context.CancellationToken);
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
