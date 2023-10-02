namespace DibariBot.Modules.ConfigCommand.Pages;

public abstract class ConfigPage : DibariModule
{
    public enum Page
    {
        Help,
        DefaultManga,
        RegexFilters
    }

    public abstract Page Id { get; }
    // could probably be replaced with humanizer stuff and use the enum but i like it mostly being in all one place
    public abstract string Label { get; }
    public abstract string Description { get; }

    public abstract Task<MessageContents> GetMessageContents(ConfigCommandService.State state);
}
