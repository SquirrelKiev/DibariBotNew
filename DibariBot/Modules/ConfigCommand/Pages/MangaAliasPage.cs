using System.Text;
using DibariBot.Database;
using DibariBot.Database.Models;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DibariBot.Modules.ConfigCommand.Pages;

[ConfigPage(
    Id,
    "Manga Command Aliases",
    "Custom commands to open a specific manga.",
    Conditions.NotInDm
)]
public class MangaAliasPage(
    DbService db,
    ConfigCommandService configCommandService,
    ColorProvider colorProvider,
    CommandHandler commandHandler
) : BotModule, IConfigPage
{
    public class AddAliasModal : IModal
    {
        public string Title => "Add Alias";

        [ModalTextInput(
            customId: ModulePrefixes.CONFIG_ALIASES_ADD_MODAL_URL_TEXT_BOX,
            minLength: 1,
            maxLength: DefaultManga.MaxMangaLength,
            placeholder: "https://mangadex.org/title/2e0fdb3b-632c-4f8f-a311-5b56952db647/bocchi-the-rock"
        )]
        public string MangaUrl { get; set; } = "";

        [ModalTextInput(
            customId: ModulePrefixes.CONFIG_ALIASES_ADD_MODAL_ALIAS_TEXT_BOX,
            minLength: 2,
            maxLength: MangaCommandAlias.MaxSlashCommandNameLength,
            placeholder: "bocchi"
        )]
        public string Alias { get; set; } = "";
    }

    public const Page Id = Page.MangaAlias;

    public Task<MessageContents> GetMessageContents(ConfigCommandService.State state) =>
        GetMessageContents();

    public async Task<MessageContents> GetMessageContents()
    {
        await using var context = db.GetDbContext();
        return await GetMessageContents(context);
    }

    public async Task<MessageContents> GetMessageContents(BotDbContext context)
    {
        // TODO: Paginate

        var entries = await GetEntries(context);

        var eb = await GetEmbed(context, entries);

        var cb = new ComponentBuilder()
            .WithSelectMenu(configCommandService.GetPageSelectDropdown(Id, IsDm()))
            .WithButton("Add", ModulePrefixes.CONFIG_ALIASES_ADD_BUTTON, ButtonStyle.Secondary)
            .WithButton(
                "Remove",
                ModulePrefixes.CONFIG_ALIASES_REMOVE_BUTTON,
                ButtonStyle.Secondary
            )
            .WithRedButton();

        return new MessageContents(eb, cb);
    }

    private async Task<EmbedBuilder> GetEmbed(BotDbContext context, MangaCommandAlias[] entries)
    {
        var eb = new EmbedBuilder();

        if (entries.Length != 0)
        {
            eb.WithFields(
                entries.Select(x =>
                    new EmbedFieldBuilder().WithName(x.SlashCommandName).WithValue($"`{x.Manga}`")
                )
            );
        }
        else
        {
            eb.WithDescription("No entries.");
        }

        eb.WithColor(await colorProvider.GetEmbedColor(context, Context.Guild));
        return eb;
    }

    private async Task<MangaCommandAlias[]> GetEntries(BotDbContext context)
    {
        var entries = await context
            .MangaCommandAliases.Where(x => x.GuildId == Context.Guild.Id)
            .OrderBy(x => x.Id)
            .Take(4)
            .ToArrayAsync();
        return entries;
    }

    [ComponentInteraction(ModulePrefixes.CONFIG_ALIASES_ADD_BUTTON)]
    public Task AddButton() =>
        RespondWithModalAsync<AddAliasModal>(ModulePrefixes.CONFIG_ALIASES_ADD_MODAL);

    [ModalInteraction(ModulePrefixes.CONFIG_ALIASES_ADD_MODAL)]
    public async Task AddModal(AddAliasModal modal)
    {
        await DeferAsync(true);

        await using var context = db.GetDbContext();

        var url = ParseUrl.ParseMangaUrl(modal.MangaUrl);

        if (url == null)
        {
            await FollowupAsync(
                new MessageContents(
                    new EmbedBuilder()
                        .WithDescription("Invalid URL.")
                        .WithColor(colorProvider.GetErrorEmbedColor()),
                    redButton: false
                ),
                ephemeral: true
            );
            return;
        }

        var alias = modal.Alias.ToLowerInvariant();
        if (!CompiledRegex.SlashCommandRegex().IsMatch(alias))
        {
            await FollowupAsync(
                new MessageContents(
                    new EmbedBuilder()
                        .WithDescription(
                            "Invalid alias (slash command name).\n"
                                + "Must be only alphanumeric characters (underscores and dashes are also fine)."
                        )
                        .WithColor(colorProvider.GetErrorEmbedColor()),
                    redButton: false
                ),
                ephemeral: true
            );
            return;
        }

        context.MangaCommandAliases.Add(
            new MangaCommandAlias()
            {
                GuildId = Context.Guild.Id,
                Manga = url.Value.ToString(),
                SlashCommandName = alias,
            }
        );

        await context.SaveChangesAsync();

        // Two database requests here for the same data is a bit of a waste
        await commandHandler.RegisterGuildSlashes((SocketGuild)Context.Guild, context);

        await ModifyOriginalResponseAsync(await GetMessageContents(context));
    }

    [ComponentInteraction(ModulePrefixes.CONFIG_ALIASES_REMOVE_BUTTON)]
    public async Task RemoveButton()
    {
        await DeferAsync();

        await using var context = db.GetDbContext();

        var entries = await GetEntries(context);

        var eb = await GetEmbed(context, entries);

        var selectMenuEntries = entries
            .Select(x => new SelectMenuOptionBuilder(
                x.SlashCommandName,
                x.Id.ToString(),
                description: x.Manga
            ))
            .ToList();

        var cb = new ComponentBuilder();

        if (selectMenuEntries.Count != 0)
        {
            var selectMenu = new SelectMenuBuilder()
                .WithCustomId(ModulePrefixes.CONFIG_ALIASES_REMOVE_SELECT)
                .WithOptions(selectMenuEntries);

            cb.WithSelectMenu(selectMenu);
        }

        cb.WithButton(
            "Cancel",
            $"{ModulePrefixes.CONFIG_PAGE_SELECT_PAGE_BUTTON}{StateSerializer.SerializeObject(Id)}",
            ButtonStyle.Secondary
        );

        await ModifyOriginalResponseAsync(new MessageContents(eb, cb));
    }

    [ComponentInteraction(ModulePrefixes.CONFIG_ALIASES_REMOVE_SELECT)]
    public async Task RemoveSelect(string option)
    {
        await DeferAsync();

        var id = int.Parse(option);

        await using var context = db.GetDbContext();

        await context
            .MangaCommandAliases.Where(x => x.GuildId == Context.Guild.Id && x.Id == id)
            .ExecuteDeleteAsync();

        await commandHandler.RegisterGuildSlashes((SocketGuild)Context.Guild, context);

        await ModifyOriginalResponseAsync(await GetMessageContents(context));
    }
}
