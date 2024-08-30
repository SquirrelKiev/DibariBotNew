using System.ComponentModel;
using DibariBot.Apis;
using Discord;

namespace DibariBot.Modules.MDSearch;

[DibariBot.Inject(Microsoft.Extensions.DependencyInjection.ServiceLifetime.Singleton)]
public class SearchService(MangaDexApi mdapi, BotConfig config, ColorProvider colorProvider)
{
    public struct State
    {
        public string query;

        public int page;

        [DefaultValue(false)]
        public bool isSpoiler;
    }

    public async Task<MessageContents> GetMessageContents(State state, IGuild? guild)
    {
        var res = await mdapi.GetMangas(new Apis.MangaListQueryParams
        {
            limit = config.MangaDexSearchLimit,
            offset = state.page * config.MangaDexSearchLimit,
            order = new Apis.MangaListQueryOrder()
            {
                relevance = Apis.MangaListQueryOrder.QueryOrderSchema.Descending
            },
            title = state.query
        });

        var totalPages = MathF.Ceiling((float)res.total / config.MangaDexSearchLimit);

        if (res.total <= 0)
        {
            var errorEmbed = new EmbedBuilder()
                .WithDescription("No results found!")
                .WithColor(colorProvider.GetErrorEmbedColor());

            return new MessageContents(string.Empty, errorEmbed.Build(), null);
        }

        var embed = new EmbedBuilder().WithColor(await colorProvider.GetEmbedColor(guild));

        foreach (var mangaSchema in res.data)
        {
            var manga = MangaDexApi.MangaSchemaToMetadata(mangaSchema);

            // why cant field titles have URLs
            embed
                .AddField(new EmbedFieldBuilder()
                    .WithName($"{manga.title
                        .StringOrDefault("No title (why?)")
                        .Truncate(config.MaxTitleLength)} by {manga.author.Truncate(config.MaxTitleLength)}"
                        )
                    .WithValue(manga.description
                        .StringOrDefault("No description.")
                        .Truncate(config.MaxDescriptionLength)
                        + $" [(link)]({config.MangaDexSearchUrl.Replace("{{ID}}", mangaSchema.id)})"
                        )
                    )
                .WithFooter(new EmbedFooterBuilder()
                    .WithText($"Page {state.page + 1}/{totalPages}"));
        }

        bool disableLeft = state.page <= 0;
        bool disableRight = state.page >= totalPages - 1;

        var components = new ComponentBuilder()
            .WithSelectMenu(new SelectMenuBuilder()
                    .WithOptions(res.data.Select(x =>
                        new SelectMenuOptionBuilder()
                            .WithValue(x.id.ToString())
                            .WithLabel(x.attributes.title.ToString().Truncate(config.MaxTitleLength))
                    ).ToList())
                    .WithCustomId(StateSerializer.SerializeObject(state, ModulePrefixes.MANGADEX_SEARCH_DROPDOWN_PREFIX))
                )
            .WithButton(new ButtonBuilder()
                    .WithLabel("<")
                    .WithCustomId(StateSerializer.SerializeObject(state with { page = state.page - 1 },
                        ModulePrefixes.MANGADEX_SEARCH_BUTTON_PREFIX))
                    .WithDisabled(disableLeft)
                    .WithStyle(ButtonStyle.Secondary)
                )
            .WithButton(new ButtonBuilder()
                    .WithLabel(">")
                    .WithCustomId(StateSerializer.SerializeObject(state with { page = state.page + 1 },
                        ModulePrefixes.MANGADEX_SEARCH_BUTTON_PREFIX))
                    .WithDisabled(disableRight)
                    .WithStyle(ButtonStyle.Secondary)
                )
            .WithRedButton();

        return new MessageContents(string.Empty, embed: embed.Build(), components);
    }
}
