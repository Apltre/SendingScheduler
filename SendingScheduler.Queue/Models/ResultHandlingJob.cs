using SendingScheduler.Core.Enums;

namespace SendingScheduler.Queue.Models;

public class ResultHandlingJob : Job
{
    public long SendJobId { get; set; }
    public SendingJobStatus SendJobStatus { get; set; }
    public SendJob SendJob { get; set; }
    public string? SendJobErrorData { get; set; }
    public string? ErrorData { get; set; }
}