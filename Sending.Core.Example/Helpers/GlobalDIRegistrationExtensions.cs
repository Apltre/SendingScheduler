using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;

namespace Sending.Core.Example.Helpers;

public static class GlobalDIRegistrationExtensions
{
    public static void ConfigureLogging(this ILoggingBuilder builder, IConfiguration configuration)
    {
        builder.ClearProviders();
        builder.AddNLog(new NLogLoggingConfiguration(configuration.GetSection("NLog")));
    }
    public static void AddSettingFiles(this IConfigurationBuilder config, string environment, string receiver)
    {
        var path = AppDomain.CurrentDomain.BaseDirectory;
        config.AddJsonFile($"{path}/Settings/appsettings.json");
        config.AddJsonFile($"{path}/Settings/appsettings.{environment}.json", optional: true);

        var receiverEnv = environment == "Production" ? string.Empty : $".{environment}";
        config.AddJsonFile($"{path}/Settings/appsettings.{receiver}{receiverEnv}.json");
    }
    public static IHostBuilder ConfigureSettingFiles(this IHostBuilder hostBuilder)
    {
        return hostBuilder.ConfigureAppConfiguration((context, config) =>
        {
            var env = context.HostingEnvironment;
            var receiver = context.Configuration["receiver"];
            config.AddSettingFiles(env.EnvironmentName, receiver);
        });
    }
}
