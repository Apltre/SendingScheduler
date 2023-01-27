using Sending.Core.Example.Receivers.Test.Enums;
using Sending.Core.Example.Receivers.Test.Models;
using SendingScheduler.Core.Attributes;
using SendingScheduler.Results.Models;

namespace Sending.Results.Example.Controllers;

public class TestController
{
    public TestController()
    {
    }
    [OperationType(1)]
    public async Task Send_HandleSuccess(JobInfo jobInfo, TestMessage sendData, TestResponse sendResponse)
    {
    }
    [OperationType(TestOperationTypes.SendTest)]
    public async Task Send_HandleInnerFailError(JobInfo jobInfo, string? sendData, string? errorData)
    {
    }
    [OperationType(1)]
    public async Task Test_HandleLogicalReceiverError(JobInfo jobInfo, string? sendData, string? errorData)
    {
    }
    [OperationType(1)]
    public async Task Test_HandleRepeatingError(JobInfo jobInfo, string? sendData, string? error)
    {
    }
}
