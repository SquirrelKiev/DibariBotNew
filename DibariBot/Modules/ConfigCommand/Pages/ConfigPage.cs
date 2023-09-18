namespace DibariBot.Modules.ConfigCommand.Pages;

public abstract class ConfigPage : DibariModule
{
    public enum Page
    {
        Help,
        DefaultManga
    }

    public abstract Page Id { get; }
    public abstract string Label { get; }
    public abstract string Description { get; }

    public abstract Task<MessageContents> GetMessageContents(ConfigCommandService.State state);
}
