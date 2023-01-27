using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SendingScheduler.Queue.Models;

namespace Sending.Queue.Example.Contexts.Postgres.Entites;

public class JobDataConfiguration : IEntityTypeConfiguration<SendJobData>
{
    public virtual void Configure(EntityTypeBuilder<SendJobData> model)
    {
        model.ToTable("send_jobs_data");
        model.HasKey(x => x.Id);
        model.Property(x => x.Data)
            .HasColumnType("jsonb")
            .IsRequired()
            .HasDefaultValue("{}");
        model.Property(x => x.ResponseData)
            .HasColumnType("jsonb");
        model.Property(x => x.MetaData)
          .HasColumnType("jsonb");
    }
}
