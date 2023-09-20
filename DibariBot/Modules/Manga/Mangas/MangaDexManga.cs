namespace DibariBot.Modules.Manga;

public class MangaDexManga : CubariManga
{
    private readonly MangaDexApi mangaDexApi;

    public MangaDexManga(CubariApi cubari, MangaDexApi mdapi) : base(cubari)
    {
        mangaDexApi = mdapi;
    }


    public override async Task<IManga> Initialize(SeriesIdentifier identifier)
    {
        await base.Initialize(identifier);

        var res = await mangaDexApi.GetMangaById(identifier.series!);

        Metadata = MangaDexApi.MangaSchemaToMetadata(res);

        return this;
    }
}
