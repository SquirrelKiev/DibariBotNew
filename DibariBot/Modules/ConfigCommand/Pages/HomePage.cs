using Discord;

namespace DibariBot.Modules.ConfigCommand.Pages;

[ConfigPage(Page.Help, "Help", "Brings up information about each config page.", Conditions.None)]
public class HomePage(ConfigCommandService configCommandService, ColorProvider colorProvider) : BotModule, IConfigPage
{
    public async Task<MessageContents> GetMessageContents(ConfigCommandService.State state)
    {
        var embed = new EmbedBuilder()
            .WithColor(await colorProvider.GetEmbedColor(Context.Guild));

        foreach (var page in configCommandService.FilteredConfigPages(IsUserInstallInteraction()))
        {
            embed.AddField(page.ConfigPageAttribute.Label, page.ConfigPageAttribute.Description);
        }

        var components = new ComponentBuilder()
            .WithSelectMenu(configCommandService.GetPageSelectDropdown(Page.Help, IsUserInstallInteraction()))
            .WithRedButton();

        return await Task.FromResult(new MessageContents("", embed.Build(), components));
    }
}
