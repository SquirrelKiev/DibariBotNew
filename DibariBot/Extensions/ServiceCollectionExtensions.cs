using Microsoft.Extensions.DependencyInjection;

namespace DibariBot;

public static class ServiceCollectionExtensions
{
    public static ServiceCollection AddCache(this ServiceCollection services, BotConfig config)
    {
        switch (config.Cache)
        {
            case BotConfig.CacheType.MemoryCache:
                services.AddSingleton<ICacheProvider, MemoryCacheProvider>();
                break;
        }

        return services;
    }
}
