using MassTransit;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SendingScheduler.Core.Abstractions;
using SendingScheduler.Core.Helpers;
using SendingScheduler.Core.Messages;
using SendingScheduler.Core.Models;
using SendingScheduler.Queue.Abstractions;
using SendingScheduler.Queue.Models;

namespace SendingScheduler.Queue;

internal class SendPendingService : PendingService<SendJob, SendJobMessage>
{
    public SendPendingService(
     IOptions<ServiceConfig> serviceConfig,
     ILogger<PendingService<SendJob, SendJobMessage>> logger,
     IMemoryStore memoryStore,
     ExchangesNaming exchangesNaming,
     IServiceProvider serviceProvider,
     ISendEndpointProvider sendEndpointProvider) : base(serviceConfig.Value, serviceProvider, logger, exchangesNaming, sendEndpointProvider)
    {
        MemoryStore = memoryStore;
    }

    public IMemoryStore MemoryStore { get; }

    protected async override Task<IEnumerable<SendJob>> GetPending(IJobsStore jobsStore, int batchSize)
    {
        var jobs = await jobsStore.GetPendingSendJobs(batchSize);

        var sendJobsIdsWithoutCachedData = (await Task.WhenAll(jobs.Select(async j => (id: j.Id, isCached: await MemoryStore.RefreshTtl(j.Id.SendDataKey())))))
            .Where(j => !j.isCached).Select(j => j.id).ToList();

        var jobDatasDictionary = (await jobsStore.GetSendJobsData(sendJobsIdsWithoutCachedData)).ToDictionary(k => k.id, v => v.data);

        foreach (var job in jobs)
        {
            if (jobDatasDictionary.TryGetValue(job.Id, out var data))
            {
                await MemoryStore.SetStringValue(job.Id.SendDataKey(), data);
            }
        }

        return jobs;
    }
    protected override SendJobMessage MapMessage(SendJob job)
    {
        return new SendJobMessage
        {
            JobId = job.Id,
            Status = job.Status,
            Type = job.Type,
            AttemptNumber = job.AttemptNumber
        };
    }

    protected override string SendExchangeName()
    {
        return ExchangesNaming.GetQueueToSend(ServiceConfig.Id);
    }
}