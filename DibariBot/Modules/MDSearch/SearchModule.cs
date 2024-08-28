using DibariBot.Modules.Manga;
using Discord.Interactions;

namespace DibariBot.Modules.MDSearch;

[CommandContextType(InteractionContextType.Guild, InteractionContextType.BotDm, InteractionContextType.PrivateChannel)]
[IntegrationType(ApplicationIntegrationType.GuildInstall, ApplicationIntegrationType.UserInstall)]
public class SearchModule(SearchService search, MangaService mangaService) : BotModule
{
    [SlashCommand("manga-search", "Searches MangaDex for the query provided. (searches titles, sorted by relevance)")]
    public async Task SearchSlash([Summary(description: "The manga to search for.")] string query,
        [Summary(description: "Whether the response should be ephemeral (only you can see it).")] bool ephemeral = false,
        [Summary(description: "Whether to spoiler tag the response or not.")] bool spoiler = false)
    {
        await DeferAsync(ephemeral);

        await FollowupAsync(await search.GetMessageContents(new SearchService.State() { query = query, isSpoiler = spoiler}));
    }

    [ComponentInteraction(ModulePrefixes.MANGADEX_SEARCH_BUTTON_PREFIX + "*")]
    public async Task SearchButtonInteraction(string id)
    {
        await DeferAsync();

        var state = StateSerializer.DeserializeObject<SearchService.State>(id);

        await ModifyOriginalResponseAsync(await search.GetMessageContents(state));
    }

    [ComponentInteraction(ModulePrefixes.MANGADEX_SEARCH_DROPDOWN_PREFIX + "*")]
    public async Task SearchDropdownInteraction(string id, string dexId)
    {
        await DeferAsync();

        var state = StateSerializer.DeserializeObject<SearchService.State>(id);

        var ogRes = await GetOriginalResponseAsync();

        var isEphemeral = (ogRes.Flags & MessageFlags.Ephemeral) != 0;

        await ModifyOriginalResponseAsync(await mangaService.MangaCommand(Context.Guild?.Id ?? 0ul, GetParentChannel().Id,
            new SeriesIdentifier("mangadex", dexId).ToString(), ephemeral: isEphemeral, isSpoiler: state.isSpoiler));
    }
}
