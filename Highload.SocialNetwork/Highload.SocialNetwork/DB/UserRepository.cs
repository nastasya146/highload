using System.Data;
using Highload.SocialNetwork.Contracts;
using Highload.SocialNetwork.Domain;
using Microsoft.Extensions.Options;
using Npgsql;

namespace Highload.SocialNetwork.DB;

public class UserRepository(IOptions<DatabaseSettings> settings) : IUserRepository
{
    public async Task AddUser(User user, string password, CancellationToken cancellationToken)
    {
        var dataSourceBuilder = new NpgsqlDataSourceBuilder(settings.Value.ConnectionString);
        var dataSource = dataSourceBuilder.Build();
        var connection = await dataSource.OpenConnectionAsync(cancellationToken);
        
        const string Query = @"
insert into public.users (user_id,
                          password,
                          first_name,
                          last_name,
                          birth_date,
                          gender,
                          city,
                          interests)
    values (@user_id,
            @password,
            @first_name,
            @last_name,
            @birth_date,
            @gender,
            @city,
            @interests)";

        await using var command = new NpgsqlCommand(Query, connection);
        command.Parameters.AddWithValue("user_id", user.UserId);
        command.Parameters.AddWithValue("password", password);
        command.Parameters.AddWithValue("first_name", user.FirstName);
        command.Parameters.AddWithValue("last_name", user.LastName);
        command.Parameters.AddWithValue("birth_date", user.BirthDate);
        command.Parameters.AddWithValue("gender", user.Gender);
        command.Parameters.AddWithValue("city", user.City);
        command.Parameters.AddWithValue("interests", user.Interests); 
        
        await command.PrepareAsync(cancellationToken);
        
        await command.ExecuteNonQueryAsync(cancellationToken);
    }
    
    public async Task<User> GetUser(Guid userId, CancellationToken cancellationToken)
    {
        var dataSourceBuilder = new NpgsqlDataSourceBuilder(settings.Value.ConnectionString);
        var dataSource = dataSourceBuilder.Build();
        var connection = await dataSource.OpenConnectionAsync(cancellationToken);
        
        const string Query = @"
select 
    user_id,
    first_name,
    last_name,
    birth_date,
    gender,
    city,
    interests
from public.users
where user_id = @user_id;";

        await using var command = new NpgsqlCommand(Query, connection);
        command.Parameters.AddWithValue("user_id", userId);

        await command.PrepareAsync(cancellationToken);
        
        await using var reader = await command.ExecuteReaderAsync(CommandBehavior.SingleRow, cancellationToken);
        await reader.ReadAsync(cancellationToken);
        return new User(
            reader.GetFieldValue<Guid>(0),
            reader.GetFieldValue<string>(1),
            reader.GetFieldValue<string>(2),
            reader.GetFieldValue<DateOnly>(3),
            reader.GetFieldValue<string>(4),
            reader.GetFieldValue<string>(4),
            reader.GetFieldValue<string>(5));
    }
    
    public async Task<string> GetPassword(Guid userId, CancellationToken cancellationToken)
    {
        var dataSourceBuilder = new NpgsqlDataSourceBuilder(settings.Value.ConnectionString);
        var dataSource = dataSourceBuilder.Build();
        var connection = await dataSource.OpenConnectionAsync(cancellationToken);
        
        const string Query = @"
select 
    password
from public.users
where user_id = @user_id;";

        await using var command = new NpgsqlCommand(Query, connection);
        command.Parameters.AddWithValue("user_id", userId);

        await command.PrepareAsync(cancellationToken);
        
        await using var reader = await command.ExecuteReaderAsync(CommandBehavior.SingleRow, cancellationToken);
        await reader.ReadAsync(cancellationToken);
        return reader.GetFieldValue<string>(0);
    }
}