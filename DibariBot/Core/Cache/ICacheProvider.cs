namespace DibariBot;

public interface ICacheProvider
{
    ValueTask<bool> SetAsync<T>(string key, T value, CacheValueSettings settings);
    ValueTask<Optional<T>> GetAsync<T>(string key);
    ValueTask<T?> GetOrCreateAsync<T>(string key, Func<Task<T>> createFactory, CacheValueSettings settings);
    ValueTask<bool> RemoveAsync(string key);
}

public class CacheValueSettings
{
    /// <summary>
    /// How long the object will be in cache before being deleted. default to 15 mins.
    /// </summary>
    public TimeSpan ttl = TimeSpan.FromMinutes(15);
    /// <summary>
    /// The "size" of the object in memory.
    /// </summary>
    public int size = 1;

    public CacheValueSettings()
    {
    }

    public CacheValueSettings(TimeSpan ttl, int size)
    {
        this.ttl = ttl;
        this.size = size;
    }

    public CacheValueSettings(TimeSpan ttl)
    {
        this.ttl = ttl;
    }
}
