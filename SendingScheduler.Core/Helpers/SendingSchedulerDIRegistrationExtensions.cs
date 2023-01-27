using MassTransitRMQExtensions;
using MassTransitRMQExtensions.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SendingScheduler.Core.Abstractions;
using SendingScheduler.Core.Models;
using SendingScheduler.Core.Services;
using StackExchange.Redis;

namespace SendingScheduler.Core.Helpers;

public static class SendingSchedulerDIRegistrationExtensions
{
    public static void ConfigureRedisDistributedMemoryCache(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        serviceCollection.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(configuration.GetConnectionString("RedisMemoryCache")));
        serviceCollection.AddTransient<IMemoryStore, RedisMemoryStore>();
    }
    public static void InitializeMT(this IServiceCollection serviceCollection, IConfiguration configuration, string controllerName)
    {
        var rabbitConfig = configuration.GetSection("Rabbit");
        var mtRmqConfig = new RabbitMqConfig(
            userName: rabbitConfig["UserName"],
            password: rabbitConfig["Password"],
            host: new Uri($"amqp://{rabbitConfig["HostName"]}:{rabbitConfig["Port"]}")
        );

        var serviceId = configuration.GetServiceId();
        serviceCollection.ConfigureMassTransitControllers(mtRmqConfig, controllerNameFilter: (cName) => cName == controllerName,
            queueNamingChanger: (queueName) => $"Sending{queueName}_{serviceId}", globalRetryPolicy: new RetryPolicyNone());
    }
    public static void ConfigureServiceConfig(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        serviceCollection.Configure<ServiceConfig>(configuration.GetSection("Service"));
    }
    public static IServiceCollection RegisterControllers(this IServiceCollection serviceCollection)
    {
        var controllers = AppDomain.CurrentDomain.GetAssemblies()
                                  .SelectMany(a => a.GetExportedTypes())
                                  .Where(t => t.Name.EndsWith("Controller"))
                                  .ToList();
        foreach (var controller in controllers)
        {
            serviceCollection.AddTransient(controller);
        }
        return serviceCollection;
    }
}