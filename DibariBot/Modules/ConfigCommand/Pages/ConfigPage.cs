namespace DibariBot.Modules.ConfigCommand.Pages;

public abstract class ConfigPage : BotModule
{
    public enum Page
    {
        Help,
        DefaultManga,
        RegexFilters,
        Prefix
    }

    public abstract Page Id { get; }
    public abstract string Label { get; }
    public abstract string Description { get; }
    public abstract bool EnabledInDMs { get; }
    
    // isDm is here because context isn't injected for some of these.
    // TODO: move ID, label, etc. to attributes instead of abstract properties.
    public bool ShouldShow(bool isDm)
    {
        return !isDm || EnabledInDMs;
    }

    public abstract Task<MessageContents> GetMessageContents(ConfigCommandService.State state);

    public SelectMenuBuilder GetPageSelectDropdown(Dictionary<Page, ConfigPage> pages, Page id, bool isDm)
    {
        var dropdown = new SelectMenuBuilder()
            .WithCustomId(ModulePrefixes.CONFIG_PAGE_SELECT_PAGE);

        foreach (var page in pages.Values.Where(page => page.ShouldShow(IsDm())))
        {
            dropdown
                .AddOption(new SelectMenuOptionBuilder()
                    .WithLabel(page.Label)
                    .WithValue(StateSerializer.SerializeObject(page.Id))
                    .WithDefault(page.Id.Equals(id))
                    .WithDescription(page.Description));
        }

        return dropdown;
    }
}
