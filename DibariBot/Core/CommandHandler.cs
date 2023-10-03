using DibariBot.Database;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace DibariBot
{
    [Inject(ServiceLifetime.Singleton)]
    public class CommandHandler
    {
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

        // Prefix command zone
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

            return;
        }

        private async Task RunCommand(SocketUserMessage userMessage)
        {
            int argPos = 0;
            if (!userMessage.HasStringPrefix("!", ref argPos))
            {
                return;
            }

            var context = new SocketCommandContext(client, userMessage);

            var res = await commandService.ExecuteAsync(context, argPos, services);
        }

        private Task CommandExecuted(Optional<CommandInfo> cmdInfoOpt, ICommandContext ctx, Discord.Commands.IResult res)
        {
            var cmdInfo = cmdInfoOpt.IsSpecified ? cmdInfoOpt.Value : null;

            if (res.IsSuccess)
            {
                Log.Information("Command {ModuleName}.{MethodName} successfully executed.", cmdInfo?.Module.Name, cmdInfo?.Name);
            }
            else
            {
                if (res is Discord.Commands.ExecuteResult executeResult)
                {
                    Log.Error(executeResult.Exception, "Command {ModuleName}.{MethodName} failed. {Error}, {ErrorReason}.",
                        cmdInfo?.Module?.Name, cmdInfo?.Name, executeResult.Error, executeResult.ErrorReason);
                }
                else
                {
                    Log.Error("Command {ModuleName}.{MethodName} failed. {Error}, {ErrorReason}.",
                        cmdInfo?.Module?.Name, cmdInfo?.Name, res.Error, res.ErrorReason);
                }

                var messageBody = $"{res.Error}, {res.ErrorReason}";

                if (res is Discord.Commands.PreconditionResult precondResult)
                {
                    messageBody = $"Condition to use command not met. (`{precondResult.ErrorReason}`)";
                }

                ctx.Message.AddReactionAsync(Emote.Parse("<:AiWut:821056610940092496>"));
            }

            return Task.CompletedTask;
        }

        // Interaction zone
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
            await Task.Delay(5);

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
                    ogAuthor = message.Author.Id;
                }

                if (ogAuthor != ctx.Interaction.User.Id)
                {
                    await componentInteraction.RespondAsync("You did not originally trigger this. Please run the command yourself.", ephemeral: true);

                    return;
                }
            }

            await interactionService.ExecuteCommandAsync(ctx, services);
        }

        private async Task InitializeInteractionService()
        {
            await interactionService.AddModulesAsync(System.Reflection.Assembly.GetExecutingAssembly(), services);
            await interactionService.RegisterCommandsGloballyAsync(true);

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
