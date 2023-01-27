using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SendingScheduler.Queue.Models;

namespace Sending.Queue.Example.Contexts.Postgres.Entites;

public class JobSendStateConfiguration : JobStateConfiguration<SendJob>
{
    public override void Configure(EntityTypeBuilder<SendJob> model)
    {
        base.Configure(model);
        model.ToTable("send_jobs");
        model.HasIndex(x => new { x.ServiceId, x.Status, x.StartTime, x.HandleOrder });
        model.HasOne(x => x.Data)
            .WithOne()
            .HasPrincipalKey<SendJob>(x => x.Id)
            .HasForeignKey<SendJobData>(x => x.Id);
    }
}
