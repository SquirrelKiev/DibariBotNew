namespace DibariBot.Modules;

public class DibariModule : InteractionModuleBase
{
    protected virtual Task<IUserMessage> FollowupAsync(MessageContents contents, bool ephemeral = false)
    {
        return FollowupAsync(text: contents.body, embeds: contents.embeds, components: contents.components, ephemeral: ephemeral);
    }

    protected virtual Task RespondAsync(MessageContents contents, bool ephemeral = false)
    {
        return RespondAsync(text: contents.body, embeds: contents.embeds, components: contents.components, ephemeral: ephemeral);
    }

    protected virtual Task<IUserMessage> ModifyOriginalResponseAsync(MessageContents contents, RequestOptions? options = null)
    {
        return Context.Interaction.ModifyOriginalResponseAsync(contents, options);
    }
}