
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ConcurrentWorker;
public class CountWorker(ILogger<CountWorker> logger, IServiceProvider serviceProvider)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            logger.LogInformation("CountWorker running at: {time}", DateTimeOffset.Now);

            try
            {
                using var scope = serviceProvider.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<UserContext>();

                var userCount = await context.Users!.CountAsync(stoppingToken);
                logger.LogInformation("Total number of users in DB: {userCount}", userCount);

                var orderCount = await context.Orders!.CountAsync(stoppingToken);
                logger.LogInformation("Total number of orders in DB: {orderCount}", orderCount);

                await Task.Delay(10000, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                return;
            }
        }
    }
}
