namespace DibariBot.Modules;

public class DibariModule : InteractionModuleBase
{
    protected virtual async Task<IUserMessage> FollowupAsync(MessageContents contents)
    {
        return await FollowupAsync(text: contents.body, embeds: contents.embeds, components: contents.components);
    }

    protected virtual async Task<IUserMessage> ModifyOriginalResponseAsync(MessageContents contents, RequestOptions? options = null)
    {
        return await ModifyOriginalResponseAsync((msg) =>
        {
            msg.Content = contents.body;
            msg.Embeds = contents.embeds;
            msg.Components = contents.components;
        },
        options);
    }
}
