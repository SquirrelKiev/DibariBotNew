using Discord.Commands;

namespace DibariBot.Modules
{
    public class TestPrefix : DibariPrefixModule
    {
        [Command("ping")]
        public async Task TestCommand()
        {
            await Context.Channel.TriggerTypingAsync();

            await ReplyAsync("pong!");
        }
    }
}
