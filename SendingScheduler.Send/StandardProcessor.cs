using Microsoft.Extensions.DependencyInjection;
using SendingScheduler.Core.Abstractions;
using SendingScheduler.Core.Attributes;
using SendingScheduler.Core.Enums;
using SendingScheduler.Core.Exceptions;
using SendingScheduler.Core.Helpers;
using SendingScheduler.Core.Messages;
using SendingScheduler.Send.Abstractions;
using System.Collections.Concurrent;
using System.Reflection;
using System.Text.Json;

namespace SendingScheduler.Send;

internal class StandardJobProcessor : IJobProcessor
{
    protected IServiceProvider ServiceProvider { get; }
    protected IMemoryStore MemoryStore { get; }
    protected static ConcurrentDictionary<int, MethodInfo> OperationTypeHandlerTypesCache { get; } = new();

    public StandardJobProcessor(IServiceProvider serviceProvider, IMemoryStore memoryStore)
    {
        ServiceProvider = serviceProvider;
        MemoryStore = memoryStore;
    }

    public async Task<SendJobMessage> Process(SendJobMessage job)
    {
        try
        {
            var result = await RunAsync(job);
            if (result is not null)
            {
                await MemoryStore.SetValue(job.JobId.ResponseDataKey(), result);
            }

            job = job with
            {
                Status = SendingJobStatus.FinishedSuccessfully
            };
        }
        catch (LogicalErrorException knownEx)
        {
            job = job with
            {
                Status = SendingJobStatus.FinishedWithLogicalReceiverError,
                ErrorData = JsonSerializer.SerializeToElement(knownEx.Data, JsonHelper.IgnoreNullsOption)
            };
        }
        catch (TemporaryErrorException tempEx)
        {
            job = job with
            {
                Status = SendingJobStatus.FinishedWithTemporaryReceiverError,
                ErrorData = JsonSerializer.SerializeToElement(tempEx.Data, JsonHelper.IgnoreNullsOption)
            };
        }
        catch (Exception ex)
        {
            job = job with
            {
                Status = SendingJobStatus.InnerFail,
                ErrorData = JsonSerializer.SerializeToElement(new { Exception = ex.ToString() }, JsonHelper.IgnoreNullsOption)
            };
        }

        return job with { ProcessedOn = DateTime.Now };
    }
    public MethodInfo GetOperationTypeHandler(int operationType)
    {
        return OperationTypeHandlerTypesCache.GetOrAdd(operationType, operationType =>
        {
            var handler = Assembly.GetEntryAssembly()
                .GetTypes()
                .SelectMany(t => t.GetMethods())
                .Where(m => m.GetCustomAttribute<OperationType>()?.Type == operationType
                    && m.ReturnType.GetMethod("GetAwaiter") is not null)
                .SingleOrDefault();

            if (handler is null)
            {
                throw new InvalidOperationException($"Cant find handler for operationType {operationType}");
            }

            if (handler.ReturnType.GetMethod("GetAwaiter") is null)
            {
                throw new Exception($"Controller method for operationType {operationType} must be of awaitable return type!");
            }

            if (handler.DeclaringType is null)
            {
                throw new InvalidOperationException($"Can't find controller for operationType {operationType}");
            }

            return handler;
        });
    }
    public async Task<object?> RunAsync(SendJobMessage job)
    {
        var handler = GetOperationTypeHandler(job.Type);
        var handlerControllerType = handler.DeclaringType;
        var argType = handler.GetParameters().Single().ParameterType;

        using var scope = ServiceProvider.CreateScope();
        var controller = scope.ServiceProvider.GetRequiredService(handlerControllerType);
        var arg = JsonHelper.GetControllerDeserializedParameter(await MemoryStore.GetValue(job.JobId.SendDataKey()), argType);
        return await (Task<object?>)handler.Invoke(controller, new object[] { arg });
    }
}