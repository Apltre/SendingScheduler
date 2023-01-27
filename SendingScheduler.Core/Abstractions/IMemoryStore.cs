namespace SendingScheduler.Core.Abstractions;

public interface IMemoryStore
{
    Task<bool> CheckKeyExists(string key);
    Task<string?> GetValue(string key);
    Task<T?> GetValue<T>(string key);
    Task Remove(string key);
    Task SetStringValue(string key, string value);
    Task SetValue<T>(string key, T? value);
    Task<bool> RefreshTtl(string key);
}
