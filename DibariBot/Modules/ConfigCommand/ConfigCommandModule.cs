using DibariBot.Modules.ConfigCommand.Pages;
using Discord.Interactions;

namespace DibariBot.Modules.ConfigCommand;

[CommandContextType(InteractionContextType.Guild, InteractionContextType.BotDm, InteractionContextType.PrivateChannel)]
[IntegrationType(ApplicationIntegrationType.GuildInstall, ApplicationIntegrationType.UserInstall)]
[DefaultMemberPermissions(GuildPermission.ManageGuild)]
public class ConfigCommandModule(ConfigCommandService configService) : BotModule
{
    [ComponentInteraction(ModulePrefixes.CONFIG_PAGE_SELECT_PAGE)]
    public async Task SelectInteraction(string id)
    {
        await DeferAsync();

        var page = StateSerializer.DeserializeObject<ConfigPage.Page>(id)!;

        await ModifyOriginalResponseAsync(await configService.GetMessageContents(new(page: page, data: ""), Context));
    }

    [ComponentInteraction(ModulePrefixes.CONFIG_PAGE_SELECT_PAGE_BUTTON + "*")]
    public Task SelectInteractionButton(string id)
    {
        return SelectInteraction(id);
    }

    [SlashCommand("config", "Pulls up various options for configuring the bot.")]
    public async Task ConfigSlash()
    {
        await DeferAsync();

        await FollowupAsync(await configService.GetMessageContents(new(), Context));
    }
}
