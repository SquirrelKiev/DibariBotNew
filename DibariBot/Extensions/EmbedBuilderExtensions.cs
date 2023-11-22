namespace DibariBot;

public enum CommandResult
{
    Default,
    Failure
}

public static class EmbedBuilderExtensions
{
    const uint SUCCESS_COLOR = 0xFBEED9;
    const uint FAILURE_COLOR = 0xDA373C;

    public static EmbedBuilder WithColor(this EmbedBuilder builder, CommandResult result)
    {
        return result switch
        {
            CommandResult.Default => builder.WithColor(SUCCESS_COLOR),
            CommandResult.Failure => builder.WithColor(FAILURE_COLOR),
            _ => throw new ArgumentOutOfRangeException(nameof(result), result, null)
        };
    }
}
