using BotBase.Modules;
using BotBase;
using BotBase.Modules.ConfigCommand;
using DibariBot.Modules.ConfigCommand.Pages;

namespace DibariBot.Modules.ConfigCommand;

public class ConfigCommandModule : ConfigCommandModuleBase<ConfigPage.Page>
{
    public ConfigCommandModule(ConfigCommandServiceBase<ConfigPage.Page> configService) : base(configService)
    {
    }

    [SlashCommand("config", "Pulls up various options for configuring the bot to the server's needs.")]
    [RequireUserPermission(GuildPermission.ManageGuild, Group = BaseModulePrefixes.PERMISSION_GROUP)]
    [RequireContext(ContextType.DM | ContextType.Group, Group = BaseModulePrefixes.PERMISSION_GROUP)]
    [HasOverride(Group = BaseModulePrefixes.PERMISSION_GROUP)]
    [EnabledInDm(true)]
    public override Task ConfigSlash()
    {
        return base.ConfigSlash();
    }
}