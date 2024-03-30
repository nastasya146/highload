namespace Highload.SocialNetwork.Contracts;

public interface IPasswordService
{
    string HashPassword(string password);

    bool VerifyHashedPassword(string hashedPassword, string providedPassword);
}