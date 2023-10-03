using Discord.WebSocket;

namespace DibariBot.Modules;

public abstract class DibariModule : InteractionModuleBase
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

    protected virtual IMessageChannel GetParentChannel()
    {
        var channel = Context.Channel;

        if (Context.Channel is SocketThreadChannel thread)
        {
            channel = (IMessageChannel)thread.ParentChannel;
        }

        return channel;
    }
}