using DibariBot.Database;

namespace DibariBot.Modules.ConfigCommand.Pages;

public class RegexFiltersPage : ConfigPage
{
    public class SetRegexModal : IModal
    {
        public string Title => "Set Regex";

        [ModalTextInput(customId: ModulePrefixes.CONFIG_REGEX_FILTERS_MODAL_REGEX_TEXTBOX)]
        public string Regex { get; set; } = "";

        [ModalTextInput(customId: ModulePrefixes.CONFIG_REGEX_FILTERS_MODAL_TEMPLATE_TEXTBOX)]
        public string Template { get; set; } = "";
    }

    public override Page Id => Page.RegexFilters;

    public override string Label => "Regex filters";

    public override string Description => "TODO";

    private readonly DbService dbService;
    private readonly ConfigCommandService configCommandService;
    private readonly BotConfig config;
    
    public RegexFiltersPage(DbService db, ConfigCommandService configCommandService, BotConfig config)
    {
        dbService = db;
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
        await Context.Interaction.RespondWithModalAsync<SetRegexModal>(ModulePrefixes.CONFIG_REGEX_FILTERS_MODAL + );
    }

    [ModalInteraction(ModulePrefixes.CONFIG_REGEX_FILTERS_MODAL + "*")]
    public async Task AddModalResponse(SetRegexModal modal, uint id)
    {
        await DeferAsync();
    }
}
