namespace Highload.SocialNetwork.Domain;

public sealed record User(
    Guid UserId,
    string FirstName,
    string LastName,
    DateOnly BirthDate,
    string? Gender,
    string City,
    string? Interests);