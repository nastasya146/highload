using Highload.SocialNetwork.Domain;

namespace Highload.SocialNetwork.Contracts;

public interface IUserRepository
{
    Task AddUser(User user, string password, CancellationToken cancellationToken);
    
    Task<User> GetUser(Guid userId, CancellationToken cancellationToken);
}