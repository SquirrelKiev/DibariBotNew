using BotBase.Modules.ConfigCommand;
using DibariBot.Modules.ConfigCommand.Pages;

namespace DibariBot.Modules.ConfigCommand;

public class ConfigCommandService : ConfigCommandServiceBase<ConfigPage.Page>
{
    public ConfigCommandService(IServiceProvider services) : base(services)
    {
    }
}