namespace Highload.SocialNetwork;

public static class EnvChecker
{
    public static bool IsRunningInContainer()
    {
        var environmentVariable = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"); 
        if (string.IsNullOrWhiteSpace(environmentVariable))
            return false;
        return environmentVariable == "Docker";
    }
}