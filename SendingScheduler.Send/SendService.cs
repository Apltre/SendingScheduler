using MassTransit;
using MassTransitRMQExtensions.Attributes.ConsumerAttributes;
using Microsoft.Extensions.Options;
using SendingScheduler.Core.Abstractions;
using SendingScheduler.Core.Enums;
using SendingScheduler.Core.Helpers;
using SendingScheduler.Core.Messages;
using SendingScheduler.Core.Models;
using SendingScheduler.Send.Abstractions;
using System.Text.Json;

namespace SendingScheduler.Send;

public class SendService
{
    protected IJobProcessor JobProcessor { get; }
    protected ServiceConfig ServiceConfig { get; }
    protected ExchangesNaming ExchangesNaming { get; }
    protected ISendEndpointProvider SendEndpointProvider { get; }
    protected IMemoryStore MemoryStore { get; }

    public SendService(IJobProcessor jobProcessor,
                       IOptions<ServiceConfig> serviceConfig,
                       ISendEndpointProvider sendEndpointProvider,
                       ExchangesNaming exchangesNaming,
                       IMemoryStore memoryStore)
    {
        JobProcessor = jobProcessor;
        ServiceConfig = serviceConfig.Value;
        ExchangesNaming = exchangesNaming;
        MemoryStore = memoryStore;
        SendEndpointProvider = sendEndpointProvider;
    }

    [SubscribeEndpoint()]
    public async Task<object> Handle(SendJobMessage jobEvent)
    {
        object? errorData = null;
        try
        {
            jobEvent = await JobProcessor.Process(jobEvent);
        }
        catch (Exception ex)
        {
            errorData = new { Exception = ex.ToString() };
            jobEvent = jobEvent with
            {
                Status = SendingJobStatus.InnerFail,
                ErrorData = JsonSerializer.SerializeToElement(errorData, JsonHelper.IgnoreNullsOption)
            };
        }
        var endpoint = await SendEndpointProvider.GetSendEndpoint(new Uri($"exchange:{ExchangesNaming.GetSendToQueue(ServiceConfig.Id)}"));
        await endpoint.Send(jobEvent);
        return new
        {
            jobEvent.Status,
            ErrorData = errorData
        };
    }
}