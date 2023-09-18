using DibariBot.Modules.ConfigCommand.Pages;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace DibariBot.Modules.ConfigCommand;

[Inject(ServiceLifetime.Singleton)]
public class ConfigCommandService
{
    public struct State
    {
        public ConfigPage.Page page;
        public string data;
    }

    public Dictionary<ConfigPage.Page, ConfigPage> configPages = new();

    public ConfigCommandService(IServiceProvider services)
    {
        foreach (var type in services.GetServices<ConfigPage>())
        {
            configPages.Add(type.Id, type.Initialize(configPages));
        }
    }

    public async Task<MessageContents> GetMessageContents(State state, IInteractionContext context)
    {
        var page = configPages[state.page];

        var method = page.GetType().GetMethod("SetContext", BindingFlags.Instance | BindingFlags.NonPublic) ?? throw new NullReferenceException("SetContext doesnt exist!");
        method.Invoke(page, new object[] { context });

        return await page.GetMessageContents(state);
    }
}
