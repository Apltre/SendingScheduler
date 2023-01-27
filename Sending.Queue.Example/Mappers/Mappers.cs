using Sending.Queue.Example.Dto;
using SendingScheduler.Core.Enums;
using SendingScheduler.Queue.Models;

namespace Sending.Queue.Example.Mappers
{
    public static class Mappers
    {
        public static SendJob MapJobs(this JobDto jobDto)
        {
            return new SendJob
            {
                Status = SendingJobStatus.Pending,
                Type = jobDto.Type,
                StartTime = jobDto.StartTime,
                ServiceId = jobDto.ServiceId,
                CreatedOn = DateTime.Now,
                HandleOrder = jobDto.HandleOrder,
                Data = new SendJobData
                {
                    MetaData = jobDto.Metadata.ToString(),
                    Data = jobDto.Data.ToString(),
                    CreatedOn = DateTime.Now
                }
            };
        }
    }
}
