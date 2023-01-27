using Microsoft.EntityFrameworkCore;

namespace Sending.Queue.Example.Helpers;

public class DbContextFactory<T> where T : DbContext
{
    public DbContextFactory(IServiceProvider services)
    {
        Services = services;
    }

    public IServiceProvider Services { get; }

    public T Get()
    {
        return Services.GetRequiredService<T>();
    }
}