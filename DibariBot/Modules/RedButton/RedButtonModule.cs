using DibariBot.Modules;
using Discord.Interactions;

namespace DibariBot.Modules.RedButton
{

    public class RedButtonModule : BotModule
    {
        [ComponentInteraction(ModulePrefixes.RED_BUTTON)]
        public async Task OnButton()
        {
            await DeferAsync();
            await Context.Interaction.DeleteOriginalResponseAsync();
        }
    }
}

namespace DibariBot
{
    public static class RedButtonExtensions
    {
        public static ComponentBuilder WithRedButton(this ComponentBuilder componentBuilder, string label = "X", int row = 0)
        {
            componentBuilder.WithButton(label, ModulePrefixes.RED_BUTTON, ButtonStyle.Danger, row: row);

            return componentBuilder;
        }
    }
}
