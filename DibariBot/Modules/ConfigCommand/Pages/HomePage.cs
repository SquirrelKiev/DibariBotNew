﻿namespace DibariBot.Modules.ConfigCommand.Pages;

public class HomePage : ConfigPage
{
    public override Page Id => Page.Help;

    public override string Label => "Help";

    public override string Description => "Brings up information about each config page.";

    public override bool EnabledInDMs => true;

    private readonly ConfigCommandService configCommandService;

    public HomePage(ConfigCommandService configCommandService)
    {
        this.configCommandService = configCommandService;
    }

    public override Task<MessageContents> GetMessageContents(ConfigCommandService.State state)
    {
        var embed = new EmbedBuilder()
            .WithColor(CommandResult.Default);

        foreach(var page in configCommandService.ConfigPages.Values.Where(page => page.ShouldShow(IsDm())))
        {
            embed.AddField(page.Label, page.Description);
        }

        var components = new ComponentBuilder()
            .WithSelectMenu(ConfigPageUtility.GetPageSelectDropdown(configCommandService.ConfigPages, Id, IsDm()))
            .WithRedButton();

        return Task.FromResult(new MessageContents("", embed.Build(), components));
    }
}
