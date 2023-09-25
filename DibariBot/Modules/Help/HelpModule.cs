using DibariBot.Modules.Core;
using Discord.Interactions;

namespace DibariBot.Modules.Help;

public class HelpModule : DibariModule
{
    private readonly InteractionService interactionService;
    private readonly BotConfig botConfig;

    public HelpModule(InteractionService interactionService, BotConfig botConfig)
    {
        this.interactionService = interactionService;
        this.botConfig = botConfig;
    }

    [SlashCommand("manga-help", "Help! What are all the commands?")]
    [HelpPageDescription("Pulls up this page!")]
    [EnabledInDm(false)]
    public async Task HelpSlash()
    {
        await DeferAsync();

        var embed = new EmbedBuilder();
        embed.WithColor(botConfig);

        foreach(var command in interactionService.SlashCommands)
        {
            string desc = command.Description ?? "No description.";

            var descAttr = command.Attributes.FirstOrDefault(x => x.GetType() == typeof(HelpPageDescriptionAttribute));
            if (descAttr != null)
            {
                HelpPageDescriptionAttribute descAttrCasted = (HelpPageDescriptionAttribute)descAttr;

                desc = descAttrCasted.Description;
            }

            embed.AddField(command.Name ?? "No name?", desc);
        }

        await FollowupAsync(new MessageContents("", embed.Build(), null));
    }
}
