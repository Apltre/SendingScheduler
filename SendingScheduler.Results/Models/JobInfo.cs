namespace SendingScheduler.Results.Models;
public record JobInfo
{
    public long SendJobId { get; init; }
    public int ResultAttemptNumber { get; init; }
    public long Id { get; internal set; }
}
