using SendingScheduler.Core.Messages;

namespace SendingScheduler.Send.Abstractions;

public interface IJobProcessor
{
    Task<SendJobMessage> Process(SendJobMessage job);
}
