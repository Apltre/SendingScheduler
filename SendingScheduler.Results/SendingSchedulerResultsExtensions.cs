using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SendingScheduler.Core.Helpers;

namespace SendingScheduler.Results;

public static class SendingSchedulerResultsExtensions
{
    public static IServiceCollection ConfigureSendingResultsService(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        serviceCollection.AddSingleton<ExchangesNaming>();
        serviceCollection.ConfigureServiceConfig(configuration);
        serviceCollection.ConfigureRedisDistributedMemoryCache(configuration);
        serviceCollection.RegisterControllers();
        serviceCollection.InitializeMT(configuration, nameof(ResultsService));
        return serviceCollection;
    }
}
