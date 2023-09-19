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

    [TomlMetadata(Comment = @"What color should be used in all embeds. In the format 0x00rrggbb, e.g. 0x00ffffff being white.", 
        DisplayKind = TomlPropertyDisplayKind.IntegerHexadecimal)]
    public uint DefaultEmbedColor { get; set; } = 0xFBEED9;

    [TomlMetadata(Comment = "The type of cache to use.")]
    public CacheType Cache { get; set; } = CacheType.Memory;

    [TomlMetadata(Comment = "The type of database to use.")]
    public DatabaseType Database { get; set; } = DatabaseType.Postgresql;

    [TomlMetadata(Comment = "The connection string for the database specified above.")]
    public string DatabaseConnectionString { get; set; } = "Host=127.0.0.1;Username=postgres;Password=;Database=dibari";

    [TomlMetadata(Comment = "The base URL for Cubari requests.\n" +
        "Only really should be changed if you're using a self-hosted instance for whatever reason.")]
    public string CubariUrl { get; set; } = "https://cubari.moe";

    [TomlMetadata(Comment = "The base URL for MangaDex requests.")]
    public string MangaDexUrl { get; set; } = "https://api.mangadex.com";

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

                    foreach ( var name in names)
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
