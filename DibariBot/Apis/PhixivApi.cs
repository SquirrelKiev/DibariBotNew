using BotBase;
using Microsoft.Extensions.DependencyInjection;

namespace DibariBot.Apis;

[Inject(ServiceLifetime.Singleton)]
public class PhixivApi
{
    private readonly Api api;
    private readonly Uri baseUri;

    //private readonly BotConfig botConfig;

    public PhixivApi(IHttpClientFactory http, ICacheProvider cache, BotConfig botConfig)
    {
        //this.botConfig = botConfig;
        api = new Api(http, cache);

        baseUri = new Uri(botConfig.PhixivUrl);
    }

    public Task<PhixivInfoSchema?> GetById(string id)
    {
        return Get<PhixivInfoSchema>($"/api/info?id={id}&language=en");
    }
    
    public Task<T?> Get<T>(string url, CacheValueSettings? cvs = null)
    {
        return api.Get<T>(new Uri(baseUri, url), cvs);
    }
}

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