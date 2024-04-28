using BotBase.Modules;
using BotBase;
using BotBase.Modules.ConfigCommand;
using DibariBot.Modules.ConfigCommand.Pages;

namespace DibariBot.Modules.ConfigCommand;

[CommandContextType(InteractionContextType.Guild, InteractionContextType.BotDm, InteractionContextType.PrivateChannel)]
[IntegrationType(ApplicationIntegrationType.GuildInstall, ApplicationIntegrationType.UserInstall)]
[RequireUserPermission(GuildPermission.ManageGuild, Group = BaseModulePrefixes.PERMISSION_GROUP)]
[RequireContext(ContextType.DM | ContextType.Group, Group = BaseModulePrefixes.PERMISSION_GROUP)]
[HasOverride(Group = BaseModulePrefixes.PERMISSION_GROUP)]
public class ConfigCommandModule(ConfigCommandServiceBase<ConfigPage.Page> configService)
    : ConfigCommandModuleBase<ConfigPage.Page>(configService)
{
    [SlashCommand("config", "Pulls up various options for configuring the bot.")]
    public override Task ConfigSlash()
    {
        return base.ConfigSlash();
    }
}