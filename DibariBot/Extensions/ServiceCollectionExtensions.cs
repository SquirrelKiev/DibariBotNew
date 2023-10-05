using Microsoft.Extensions.DependencyInjection;

namespace DibariBot;

public static class ServiceCollectionExtensions
{
    public static ServiceCollection AddCache(this ServiceCollection services, BotConfig config)
    {
        switch (config.Cache)
        {
            case BotConfig.CacheType.Memory:
                services.AddSingleton<ICacheProvider, MemoryCacheProvider>();
                break;
            default:
                throw new NotSupportedException(config.Cache.ToString());
        }

        return services;
    }
}
