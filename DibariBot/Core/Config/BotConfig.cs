using System.Reflection;
using System.Text.Json.Serialization;
using Tomlyn;
using Tomlyn.Helpers;
using Tomlyn.Model;

namespace DibariBot;

public class BotConfig : ITomlMetadataProvider
{
    [JsonIgnore]
    public TomlPropertiesMetadata? PropertiesMetadata { get; set; }

    [TomlMetadata(Comment = "DON'T CHANGE!", NewlineAfter = true)]
    public int Version { get; set; } = 1;

    [TomlMetadata(Comment = @"Your bot token from https://discord.com/developers/applications. Don't share!", NewlineAfter = true)]
    public string BotToken { get; set; } = "BOT_TOKEN_HERE";

    [TomlMetadata(Comment = @"What color should be used in all embeds. In the format 0x00rrggbb, e.g. 0x00ffffff being white.", 
        DisplayKind = TomlPropertyDisplayKind.IntegerHexadecimal, 
        NewlineAfter = true)]
    public uint DefaultEmbedColor { get; set; } = 0xFBEED9;

    [TomlMetadata(Comment = "The type of cache to use. Currently only MemoryCache.",
        NewlineAfter = true)]
    public CacheType Cache { get; set; } = CacheType.MemoryCache;

    [TomlMetadata(Comment = "The base URL for Cubari requests.\n" +
        "Only really should be changed if you're using a self-hosted instance for whatever reason.",
        NewlineAfter = true)]
    public string CubariUrl { get; set; } = "https://cubari.moe";

    [TomlMetadata(Comment = "Specifies how the URL to proxy should be encoded.\n" +
        "Options are:\n" +
        "* \"UrlEncoded\"\n" +
        "* \"Base64Encoded\"")]
    public ProxyUrlEncodingFormat ProxyUrlEncoding { get; set; } = ProxyUrlEncodingFormat.Base64Encoded;

    [TomlMetadata(Comment = "The URL pattern to use for proxying images (will only be used if the platform requires it.\n" +
        "{{URL}} will be replaced with the url in the encoding specified above.")]
    public string ProxyUrl { get; set; } = "https://services.f-ck.me/v1/image/{{URL}}?source=dibari_bot";

    [TomlMetadata(Comment = "What platforms should have their images proxied.", NewlineAfter = true)]
    public string[] PlatformsToProxy { get; set; } = new string[] { "mangadex" };

    public enum CacheType
    {
        MemoryCache
    }

    public enum ProxyUrlEncodingFormat
    {
        UrlEncoded,
        Base64Encoded
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
                foreach(var line in attr.Comment.SplitToLines())
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
