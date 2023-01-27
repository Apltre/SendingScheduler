using SendingScheduler.Queue.Models;

namespace SendingScheduler.Queue.Abstractions;

public interface IJobsStore
{
    Task<List<SendJob>> GetPendingSendJobs(int batchSize);
    Task<List<ResultHandlingJob>> GetPendingResultHandlingJobs(int batchSize);
    Task<List<(long id, string data)>> GetSendJobsData(IEnumerable<long> sendJobKeys);
    Task<List<(long id, string data)>> GetSendJobsResponse(IEnumerable<long> sendJobKeys);
    Task<SendJob?> FindSendJob(long id);
    Task<ResultHandlingJob?> FindResultHandlingJob(long id);
    Task AddNewResultHandlingJobWithoutCommit(ResultHandlingJob resultHandlingJob);
    Task UpdateJobDataResponseWithoutCommit(long id, string? jsonResponse);
    Task<IEnumerable<long>> AddNewJobs(IEnumerable<SendJob> jobs);
    Task Commit();
}