namespace DibariBot.Modules.ConfigCommand;

public class ConfigCommandModule : DibariModule
{
    private readonly ConfigCommandService configService;

    public ConfigCommandModule(ConfigCommandService configService)
    {
        this.configService = configService;
    }

    [SlashCommand("manga-config", "Pulls up various options for configuring the bot to the server's needs.")]
    [RequireUserPermission(GuildPermission.ManageGuild, Group = ModulePrefixes.PERMISSION_GROUP)]
    [RequireContext(ContextType.DM | ContextType.Group, Group = ModulePrefixes.PERMISSION_GROUP)]
    [HasOverride(Group = ModulePrefixes.PERMISSION_GROUP)]
    [EnabledInDm(true)]
    public async Task ConfigSlash()
    {
        await DeferAsync();

        await FollowupAsync(await configService.GetMessageContents(new(page: Pages.ConfigPage.Page.Help, data: ""), Context));
    }

    [ComponentInteraction(ModulePrefixes.CONFIG_PAGE_SELECT_PAGE)]
    [RequireUserPermission(GuildPermission.ManageGuild, Group = ModulePrefixes.PERMISSION_GROUP)]
    [HasOverride(Group = ModulePrefixes.PERMISSION_GROUP)]
    public async Task SelectInteraction(string id)
    {
        await DeferAsync();

        var page = StateSerializer.DeserializeObject<Pages.ConfigPage.Page>(id);

        await ModifyOriginalResponseAsync(await configService.GetMessageContents(new(page: page, data: ""), Context));
    }

    [ComponentInteraction(ModulePrefixes.CONFIG_PAGE_SELECT_PAGE_BUTTON + "*")]
    [RequireUserPermission(GuildPermission.ManageGuild, Group = ModulePrefixes.PERMISSION_GROUP)]
    [HasOverride(Group = ModulePrefixes.PERMISSION_GROUP)]
    public Task SelectInteractionButton(string id)
    {
        return SelectInteraction(id);
    }
}
