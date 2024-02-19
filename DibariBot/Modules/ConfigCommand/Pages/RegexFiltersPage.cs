using System.Text;
using System.Text.RegularExpressions;
using BotBase;
using BotBase.Modules;
using BotBase.Modules.ConfigCommand;
using DibariBot.Database.Models;
using DibariBot.Modules.Manga;
using Humanizer;

namespace DibariBot.Modules.ConfigCommand.Pages;

public partial class RegexFiltersPage : ConfigPage
{
    public class SetRegexModal : IModal
    {
        public string Title => "Set Regex";

        [ModalTextInput(customId: ModulePrefixes.CONFIG_FILTERS_MODAL_TEMPLATE_TEXTBOX, maxLength: 200)]
        public string Template { get; set; } = "";

        [ModalTextInput(customId: ModulePrefixes.CONFIG_FILTERS_MODAL_FILTER_TEXTBOX, maxLength: 200)]
        public string Filter { get; set; } = "";
    }

    public override Page Id => Page.RegexFilters;

    public override string Label => "Regex filters";

    public override string Description => "Specify filters to limit what mangas can be pulled up.";

    public override bool EnabledInDMs => false;

    private const string EMBED_NAME_TEMPLATE = "Template";
    private const string EMBED_NAME_FILTER = "Filter";
    private const string EMBED_NAME_CHANNELS = "Channels";
    private const string EMBED_NAME_FILTER_TYPE = "Filter Type";
    private const string EMBED_NAME_SCOPE = "Channel Scope";

    private readonly MangaService mangaService;
    private readonly ConfigCommandServiceBase<Page> configCommandService;
    private readonly BotConfig config;

    public RegexFiltersPage(MangaService mangaService, ConfigCommandServiceBase<Page> configCommandService, BotConfig config)
    {
        this.mangaService = mangaService;
        this.configCommandService = configCommandService;
        this.config = config;
    }

