using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SendingScheduler.Queue.Models;

namespace Sending.Queue.Example.Contexts.Postgres.Entites;

public class JobResultHandlingStateConfiguration : JobStateConfiguration<ResultHandlingJob>
{
    public override void Configure(EntityTypeBuilder<ResultHandlingJob> model)
    {
        base.Configure(model);
        model.ToTable("result_handling_jobs");
        model.HasIndex(x => new { x.ServiceId, x.Status, x.StartTime });

        model.HasIndex(x => new { x.SendJobId });
        model.Property(x => x.SendJobId)
            .IsRequired();

        model.Property(x => x.ErrorData).HasColumnType("jsonb");

        model.Property(x => x.SendJobErrorData).HasColumnType("jsonb");

        model.HasOne(x => x.SendJob)
            .WithMany()
            .HasForeignKey(x => x.SendJobId)
            .HasPrincipalKey(x => x.Id);
    }
}
