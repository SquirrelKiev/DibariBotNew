using System.Reflection;
using System.Text;
using DibariBot.Database;
using DibariBot.Database.Extensions;
using Discord.Commands;

namespace DibariBot.Modules.Help;

public class HelpModule : DibariModule
{
    private readonly CommandService commandService;
    private readonly InteractionService interactionService;
    private readonly BotConfig botConfig;
    private readonly DbService dbService;

    public HelpModule(InteractionService interactionService, CommandService commandService, BotConfig botConfig, DbService dbService)
    {
        this.interactionService = interactionService;
        this.botConfig = botConfig;
        this.commandService = commandService;
        this.dbService = dbService;
    }

    [SlashCommand("manga-help", "Help! What are all the commands?")]
    [HelpPageDescription("Pulls up this page!")]
    [EnabledInDm(true)]
    public async Task HelpSlash()
    {
        await DeferAsync();

        var prefix = Context.Guild != null ? await dbService.GetPrefix(Context.Guild.Id) : null;

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
                                 .Append(prefixVariant.Name)
                                 .Append(' ');

                    var firstParamLoop = true;
                    foreach (var parameter in prefixVariant.Parameters)
                    {
                        if (!firstParamLoop)
                        {
                            stringBuilder.Append(' ');
                        }

                        if (parameter.Type.GetCustomAttribute<NamedArgumentTypeAttribute>() is not null)
                        {
                            var namedParams = parameter.Type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

                            var paramInstance = Activator.CreateInstance(parameter.Type);

                            var firstNamedParamLoop = true;
                            foreach (var namedParam in namedParams)
                            {
                                if (!firstNamedParamLoop)
                                {
                                    stringBuilder.Append(' ');
                                }
                                stringBuilder.Append('[')
                                             .Append(namedParam.Name.ToLowerInvariant())
                                             .Append(':');

                                if (namedParam.PropertyType == typeof(string)) stringBuilder.Append('"');

                                var def = namedParam.GetValue(paramInstance);
                                stringBuilder.Append(def?.GetType() == typeof(bool) ? def.ToString()?.ToLowerInvariant() : def.ToString());

                                if (namedParam.PropertyType == typeof(string)) stringBuilder.Append('"');

                                stringBuilder.Append(']');

                                firstNamedParamLoop = false;
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

                        firstParamLoop = false;
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

        await FollowupAsync(new MessageContents("", embed.Build(), null));
    }
}
