using Microsoft.EntityFrameworkCore;
using Sending.Queue.Example.Contexts.Postgres.Entites;
using SendingScheduler.Queue.Models;

namespace Sending.Queue.Example.Contexts.Postgres;

public class PostgresContext : DbContext
{
    public DbSet<SendJob> SendJobs { get; set; }
    public DbSet<ResultHandlingJob> ResultHandlingJobs { get; set; }
    public DbSet<SendJobData> JobsDatas { get; set; }

    public PostgresContext(DbContextOptions<PostgresContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfiguration(new JobSendStateConfiguration());
        modelBuilder.ApplyConfiguration(new JobResultHandlingStateConfiguration());
        modelBuilder.ApplyConfiguration(new JobDataConfiguration());
    }
}
