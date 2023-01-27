namespace SendingScheduler.Core.Models;

public record ServiceConfig
{
    public int Id { get; init; }
    public int? QueryBatchSize { get; init; }
    public int? RetriesMaximum { get; init; }
    public int? MemoryCacheSlidingInMinutes { get; init; }
}
