using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Serilog;

namespace DibariBot;

public class MemoryCacheProvider(ILogger<MemoryCacheProvider> logger) : ICacheProvider
{
    private readonly MemoryCache cache = new(new MemoryCacheOptions()
    {
        SizeLimit = 1000
    });

    public ValueTask<Optional<T>> GetAsync<T>(string key)
    {
        if (!cache.TryGetValue(key, out var val) || val is null)
        {
            LogHitOrMiss(key, false);
            return new ValueTask<Optional<T>>(new Optional<T>());
        }

        LogHitOrMiss(key, true);
        return new((T)val);
    }

    public async ValueTask<T?> GetOrCreateAsync<T>(string key, Func<Task<T>> createFactory, CacheValueSettings settings)
    {
        var cacheHit = true;

        var val = await cache.GetOrCreateAsync(key,
            async x =>
            {
                cacheHit = false;

                var value = await createFactory();

                Set(x, value, settings);

                return value;
            });

        LogHitOrMiss(key, cacheHit);

        return val;
    }

    public ValueTask<bool> RemoveAsync(string key)
    {
        var exists = cache.TryGetValue(key, out var old) && old is not null;

        if (exists)
            cache.Remove(key);

        return new(exists);
    }

    public ValueTask<bool> SetAsync<T>(string key, T value, CacheValueSettings settings)
    {
        if (value == null) throw new ArgumentNullException(nameof(value));

        using var item = cache.CreateEntry(key);

        return new(Set(item, value, settings));
    }

    private static bool Set<T>(ICacheEntry cacheEntry, T? value, CacheValueSettings settings)
    {
        ArgumentNullException.ThrowIfNull(cacheEntry, nameof(cacheEntry));

        cacheEntry
            .SetSize(settings.size);

        if (settings.ttl.HasValue)
        {
            cacheEntry
                .SetAbsoluteExpiration(settings.ttl.Value);
        }

        cacheEntry.Value = value;

        return true;
    }

    private void LogHitOrMiss(string key, bool cacheHit)
    {
        logger.LogTrace(cacheHit ? "Cache hit! Key is {Key}." : "Cache miss. Key is {Key}.", key);
    }
}
