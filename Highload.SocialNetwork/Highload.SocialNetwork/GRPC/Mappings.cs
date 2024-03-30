using Highload.SocialNetwork.Domain;

namespace Highload.SocialNetwork.Services;

public static class Mappings
{
    public static User MapToUser(this RegisterUserRequest request, Guid id)
        => new(
            id,
            request.UserInfo.FirstName,
            request.UserInfo.LastName,
            request.UserInfo.BirthDate.MapToDateOnly(),
            request.UserInfo.Gender,
            request.UserInfo.City,
            request.UserInfo.Interests);
    
    private static DateOnly MapToDateOnly(this Date date)
        => new(date.Year, date.Month, date.Day);
    
    public static UserResponse MapToResponse(this User user)
        => new()
        {
            UserInfo = user.MapToUserInfo()
        };
    
    public static UserInfo MapToUserInfo(this User user)
        => new()
        {
            FirstName = user.FirstName,
            LastName = user.LastName,
            BirthDate = user.BirthDate.MapToDate(),
            Gender = user.Gender,
            City = user.City,
            Interests = user.Interests
        };

    private static Date MapToDate(this DateOnly date)
        => new()
        {
            Day = date.Day,
            Month = date.Month,
            Year = date.Year
        };
}
