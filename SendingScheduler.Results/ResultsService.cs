using MassTransit;
using MassTransitRMQExtensions.Attributes.ConsumerAttributes;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SendingScheduler.Core.Abstractions;
using SendingScheduler.Core.Attributes;
using SendingScheduler.Core.Enums;
using SendingScheduler.Core.Exceptions;
using SendingScheduler.Core.Helpers;
using SendingScheduler.Core.Messages;
using SendingScheduler.Core.Models;
using SendingScheduler.Results.Models;
using System.Collections.Concurrent;
using System.Reflection;
using System.Text.Json;

namespace SendingScheduler.Results;

public class ResultsService
{
    protected IServiceProvider ServiceProvider { get; }
    protected ServiceConfig ServiceConfig { get; }
    protected ISendEndpointProvider SendEndpointProvider { get; }
    protected ExchangesNaming ExchangesNaming { get; }
    protected IMemoryStore MemoryStore { get; }
    private static ConcurrentDictionary<OperationResultKey, MethodInfo?> _operationTypeHandlerTypesCache { get; } = new();

    public ResultsService(IServiceProvider serviceProvider,
                                 IOptions<ServiceConfig> serviceConfig,
                                 ISendEndpointProvider sendEndpointProvider,
                                 ExchangesNaming exchangesNaming,
                                 IMemoryStore memoryStore)
    {
        ServiceProvider = serviceProvider;
        ServiceConfig = serviceConfig.Value;
        SendEndpointProvider = sendEndpointProvider;
        ExchangesNaming = exchangesNaming;
        MemoryStore = memoryStore;
    }

    [SubscribeEndpoint()]
    public async Task<object> Handle(ResultHandlingJobMessage jobEvent)
    {
        var status = SendingJobStatus.FinishedSuccessfully;
        object? errorData = null;

        try
        {
            await ProcessJob(jobEvent);
        }
        catch (TemporaryErrorException ex)
        {
            status = jobEvent.AttemptNumber < ServiceConfig.RetriesMaximum ? SendingJobStatus.FinishedWithTemporaryReceiverError : SendingJobStatus.InnerFail;
            errorData = new { Exception = ex.ToString() };
        }
        catch (Exception ex)
        {
            status = SendingJobStatus.InnerFail;
            errorData = new { Exception = ex.ToString() };
        }

        var endpoint = await SendEndpointProvider.GetSendEndpoint(new Uri($"exchange:{ExchangesNaming.GetResultsToQueue(ServiceConfig.Id)}"));
        await endpoint.Send(jobEvent with
        {
            ProcessedOn = DateTime.Now,
            ErrorData = errorData is not null ? JsonSerializer.SerializeToElement(errorData) : null,
            Status = status
        });

        return new
        {
            Status = status,
            ErrorData = errorData
        };
    }
    internal async Task<object?> GetResponseData(long sendJobId, Type type)
    {
        return JsonHelper.GetControllerDeserializedParameter(await MemoryStore.GetValue(sendJobId.ResponseDataKey()), type);
    }
    internal async Task<object?> GetSendJobData(long sendJobId, Type type)
    {
        return JsonHelper.GetControllerDeserializedParameter(await MemoryStore.GetValue(sendJobId.SendDataKey()), type);
    }
    internal async Task<object?> GetParameterDataByParameterInfo(long sendJobId, JsonElement? sendJobError, ParameterInfo parameter)
    {
        return parameter.Name switch
        {
            "sendData" => await GetSendJobData(sendJobId, parameter.ParameterType),
            "sendResponse" => await GetResponseData(sendJobId, parameter.ParameterType),
            "errorData" => JsonHelper.GetControllerDeserializedParameter(sendJobError, parameter.ParameterType),
            _ => throw new Exception("Unexpected result handler parameter name")
        };
    }
    internal string ResultMethodSuffix(SendingJobStatus jobStatus)
    {
        return jobStatus switch
        {
            SendingJobStatus.FinishedSuccessfully => $"HandleSuccess",
            SendingJobStatus.FinishedWithLogicalReceiverError => $"HandleLogicalReceiverError",
            SendingJobStatus.FinishedWithTemporaryReceiverError => $"HandleRepeatingError",
            SendingJobStatus.InnerFail => $"HandleInnerFailError",
            _ => throw new InvalidOperationException("Недопустимый статус"),
        };
    }
    internal MethodInfo? GetOperationTypeKeyHandler(OperationResultKey key)
    {
        return _operationTypeHandlerTypesCache.GetOrAdd(key, key => {
            return Assembly.GetEntryAssembly()
                .GetTypes()
                .SelectMany(t => t.GetMethods())
                .Where(m => m.GetCustomAttribute<OperationType>()?.Type == key.OperationType 
                    && m.Name.EndsWith(ResultMethodSuffix(key.JobStatus))
                    && m.ReturnType.GetMethod("GetAwaiter") is not null)
                .SingleOrDefault();
        });
    }
    internal async Task ProcessJob(ResultHandlingJobMessage job)
    {
        var method = GetOperationTypeKeyHandler(new OperationResultKey { JobStatus = job.SendJobStatus, OperationType = job.Type});

        if (method == null)
        {
            return;
        }

        var parametersList = new List<object>
        {
            new JobInfo
            {
                Id = job.JobId,
                SendJobId = job.SendJobId,
                ResultAttemptNumber = job.AttemptNumber
            }
        };

        var parameters = method.GetParameters().Skip(1).ToList();

        if (parameters.Count > 2)
        {
            throw new Exception("Метод обработки результата имеет больше трех аргументов.");
        }

        foreach (var parameter in parameters)
        {
            parametersList.Add(await GetParameterDataByParameterInfo(job.SendJobId, job.SendJobErrorData, parameter));
        }

        var controller = ServiceProvider.GetRequiredService(method.ReflectedType);

        try
        {
            await (Task)method.Invoke(controller, parametersList.ToArray());
        }
        catch (Exception ex)
        {
            throw new TemporaryErrorException(ex.ToString());
        }
    }
}
