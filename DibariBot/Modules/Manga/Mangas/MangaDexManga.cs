using DibariBot.Apis;

namespace DibariBot.Modules.Manga;

public class MangaDexManga : CubariManga
{
    private readonly MangaDexApi mangaDexApi;

    public MangaDexManga(CubariApi cubari, MangaDexApi mdapi) : base(cubari)
    {
        mangaDexApi = mdapi;
    }


    public override async Task<IManga> Initialize(SeriesIdentifier id)
    {
        await base.Initialize(id);

        var res = await mangaDexApi.GetMangaById(id.series!);

        Metadata = MangaDexApi.MangaSchemaToMetadata(res);

        return this;
    }
}
