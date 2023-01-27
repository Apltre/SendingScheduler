using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SendingScheduler.Core.Helpers;
using SendingScheduler.Send.Abstractions;

namespace SendingScheduler.Send;

public static class SendingSchedulerSendExtensions
{
    public static IServiceCollection ConfigureSendingSendService(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        serviceCollection.AddSingleton<ExchangesNaming>();
        serviceCollection.ConfigureServiceConfig(configuration);
        serviceCollection.AddTransient<IJobProcessor, StandardJobProcessor>();
        serviceCollection.RegisterControllers();
        serviceCollection.ConfigureRedisDistributedMemoryCache(configuration);
        serviceCollection.InitializeMT(configuration, nameof(SendService));
        return serviceCollection;
    }
}
