using MassTransitRMQExtensions.Attributes.ConsumerAttributes;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SendingScheduler.Core.Abstractions;
using SendingScheduler.Core.Enums;
using SendingScheduler.Core.Helpers;
using SendingScheduler.Core.Messages;
using SendingScheduler.Core.Models;
using SendingScheduler.Queue.Abstractions;
using SendingScheduler.Queue.Models;
using System.Text.Json;

namespace SendingScheduler.Queue;

public class QueueService
{
    protected int ServiceId { get; }
    protected IJobsStore JobsStore { get; }
    protected IMemoryStore MemoryStore { get; }
    protected ILogger<QueueService> Logger { get; }

    public QueueService(
        IOptions<ServiceConfig> serviceConfig,
        IJobsStore jobsStore,
        IMemoryStore memoryStore,
        ILogger<QueueService> logger)
    {
        JobsStore = jobsStore;
        MemoryStore = memoryStore;
        ServiceId = serviceConfig.Value.Id;
        Logger = logger;
    }

    [SubscribeEndpoint()]
    public async Task HandleSending(SendJobMessage jobEvent)
    {
        var job = await JobsStore.FindSendJob(jobEvent.JobId);

        await JobsStore.AddNewResultHandlingJobWithoutCommit(new ResultHandlingJob
        {
            SendJobId = jobEvent.JobId,
            Type = jobEvent.Type,
            Status = SendingJobStatus.Pending,
            SendJobStatus = jobEvent.Status,
            SendJobErrorData = JsonSerializer.Serialize(jobEvent.ErrorData, JsonHelper.IgnoreNullsOption),
            StartTime = DateTime.Now,
            CreatedOn = DateTime.Now
        });

        job.Status = jobEvent.Status;

        switch (jobEvent.Status)
        {
            case SendingJobStatus.FinishedWithTemporaryReceiverError:
                job.AttemptNumber++;
                job.StartTime = DateTime.Now.Add(Reprocessing.GetDelay(jobEvent.AttemptNumber));
                job.Status = SendingJobStatus.Pending;
                break;
            case SendingJobStatus.FinishedSuccessfully:
                var jobData = await MemoryStore.GetValue(jobEvent.JobId.ResponseDataKey());
                await JobsStore.UpdateJobDataResponseWithoutCommit(jobEvent.JobId, jobData);
                break;
            default:
                break;
        }

        await JobsStore.Commit();
    }

    [SubscribeEndpoint()]
    public async Task HandleResult(ResultHandlingJobMessage jobEvent)
    {
        var job = await JobsStore.FindResultHandlingJob(jobEvent.JobId);
        job.ProcessedOn = jobEvent.ProcessedOn;
        job.Status = jobEvent.Status;

        switch (jobEvent.Status)
        {
            case SendingJobStatus.FinishedWithTemporaryReceiverError:
                job.AttemptNumber++;
                job.StartTime = DateTime.Now.Add(Reprocessing.GetDelay(jobEvent.AttemptNumber));
                job.Status = SendingJobStatus.Pending;
                break;
            default:
                job.ErrorData = JsonSerializer.Serialize(jobEvent.ErrorData, JsonHelper.IgnoreNullsOption);
                break;
        }

        await JobsStore.Commit();

        if (jobEvent.Status != SendingJobStatus.FinishedWithTemporaryReceiverError)
        {
            switch (jobEvent.SendJobStatus)
            {
                case SendingJobStatus.FinishedWithTemporaryReceiverError:
                    break;
                default:
                    await MemoryStore.Remove(jobEvent.SendJobId.ResponseDataKey());
                    await MemoryStore.Remove(jobEvent.SendJobId.SendDataKey());
                    break;
            }
        }
    }
}