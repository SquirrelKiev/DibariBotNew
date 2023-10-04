namespace DibariBot.Modules.ConfigCommand;

public class ConfigCommandModule : DibariModule
{
    private const string PERMISSION_GROUP = "'User has Manage Guild or is in DM'";
    private readonly ConfigCommandService configService;
    
    public ConfigCommandModule(ConfigCommandService configService)
    {
        this.configService = configService;
    }

    [SlashCommand("manga-config", "Pulls up various options for configuring the bot to the server's needs.")]
    [RequireUserPermission(GuildPermission.ManageGuild, Group = PERMISSION_GROUP)]
    [RequireContext(ContextType.DM | ContextType.Group, Group = PERMISSION_GROUP)]
    [EnabledInDm(true)]
    public async Task ConfigSlash()
    {
        await DeferAsync();

        await FollowupAsync(await configService.GetMessageContents(new(page: Pages.ConfigPage.Page.Help, data: ""), Context));
    }

    [ComponentInteraction(ModulePrefixes.CONFIG_PAGE_SELECT_PAGE)]
    public async Task SelectInteraction(string id)
    {
        await DeferAsync();

        var page = StateSerializer.DeserializeObject<Pages.ConfigPage.Page>(id);

        await ModifyOriginalResponseAsync(await configService.GetMessageContents(new(page: page, data: ""), Context));
    }

    [ComponentInteraction(ModulePrefixes.CONFIG_PAGE_SELECT_PAGE_BUTTON + "*")]
    public Task SelectInteractionButton(string id)
    {
        return SelectInteraction(id);
    }
}
