namespace DibariBot.Modules.ConfigCommand.Pages;

public abstract class ConfigPage
{
    public enum Page
    {
        Help,
        DefaultManga
    }

    public abstract Page Id { get; }
    public abstract string Label { get; }
    public abstract string Description { get; }

#nullable disable
    protected Dictionary<Page, ConfigPage> configPages;
#nullable enable

    public abstract Task<MessageContents> GetMessageContents(ConfigCommandService.State state);

    public virtual ConfigPage Initialize(Dictionary<Page, ConfigPage> configPages)
    {
        this.configPages = configPages;

        return this;
    }
}
