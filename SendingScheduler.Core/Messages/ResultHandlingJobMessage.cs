using SendingScheduler.Core.Enums;
using System.Text.Json;

namespace SendingScheduler.Core.Messages;

public record ResultHandlingJobMessage : JobMessage
{
    public long SendJobId { get; set; }
    public SendingJobStatus SendJobStatus { get; init; }
    public JsonElement? SendJobErrorData { get; init; }
}
