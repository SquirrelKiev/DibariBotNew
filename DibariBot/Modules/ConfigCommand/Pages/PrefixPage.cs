using DibariBot.Database;
using DibariBot.Database.Extensions;

namespace DibariBot.Modules.ConfigCommand.Pages;

public class PrefixPage : ConfigPage
{
    public class SetPrefixModal : IModal
    {
        public string Title => "Set Prefix";

        [ModalTextInput(customId: ModulePrefixes.CONFIG_PREFIX_MODAL_PREFIX_TEXTBOX, minLength: 1, maxLength: 10)]
        public string Prefix { get; set; } = "";
    }

    public override Page Id => Page.Prefix;
    
    public override string Label => "Prefix";

    public override string Description => "What prefix to use for prefix commands.";

    public override bool EnabledInDMs => false;

    private readonly ConfigCommandService configCommandService;
    private readonly DbService dbService;

    public PrefixPage(ConfigCommandService configCommandService, DbService dbService)
    {
        this.configCommandService = configCommandService;
        this.dbService = dbService;
    }

    public override async Task<MessageContents> GetMessageContents(ConfigCommandService.State state)
    {
        var prefix = await dbService.GetPrefix(Context.Guild.Id);

        var embed = new EmbedBuilder()
            .WithFields(new EmbedFieldBuilder()
                .WithName("Prefix")
                .WithValue($"`{prefix}`"))
            .WithColor(CommandResult.Default);

        var components = new ComponentBuilder()
            .WithSelectMenu(ConfigPageUtility.GetPageSelectDropdown(configCommandService.ConfigPages, Id, IsDm()))
            .WithButton("Change Prefix", ModulePrefixes.CONFIG_PREFIX_MODAL_BUTTON, ButtonStyle.Secondary)
            .WithRedButton();

        return new MessageContents(string.Empty, embed.Build(), components);
    }

    [ComponentInteraction(ModulePrefixes.CONFIG_PREFIX_MODAL_BUTTON)]
    public async Task OnChangeButton()
    {
        var prefix = await dbService.GetPrefix(Context.Guild.Id);

        await Context.Interaction.RespondWithModalAsync<SetPrefixModal>(ModulePrefixes.CONFIG_PREFIX_MODAL, modifyModal:
            builder =>
            {
                builder.UpdateTextInput(ModulePrefixes.CONFIG_PREFIX_MODAL_PREFIX_TEXTBOX,
                    input => input.Value = prefix);
            });
    }

    [ModalInteraction(ModulePrefixes.CONFIG_PREFIX_MODAL)]
    public async Task OnModal(SetPrefixModal modal)
    {
        await DeferAsync();

        await dbService.SetPrefix(Context.Guild.Id, modal.Prefix);

        await ModifyOriginalResponseAsync(
            await configCommandService.GetMessageContents(new ConfigCommandService.State(page: Id), Context));
    }
}