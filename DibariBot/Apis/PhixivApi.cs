using BotBase;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;

namespace DibariBot.Apis;

[Inject(ServiceLifetime.Singleton)]
public class PhixivApi(IHttpClientFactory http, ICacheProvider cache, BotConfig botConfig, ILogger<PhixivApi> logger)
{
    private readonly Api api = new(http, cache, logger);
    private readonly Uri baseUri = new(botConfig.PhixivUrl);

    //private readonly BotConfig botConfig;

    //this.botConfig = botConfig;

    public Task<PhixivInfoSchema?> GetById(string id)
    {
        return Get<PhixivInfoSchema>($"/api/info?id={id}&language=en");
    }
    
    public Task<T?> Get<T>(string url, CacheValueSettings? cvs = null)
    {
        return api.Get<T>(new Uri(baseUri, url), cvs);
    }
}

#nullable disable
public class PhixivInfoSchema
{
    public string[] urls;
    public string title;
    public string description;
    public string[] tags;
    public PhixivAuthorSchema author;
}

public class PhixivAuthorSchema
{
    public string id;
    public string name;
}
#nullable enable