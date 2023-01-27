using SendingScheduler.Core.Enums;

namespace SendingScheduler.Queue.Models;

public abstract class Job
{
    public long Id { get; init; }
    public int Type { get; init; }
    public SendingJobStatus Status { get; set; }
    public int AttemptNumber { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime CreatedOn { get; init; }
    public DateTime? ProcessedOn { get; set; }
    public int ServiceId { get; init; }
}
