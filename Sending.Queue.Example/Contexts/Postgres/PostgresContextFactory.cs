using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Sending.Queue.Example.Contexts.Postgres
{
    public class PostgresContextFactory : IDesignTimeDbContextFactory<PostgresContext>
    {
        public PostgresContext CreateDbContext(string[] args)
        {
            var options = new DbContextOptionsBuilder<PostgresContext>()
                .UseNpgsql("address string for migration", b =>
                {
                    b.MigrationsAssembly("Sending.Queue.Example");
                })
                .UseSnakeCaseNamingConvention()
                .Options;

            return new PostgresContext(options);
        }
    }
}