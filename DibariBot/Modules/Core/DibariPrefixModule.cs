using Discord.Commands;
using Discord.WebSocket;

namespace DibariBot.Modules;

public abstract class DibariPrefixModule : ModuleBase<SocketCommandContext>
{
    protected Task<IUserMessage> ReplyAsync(MessageContents contents)
    {
        return base.ReplyAsync(contents.body, embeds: contents.embeds, components: contents.components, 
            messageReference: new MessageReference(Context.Message.Id, Context.Channel.Id, Context.Guild?.Id), allowedMentions: AllowedMentions.None);
    }

    protected Task DeferAsync() => Context.Channel.TriggerTypingAsync();

    protected virtual IMessageChannel GetParentChannel()
    {
        var channel = Context.Channel;

        if (Context.Channel is SocketThreadChannel thread)
        {
            channel = (ISocketMessageChannel)thread.ParentChannel;
        }

        return channel;
    }
}