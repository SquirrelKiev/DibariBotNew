using BotBase.Modules.ConfigCommand;

namespace DibariBot.Modules.ConfigCommand.Pages;

public abstract class ConfigPage : ConfigPageBase<ConfigPage.Page>
{
    public enum Page
    {
        Help,
        DefaultManga,
        RegexFilters,
        Prefix
    }
}
