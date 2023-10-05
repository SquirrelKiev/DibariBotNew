using DibariBot.Database;
using DibariBot.Database.Extensions;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
// ReSharper disable ConditionalAccessQualifierIsNonNullableAccordingToAPIContract

namespace DibariBot
{
    [Inject(ServiceLifetime.Singleton)]
    public class CommandHandler
    {
        public const string DEFAULT_PREFIX = "m.";

        private readonly InteractionService interactionService;
        private readonly CommandService commandService;
        private readonly DbService dbService;
        private readonly DiscordSocketClient client;
        private readonly BotConfig botConfig;
        private readonly IServiceProvider services;

        public CommandHandler(InteractionService interactionService, CommandService commandService, DbService dbService, DiscordSocketClient client, BotConfig botConfig, IServiceProvider services)
        {
            this.interactionService = interactionService;
            this.commandService = commandService;
            this.dbService = dbService;
            this.client = client;
            this.botConfig = botConfig;
            this.services = services;
        }

        public async Task OnReady()
        {
            await InitializeInteractionService();
            await InitializeCommandService();
        }

        #region Prefix Command Handling

        private async Task MessageReceived(SocketMessage msg)
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
                Log.Error(ex, "Command failed: ");
            }
        }

        private async Task RunCommand(SocketUserMessage userMessage)
        {
            var prefix = DEFAULT_PREFIX;

            if (userMessage.Channel is SocketTextChannel textChannel)
            {
                prefix = await dbService.GetPrefix(textChannel.Guild.Id);
            }

            var argPos = 0;
            if (!userMessage.HasStringPrefix(prefix, ref argPos))
            {
                return;
            }

            var context = new SocketCommandContext(client, userMessage);

            await commandService.ExecuteAsync(context, argPos, services);
        }

        private async Task CommandExecuted(Optional<CommandInfo> cmdInfoOpt, ICommandContext ctx, Discord.Commands.IResult res)
        {
            var cmdInfo = cmdInfoOpt.IsSpecified ? cmdInfoOpt.Value : null;

            if (res.IsSuccess)
            {
                Log.Information("Command {ModuleName}.{MethodName} successfully executed. Message contents: {contents}",
                    cmdInfo?.Module.Name, cmdInfo?.Name, ctx.Message.CleanContent);
            }
            else
            {
                if (res.Error == CommandError.UnknownCommand)
                    return;

                if (res is Discord.Commands.ExecuteResult executeResult)
                {
                    Log.Error(executeResult.Exception, "Command {ModuleName}.{MethodName} failed. {Error}, {ErrorReason}. Message contents: {contents}",
                        cmdInfo?.Module?.Name, cmdInfo?.Name, executeResult.Error, executeResult.ErrorReason, ctx.Message.CleanContent);
                }
                else
                {
                    Log.Error("Command {ModuleName}.{MethodName} failed. {Error}, {ErrorReason}. Message contents: {contents}",
                        cmdInfo?.Module?.Name, cmdInfo?.Name, res.Error, res.ErrorReason, ctx.Message.CleanContent);
                }

                try
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
                catch (Exception e)
                {
                    Log.Warning(e, "Failed to add the error reaction!");
                }
            }
        }

        #endregion

        #region Interaction Handling

        private Task InteractionExecuted(ICommandInfo cmdInfo, IInteractionContext ctx, Discord.Interactions.IResult res)
        {
            if (res.IsSuccess)
            {
                Log.Information("Interaction {ModuleName}.{MethodName} successfully executed.", cmdInfo.Module.Name, cmdInfo.MethodName);
            }
            else
            {
                if (res is Discord.Interactions.ExecuteResult executeResult)
                {
                    Log.Error(executeResult.Exception, "Interaction {ModuleName}.{MethodName} failed. {Error}, {ErrorReason}.",
                        cmdInfo?.Module?.Name, cmdInfo?.MethodName, executeResult.Error, executeResult.ErrorReason);
                }
                else
                {
                    Log.Error("Interaction {ModuleName}.{MethodName} failed. {Error}, {ErrorReason}.",
                        cmdInfo?.Module?.Name, cmdInfo?.MethodName, res.Error, res.ErrorReason);
                }

                var messageBody = $"{res.Error}, {res.ErrorReason}";

                if (res is Discord.Interactions.PreconditionResult precondResult)
                {
                    messageBody = $"Condition to use interaction not met. (`{precondResult.ErrorReason}`)";
                }

                if (ctx.Interaction.HasResponded)
                {
                    ctx.Interaction.ModifyOriginalResponseAsync(new MessageContents(messageBody, embed: null, null));
                }
                else
                {
                    ctx.Interaction.RespondAsync(messageBody, ephemeral: true);
                }
            }

            return Task.CompletedTask;
        }


        private async Task InteractionCreated(SocketInteraction arg)
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

        private async Task InitializeInteractionService()
        {
            await interactionService.AddModulesAsync(System.Reflection.Assembly.GetExecutingAssembly(), services);
            await interactionService.RegisterCommandsGloballyAsync();

            client.InteractionCreated += InteractionCreated;
            interactionService.InteractionExecuted += InteractionExecuted;
        }

        private async Task InitializeCommandService()
        {
            await commandService.AddModulesAsync(System.Reflection.Assembly.GetExecutingAssembly(), services);

            client.MessageReceived += MessageReceived;
            commandService.CommandExecuted += CommandExecuted;
        }
    }
}
