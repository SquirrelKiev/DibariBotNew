using Microsoft.Extensions.DependencyInjection;

namespace DibariBot.Mangas;

public class MangaFactory
{
    private readonly IServiceProvider services;

    public MangaFactory(IServiceProvider services)
    {
        this.services = services;
    }

    public async Task<IManga?> GetManga(SeriesIdentifier identifier)
    {
        // TODO: figure out from manga class itself via reflection?
        return identifier.platform switch
        {
            "imgur" or
            "gist" or
            "nhentai" or
            "mangadex" or
            "mangasee" or
            "reddit" or
            "imgchest" => await services.GetRequiredService<CubariApi>().GetManga(identifier),
            _ => null,
        };
    }
}
