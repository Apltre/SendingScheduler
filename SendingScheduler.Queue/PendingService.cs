using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SendingScheduler.Core.Enums;
using SendingScheduler.Core.Helpers;
using SendingScheduler.Core.Messages;
using SendingScheduler.Core.Models;
using SendingScheduler.Queue.Abstractions;
using SendingScheduler.Queue.Models;
using System.Collections.Concurrent;

namespace SendingScheduler.Queue;

internal abstract class PendingService<T, M> : IHostedService
    where T : Job
    where M : JobMessage
{
    public ISendEndpointProvider SendEndpointProvider { get; }
    public ServiceConfig ServiceConfig { get; }
    private IServiceProvider ServiceProvider { get; }
    public ILogger<PendingService<T, M>> Logger { get; }
    public ExchangesNaming ExchangesNaming { get; }
    protected ConcurrentQueue<M> MessagesPushQueue { get; } = new ConcurrentQueue<M>();

    public PendingService(
        ServiceConfig serviceConfig,
        IServiceProvider serviceProvider,
        ILogger<PendingService<T, M>> logger,
        ExchangesNaming exchangesNaming,
        ISendEndpointProvider sendEndpointProvider)
    {
        ServiceConfig = serviceConfig;
        ServiceProvider = serviceProvider;
        Logger = logger;
        ExchangesNaming = exchangesNaming;
        SendEndpointProvider = sendEndpointProvider;
    }

    protected abstract string SendExchangeName();

    protected async Task MessagesSender(CancellationToken cancellationToken)
    {
        var list = new List<M>();
        while (!cancellationToken.IsCancellationRequested)
        {
            var sendListIndex = 0;
            try
            {
                if (MessagesPushQueue.Count == 0)
                {
                    await Task.Delay(50);
                    continue;
                }

                for (int i = 0; i < MessagesPushQueue.Count; i++)
                {
                    if (MessagesPushQueue.TryDequeue(out var messageToSend))
                    {
                        list.Add(messageToSend);
                    }
                }

                var sendEndpoint = await SendEndpointProvider.GetSendEndpoint(new Uri($"exchange:{SendExchangeName()}"));

                for (sendListIndex = 0; sendListIndex < list.Count; sendListIndex++)
                {
                    await sendEndpoint.Send(list[sendListIndex]);
                }
                list.Clear();
            }
            catch (Exception ex)
            {
                for (; sendListIndex < list.Count; sendListIndex++)
                {
                    MessagesPushQueue.Enqueue(list[sendListIndex]);
                }
                Logger.LogInfo(false, LogSource.QueueService, "Message rmq queueing fail.", ex: ex);
                await Task.Delay(1000);
            }
        }
    }
    private async Task RandomWait(int minMilliseconds, int maxMilliseconds, CancellationToken cancellation)
    {
        var rand = new Random();
        var randomDelay = rand.Next(minMilliseconds, maxMilliseconds);
        await Task.Delay(randomDelay, cancellation);
    }
    protected abstract Task<IEnumerable<T>> GetPending(IJobsStore jobsStore, int batchSize);
    protected abstract M MapMessage(T job);
    public async Task StartAsync(CancellationToken cancellationToken)
    {

        _ = Task.Run(async () => await MessagesSender(cancellationToken));
        _ = Task.Run(async () =>
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var messagesPushCount = MessagesPushQueue.Count;
                try
                {
                    if (MessagesPushQueue.Count > ServiceConfig.QueryBatchSize * 3)
                    {
                        await RandomWait(1000, 1200, cancellationToken);
                        continue;
                    }

                    using var scope = ServiceProvider.CreateScope();
                    var jobsStore = scope.ServiceProvider.GetRequiredService<IJobsStore>();
                    var pending = await GetPending(jobsStore, ServiceConfig!.QueryBatchSize!.Value);

                    if (pending.Count() == 0)
                    {
                        await RandomWait(1000, 1200, cancellationToken);
                        continue;
                    }

                    foreach (var job in pending)
                    {
                        job.Status = SendingJobStatus.BeingProcessed;
                        MessagesPushQueue.Enqueue(MapMessage(job));
                    }
                    await jobsStore.Commit();

                }
                catch (Exception ex)
                {
                    Logger.LogInfo(false, LogSource.QueueService, $"Error occurred while quering objects of type {nameof(T)}.", ex: ex);
                    await RandomWait(1500, 2500, cancellationToken);
                }
            }
        });
    }
    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
