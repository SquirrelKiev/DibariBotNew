using Microsoft.Extensions.Caching.Memory;
using OneOf;
using OneOf.Types;

namespace DibariBot;

public class MemoryCacheProvider : ICacheProvider
{
    private readonly MemoryCache cache;

    public MemoryCacheProvider()
    {
        cache = new MemoryCache(new MemoryCacheOptions()
        {
            SizeLimit = 1000
        });
    }

    // hate how i have to do OneOf i thought Nullable was for this exact case
    public ValueTask<OneOf<T, None>> GetAsync<T>(string key)
    {
        if (!cache.TryGetValue(key, out var val) || val is null)
        {
            LogHitOrMiss(key, false);
            return new ValueTask<OneOf<T, None>>(new None());
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
            .SetAbsoluteExpiration(settings.ttl ?? CacheConsts.defaultTTL)
            .SetSize(settings.size);

        cacheEntry.Value = value;

        return true;
    }

    private static void LogHitOrMiss(string key, bool cacheHit)
    {
        if (cacheHit)
            Log.Debug("Cache hit! Key is {Key}.", key);
        else
            Log.Debug("Cache miss. Key is {Key}.", key);
    }
}
