namespace DibariBot.Modules.About;

[Inject(Microsoft.Extensions.DependencyInjection.ServiceLifetime.Singleton)]
public class AboutService
{
    private readonly BotConfig botConfig;

    public AboutService(BotConfig botConfig)
    {
        this.botConfig = botConfig;
    }

    public MessageContents GetMessageContents(params KeyValuePair<string, string>[] placeholders)
    {
        var fields = new EmbedFieldBuilder[botConfig.AboutPageFields.Length];
        for (int i = 0; i < fields.Length; i++)
        {
            var configField = botConfig.AboutPageFields[i];

            var builder = new EmbedFieldBuilder()
                .WithName(ReplacePlaceholders(configField.Name, placeholders))
                .WithValue(ReplacePlaceholders(configField.Value, placeholders))
                .WithIsInline(configField.Inline);

            fields[i] = builder;
        }

        var embed = new EmbedBuilder()
            .WithAuthor(ReplacePlaceholders(botConfig.AboutPageTitle, placeholders))
            .WithDescription(ReplacePlaceholders(botConfig.AboutPageDescription, placeholders))
            .WithColor(CommandResult.Default)
            .WithFields(fields);

        return new MessageContents(string.Empty, embed.Build(), null);
    }

    public static string ReplacePlaceholders(string original, params KeyValuePair<string, string>[] placeholders)
    {
        var newString = original;

        foreach (var placeholder in placeholders)
        {
            newString = newString.Replace($"{{{{{placeholder.Key}}}}}", placeholder.Value);
        }

        return newString;
    }

    public static async Task<KeyValuePair<string, string>[]> GetPlaceholders(IDiscordClient client)
    {
        return new KeyValuePair<string, string>[]
        {
            new("guilds", (await client.GetGuildsAsync()).Count.ToString()),
            new("botUsername", client.CurrentUser.Username)
        };
    }
}