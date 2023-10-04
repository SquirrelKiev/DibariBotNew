using static DibariBot.Modules.ConfigCommand.Pages.ConfigPage;

namespace DibariBot.Modules.ConfigCommand.Pages;

public abstract class ConfigPage : DibariModule
{
    public enum Page
    {
        Help,
        DefaultManga,
        RegexFilters,
        Prefix
    }

    public abstract Page Id { get; }
    // could probably be replaced with humanizer stuff and use the enum but i like it mostly being in all one place
    public abstract string Label { get; }
    public abstract string Description { get; }
    public abstract bool EnabledInDMs { get; }

    public bool ShouldShow(bool isDm)
    {
        return !isDm || EnabledInDMs;
    }

    public abstract Task<MessageContents> GetMessageContents(ConfigCommandService.State state);
}
