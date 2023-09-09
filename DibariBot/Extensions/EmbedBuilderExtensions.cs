namespace DibariBot;

public static class EmbedBuilderExtensions
{
    public static EmbedBuilder WithColor(this EmbedBuilder builder, BotConfig config)
    {
        return builder.WithColor(config.DefaultEmbedColor);
    }
}
