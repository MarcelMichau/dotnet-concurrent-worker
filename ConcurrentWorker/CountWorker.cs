
using Microsoft.EntityFrameworkCore;

namespace ConcurrentWorker;
public class CountWorker : BackgroundService
{
    private readonly ILogger<CountWorker> _logger;
    private readonly IServiceProvider _serviceProvider;

    public CountWorker(ILogger<CountWorker> logger, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("CountWorker running at: {time}", DateTimeOffset.Now);

            try
            {
                using var scope = _serviceProvider.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<UserContext>();

                var userCount = await context.Users!.CountAsync(stoppingToken);
                _logger.LogInformation("Total number of users in DB: {userCount}", userCount);

                var orderCount = await context.Orders!.CountAsync(stoppingToken);
                _logger.LogInformation("Total number of orders in DB: {orderCount}", orderCount);

                await Task.Delay(10000, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                return;
            }
        }
    }
}
