using DibariBot.Modules.ConfigCommand.Pages;
using Microsoft.Extensions.DependencyInjection;

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

    public async Task<MessageContents> GetMessageContents(State state)
    {
        return await configPages[state.page].GetMessageContents(state);
    }
}
