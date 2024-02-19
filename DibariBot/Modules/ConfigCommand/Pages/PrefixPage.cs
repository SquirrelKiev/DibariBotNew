using BotBase;
using BotBase.Database;
using BotBase.Modules;
using BotBase.Modules.ConfigCommand;
using DibariBot.Database;

namespace DibariBot.Modules.ConfigCommand.Pages;

public class PrefixPage : ConfigPage
{
    public override Page Id => Page.Prefix;
    
    public override string Label => "Prefix";

    public override string Description => "What prefix to use for prefix commands.";

    public override bool EnabledInDMs => false;

    private readonly ConfigCommandServiceBase<Page> configCommandService;
    private readonly PrefixPageImpl<Page, BotDbContext> prefixPageImpl;

    public PrefixPage(ConfigCommandServiceBase<Page> configCommandService, DbService dbService, BotConfigBase botConfig)
    {
        this.configCommandService = configCommandService;

        prefixPageImpl = new PrefixPageImpl<Page,BotDbContext>(configCommandService, dbService, this, botConfig);
    }

    public override Task<MessageContents> GetMessageContents(ConfigCommandServiceBase<Page>.State state)
    {
        return prefixPageImpl.GetMessageContents();
    }

    [ComponentInteraction(BaseModulePrefixes.CONFIG_PREFIX_MODAL_BUTTON)]
    [RequireUserPermission(GuildPermission.ManageGuild, Group = BaseModulePrefixes.PERMISSION_GROUP)]
    [HasOverride(Group = BaseModulePrefixes.PERMISSION_GROUP)]
    public Task OnChangeButton()
    {
        return prefixPageImpl.OnChangeButton();
    }

    [ModalInteraction(BaseModulePrefixes.CONFIG_PREFIX_MODAL)]
    [RequireUserPermission(GuildPermission.ManageGuild, Group = BaseModulePrefixes.PERMISSION_GROUP)]
    [HasOverride(Group = BaseModulePrefixes.PERMISSION_GROUP)]
    public async Task OnModal(SetPrefixModal modal)
    {
        await DeferAsync();

        await prefixPageImpl.OnModal(modal);

        await ModifyOriginalResponseAsync(
            await configCommandService.GetMessageContents(new ConfigCommandServiceBase<Page>.State(page: Id), Context));
    }
}