    public override async Task<MessageContents> GetMessageContents(ConfigCommandServiceBase<Page>.State state)
    {
        var embed = new EmbedBuilder().WithColor(CommandResult.Default);

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

                    .AppendLine("**Filter Type**")
                    .AppendLine(filter.FilterType.ToString())

                    .AppendLine("**Channel Scope**")
                    .AppendLine(filter.ChannelFilterScope.ToString())
                    .AppendLine("**Channels**");

                foreach (var channelEntry in filter.RegexChannelEntries)
                {
                    value.AppendLine($"<#{channelEntry.ChannelId}>");
                }

                embed.AddField(new EmbedFieldBuilder()
                    .WithName("Filter " + (i + 1))
                    .WithValue(value.ToString())
                    .WithIsInline(true));
            }
        }
        else
        {
            embed.WithDescription("No filters!");
        }

        var components = new ComponentBuilder()
            .WithSelectMenu(GetPageSelectDropdown(configCommandService.ConfigPages, Id, IsDm()))
            .WithButton(new ButtonBuilder()
                .WithLabel("Add")
                .WithCustomId($"{ModulePrefixes.CONFIG_FILTERS_OPEN_MODAL_BUTTON}{0ul}")
                .WithStyle(ButtonStyle.Secondary))
            .WithButton(new ButtonBuilder()
                .WithLabel("Edit")
                .WithCustomId($"{ModulePrefixes.CONFIG_FILTERS_EDIT_BUTTON}")
                .WithStyle(ButtonStyle.Secondary)
                .WithDisabled(filters.Length <= 0))
            .WithButton(new ButtonBuilder()
                .WithLabel("Remove")
                .WithCustomId($"{ModulePrefixes.CONFIG_FILTERS_REMOVE_BUTTON}")
                .WithStyle(ButtonStyle.Secondary)
                .WithDisabled(filters.Length <= 0))
            .WithRedButton();

        return new MessageContents("", embed.Build(), components);
    }

    private static SelectMenuBuilder GetFilterSelectMenu(string customId, IEnumerable<RegexFilter> filters)
    {
        return new SelectMenuBuilder()
            .WithCustomId(customId)
            .WithOptions(filters.Select(y =>
                new SelectMenuOptionBuilder().WithLabel(y.Filter).WithDescription(y.Template)
                    .WithValue(y.Id.ToString())).ToList());
    }

    [ComponentInteraction(ModulePrefixes.CONFIG_FILTERS_REMOVE_BUTTON)]
    [RequireUserPermission(GuildPermission.ManageGuild, Group = BaseModulePrefixes.PERMISSION_GROUP)]
    [HasOverride(Group = BaseModulePrefixes.PERMISSION_GROUP)]
    private async Task RemoveButton()
    {
        await DeferAsync();

        var filters = await mangaService.GetFilters(Context.Guild.Id);

        await ModifyOriginalResponseAsync(x =>
        {
            x.Components = new ComponentBuilder()
                .WithSelectMenu(GetFilterSelectMenu(ModulePrefixes.CONFIG_FILTERS_REMOVE_FILTER_SELECT, filters))
                .WithButton("Back", BaseModulePrefixes.CONFIG_PAGE_SELECT_PAGE_BUTTON + StateSerializer.SerializeObject(Id), ButtonStyle.Secondary)
                .Build();
        });
    }

    [ComponentInteraction(ModulePrefixes.CONFIG_FILTERS_REMOVE_FILTER_SELECT)]
    [RequireUserPermission(GuildPermission.ManageGuild, Group = BaseModulePrefixes.PERMISSION_GROUP)]
    [HasOverride(Group = BaseModulePrefixes.PERMISSION_GROUP)]
    private async Task RemoveFilterSelectChanged(string id)
    {
        await DeferAsync();

        var filter = await mangaService.GetFilter(uint.Parse(id), Context.Guild.Id);

        if (filter == null)
        {
            await FollowupAsync("That filter no longer exists anyway!", ephemeral: true);
            return;
        }

        await mangaService.RemoveFilter(filter);

        await ModifyOriginalResponseAsync(await GetMessageContents(new ConfigCommandServiceBase<Page>.State(page: Id)));
    }

    [ComponentInteraction(ModulePrefixes.CONFIG_FILTERS_EDIT_BUTTON)]
    [RequireUserPermission(GuildPermission.ManageGuild, Group = BaseModulePrefixes.PERMISSION_GROUP)]
    [HasOverride(Group = BaseModulePrefixes.PERMISSION_GROUP)]
    private async Task EditButton()
    {
        await DeferAsync();

        var filters = await mangaService.GetFilters(Context.Guild.Id);

        await ModifyOriginalResponseAsync(x =>
        {
            x.Components = new ComponentBuilder()
                .WithSelectMenu(GetFilterSelectMenu(ModulePrefixes.CONFIG_FILTERS_EDIT_FILTER_SELECT, filters))
                .WithButton("Back", BaseModulePrefixes.CONFIG_PAGE_SELECT_PAGE_BUTTON + StateSerializer.SerializeObject(Id), ButtonStyle.Secondary)
                .Build();
        });
    }

    [ComponentInteraction(ModulePrefixes.CONFIG_FILTERS_EDIT_FILTER_SELECT)]
    [RequireUserPermission(GuildPermission.ManageGuild, Group = BaseModulePrefixes.PERMISSION_GROUP)]
    [HasOverride(Group = BaseModulePrefixes.PERMISSION_GROUP)]
    private async Task EditFilterSelectChanged(string id)
    {
        await DeferAsync();

        var filter = await mangaService.GetFilter(uint.Parse(id), Context.Guild.Id);

        if (filter == null)
        {
            await FollowupAsync("That filter no longer exists!", ephemeral: true);
            return;
        }

        await ModifyOriginalResponseAsync(UpsertConfirmation(filter));
    }

    [ComponentInteraction(ModulePrefixes.CONFIG_FILTERS_OPEN_MODAL_BUTTON + "*")]
    [RequireUserPermission(GuildPermission.ManageGuild, Group = BaseModulePrefixes.PERMISSION_GROUP)]
    [HasOverride(Group = BaseModulePrefixes.PERMISSION_GROUP)]
    public async Task OpenModalButton(uint id)
    {
        RegexFilter? filter = null;

        // stupid but i cant get the original message
        if (id != 0ul)
        {
            filter = await mangaService.GetFilter(id, Context.Guild.Id);
            if (filter == null)
            {
                await RespondAsync("That filter no longer exists!", ephemeral: true);
                return;
            }
        }

        await Context.Interaction.RespondWithModalAsync<SetRegexModal>(ModulePrefixes.CONFIG_FILTERS_MODAL + (id != 0ul), modifyModal:
            x =>
            {
                if (filter == null) return;

                x.UpdateTextInput(ModulePrefixes.CONFIG_FILTERS_MODAL_FILTER_TEXTBOX, i =>
                {
                    i.Value = filter.Filter;
                });
                x.UpdateTextInput(ModulePrefixes.CONFIG_FILTERS_MODAL_TEMPLATE_TEXTBOX, i =>
                {
                    i.Value = filter.Template;
                });
            });
    }

    [ModalInteraction(ModulePrefixes.CONFIG_FILTERS_MODAL + "*")]
    public async Task ModalResponse(bool existing, SetRegexModal modal)
    {
        await DeferAsync();

        RegexFilter? filter = null;

        if (existing)
        {
            filter = await GetRegexFilterFromContext();

            filter.Filter = modal.Filter;
            filter.Template = modal.Template;
        }

        filter ??= new RegexFilter(id: 0, guildId: Context.Guild.Id, filter: modal.Filter, template: modal.Template,
            filterType: FilterType.Block, channelFilterScope: ChannelFilterScope.Exclude
            );

        var contents = UpsertConfirmation(filter);

        await ModifyOriginalResponseAsync(contents);
    }

    [ComponentInteraction(ModulePrefixes.CONFIG_FILTERS_CONFIRMATION_ADD_BUTTON)]
    [RequireUserPermission(GuildPermission.ManageGuild, Group = BaseModulePrefixes.PERMISSION_GROUP)]
    [HasOverride(Group = BaseModulePrefixes.PERMISSION_GROUP)]
    private async Task ConfirmationAddButton()
    {
        await DeferAsync();

        var filter = await GetRegexFilterFromContext();

        var newId = await mangaService.UpdateOrAddRegexFilter(filter);

        if (newId == 0ul)
        {
            await FollowupAsync("Just noting nothing was actually added or changed as the filter no longer exists.",
                ephemeral: true);
        }

        await ModifyOriginalResponseAsync(await configCommandService.GetMessageContents(
            new ConfigCommandServiceBase<Page>.State(page: Id), Context));
    }

    private MessageContents UpsertConfirmation(RegexFilter filter)
    {
        var embed = new EmbedBuilder().WithColor(CommandResult.Default);

        var channels = filter.RegexChannelEntries;

        embed.WithFields
        (
            new EmbedFieldBuilder()
                .WithName(EMBED_NAME_TEMPLATE)
                .WithValue($"```{filter.Template}```"),
            new EmbedFieldBuilder()
                .WithName(EMBED_NAME_FILTER)
                .WithValue($"```{filter.Filter}```"),
            new EmbedFieldBuilder()
                .WithName(EMBED_NAME_CHANNELS)
                .WithValue(channels
                    .Aggregate("", (current, next) => current + $"<#{next.ChannelId}>{Environment.NewLine}")
                    .StringOrDefault("None!")),
            new EmbedFieldBuilder()
                .WithName(EMBED_NAME_FILTER_TYPE)
                .WithValue(filter.FilterType.Humanize())
                .WithIsInline(true),
            new EmbedFieldBuilder()
                .WithName(EMBED_NAME_SCOPE)
                .WithValue(filter.ChannelFilterScope.Humanize())
                .WithIsInline(true)
        );

        embed.WithFooter(new EmbedFooterBuilder()
            .WithText(filter.Id.ToString()));

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
                            .WithValue(StateSerializer.SerializeObject(x))
                            ).ToList()
                    )
                )
            .WithSelectMenu(new SelectMenuBuilder()
                .WithCustomId(ModulePrefixes.CONFIG_FILTERS_CONFIRMATION_CHANNEL_SCOPE)
                .WithOptions(
                    Enum.GetValues<ChannelFilterScope>().Select(x =>
                        new SelectMenuOptionBuilder()
                            .WithDefault(x == filter.ChannelFilterScope)
                            .WithLabel("Channel scope: " + x.Humanize())
                            // ehh i could do attributes but this is way easier (if against the whole, easily expandable principle ive been doing)
                            // TODO: should be reusable tho for like help pages and stuff
                            .WithDescription(x switch
                            {
                                ChannelFilterScope.Include => "The filter will only apply in the specified channels.",
                                ChannelFilterScope.Exclude => "The filter will apply in every channel except the specified channels.",
                                _ => "looks like i forgot a description??",
                            })
                            .WithValue(StateSerializer.SerializeObject(x))
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
            // Rare case where button state is not used and we're gonna grab from the embed etc instead (100 characters too small for what people probably need)
            .WithButton(new ButtonBuilder()
                .WithLabel("Save")
                .WithCustomId(ModulePrefixes.CONFIG_FILTERS_CONFIRMATION_ADD_BUTTON)
                .WithStyle(ButtonStyle.Success))
            .WithButton(new ButtonBuilder()
                .WithLabel("Edit")
                .WithCustomId(ModulePrefixes.CONFIG_FILTERS_OPEN_MODAL_BUTTON + filter.Id)
                .WithStyle(ButtonStyle.Secondary))
            .WithButton(new ButtonBuilder()
                .WithLabel("Back")
                .WithCustomId(BaseModulePrefixes.CONFIG_PAGE_SELECT_PAGE_BUTTON +
                              StateSerializer.SerializeObject(StateSerializer.SerializeObject(Id)))

                .WithStyle(ButtonStyle.Danger))
            ;

        return new MessageContents(string.Empty, embed.Build(), components);
    }

    [ComponentInteraction(ModulePrefixes.CONFIG_FILTERS_CONFIRMATION_CHANNEL_SELECT)]
    [RequireUserPermission(GuildPermission.ManageGuild, Group = BaseModulePrefixes.PERMISSION_GROUP)]
    [HasOverride(Group = BaseModulePrefixes.PERMISSION_GROUP)]
    private async Task UpsertConfirmationChannelSelectChanged(IChannel[] channels)
    {
        await DeferAsync();

        var filter = await GetRegexFilterFromContext(channels.Select(x => new RegexChannelEntry { ChannelId = x.Id}).ToList());

        await ModifyOriginalResponseAsync(UpsertConfirmation(filter));
    }

    [ComponentInteraction(ModulePrefixes.CONFIG_FILTERS_CONFIRMATION_FILTER_TYPE)]
    private async Task UpsertConfirmationFilterTypeChanged(string id)
    {
        await DeferAsync();

        var filter = await GetRegexFilterFromContext();

        filter.FilterType = StateSerializer.DeserializeObject<FilterType>(id);

        await ModifyOriginalResponseAsync(UpsertConfirmation(filter));
    }

    [ComponentInteraction(ModulePrefixes.CONFIG_FILTERS_CONFIRMATION_CHANNEL_SCOPE)]
    [RequireUserPermission(GuildPermission.ManageGuild, Group = BaseModulePrefixes.PERMISSION_GROUP)]
    [HasOverride(Group = BaseModulePrefixes.PERMISSION_GROUP)]
    private async Task UpsertConfirmationChannelScopeChanged(string id)
    {
        await DeferAsync();

        var filter = await GetRegexFilterFromContext();

        filter.ChannelFilterScope = StateSerializer.DeserializeObject<ChannelFilterScope>(id);

        await ModifyOriginalResponseAsync(UpsertConfirmation(filter));
    }

    private async Task<RegexFilter> GetRegexFilterFromContext(List<RegexChannelEntry>? channels = null)
    {
        var ogRes = await Context.Interaction.GetOriginalResponseAsync();

        var embed = ogRes.Embeds.First(x => x.Type == EmbedType.Rich);

        var template = CodeBlockMatcher().Match(embed.Fields.First(x => x.Name == EMBED_NAME_TEMPLATE).Value).Groups[1].Value;
        var filter = CodeBlockMatcher().Match(embed.Fields.First(x => x.Name == EMBED_NAME_FILTER).Value).Groups[1].Value;
        var filterType = embed.Fields.First(x => x.Name == EMBED_NAME_FILTER_TYPE).Value.DehumanizeTo<FilterType>();
        var scope = embed.Fields.First(x => x.Name == EMBED_NAME_SCOPE).Value.DehumanizeTo<ChannelFilterScope>();

        channels ??= ChannelMatcher().Matches(embed.Fields.First(x => x.Name == EMBED_NAME_CHANNELS).Value).Select(x => new RegexChannelEntry
        {
            ChannelId = ulong.Parse(x.Groups[1].Value)
        }).ToList();

        var idTxt = embed.Footer?.Text ?? throw new InvalidOperationException("Footer missing?");
        var id = uint.Parse(idTxt);

        return new RegexFilter(id: id, guildId: Context.Guild.Id, filter: filter,
            template: template, filterType: filterType, channelFilterScope: scope,
            regexChannelEntries: channels);
    }

    [GeneratedRegex(@"<#(\d+)>")]
    private static partial Regex ChannelMatcher();

    [GeneratedRegex(@"```([^`]*)```")]
    private static partial Regex CodeBlockMatcher();
}
