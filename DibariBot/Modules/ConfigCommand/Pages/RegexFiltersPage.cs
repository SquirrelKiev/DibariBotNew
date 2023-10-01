using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using DibariBot.Core.Database.Models;
using DibariBot.Modules.Manga;
using Humanizer;

namespace DibariBot.Modules.ConfigCommand.Pages;

public partial class RegexFiltersPage : ConfigPage
{
    public class SetRegexModal : IModal
    {
        public string Title => "Set Regex";

        [ModalTextInput(customId: ModulePrefixes.CONFIG_FILTERS_MODAL_TEMPLATE_TEXTBOX, maxLength: 100)]
        public string Template { get; set; } = "";

        [ModalTextInput(customId: ModulePrefixes.CONFIG_FILTERS_MODAL_FILTER_TEXTBOX)]
        public string Filter { get; set; } = "";
    }

    public override Page Id => Page.RegexFilters;

    public override string Label => "Regex filters";

    public override string Description => "TODO";

    private const string EMBED_NAME_TEMPLATE = "Template";
    private const string EMBED_NAME_FILTER = "Filter";
    private const string EMBED_NAME_INCLUDE = "Include Channels";
    private const string EMBED_NAME_EXCLUDE = "Exclude Channels";

    private readonly MangaService mangaService;
    private readonly ConfigCommandService configCommandService;
    private readonly BotConfig config;

    public RegexFiltersPage(MangaService mangaService, ConfigCommandService configCommandService, BotConfig config)
    {
        this.mangaService = mangaService;
        this.configCommandService = configCommandService;
        this.config = config;
    }

    public override async Task<MessageContents> GetMessageContents(ConfigCommandService.State state)
    {
        var embed = new EmbedBuilder();

        var filters = await mangaService.GetFilters(Context.Guild.Id);

        if (filters.Length > 0)
        {
            for (var i = 0; i < filters.Length; i++)
            {
                var filter = filters[i];

                var value = new StringBuilder()

                    .AppendLine("**Template**")
                    .AppendLine("```")
                    .AppendLine(filter.Template)
                    .AppendLine("```")

                    .AppendLine("**Filter**")
                    .AppendLine("```")
                    .AppendLine(filter.Filter)
                    .AppendLine("```")

                    .AppendLine("**Channel Scope**")
                    .AppendLine(filter.ChannelFilterScope.Humanize())
                    .AppendLine("**Channels**");

                foreach (var channelEntry in filter.RegexChannelEntries)
                {
                    value.AppendLine($"<#{channelEntry.ChannelId}>");
                }

                embed.AddField(new EmbedFieldBuilder()
                    .WithName("Filter " + i)
                    .WithValue(value.ToString())
                    .WithIsInline(true));
            }
        }
        else
        {
            embed.WithDescription("No filters!");
        }

        var components = new ComponentBuilder()
            .WithSelectMenu(ConfigPageUtility.GetPageSelectDropdown(configCommandService.ConfigPages, Id))
            .WithButton(new ButtonBuilder()
                .WithLabel("Add")
                .WithCustomId($"{ModulePrefixes.CONFIG_FILTERS_ADD_BUTTON}")
                .WithStyle(config.PrimaryButtonStyle))
            .WithButton(new ButtonBuilder()
                .WithLabel("Edit")
                .WithCustomId($"{ModulePrefixes.CONFIG_FILTERS_EDIT}")
                .WithStyle(config.PrimaryButtonStyle))
            .WithButton(new ButtonBuilder()
                .WithLabel("Remove")
                .WithCustomId($"{ModulePrefixes.CONFIG_FILTERS_REMOVE}")
                .WithStyle(config.PrimaryButtonStyle))
            .WithRedButton();

        return new MessageContents("", embed.Build(), components);
    }

    [ComponentInteraction(ModulePrefixes.CONFIG_FILTERS_ADD_BUTTON)]
    public async Task AddButton()
    {
        await Context.Interaction.RespondWithModalAsync<SetRegexModal>(ModulePrefixes.CONFIG_FILTERS_MODAL + 0u);
    }

    [ModalInteraction(ModulePrefixes.CONFIG_FILTERS_MODAL + "*")]
    public async Task ModalResponse(uint regexKey, SetRegexModal modal)
    {
        await DeferAsync();

        var filter = await mangaService.GetFilter(regexKey);

        filter ??= new RegexFilter(filterType: FilterType.Block, guildId: Context.Guild.Id, id: 0, filter: modal.Filter,
            template: modal.Template);

        var contents = UpsertConfirmation(filter);

        await ModifyOriginalResponseAsync(contents);
    }

    [ComponentInteraction(ModulePrefixes.CONFIG_FILTERS_CONFIRMATION_ADD_BUTTON + "*")]
    private async Task ConfirmationAddButton()
    {
        await DeferAsync();

        var filter = await GetRegexFilterFromContext();

        await mangaService.UpdateOrAddRegexFilter(filter);

        await ModifyOriginalResponseAsync(await configCommandService.GetMessageContents(new ConfigCommandService.State { page = Id }, Context));
    }

