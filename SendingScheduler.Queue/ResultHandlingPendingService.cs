using MassTransit;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SendingScheduler.Core.Abstractions;
using SendingScheduler.Core.Helpers;
using SendingScheduler.Core.Messages;
using SendingScheduler.Core.Models;
using SendingScheduler.Queue.Abstractions;
using SendingScheduler.Queue.Models;
using System.Text.Json;

namespace SendingScheduler.Queue;

internal class ResultHandlingPendingService : PendingService<ResultHandlingJob, ResultHandlingJobMessage>
{
    public ResultHandlingPendingService(
    IOptions<ServiceConfig> serviceConfig,
    ILogger<PendingService<ResultHandlingJob, ResultHandlingJobMessage>> logger,
    IMemoryStore memoryStore,
    IServiceProvider serviceProvider,
    ExchangesNaming exchangesNaming,
    ISendEndpointProvider sendEndpointProvider) : base(serviceConfig.Value, serviceProvider, logger, exchangesNaming, sendEndpointProvider)
    {
        MemoryStore = memoryStore;
    }

    public IMemoryStore MemoryStore { get; }

    protected async Task RefreshOrSetJobData(IJobsStore jobsStore, List<ResultHandlingJob> jobs, Func<long, string> keyGenerator)
    {
        var sendJobsIdsWithoutCachedData = (await Task.WhenAll(jobs.Select(async j => (id: j.Id, isCached: await MemoryStore.RefreshTtl(keyGenerator(j.SendJobId))))))
            .Where(j => !j.isCached).Select(j => j.id).ToList();

        var jobDataDictionary = (await jobsStore.GetSendJobsResponse(sendJobsIdsWithoutCachedData)).ToDictionary(k => k.id, v => v.data);

        foreach (var job in jobs)
        {
            if (jobDataDictionary.TryGetValue(job.SendJobId, out var data))
            {
                await MemoryStore.SetStringValue(keyGenerator(job.SendJobId), data);
            }
        }
    }
    protected async override Task<IEnumerable<ResultHandlingJob>> GetPending(IJobsStore jobsStore, int batchSize)
    {
        var jobs = await jobsStore.GetPendingResultHandlingJobs(batchSize);

        await RefreshOrSetJobData(jobsStore, jobs, (id) => id.SendDataKey());
        await RefreshOrSetJobData(jobsStore, jobs, (id) => id.ResponseDataKey());

        return jobs;
    }
    protected override ResultHandlingJobMessage MapMessage(ResultHandlingJob job)
    {
        return new ResultHandlingJobMessage
        {
            JobId = job.Id,
            Status = job.Status,
            Type = job.Type,
            AttemptNumber = job.AttemptNumber,
            SendJobStatus = job.SendJobStatus,
            SendJobId = job.SendJobId,
            SendJobErrorData = job.SendJobErrorData is not null ? JsonSerializer.SerializeToElement(job.SendJobErrorData) : null
        };
    }
    protected override string SendExchangeName()
    {
        return ExchangesNaming.GetQueueToResults(ServiceConfig.Id);
    }
}