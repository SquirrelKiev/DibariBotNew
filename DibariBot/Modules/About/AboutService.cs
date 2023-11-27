namespace DibariBot.Modules.About;

[Inject(Microsoft.Extensions.DependencyInjection.ServiceLifetime.Singleton)]
public class AboutService
{
    private readonly BotConfig botConfig;
    private readonly OverrideTrackerService overrideService;

    public AboutService(BotConfig botConfig, OverrideTrackerService overrideService)
    {
        this.botConfig = botConfig;
        this.overrideService = overrideService;
    }

    /// <remarks>About page is also used as a secret place for the manager stuff (like overrides). For more info, see <see cref="BotConfig.ManagerUserIds"/></remarks>
    ///
    /// <param name="placeholders">Array of KVPs, with key as the phrase to replace and value as what to replace with.
    /// Recommended to use with <see cref="GetPlaceholders"/>.</param>
    /// 
    /// <param name="userId">The caller's User ID.
    /// The user ID is used to determine if they should see the manager controls.</param>
    public async Task<MessageContents> GetMessageContents(KeyValuePair<string, string>[] placeholders, ulong userId)
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

        var components = new ComponentBuilder();

        if (botConfig.ManagerUserIds.Contains(userId))
        {
            components.WithButton("Toggle override", ModulePrefixes.ABOUT_OVERRIDE_TOGGLE,
                await overrideService.HasOverride(userId) ? ButtonStyle.Danger : ButtonStyle.Success);
        }

        components.WithRedButton();

        var embed = new EmbedBuilder()
            .WithAuthor(ReplacePlaceholders(botConfig.AboutPageTitle, placeholders))
            .WithDescription(ReplacePlaceholders(botConfig.AboutPageDescription, placeholders))
            .WithColor(CommandResult.Default)
            .WithFields(fields);

        return new MessageContents(string.Empty, embed.Build(), components);
    }

    // TODO: in the wrong class really
    public async ValueTask<bool> TryToggleOverride(ulong userId)
    {
        if (!botConfig.ManagerUserIds.Contains(userId)) return false;

        if (!await overrideService.HasOverride(userId))
        {
            await overrideService.SetOverride(userId);
        }
        else
        {
            await overrideService.ClearOverride(userId);
        }

        return true;

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