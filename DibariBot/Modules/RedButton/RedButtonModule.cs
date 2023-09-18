using DibariBot.Modules;
using System.Runtime.CompilerServices;

namespace DibariBot.Modules.RedButton
{
    public class RedButtonModule : DibariModule
    {
        [ComponentInteraction(ModulePrefixes.RED_BUTTON)]
        public Task OnButton()
        {
            return DeleteOriginalResponseAsync();
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
