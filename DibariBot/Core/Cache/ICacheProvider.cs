using OneOf;
using OneOf.Types;

namespace DibariBot;

public interface ICacheProvider
{
    ValueTask<bool> SetAsync<T>(string key, T value, CacheValueSettings settings);
    ValueTask<OneOf<T, None>> GetAsync<T>(string key);
    ValueTask<T?> GetOrCreateAsync<T>(string key, Func<Task<T>> createFactory, CacheValueSettings settings);
    ValueTask<bool> RemoveAsync(string key);
}

public struct CacheValueSettings
{
    public TimeSpan? ttl = null;
    public int size = 1;

    public CacheValueSettings()
    {
    }
}