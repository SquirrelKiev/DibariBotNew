﻿namespace DibariBot.Modules.ConfigCommand.Pages;

public static class ConfigPageUtility
{
    public static SelectMenuBuilder GetPageSelectDropdown(Dictionary<ConfigPage.Page, ConfigPage> pages, ConfigPage.Page id, bool isDm)
    {
        var dropdown = new SelectMenuBuilder()
                .WithCustomId(ModulePrefixes.CONFIG_PAGE_SELECT_PAGE);

        foreach (var page in pages.Values.Where(page => page.ShouldShow(isDm)))
        {
            dropdown
                .AddOption(new SelectMenuOptionBuilder()
                    .WithLabel(page.Label)
                    .WithValue(StateSerializer.SerializeObject(page.Id))
                    .WithDefault(page.Id == id)
                    .WithDescription(page.Description));
        }

        return dropdown;
    }
}
