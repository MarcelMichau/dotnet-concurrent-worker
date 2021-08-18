using Bogus;
using Bogus.Extensions;
using Bogus.Extensions.UnitedStates;
using System.Diagnostics;

namespace ConcurrentWorker;

public class InsertWorker : BackgroundService
{
    private readonly ILogger<CountWorker> _logger;
    private readonly IServiceProvider _serviceProvider;

    public InsertWorker(ILogger<CountWorker> logger, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("InsertWorker running at: {time}", DateTimeOffset.Now);

            try
            {
                await Parallel.ForEachAsync(new List<int> { 10000, 10000, 10000 }, stoppingToken, async (recordCount, cancellationToken) =>
                {
                    await DoInserts(recordCount, cancellationToken);
                });
            }
            catch (OperationCanceledException)
            {
                return;
            }
        }
    }

    private async Task<IServiceScope> DoInserts(int recordCount, CancellationToken stoppingToken)
    {
        _logger.LogInformation("DoInserts executed at: {time}, Thread ID: {threadId}", DateTimeOffset.Now, Environment.CurrentManagedThreadId);

        var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<UserContext>();

        var stopwatch = new Stopwatch();
        stopwatch.Start();

        var testUsers = GenerateUsers(recordCount);

        var generationTime = stopwatch.Elapsed;
        _logger.LogInformation("Generation time ({recordCount}): {generationTime} ms", recordCount, generationTime.TotalMilliseconds);

        stopwatch.Restart();

        context.Users!.AddRange(testUsers);
        await context.SaveChangesAsync(stoppingToken);

        var insertTime = stopwatch.Elapsed;
        _logger.LogInformation("Insert time ({recordCount}): {insertTime} ms", recordCount, insertTime.TotalMilliseconds);

        await Task.Delay(10000, stoppingToken);
        return scope;
    }

    private static List<User> GenerateUsers(int count)
    {
        var fruit = new[] { "apple", "banana", "orange", "strawberry", "kiwi" };

        var testOrders = new Faker<Order>()
            .RuleFor(o => o.Item, f => f.PickRandom(fruit))
            .RuleFor(o => o.Quantity, f => f.Random.Number(1, 10))
            .RuleFor(o => o.LotNumber, f => f.Random.Int(0, 100).OrNull(f, .8f));

        var userGenerator = new Faker<User>()
            .RuleFor(u => u.FirstName, f => f.Name.FirstName())
            .RuleFor(u => u.LastName, f => f.Name.LastName())
            .RuleFor(u => u.Avatar, f => f.Internet.Avatar())
            .RuleFor(u => u.UserName, (f, u) => f.Internet.UserName(u.FirstName, u.LastName))
            .RuleFor(u => u.Email, (f, u) => f.Internet.Email(u.FirstName, u.LastName))
            .RuleFor(u => u.SomethingUnique, f => $"Value {f.UniqueIndex}")
            .RuleFor(u => u.SomeGuid, Guid.NewGuid)
            .RuleFor(u => u.SSN, f => f.Person.Ssn())
            .RuleFor(u => u.Gender, f => f.PickRandom<Gender>())
            .RuleFor(u => u.CartId, f => Guid.NewGuid())
            .RuleFor(u => u.FullName, (f, u) => u.FirstName + " " + u.LastName)
            .RuleFor(u => u.Orders, f => testOrders.Generate(3));

        return userGenerator.Generate(count);
    }
}
