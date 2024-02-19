using BotBase;
using YamlDotNet.Serialization;

namespace DibariBot
{
    public class BotConfig : BotConfigBase
    {
        [YamlMember(Description = "DON'T CHANGE!")]
        public int Version { get; set; } = 1;
        public override string BotToken { get; set; } = "BOT_TOKEN_HERE";
        public override CacheType Cache { get; set; } = CacheType.Memory;
        public override DatabaseType Database { get; set; } = DatabaseType.Sqlite;
        public override string DatabaseConnectionString { get; set; } = "Data Source=data/DibariBot.db";
        public override string ErrorEmote { get; set; } = "\u2753";
        public override HashSet<ulong> ManagerUserIds { get; set; } = new()
        {
            0ul
        };

        public override string SeqUrl { get; set; } = "";
        public override string SeqApiKey { get; set; } = "";

        [YamlMember(Description = "The base URL for Cubari requests.\n" +
                                  "Only really should be changed if you're using a self-hosted instance for whatever reason.")]
        public string CubariUrl { get; set; } = "https://cubari.moe";

        [YamlMember(Description = "The URL for MangaDex searches.")]
        public string MangaDexApiUrl { get; set; } = "https://api.mangadex.org";

        [YamlMember(Description = "The URL to link to for MangaDex searches. '{{ID}}' will be replaced with the mangadex id.")]
        public string MangaDexSearchUrl { get; set; } = "https://mangadex.org/title/{{ID}}";

        [YamlMember(Description = "The URL for Phixiv requests. expects an instance of https://github.com/HazelTheWitch/phixiv")]
        public string PhixivUrl { get; set; } = "https://www.phixiv.net";

        [YamlMember(Description = "How many results to show per page when using the search command.")]
        public int MangaDexSearchLimit { get; set; } = 5;

        [YamlMember(Description = "How long a manga title should be before it is truncated.")]
        public int MaxTitleLength { get; set; } = 50;

        [YamlMember(Description = "How long a manga description should be before it is truncated.")]
        public int MaxDescriptionLength { get; set; } = 200;

        [YamlMember(Description = "The timeout for user specified regex. Applies to all regex as a whole, not per regex.\n" +
                                "e.g. if there were 5 filters, each filter's time running would be deducted from the total timeout for that filter check.\n" +
                                "(hope that makes sense)")]
        public uint RegexTimeoutMilliseconds { get; set; } = 250;

        [YamlIgnore]
        public TimeSpan RegexTimeout => TimeSpan.FromMilliseconds(RegexTimeoutMilliseconds);

        [YamlMember(Description = "***** IMAGE PROXY CONFIG *****\n\n" +
                                "Specifies how the URL to proxy should be encoded.")]
        public ProxyUrlEncodingFormat ProxyUrlEncoding { get; set; } = ProxyUrlEncodingFormat.Base64;

        [YamlMember(Description = "The URL pattern to use for proxying images (will only be used if the platform requires it.\n" +
            "{{URL}} will be replaced with the url in the encoding specified above.")]
        public string ProxyUrl { get; set; } = "https://services.f-ck.me/v1/image/{{URL}}?source=dibari_bot";

        [YamlMember(Description = "What platforms should have their images proxied.")]
        public string[] PlatformsToProxy { get; set; } = { "mangadex" };

        public override string DefaultPrefix { get; set; } = "m.";

        public override string AboutPageTitle { get; set; } = "About {{botUsername}}";

        public override string AboutPageDescription { get; set; } = "A discord bot for reading manga, within Discord.";

        public override AboutField[] AboutPageFields { get; set; } = {
            new()
            {
                Name = "Total Servers:",
                Value = "{{guilds}}"
            },
            new()
            {
                Name = "Credits:",
                Value = "Bot by [SquirrelKiev](https://github.com/SquirrelKiev)"
            },
            new()
            {
                Name = "Source Code:",
                Value = "https://github.com/SquirrelKiev/DibariBotNew"
            }
        };

        public enum ProxyUrlEncodingFormat
        {
            UrlEscaped,
            Base64
        }
    }
}
