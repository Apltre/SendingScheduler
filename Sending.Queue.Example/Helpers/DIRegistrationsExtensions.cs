using Microsoft.EntityFrameworkCore;
using Sending.Queue.Example.Contexts.Postgres;

namespace Sending.Queue.Example.Helpers;
public static class DIRegistrationsExtensions
{
    public static void InitializeSendingPostgresContext<TContex>(this WebApplicationBuilder builder)
       where TContex : DbContext
    {
        var serviceCollection = builder.Services;
        var connectionString = builder.Configuration.GetConnectionString("SendingDB");
        serviceCollection.AddDbContextPool<TContex>(o =>
        {
            o.UseNpgsql(connectionString, builder =>
            {
                builder.MigrationsAssembly("Sending.Queue.Example");
                builder.MigrationsHistoryTable("__SendingQueueMigrationsHistory");
                builder.EnableRetryOnFailure(2);
            }).UseSnakeCaseNamingConvention();
        });
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
    }
    public static void MigrateDb(this IServiceProvider serviceProvider)
    {
        using (var scope = serviceProvider.CreateScope())
        {
            var services = scope.ServiceProvider;
            var context = services.GetRequiredService<PostgresContext>();
            
            context.Database.Migrate();
        }
    }
}
