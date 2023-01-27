using SendingScheduler.Core.Enums;

namespace SendingScheduler.Results.Models
{
    internal record OperationResultKey
    {
        public int OperationType { get; init; }
        public SendingJobStatus JobStatus {get; init;}
    }
}
