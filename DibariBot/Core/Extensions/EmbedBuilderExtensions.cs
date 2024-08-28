namespace DibariBot;

public enum CommandResult
{
    Default,
    Failure
}

public static class EmbedBuilderExtensions
{
    public static uint SuccessColor = 0xFBEED9;
    public static uint FailureColor = 0xDA373C;

    public static EmbedBuilder WithColor(this EmbedBuilder builder, CommandResult result)
    {
        return result switch
        {
            CommandResult.Default => builder.WithColor(SuccessColor),
            CommandResult.Failure => builder.WithColor(FailureColor),
            _ => throw new ArgumentOutOfRangeException(nameof(result), result, null)
        };
    }
}
