using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace SendingScheduler.Core.Helpers;

public class LogSource
{
    protected LogSource(string source)
    {
        Source = source;
    }
    public string Source { get; }
    public static readonly LogSource SendingService = new("SendingService");
    public static readonly LogSource QueueService = new("QueueService");
    public static readonly LogSource ResultsService = new("ResultsService");
}

public static class LoggingHelper
{
    public static void LogInfo(this ILogger logger, bool IsSuccessful, LogSource logSource, string message, string? jobId = null, Exception? ex = null)
    {
        var record = new
        {
            IsSuccessful,
            LogSource = logSource.Source,
            DateTime = DateTime.Now,
            Message = message,
            JobId = jobId,
            Exception = ex?.ToString()
        };
        logger.LogInformation(JsonSerializer.Serialize(record, JsonHelper.IgnoreNullsOption));
    }
}
