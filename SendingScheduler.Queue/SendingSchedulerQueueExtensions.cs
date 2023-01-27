using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SendingScheduler.Core.Helpers;

namespace SendingScheduler.Queue;

public static class SendingSchedulerQueueExtensions
{
    public static IServiceCollection ConfigureSendingQueueService(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        serviceCollection.AddSingleton<ExchangesNaming>();
        serviceCollection.ConfigureServiceConfig(configuration);
        serviceCollection.ConfigureRedisDistributedMemoryCache(configuration);
        serviceCollection.InitializeMT(configuration, nameof(QueueService));
        serviceCollection.AddHostedService<SendPendingService>();
        serviceCollection.AddHostedService<ResultHandlingPendingService>();
        return serviceCollection;
    }
}
