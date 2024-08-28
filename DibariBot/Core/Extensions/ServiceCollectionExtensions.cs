namespace DibariBot;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCache(this IServiceCollection services, BotConfig config)
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
