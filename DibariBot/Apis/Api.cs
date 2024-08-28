using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace DibariBot.Apis;

public class Api(IHttpClientFactory httpFactory, ICacheProvider cache, ILogger logger)
{
    /// <summary>
    /// Gets and deserializes the result from whatever url is.
    /// </summary>
    /// <typeparam name="T">the type to deserialize as</typeparam>
    /// <returns></returns>
    /// <exception cref="HttpRequestException"></exception>
    public virtual async Task<T?> Get<T>(Uri uri, CacheValueSettings? cvs = null)
    {
        cvs ??= new CacheValueSettings(TimeSpan.FromMinutes(15));

        return await cache.GetOrCreateAsync($"apiCache:{Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(uri.ToString()))}", async () =>
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            logger.LogTrace("Fetching {url}", uri);

            using var client = httpFactory.CreateClient();

            var res = await client.GetAsync(uri);

            if (res.StatusCode != System.Net.HttpStatusCode.OK)
            {
                throw new HttpRequestException($"{((int)res.StatusCode)}: {res.StatusCode}");
            }

            var json = await res.Content.ReadAsStringAsync();
            var obj = JsonConvert.DeserializeObject<T>(json);

            logger.LogTrace("Took {time:fff}ms to fetch {uri}", stopwatch.Elapsed, uri);
            stopwatch.Stop();

            return obj;
        }, cvs);
    }
}
