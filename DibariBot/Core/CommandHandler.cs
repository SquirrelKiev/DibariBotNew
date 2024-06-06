using System.Reflection;
using BotBase;
using BotBase.Database;
using BotBase.Modules.RedButton;
using DibariBot.Database;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;

// ReSharper disable ConditionalAccessQualifierIsNonNullableAccordingToAPIContract

namespace DibariBot;

public class CommandHandler(
    InteractionService interactionService,
    CommandService commandService,
    DiscordSocketClient client,
    BotConfigBase botConfig,
    IServiceProvider services,
    DbService dbService,
    ILogger<CommandHandler> logger)
{
    public async Task OnReady(params Assembly[] assemblies)
    {
        await InitializeInteractionService(assemblies);
        await InitializeCommandService(assemblies);
    }

    #region Prefix Command Handling

    protected async Task MessageReceived(SocketMessage msg)
    {
        if (msg.Author.IsBot)
            return;

        if (msg is not SocketUserMessage userMessage)
            return;

        try
        {
            await RunCommand(userMessage);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Command failed: ");
        }
    }

    protected async Task RunCommand(SocketUserMessage userMessage)
    {
        var prefix = await GetPrefix(userMessage.Channel);

        var argPos = 0;
        if (!userMessage.HasStringPrefix(prefix, ref argPos))
        {
            return;
        }

        var context = new SocketCommandContext(client, userMessage);

        await commandService.ExecuteAsync(context, argPos, services);
    }

    public async Task<string> GetPrefix(IChannel? channel)
    {
        var prefix = botConfig.DefaultPrefix;

        if (channel is SocketTextChannel textChannel)
        {
            prefix = await dbService.GetPrefix(textChannel.Guild.Id, botConfig.DefaultPrefix);
        }

        return prefix;
    }

    protected async Task CommandExecuted(Optional<CommandInfo> cmdInfoOpt, ICommandContext ctx, Discord.Commands.IResult res)
    {
        if (res.IsSuccess)
            return;

        if (res.Error != CommandError.Exception && res.Error != CommandError.UnmetPrecondition)
            return;

        try
        {
            if (res is Discord.Commands.PreconditionResult precondResult)
            {
                var messageBody = $"Condition to use the command not met. {precondResult.ErrorReason}";
                await ctx.Message.ReplyAsync(messageBody);
            }
            else
            {
                IEmote emote;

                if (Emote.TryParse(botConfig.ErrorEmote, out var result))
                {
                    emote = result;
                }
                else
                {
                    emote = Emoji.Parse(botConfig.ErrorEmote);
                }


                await ctx.Message.AddReactionAsync(emote);
            }
        }
        catch (Exception e)
        {
            logger.LogWarning(e, "Failed to add the error reaction!");
        }
    }

    #endregion

    #region Interaction Handling

    protected static Task InteractionExecuted(ICommandInfo cmdInfo, IInteractionContext ctx, Discord.Interactions.IResult res)
    {
        if (res.IsSuccess)
            return Task.CompletedTask;

        var messageBody = $"{res.Error}, {res.ErrorReason}";

        if (res is Discord.Interactions.PreconditionResult precondResult)
        {
            messageBody = $"Condition to use this command not met. {precondResult.ErrorReason}";
        }

        if (ctx.Interaction.HasResponded)
        {
            ctx.Interaction.ModifyOriginalResponseAsync(new MessageContents(messageBody, embed: null, null));
        }
        else
        {
            ctx.Interaction.RespondAsync(messageBody, ephemeral: true);
        }

        return Task.CompletedTask;
    }


    protected async Task InteractionCreated(SocketInteraction arg)
    {
        var ctx = new SocketInteractionContext(client, arg);

        if (ctx.Interaction is SocketMessageComponent componentInteraction)
        {
            var ogRes = componentInteraction.Message;

            var ogAuthor = ogRes.Interaction?.User.Id;

            // horrible
            if (ogAuthor == null)
            {
                var channel = (ISocketMessageChannel)await client.GetChannelAsync(ogRes.Reference.ChannelId);
                var message = await channel.GetMessageAsync(ogRes.Reference.MessageId.Value);
                ogAuthor = message?.Author?.Id;
            }

            if (ogAuthor != null && ogAuthor != ctx.Interaction.User.Id)
            {
                await componentInteraction.RespondAsync("You did not originally trigger this. Please run the command yourself.", ephemeral: true);

                return;
            }
        }

        await interactionService.ExecuteCommandAsync(ctx, services);
    }

    #endregion

    protected async Task InitializeInteractionService(params Assembly[] assemblies)
    {
        foreach (var assembly in assemblies.Append(typeof(RedButtonModule).Assembly))
        {
            var modules = await interactionService.AddModulesAsync(assembly, services);

            foreach (var moduleInfo in modules)
            {
                logger.LogTrace("Registered Interaction Module: {moduleName}", moduleInfo.Name);
            }
        }

        await interactionService.RegisterCommandsGloballyAsync();

        client.InteractionCreated += InteractionCreated;
        interactionService.InteractionExecuted += InteractionExecuted;
    }

    protected async Task InitializeCommandService(params Assembly[] assemblies)
    {
        foreach (var assembly in assemblies)
        {
            var modules = await commandService.AddModulesAsync(assembly, services);

            foreach (var moduleInfo in modules)
            {
                logger.LogTrace("Registered Prefix Module: {moduleName}", moduleInfo.Name);
            }
        }

        client.MessageReceived += MessageReceived;
        commandService.CommandExecuted += CommandExecuted;
    }
}