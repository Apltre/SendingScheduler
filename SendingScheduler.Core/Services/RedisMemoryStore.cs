using Microsoft.Extensions.Options;
using SendingScheduler.Core.Abstractions;
using SendingScheduler.Core.Helpers;
using SendingScheduler.Core.Models;
using StackExchange.Redis;
using System.Text;
using System.Text.Json;

namespace SendingScheduler.Core.Services;

public class RedisMemoryStore : IMemoryStore
{
    public IDatabase Database => Connection.GetDatabase();
    public TimeSpan MemoryCacheSliding { get; }
    public ServiceConfig ServiceConfig { get; }
    public IConnectionMultiplexer Connection { get; }
    public RedisMemoryStore(IConnectionMultiplexer connection, IOptions<ServiceConfig> serviceConfig)
    {
        ServiceConfig = serviceConfig.Value;
        Connection = connection;
        MemoryCacheSliding = TimeSpan.FromMinutes(ServiceConfig!.MemoryCacheSlidingInMinutes!.Value);
    }

    public async Task<bool> CheckKeyExists(string key)
    {
        return await Database.KeyExistsAsync(key);
    }
    public async Task<bool> RefreshTtl(string key)
    {
        return await Database.KeyExpireAsync(key, MemoryCacheSliding, ExpireWhen.Always);
    }
    public async Task<string?> GetValue(string key)
    {
        var value = await Database.StringGetAsync(key);
        if (!value.HasValue)
        {
            return null;
        }
        return Encoding.UTF8.GetString(value);
    }
    public async Task<T?> GetValue<T>(string key)
    {
        var value = await GetValue(key);
        if (string.IsNullOrEmpty(value))
        {
            return default;
        }
        return JsonSerializer.Deserialize<T>(value);
    }
    public async Task Remove(string key)
    {
        await Database.KeyDeleteAsync(key);
    }
    public async Task SetStringValue(string key, string value)
    {
        byte[] bytes = Encoding.UTF8.GetBytes(value);
        await Database.StringSetAsync(key, bytes, MemoryCacheSliding, When.Always);
    }
    public async Task SetValue<T>(string key, T? value)
    {
        if (value == null)
        {
            return;
        }

        var jsonString = value.GetType() != typeof(string) ? JsonSerializer.Serialize(value, JsonHelper.IgnoreNullsOption) : value.ToString();
        await SetStringValue(key, jsonString);
    }
}