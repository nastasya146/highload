using Highload.SocialNetwork.Contracts;
using Highload.SocialNetwork.DB;
using Highload.SocialNetwork.GRPC;
using Highload.SocialNetwork.Services;

var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").AddEnvironmentVariables().Build();
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddGrpc();
builder.Services.Configure<DatabaseSettings>(config.GetSection("DatabaseSettings"));
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IPasswordService, PasswordService>();

var app = builder.Build();

//app.UseAuthentication();
//app.UseAuthorization();

app.MapGrpcService<UserService>();

app.Run();
