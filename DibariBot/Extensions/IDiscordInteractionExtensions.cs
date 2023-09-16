using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DibariBot
{
    public static class IDiscordInteractionExtensions
    {
        public static Task<IUserMessage> ModifyOriginalResponseAsync(this IDiscordInteraction interaction, MessageContents contents, RequestOptions? options = null)
        {
            return interaction.ModifyOriginalResponseAsync((msg) =>
            {
                msg.Content = contents.body;
                msg.Embeds = contents.embeds;
                msg.Components = contents.components;
            },
            options);
        }
    }
}
