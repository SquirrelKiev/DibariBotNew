using DibariBot.Core.Database.Models;
using DibariBot.Modules.Manga;
using Humanizer;

namespace DibariBot.Modules.ConfigCommand.Pages;

public class RegexFiltersPage : ConfigPage
{
    public class SetRegexModal : IModal
    {
        public string Title => "Set Regex";

        [ModalTextInput(customId: ModulePrefixes.CONFIG_REGEX_FILTERS_MODAL_TEMPLATE_TEXTBOX)]
        public string Template { get; set; } = "";

        [ModalTextInput(customId: ModulePrefixes.CONFIG_REGEX_FILTERS_MODAL_REGEX_TEXTBOX)]
        public string Regex { get; set; } = "";
    }

    public override Page Id => Page.RegexFilters;

    public override string Label => "Regex filters";

    public override string Description => "TODO";

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

        embed.WithDescription("TODO");

        var components = new ComponentBuilder()
            .WithSelectMenu(ConfigPageUtility.GetPageSelectDropdown(configCommandService.ConfigPages, Id))
            .WithButton(new ButtonBuilder()
                .WithLabel("Add")
                .WithCustomId($"{ModulePrefixes.CONFIG_REGEX_FILTERS_ADD_BUTTON}")
                .WithStyle(config.PrimaryButtonStyle))
            .WithButton(new ButtonBuilder()
                .WithLabel("Edit")
                .WithCustomId($"{ModulePrefixes.CONFIG_REGEX_FILTERS_EDIT}")
                .WithStyle(config.PrimaryButtonStyle))
            .WithButton(new ButtonBuilder()
                .WithLabel("Remove")
                .WithCustomId($"{ModulePrefixes.CONFIG_REGEX_FILTERS_REMOVE}")
                .WithStyle(config.PrimaryButtonStyle))
            .WithRedButton();

        return new MessageContents("", embed.Build(), components);
    }

    [ComponentInteraction(ModulePrefixes.CONFIG_REGEX_FILTERS_ADD_BUTTON)]
    public async Task AddButton()
    {
        await Context.Interaction.RespondWithModalAsync<SetRegexModal>(ModulePrefixes.CONFIG_REGEX_FILTERS_MODAL + 0u);
    }

    [ModalInteraction(ModulePrefixes.CONFIG_REGEX_FILTERS_MODAL + "*")]
    public async Task ModalResponse(uint regexKey, SetRegexModal modal)
    {
        await DeferAsync();

        var contents = await UpsertConfirmation(regexKey, modal.Template, modal.Regex, FilterType.Block);

        await ModifyOriginalResponseAsync(contents);
    }

    private async Task<MessageContents> UpsertConfirmation(uint regexKey, string template, string regex, FilterType filterType)
    {
        var filter = await mangaService.GetFilter(regexKey);

        filter ??= new RegexFilter()
        {
            Id = 0,
            GuildId = Context.Guild.Id,
            Regex = regex,
            Template = template,
            FilterType = filterType,
        };

        filter.FilterType = filterType;

        var embed = new EmbedBuilder();

        embed.WithFields
            (
            new EmbedFieldBuilder()
                .WithName("Template")
                .WithValue(template),
            new EmbedFieldBuilder()
                .WithName("Regex")
                .WithValue(regex)
            );

        var components = new ComponentBuilder()
            .WithSelectMenu(new SelectMenuBuilder()
                .WithCustomId(ModulePrefixes.CONFIG_REGEX_FILTERS_CONFIRMATION_FILTER_TYPE)
                .WithOptions(
                    Enum.GetValues<FilterType>().Select(x =>
                        new SelectMenuOptionBuilder()
                            .WithDefault(x == filterType)
                            .WithLabel("Filter type: " + x.Humanize())
                            // ehh i could do attributes but this is way easier (if against the whole, easily expandable principle ive been doing)
                            // TODO: should be reusable tho for like help pages and stuff
                            .WithDescription(x switch
                            {
                                FilterType.Allow => "If the manga matches any allows, it can be shown.",
                                FilterType.Block => "If the manga matches any blocks, it will not be shown.",
                                _ => "looks like i forgot a description??",
                            })
                            .WithValue(x.ToString())
                            ).ToList())
                )
            .WithButton("Add", ModulePrefixes.CONFIG_REGEX_FILTERS_CONFIRMATION_ADD_BUTTON)
            .WithButton("Back", ModulePrefixes.CONFIG_PAGE_SELECT_PAGE_BUTTON +
                    StateSerializer.SerializeObject(StateSerializer.SerializeObject(Id))
                );

        return new MessageContents(string.Empty, embed.Build(), components);
    }
}
