using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Sending.Queue.Example.Contexts.Postgres;
using SendingScheduler.Core.Enums;
using SendingScheduler.Core.Models;
using SendingScheduler.Queue.Abstractions;
using SendingScheduler.Queue.Models;

namespace Sending.Queue.Example.JobsStores
{
    public class PostgresJobsStore : IJobsStore
    {
        public PostgresJobsStore(PostgresContext dbContext, IOptions<ServiceConfig> serviceConfig)
        {
            DbContext = dbContext;
            ServiceConfig = serviceConfig.Value;
        }

        public PostgresContext DbContext { get; }
        public ServiceConfig ServiceConfig { get; }

        public async Task AddNewResultHandlingJobWithoutCommit(ResultHandlingJob resultHandlingJob)
        {
            await DbContext.ResultHandlingJobs.AddAsync(resultHandlingJob);
        }
        public async Task<List<ResultHandlingJob>> GetPendingResultHandlingJobs(int batchSize)
        {
            return await DbContext.ResultHandlingJobs
                        .Where(j => j.Status == SendingJobStatus.Pending &&
                                    j.ServiceId == ServiceConfig.Id &&
                                    j.StartTime <= DateTime.Now)
                        .OrderBy(j => j.Id)
                        .ThenBy(j => j.Id)
                        .Take(batchSize)
                        .ToListAsync();
        }
        public async Task<List<SendJob>> GetPendingSendJobs(int batchSize)
        {
            return await DbContext.SendJobs
                      .Where(j => j.Status == SendingJobStatus.Pending &&
                                  j.ServiceId == ServiceConfig.Id &&
                                  j.StartTime <= DateTime.Now)
                      .OrderBy(j => j.HandleOrder)
                      .ThenBy(j => j.Id)
                      .Take(batchSize)
                      .ToListAsync();
        }
        public async Task<ResultHandlingJob?> FindResultHandlingJob(long id)
        {
            return await DbContext.ResultHandlingJobs.FindAsync(id);
        }
        public async Task<SendJob?> FindSendJob(long id)
        {
            return await DbContext.SendJobs.FindAsync(id);
        }
        public async Task Commit()
        {
            await DbContext.SaveChangesAsync();
        }
        public async Task UpdateJobDataResponseWithoutCommit(long id, string? jsonResponse)
        {
            if (string.IsNullOrEmpty(jsonResponse))
            {
                return;
            }

            var jobData = new SendJobData
            {
                Id = id,
                ResponseData = jsonResponse,
                ModifiedOn = DateTime.Now
            };
            DbContext.Attach(jobData);
            DbContext.Entry(jobData).Property(x => x.ResponseData).IsModified = true;
        }
        public async Task<List<(long id, string data)>> GetSendJobsData(IEnumerable<long> sendJobKeys)
        {
            if (!sendJobKeys.Any())
            {
                return new List<(long id, string data)>();
            }
            var datas = await DbContext.JobsDatas
                .Where(d => sendJobKeys.Contains(d.Id))
                .Select(d => new { d.Id, d.Data })
                .ToListAsync();
            return datas.Select(d => (d.Id, d.Data)).ToList();
        }
        public async Task<List<(long id, string data)>> GetSendJobsResponse(IEnumerable<long> sendJobKeys)
        {
            if (!sendJobKeys.Any())
            {
                return new List<(long id, string data)>();
            }
            var datas = await DbContext.JobsDatas
                .Where(d => sendJobKeys.Contains(d.Id))
                .Select(d => new { d.Id, d.ResponseData })
                .ToListAsync();
            return datas.Select(d => (d.Id, d.ResponseData)).ToList();
        }

        public async Task<IEnumerable<long>> AddNewJobs(IEnumerable<SendJob> jobs)
        {
            //evil migic to get generated ids x_X
            var jobsArray = jobs.ToArray();
            await DbContext.AddRangeAsync(jobsArray);
            await DbContext.SaveChangesAsync();

            return jobsArray.Select(j => j.Id).ToList();
        }
    }
}