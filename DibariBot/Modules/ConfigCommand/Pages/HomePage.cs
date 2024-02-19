using BotBase;
using BotBase.Modules.ConfigCommand;

namespace DibariBot.Modules.ConfigCommand.Pages;

public class HomePage : ConfigPage
{
    public override Page Id => Page.Help;

    public override string Label => "Help";

    public override string Description => "Brings up information about each config page.";

    public override bool EnabledInDMs => true;

    private readonly HomePageImpl<Page> homePageImpl;

    public HomePage(ConfigCommandService configCommandService)
    {
        this.homePageImpl = new HomePageImpl<Page>(configCommandService, this);
    }

    public override Task<MessageContents> GetMessageContents(ConfigCommandServiceBase<Page>.State state) =>
        homePageImpl.GetMessageContents(state);
}
