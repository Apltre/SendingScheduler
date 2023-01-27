using Sending.Core.Example.Consul;
using Sending.Core.Example.Helpers;
using Sending.Queue.Example.Contexts.Postgres;
using Sending.Queue.Example.Dto;
using Sending.Queue.Example.Helpers;
using Sending.Queue.Example.JobsStores;
using Sending.Queue.Example.Mappers;
using SendingScheduler.Core.Helpers;
using SendingScheduler.Queue;
using SendingScheduler.Queue.Abstractions;

var builder = WebApplication.CreateBuilder(args);
builder.Host.ConfigureSettingFiles();
builder.Configuration.AddConsul(builder.Configuration);
builder.InitializeSendingPostgresContext<PostgresContext>();
builder.Services.AddTransient<IJobsStore, PostgresJobsStore>();
builder.Services.ConfigureSendingQueueService(builder.Configuration);
builder.Logging.ConfigureLogging(builder.Configuration);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();
app.MapGet("/About", context =>
        context.Response.WriteAsJsonAsync(new
        {
            Description = $"Sending.Queue.Example {app.Configuration.GetServiceId()}",
            Environment = app.Environment.EnvironmentName,
            Receiver = app.Configuration.GetReceiver(),
            Version = typeof(Program).Assembly.GetName().Version.ToString()
        }));
app.MapPost("/enqueue", async (JobDto[] jobs, IJobsStore jobsStore) => 
{
    return new JobsAddResponseDto { JobIds = await jobsStore.AddNewJobs(jobs.Select(j => j.MapJobs())) };
});
app.Services.MigrateDb();
app.Run();