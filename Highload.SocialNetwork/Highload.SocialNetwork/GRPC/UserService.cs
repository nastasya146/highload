using Grpc.Core;
using Highload.SocialNetwork.Contracts;
using Highload.SocialNetwork.Services;
using Microsoft.AspNetCore.Authorization;

namespace Highload.SocialNetwork.GRPC;

public class UserService(
    IUserRepository userRepository,
    IPasswordService passwordService)
    : UserApi.UserApiBase
{
    public override async Task<RegisterUserResponse> RegisterUser(
        RegisterUserRequest request, 
        ServerCallContext context)
    {
        var id = Guid.NewGuid();
        var user = request.MapToUser(id);

        var hashedPassword = passwordService.HashPassword(request.Password);
        await userRepository.AddUser(user, hashedPassword, context.CancellationToken);
        return new RegisterUserResponse
        {
            UserId = id.ToString()
        };
    }
    
    [Authorize]
    public override async Task<UserResponse> User(
        UserRequest request, 
        ServerCallContext context)
    {
        try
        {
            var user = await userRepository.GetUser(new Guid(request.UserId), context.CancellationToken);
            return user.MapToResponse();
        }
        catch (Exception ex)
        {
            throw new RpcException(new Status(StatusCode.NotFound, "UserNotFound", ex));
        }
    }
    
    [Authorize]
    public override async Task<SearchResponse> Search(
        SearchRequest request,
        ServerCallContext context)
    {
        var users = await userRepository.Search(request.FirstName, request.LastName, context.CancellationToken);
        return new SearchResponse
        {
            UserInfo = { users.Select(x => x.MapToUserInfo()).ToArray() }
        };
    }
}
