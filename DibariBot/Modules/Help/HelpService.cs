using Discord.Commands;
using System.Reflection;
using System.Text;

namespace DibariBot.Modules.Help;

[Inject(Microsoft.Extensions.DependencyInjection.ServiceLifetime.Singleton)]
public class HelpService
{
    private readonly CommandService commandService;
    private readonly InteractionService interactionService;
    private readonly BotConfig botConfig;

    public HelpService(InteractionService interactionService, CommandService commandService, BotConfig botConfig)
    {
        this.interactionService = interactionService;
        this.botConfig = botConfig;
        this.commandService = commandService;
    }

    public MessageContents GetMessageContents(string? prefix)
    {
        var embed = new EmbedBuilder();
        embed.WithColor(botConfig);
        embed.WithDescription(
            "Noting that any prefix command parameter wrapped in square brackets is an optional named parameter.\n" +
            $"Usage is pretty much how the command shows, just without square brackets. e.g.\n" +
            $"`{prefix}manga url:https://mangadex.org/title/2e0fdb3b-632c-4f8f-a311-5b56952db647/bocchi-the-rock`. ");

        var prefixVariantsDictionary = commandService.Commands
            .Select(x => new
            {
                Command = x,
                Attribute = x.Attributes.FirstOrDefault(attr => attr is ParentModulePrefixAttribute) as ParentModulePrefixAttribute
            })
            .Where(pair => pair.Attribute is not null)
            .GroupBy(pair => pair.Attribute!.ParentModule.Name, pair => pair.Command)
            .ToDictionary(x => x.Key, x => x);


        foreach (var command in interactionService.SlashCommands)
        {
            string desc = command.Description ?? "No description.";

            var descAttr = command.Attributes.FirstOrDefault(x => x.GetType() == typeof(HelpPageDescriptionAttribute));
            if (descAttr != null)
            {
                var descAttrCasted = (HelpPageDescriptionAttribute)descAttr;

                desc = descAttrCasted.Description;
            }

            if (prefix != null && prefixVariantsDictionary.TryGetValue(command.Module.Name, out var prefixVariants))
            {
                var stringBuilder = new StringBuilder(desc);
                stringBuilder.AppendLine()
                    .AppendLine("**Prefix versions**");

                foreach (var prefixVariant in prefixVariants)
                {
                    stringBuilder.Append('`')
                        .Append(prefix)
                        .Append(prefixVariant.Name);

                    foreach (var parameter in prefixVariant.Parameters)
                    {
                        stringBuilder.Append(' ');

                        if (parameter.Type.GetCustomAttribute<NamedArgumentTypeAttribute>() is not null)
                        {
                            var namedParams = parameter.Type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

                            var paramInstance = Activator.CreateInstance(parameter.Type);

                            var shouldPlaceSpace = false;
                            foreach (var namedParam in namedParams)
                            {
                                if (shouldPlaceSpace)
                                {
                                    stringBuilder.Append(' ');
                                }
                                stringBuilder.Append('[')
                                    .Append(namedParam.Name.ToLowerInvariant())
                                    .Append(':');

                                if (namedParam.PropertyType == typeof(string)) stringBuilder.Append('"');

                                var def = namedParam.GetValue(paramInstance);
                                stringBuilder.Append(def?.GetType() == typeof(bool) ? def.ToString()?.ToLowerInvariant() : def?.ToString());

                                if (namedParam.PropertyType == typeof(string)) stringBuilder.Append('"');

                                stringBuilder.Append(']');

                                shouldPlaceSpace = true;
                            }
                        }
                        else
                        {
                            stringBuilder.Append(parameter.Name);
                            if (parameter.DefaultValue != null)
                            {
                                stringBuilder.Append(':');
                                if (parameter.Type == typeof(string)) stringBuilder.Append('"');
                                stringBuilder.Append(parameter.DefaultValue);
                                if (parameter.Type == typeof(string)) stringBuilder.Append('"');
                            }
                        }
                    }

                    stringBuilder.Append('`');

                    if (prefixVariant.Aliases.Count <= 0)
                    {
                        stringBuilder.Append('(');

                        var firstAliasLoop = true;
                        foreach (var alias in prefixVariant.Aliases.Distinct())
                        {
                            stringBuilder.Append(firstAliasLoop ? "`" : ", `");

                            firstAliasLoop = false;

                            stringBuilder.Append(alias)
                                .Append('`');
                        }

                        stringBuilder.Append(')');
                    }

                    stringBuilder.AppendLine();
                }

                desc = stringBuilder.ToString();
            }

            embed.AddField(command.Name ?? "No name?", desc);

        }

        return new MessageContents(string.Empty, embed.Build(), null);
    }
}