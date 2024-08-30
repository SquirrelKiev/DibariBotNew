namespace DibariBot.Modules.ConfigCommand;

public interface IConfigPage
{
    public Task<MessageContents> GetMessageContents(ConfigCommandService.State state);
}