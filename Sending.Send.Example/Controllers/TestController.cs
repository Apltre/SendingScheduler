using Sending.Core.Example.Receivers.Test.Enums;
using Sending.Core.Example.Receivers.Test.Models;
using SendingScheduler.Core.Attributes;

namespace Sending.Send.Example.Controllers;

public class TestController
{
    public TestController()
    {
    }

    [OperationType(1)]
    public async Task<object?> Send(TestMessage jobData)
    {
        return new TestResponse { ResponseId = 3453534, Data = "65476577" };
    }

    [OperationType(TestOperationTypes.SendTest2)]
    public async Task<object?> Send2(TestMessage jobData)
    {
        return new TestResponse { ResponseId = 3453534, Data = "65476577" };
    }
}
