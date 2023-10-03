using DibariBot.Database;
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
        private Task MessageRecieved(SocketMessage msg)
        {
            if(msg.Author.IsBot)
                return Task.CompletedTask;

            if(msg is not SocketUserMessage userMessage)
                return Task.CompletedTask;

            Task.Run(async () =>
            {
                try
                {
                    await RunCommand(userMessage);
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Command failed: ");
                }
            });

            return Task.CompletedTask;
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

            if (res.IsSuccess)
            {
                Log.Information("Prefix command successfully executed. Message: {message}",
                    userMessage.Content);
            }
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
                    Log.Error(executeResult.Exception, "Command {ModuleName}.{MethodName} failed. {Error}, {ErrorReason}.",
                        cmdInfo?.Module?.Name, cmdInfo?.MethodName, executeResult.Error, executeResult.ErrorReason);
                }
                else
                {
                    Log.Error("Command {ModuleName}.{MethodName} failed. {Error}, {ErrorReason}.",
                        cmdInfo?.Module?.Name, cmdInfo?.MethodName, res.Error, res.ErrorReason);
                }

                var messageBody = $"{res.Error}, {res.ErrorReason}";

                if(res is Discord.Interactions.PreconditionResult precondResult)
                {
                    messageBody = $"Condition to use command not met. (`{precondResult.ErrorReason}`)";
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

            if(ctx.Interaction is SocketMessageComponent componentInteraction)
            {
                var ogRes = componentInteraction.Message;

                if (ogRes.Interaction.User.Id != ctx.Interaction.User.Id)
                {
                    await componentInteraction.RespondAsync("You did not originally trigger this. Please run the command yourself.", ephemeral: true);

                    return;
                }
            }

            // something something blocking bad?
            await interactionService.ExecuteCommandAsync(ctx, services);
        }

        private async Task InitializeInteractionService()
        {
            await interactionService.AddModulesAsync(System.Reflection.Assembly.GetExecutingAssembly(), services);
            await interactionService.RegisterCommandsGloballyAsync(true);

            client.InteractionCreated += x =>
            {
                Task.Run(() => InteractionCreated(x));
                return Task.CompletedTask;
            };
            interactionService.InteractionExecuted += InteractionExecuted;
        }

        private async Task InitializeCommandService()
        {
            await commandService.AddModulesAsync(System.Reflection.Assembly.GetExecutingAssembly(), services);

            client.MessageReceived += MessageRecieved;
        }
    }
}