    private MessageContents UpsertConfirmation(RegexFilter filter)
    {
        var embed = new EmbedBuilder();

        var channels = filter.RegexChannelEntries;

        embed.WithFields
        (
            new EmbedFieldBuilder()
                .WithName(EMBED_NAME_TEMPLATE)
                .WithValue(filter.Template),
            new EmbedFieldBuilder()
                .WithName(EMBED_NAME_FILTER)
                .WithValue(filter.Filter),
            new EmbedFieldBuilder()
                .WithName(EMBED_NAME_INCLUDE)
                .WithValue(channels
                    .Aggregate("", (current, next) => current + $"<#{next.ChannelId}>{Environment.NewLine}")
                    .StringOrDefault("None!")));

        var components = new ComponentBuilder()
            .WithSelectMenu(new SelectMenuBuilder()
                .WithCustomId(ModulePrefixes.CONFIG_FILTERS_CONFIRMATION_FILTER_TYPE)
                .WithOptions(
                    Enum.GetValues<FilterType>().Select(x =>
                        new SelectMenuOptionBuilder()
                            .WithDefault(x == filter.FilterType)
                            .WithLabel("Filter type: " + x.Humanize())
                            // ehh i could do attributes but this is way easier (if against the whole, easily expandable principle ive been doing)
                            // TODO: should be reusable tho for like help pages and stuff
                            .WithDescription(x switch
                            {
                                FilterType.Allow => "If the manga matches the filter, it can be shown.",
                                FilterType.Block => "If the manga matches the filter, it will not be shown.",
                                _ => "looks like i forgot a description??",
                            })
                            .WithValue(x.ToString())
                            ).ToList()
                    )
                )
            .WithSelectMenu(new SelectMenuBuilder()
                .WithCustomId(ModulePrefixes.CONFIG_FILTERS_CONFIRMATION_CHANNEL_SCOPE)
                .WithOptions(
                    Enum.GetValues<ChannelFilterScope>().Select(x =>
                        new SelectMenuOptionBuilder()
                            .WithDefault(x == filter.ChannelFilterScope)
                            .WithLabel("Filter type: " + x.Humanize())
                            // ehh i could do attributes but this is way easier (if against the whole, easily expandable principle ive been doing)
                            // TODO: should be reusable tho for like help pages and stuff
                            .WithDescription(x switch
                            {
                                ChannelFilterScope.Include => "The filter will only apply in the specified channels.",
                                ChannelFilterScope.Exclude => "The filter will apply in every channel except the specified channels.",
                                _ => "looks like i forgot a description??",
                            })
                            .WithValue(x.ToString())
                    ).ToList()
                )
            )
            .WithSelectMenu(new SelectMenuBuilder()
                .WithPlaceholder("Channels")
                .WithType(ComponentType.ChannelSelect)
                .WithChannelTypes(ChannelType.Text)
                .WithCustomId(ModulePrefixes.CONFIG_FILTERS_CONFIRMATION_CHANNEL_SELECT)
                .WithDefaultValues
                (
                    channels
                        .Select(x => new SelectMenuDefaultValue(x.ChannelId, SelectDefaultValueType.Channel))
                        .ToArray()
                )
                .WithMinValues(0)
                .WithMaxValues(25)
            )
            // Rare case where button state is not used and we're gonna grab from the embed etc instead (100 characters too small for what people probably want)
            .WithButton(new ButtonBuilder()
                .WithLabel("Add")
                .WithCustomId(ModulePrefixes.CONFIG_FILTERS_CONFIRMATION_ADD_BUTTON +
                              StateSerializer.SerializeObject(StateSerializer.SerializeObject(filter.Id)))
                .WithStyle(ButtonStyle.Success))
            .WithButton(new ButtonBuilder()
                .WithLabel("Back")
                .WithCustomId(ModulePrefixes.CONFIG_PAGE_SELECT_PAGE_BUTTON +
                              StateSerializer.SerializeObject(StateSerializer.SerializeObject(Id)))
                .WithStyle(ButtonStyle.Danger))
            ;

        return new MessageContents(string.Empty, embed.Build(), components);
    }

    [ComponentInteraction(ModulePrefixes.CONFIG_FILTERS_CONFIRMATION_CHANNEL_SELECT)]
    private async Task UpsertConfirmationChannelSelectChanged(IChannel[] channels)
    {
        await DeferAsync();

        var filter = await GetRegexFilterFromContext(channels: channels.Select(x => new RegexChannelEntry { ChannelId = x.Id}).ToList());

        await ModifyOriginalResponseAsync(UpsertConfirmation(filter));
    }

    private async Task<RegexFilter> GetRegexFilterFromContext(FilterType? filterType = null, ChannelFilterScope? scope = null, List<RegexChannelEntry>? channels = null)
    {
        var ogRes = await Context.Interaction.GetOriginalResponseAsync();

        var embed = ogRes.Embeds.First(x => x.Type == EmbedType.Rich);

        var template = embed.Fields.First(x => x.Name == EMBED_NAME_TEMPLATE).Value;
        var filter = embed.Fields.First(x => x.Name == EMBED_NAME_FILTER).Value;

        channels ??= ChannelMatcher().Matches(embed.Fields.First(x => x.Name == EMBED_NAME_INCLUDE).Value).Select(x => new RegexChannelEntry
        {
            ChannelId = ulong.Parse(x.Groups[1].Value)
        }).ToList();

        return new RegexFilter(id: 0, guildId: Context.Guild.Id, filter: filter,
            template: template, filterType: FilterType.Block,
            regexChannelEntries: channels);
    }

    [GeneratedRegex("<#(\\d+)>")]
    private static partial Regex ChannelMatcher();
}
