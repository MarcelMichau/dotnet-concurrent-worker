using ConcurrentWorker;
using Microsoft.EntityFrameworkCore;

IHost host = Host.CreateDefaultBuilder(args)
    .UseWindowsService(options =>
    {
        options.ServiceName = ".NET Concurrent Worker";
    })
    .ConfigureServices(services =>
    {
        services.AddDbContext<UserContext>(options =>
            options.UseSqlServer("Server=localhost;Database=UserDatabase;user id=SA;pwd=yourStrong(!)Password;MultipleActiveResultSets=true"));

        services.AddHostedService<CountWorker>();
        services.AddHostedService<InsertWorker>();
    })
    .Build();

CreateDbIfNotExists(host);

await host.RunAsync();

static void CreateDbIfNotExists(IHost host)
{
    using var scope = host.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<UserContext>();

    context.Database.EnsureCreated();
}
