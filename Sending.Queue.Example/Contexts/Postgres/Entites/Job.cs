using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SendingScheduler.Queue.Models;

namespace Sending.Queue.Example.Contexts.Postgres.Entites;

public class JobStateConfiguration<T> : IEntityTypeConfiguration<T> where T : Job
{
    public virtual void Configure(EntityTypeBuilder<T> model)
    {
        model.HasKey(x => x.Id);
    }
}
