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

    [TomlMetadata(Comment = @"DON'T CHANGE!", NewlineAfter = true)]
    public int Version { get; set; } = 1;

    [TomlMetadata(Comment = @"Your bot token. Don't share!", NewlineAfter = true)]
    public string BotToken { get; set; } = "BOT_TOKEN_HERE";

    [TomlMetadata(Comment = @"What color should be used in all embeds. In the format 0x00rrggbb, e.g. 0x00ffffff being white.", DisplayKind = TomlPropertyDisplayKind.IntegerHexadecimal)]
    public uint DefaultEmbedColor { get; set; } = 0xFBEED9;

    public static string GetDefaultToml()
    {
        var botConfig = new BotConfig();
        botConfig.GenerateMetadata();

        return Toml.FromModel(botConfig);
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
        catch (Exception e)
        {
            Log.Fatal(e, "Supplied bot token is invalid.");
            return false;
        }

        return true;
    }
}

[AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
sealed class TomlMetadataAttribute : Attribute
{
    public string Comment { get; set; } = string.Empty;
    public bool NewlineBefore { get; set; } = false;
    public bool NewlineAfter { get; set; } = false;
    public TomlPropertyDisplayKind DisplayKind { get; set; } = TomlPropertyDisplayKind.Default;
}