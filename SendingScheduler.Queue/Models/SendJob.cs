namespace SendingScheduler.Queue.Models;

public class SendJob : Job
{
    public int HandleOrder { get; init; }
    public SendJobData Data { get; init; }
}