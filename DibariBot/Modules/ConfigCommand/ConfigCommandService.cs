using DibariBot.Modules.ConfigCommand.Pages;
using System.Reflection;

namespace DibariBot.Modules.ConfigCommand;

public class ConfigCommandService(IServiceProvider services)
{
    public struct State
    {
        public ConfigPage.Page page;
        public string data;

        public State()
        {
            page = default;
            data = string.Empty;
        }

        public State(ConfigPage.Page page) : this()
        {
            this.page = page;
            data = string.Empty;
        }

        public State(ConfigPage.Page page, string data)
        {
            this.page = page;
            this.data = data;
        }
    }

    public Dictionary<ConfigPage.Page, ConfigPage> ConfigPages
    {
        get
        {
            return configPages ??= GetConfigPages(services);
        }
    }
    private Dictionary<ConfigPage.Page, ConfigPage>? configPages;

    public async Task<MessageContents> GetMessageContents(State state, IInteractionContext context)
    {
        var page = ConfigPages[state.page];

        var method = page.GetType().GetMethod("SetContext", BindingFlags.Instance | BindingFlags.NonPublic) ?? throw new NullReferenceException("SetContext doesnt exist!");
        method.Invoke(page, new object[] { context });

        return await page.GetMessageContents(state);
    }

    public static Dictionary<ConfigPage.Page, ConfigPage> GetConfigPages(IServiceProvider services)
    {
        return services.GetServices<ConfigPage>().ToDictionary(type => type.Id);
    }
}