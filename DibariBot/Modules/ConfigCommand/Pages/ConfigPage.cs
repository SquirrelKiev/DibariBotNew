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

#nullable disable
    protected Dictionary<Page, ConfigPage> ConfigPages { get; private set; }
#nullable enable

    public abstract Task<MessageContents> GetMessageContents(ConfigCommandService.State state);

    public virtual ConfigPage Initialize(Dictionary<Page, ConfigPage> configPages)
    {
        ConfigPages = configPages;

        return this;
    }
}
