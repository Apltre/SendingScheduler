using SendingScheduler.Core.Abstractions;

namespace SendingScheduler.Core.Helpers;

public class ExchangesNaming
{
    public string GetResultsToQueue(int serviceId) => $"SendingQueueService_HandleResult_{serviceId}";
    public string GetSendToQueue(int serverId) => $"SendingQueueService_HandleSending_{serverId}";
    public string GetQueueToSend(int serverId) => $"SendingSendService_Handle_{serverId}";
    public string GetQueueToResults(int serverId) => $"SendingResultsService_Handle_{serverId}";
}
