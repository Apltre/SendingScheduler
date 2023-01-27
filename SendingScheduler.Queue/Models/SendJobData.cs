namespace SendingScheduler.Queue.Models;
public class SendJobData
{
    public long Id { get; init; }
    public string Data { get; init; }
    public string? ResponseData { get; set; }
    public string? MetaData { get; init; }
    public DateTime CreatedOn { get; init; }
    public DateTime? ModifiedOn { get; set; }
}