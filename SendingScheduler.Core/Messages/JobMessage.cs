using MassTransit;
using System.Text.Json;
using SendingScheduler.Core.Enums;

namespace SendingScheduler.Core.Messages;

[ExcludeFromTopology]
public abstract record JobMessage
{
    public string MessageId { get; } = Guid.NewGuid().ToString();
    public long JobId { get; init; }
    public int Type { get; init; }
    public SendingJobStatus Status { get; init; }
    public DateTime? ProcessedOn { get; init; }
    public int AttemptNumber { get; init; }
    public JsonElement? ErrorData { get; init; }
}
