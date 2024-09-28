namespace Highload.SocialNetwork.DB;

public class DatabaseSettings
{
    public string ConnectionString { get; set; } = string.Empty;
    
    public string ConnectionStringSlave { get; set; } = string.Empty;
}

public static class ReplicaSettings
{
    public static bool IsConnectionStringMaster { get; set; } = true;
}