using BotBase.Modules.ConfigCommand;
using DibariBot.Modules.ConfigCommand.Pages;

namespace DibariBot.Modules.ConfigCommand;

public class ConfigCommandService(IServiceProvider services) : ConfigCommandServiceBase<ConfigPage.Page>(services);