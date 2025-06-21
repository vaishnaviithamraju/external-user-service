using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Caching.Memory;
using UserApiIntegration.Core;
using System.Buffers.Text;

var builder = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((hostingContext, config) =>
    {
        config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
    })
    .ConfigureServices((context, services) =>
    {
        var baseUrl = context.Configuration["UserApi:BaseUrl"] ?? "https://reqres.in/api/";
        services.AddMemoryCache();
        services.AddHttpClient<IApiClient,ApiClient>(client =>
        {
            client.BaseAddress = new Uri(baseUrl);
        });
        services.AddTransient<ExternalUserService>();
    });

var host = builder.Build();

using var scope = host.Services.CreateScope();

var service = scope.ServiceProvider.GetRequiredService<ExternalUserService>();

Console.WriteLine("Fetching all users...");
var users = await service.GetAllUsersAsync();
foreach (var user in users)
{
    Console.WriteLine($"{user.Id}: {user.FirstName} {user.LastName} - {user.Email}");
}

Console.WriteLine("\nFetching user with ID 2...");
var user2 = await service.GetUserByIdAsync(2);
if (user2 != null)
{
    Console.WriteLine($"Found: {user2.FirstName} {user2.LastName} - {user2.Email}");
}
else
{
    Console.WriteLine("User not found.");
}
Console.ReadLine();
