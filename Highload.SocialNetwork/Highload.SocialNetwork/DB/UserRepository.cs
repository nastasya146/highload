using System.Data;
using Highload.SocialNetwork.Contracts;
using Highload.SocialNetwork.Domain;
using Microsoft.Extensions.Options;
using Npgsql;

namespace Highload.SocialNetwork.DB;

public class UserRepository(IOptions<DatabaseSettings> settings) : IUserRepository
{
    public async Task AddUser(
        User user, 
        string password, 
        bool useMaster,
        CancellationToken cancellationToken)
    {
        var connectionString = settings.Value.ConnectionString;
        if (!useMaster)
        {
            connectionString = settings.Value.ConnectionStringSlave;
        }
        
        var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString);
        
        var dataSource = dataSourceBuilder.Build();
        await using var connection = await dataSource.OpenConnectionAsync(cancellationToken);
        
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
        await dataSource.DisposeAsync();
    }
    
    public async Task<User> GetUser(Guid userId, CancellationToken cancellationToken)
    {
        var dataSourceBuilder = new NpgsqlDataSourceBuilder(settings.Value.ConnectionStringSlave);
        var dataSource = dataSourceBuilder.Build();
        await using var connection = await dataSource.OpenConnectionAsync(cancellationToken);
        
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
        var user = new User(
            reader.GetFieldValue<Guid>(0),
            reader.GetFieldValue<string>(1),
            reader.GetFieldValue<string>(2),
            reader.GetFieldValue<DateOnly>(3),
            reader.GetFieldValue<string>(4),
            reader.GetFieldValue<string>(4),
            reader.GetFieldValue<string>(5));
        
        await dataSource.DisposeAsync();
        return user;
    }

    public async Task<List<User>> Search(string firstName, string lastName, CancellationToken cancellationToken)
    {
        var dataSourceBuilder = new NpgsqlDataSourceBuilder(settings.Value.ConnectionStringSlave);
        var dataSource = dataSourceBuilder.Build();
        await using var connection = await dataSource.OpenConnectionAsync(cancellationToken);
        
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
where first_name LIKE @first_name and last_name LIKE @last_name
order by user_id;";

        await using var command = new NpgsqlCommand(Query, connection);
        command.Parameters.AddWithValue("first_name", firstName + "%");
        command.Parameters.AddWithValue("last_name", lastName + "%");

        await command.PrepareAsync(cancellationToken);
        
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);

        var results = new List<User>();
        while (await reader.ReadAsync(cancellationToken))
        {
            var gender = string.Empty;
            if (!reader.IsDBNull(4))
            { 
                gender = reader.GetFieldValue<string?>(4);
            }
            var interests = string.Empty;
            if (!reader.IsDBNull(6))
            { 
                interests = reader.GetFieldValue<string?>(6);
            }

            results.Add(
                new User(
                    reader.GetFieldValue<Guid>(0),
                    reader.GetFieldValue<string>(1),
                    reader.GetFieldValue<string>(2),
                    reader.GetFieldValue<DateOnly>(3),
                    gender,
                    reader.GetFieldValue<string>(5),
                    interests));
        }
        await dataSource.DisposeAsync();
        return results;
    }
    
    public async Task<string> GetPassword(Guid userId, CancellationToken cancellationToken)
    {
        var dataSourceBuilder = new NpgsqlDataSourceBuilder(settings.Value.ConnectionString);
        var dataSource = dataSourceBuilder.Build();
        await using var connection = await dataSource.OpenConnectionAsync(cancellationToken);
        
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
        var password = reader.GetFieldValue<string>(0);
        
        await dataSource.DisposeAsync();
        return password;
    }
}