using BotBase;
using DibariBot.Modules.Manga;

namespace DibariBot.Apis;

[Inject(Microsoft.Extensions.DependencyInjection.ServiceLifetime.Singleton)]
public class MangaDexApi
{
    private readonly Api api;
    private readonly Uri baseUri;

    public MangaDexApi(IHttpClientFactory http, ICacheProvider cache, BotConfig botConfig)
    {
        api = new Api(http, cache);

        baseUri = new Uri(botConfig.MangaDexApiUrl);
    }

    public async Task<MangaListSchema> GetMangas(MangaListQueryParams queryParams)
    {
        queryParams.includes = new ReferenceExpansionMangaSchema[]
        {
            ReferenceExpansionMangaSchema.Author
        };

        var uri = new Uri(baseUri, "manga?" + QueryStringSerializer.ToQueryParams(queryParams));

        var res = await api.Get<MangaListSchema>(uri);

        return res ?? throw new NullReferenceException();
    }

    public async Task<MangaSchema> GetMangaById(string id)
    {
        var queryParams = new GetMangaIdParamsSchema()
        {
            includes = new ReferenceExpansionMangaSchema[]
            {
                ReferenceExpansionMangaSchema.Author,
                ReferenceExpansionMangaSchema.Artist
            }
        };

        var uri = new Uri(baseUri, "manga/" + id + '?' + QueryStringSerializer.ToQueryParams(queryParams));

        var res = await api.Get<MangaResponseSchema>(uri) ?? throw new NullReferenceException();

        return res.data;
    }

    public static MangaMetadata MangaSchemaToMetadata(MangaSchema schema)
    {
        var authorRelation = schema.relationships.FirstOrDefault(x => x.type == ReferenceExpansionMangaSchema.Author);
        var artistRelation = schema.relationships.FirstOrDefault(x => x.type == ReferenceExpansionMangaSchema.Artist);

        var author = authorRelation?.attributes?.ToObject<AuthorAttributesSchema>();
        var artist = artistRelation?.attributes?.ToObject<AuthorAttributesSchema>();

        var tags = schema.attributes.tags.Select(t => t.attributes.name.ToString()).ToArray();

        return new MangaMetadata()
        {
            author = author?.name ?? "No author",
            artist = artist?.name ?? "No artist",
            description = schema.attributes.description.ToString(),
            title = schema.attributes.title.ToString(),
            tags = tags,
            contentRating = schema.attributes.contentRating
        };
    }


    // TODO: plug into cover API
    //public async Task<Cover[]> GetCovers(MangaSchema schema)
    //{
    //    var coverRelations = schema.relationships.Where(x => x.type == ReferenceExpansionMangaSchema.CoverArt);
    //    var covers = coverRelations.Where(x => x.attributes != null)
    //        .Select(x =>
    //        {
    //            var obj = x.attributes!.ToObject<CoverAttributesSchema>();
    //            return new Cover()
    //            {
    //                url = $"https://uploads.mangadex.org/covers/{schema.id}/{obj!.fileName}"
    //            };
    //        }).ToArray();

    //    return covers;
    //}
}
