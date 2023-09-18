﻿namespace DibariBot.Modules.ConfigCommand.Pages;

public class HomePage : ConfigPage
{
    public override Page Id => Page.Help;

    public override string Label => "Help";

    public override string Description => "Brings up information about each config page.";

    private readonly ConfigCommandService configCommandService;

    public HomePage(ConfigCommandService configCommandService)
    {
        this.configCommandService = configCommandService;
    }

    public override Task<MessageContents> GetMessageContents(ConfigCommandService.State state)
    {
        var embed = new EmbedBuilder();

        foreach(var page in configCommandService.ConfigPages.Values)
        {
            embed.AddField(page.Label, page.Description);
        }

        var components = new ComponentBuilder()
            .WithSelectMenu(ConfigPageUtility.GetPageSelectDropdown(configCommandService.ConfigPages, Id))
            .WithRedButton();

        return Task.FromResult(new MessageContents("", embed.Build(), components));
    }
}