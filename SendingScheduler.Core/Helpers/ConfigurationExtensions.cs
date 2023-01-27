using Microsoft.Extensions.Configuration;

namespace SendingScheduler.Core.Helpers;

public static class ConfigurationExtensions
{
    public static int GetServiceId(this IConfiguration configuration)
    {
        return Int32.Parse(configuration["Service:Id"]);
    }

    public static string GetReceiver(this IConfiguration configuration)
    {
        return configuration["receiver"];
    }
}
