using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DibariBot.Modules.ConfigCommand.Pages;

public class DefaultMangaPage : ConfigPage
{
    public override Page Id => Page.DefaultManga;

    public override string Label => "Default manga";

    public override string Description => "Change the manga that opens when no URL is specified. Can be per-server and per-channel.";

    public override Task<MessageContents> GetMessageContents(ConfigCommandService.State state)
    {
        // TODO
        var components = new ComponentBuilder()
            .WithSelectMenu(ConfigPageUtility.GetPageSelectDropdown(configPages, Id));

        return Task.FromResult(new MessageContents("", embed: null, components.Build()));
    }
}
