﻿using Microsoft.Extensions.DependencyInjection;

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
        Type? mangaType;

        // TODO: ideally i dont have to change anything here to add a new manga/platform
        mangaType = identifier.platform switch
        {
            "imgur" or
            "gist" or
            "nhentai" or
            "mangadex" or
            "mangasee" or
            "reddit" or
            "imgchest" => typeof(CubariManga),
            "xkcd" => typeof(XkcdManga),
            _ => null,
        };

        if(mangaType == null)
        {
            return null;
        }

        return await ((IManga)services.GetRequiredService(mangaType)).Initialize(identifier);
    }
}
