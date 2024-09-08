using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace DibariBot.Modules;

public static partial class CompiledRegex
{
    [GeneratedRegex(@"\{\{(\w+)\}\}")]
    public static partial Regex CaptureTemplatedString();

    [GeneratedRegex(@"<#(\d+)>")]
    public static partial Regex ChannelMatcher();

    [GeneratedRegex(@"```([^`]*)```")]
    public static partial Regex CodeBlockMatcher();

    [StringSyntax(StringSyntaxAttribute.Regex)]
    public const string SlashCommandRegexPattern = @"^[\w\-_]{1,32}$";

    [GeneratedRegex(SlashCommandRegexPattern)]
    public static partial Regex SlashCommandRegex();
}
