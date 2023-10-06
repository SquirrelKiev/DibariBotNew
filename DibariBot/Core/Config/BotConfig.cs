using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;
using Tomlyn.Helpers;
using Tomlyn.Model;

namespace DibariBot;

public class BotConfig : ITomlMetadataProvider
{
    [JsonIgnore]
    public TomlPropertiesMetadata? PropertiesMetadata { get; set; }

    [TomlMetadata(Comment = "DON'T CHANGE!")]
    public int Version { get; set; } = 1;

    [TomlMetadata(Comment = @"Your bot token from https://discord.com/developers/applications. Don't share!")]
    public string BotToken { get; set; } = "BOT_TOKEN_HERE";

    [TomlMetadata(Comment = @"What color should be used in manga-related embeds. In the format 0x00rrggbb, e.g. 0x00ffffff being white.",
        DisplayKind = TomlPropertyDisplayKind.IntegerHexadecimal)]
    public uint DefaultEmbedColor { get; set; } = 0xFBEED9;

    [TomlMetadata(Comment = "The type of cache to use.")]
    public CacheType Cache { get; set; } = CacheType.Memory;

    [TomlMetadata(Comment = "The type of database to use.")]
    public DatabaseType Database { get; set; } = DatabaseType.Sqlite;

    [TomlMetadata(Comment = "The connection string for the database specified above.\n" +
                            "Example Postgres string: Host=127.0.0.1;Username=postgres;Password=;Database=dibari\n" +
                            "Example Sqlite string: Data Source=data/DibariBot.db")]
    public string DatabaseConnectionString { get; set; } = "Data Source=data/DibariBot.db";

    [TomlMetadata(Comment = "The base URL for Cubari requests.\n" +
        "Only really should be changed if you're using a self-hosted instance for whatever reason.")]
    public string CubariUrl { get; set; } = "https://cubari.moe";

    [TomlMetadata(Comment = "The URL for MangaDex searches. Leave empty to disable.")]
    public string MangaDexApiUrl { get; set; } = "https://api.mangadex.org";

    [TomlMetadata(Comment = "The URL to link to for MangaDex searches. '{{ID}}' will be replaced with the mangadex id.")]
    public string MangaDexSearchUrl { get; set; } = "https://mangadex.org/title/{{ID}}";
    
    [TomlMetadata(Comment = "The URL for Phixiv requests. expects an instance of https://github.com/HazelTheWitch/phixiv")]
    public string PhixivUrl { get; set; } = "https://www.phixiv.net";

    [TomlMetadata(Comment = "How many results to show per page when using the search command.")]
    public int MangaDexSearchLimit { get; set; } = 5;

    [TomlMetadata(Comment = "How long a manga title should be before it is truncated.")]
    public int MaxTitleLength { get; set; } = 50;

    [TomlMetadata(Comment = "How long a manga description should be before it is truncated.")]
    public int MaxDescriptionLength { get; set; } = 200;

    [TomlMetadata(Comment = "The style to use for pretty much every button that isn't a close button.")]
    public ButtonStyle PrimaryButtonStyle { get; set; } = ButtonStyle.Secondary;

    [TomlMetadata(Comment = "The timeout for user specified regex. Applies to all regex as a whole, not per regex.\n" +
                            "e.g. if there were 5 filters, each filter's time running would be deducted from the total timeout for that filter check.\n" +
                            "(hope that makes sense)")]
    public uint RegexTimeoutMilliseconds { get; set; } = 250;

    public TimeSpan RegexTimeout => TimeSpan.FromMilliseconds(RegexTimeoutMilliseconds);


    [TomlMetadata(Comment = "The reaction to put on prefix commands when an unhandled error occurs. Will only appear on prefix commands.")]
    public string ErrorEmote { get; set; } = "\u2753";

    [TomlMetadata(Comment = "An optional URL to an instance of Seq. Empty string is interpreted as not wanting Seq.")]
    public string SeqUrl { get; set; } = "";

    [TomlMetadata(Comment = "An optional API key for Seq. Empty string is interpreted as no API key.")]
    public string SeqApiKey { get; set; } = "";

    [TomlMetadata(Comment = "***** IMAGE PROXY CONFIG *****\n\n" +
        "Specifies how the URL to proxy should be encoded.")]
    public ProxyUrlEncodingFormat ProxyUrlEncoding { get; set; } = ProxyUrlEncodingFormat.Base64;

    [TomlMetadata(Comment = "The URL pattern to use for proxying images (will only be used if the platform requires it.\n" +
        "{{URL}} will be replaced with the url in the encoding specified above.")]
    public string ProxyUrl { get; set; } = "https://services.f-ck.me/v1/image/{{URL}}?source=dibari_bot";

    [TomlMetadata(Comment = "What platforms should have their images proxied.")]
    public string[] PlatformsToProxy { get; set; } = new string[] { "mangadex" };

    public enum CacheType
    {
        Memory
    }

    public enum DatabaseType
    {
        Sqlite,
        Postgresql
    }

    public enum ProxyUrlEncodingFormat
    {
        UrlEscaped,
        Base64
    }

    public void GenerateMetadata()
    {
        var newLine = new TomlSyntaxTriviaMetadata()
        {
            Kind = Tomlyn.Syntax.TokenKind.NewLine,
            Text = Environment.NewLine
        };

        PropertiesMetadata = new();

        foreach (var prop in GetType().GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
        {
            var key = TomlNamingHelper.PascalToSnakeCase(prop.Name);

            var attr = prop.GetCustomAttribute<TomlMetadataAttribute>();

            if (attr == null)
                continue;

            var tpMetadata = new TomlPropertyMetadata
            {
                LeadingTrivia = new(),
                TrailingTrivia = new(),
                DisplayKind = attr.DisplayKind
            };

            if (attr.NewlineBefore)
            {
                tpMetadata.LeadingTrivia.Add(newLine);
            }

            if (!string.IsNullOrEmpty(attr.Comment))
            {
                var comment = attr.Comment;

                if (prop.PropertyType.IsEnum)
                {
                    var stringBuilder = new StringBuilder(comment);

                    var names = Enum.GetNames(prop.PropertyType);

                    stringBuilder.AppendLine();
                    stringBuilder.AppendLine("Options are:");

                    foreach (var name in names)
                    {
                        stringBuilder.AppendLine($"* {name.ToLowerInvariant()}");
                    }

                    comment = stringBuilder.ToString();
                }

                foreach (var line in comment.SplitToLines())
                {
                    tpMetadata.LeadingTrivia.Add(
                        new TomlSyntaxTriviaMetadata()
                        {
                            Kind = Tomlyn.Syntax.TokenKind.Comment,
                            Text = $"# {line}"
                        }
                    );
                    tpMetadata.LeadingTrivia.Add(newLine);
                }
            }

            if (attr.NewlineAfter)
            {
                tpMetadata.TrailingTrivia.Add(newLine);
            }

            PropertiesMetadata.SetProperty(key, tpMetadata);
        }
    }

    public bool IsValid()
    {
        try
        {
            TokenUtils.ValidateToken(TokenType.Bot, BotToken);
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Supplied bot token is invalid.");
            return false;
        }

        return true;
    }
}
