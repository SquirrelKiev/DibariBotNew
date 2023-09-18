using DibariBot.Database;
using DibariBot.Modules.Common;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace DibariBot.Modules.ConfigCommand.Pages;

public class DefaultMangaPage : ConfigPage
{
    public class DefaultMangaSetModal : IModal
    {
        public string Title => "Set Default Manga - Step 1";

        [ModalTextInput(customId: ModulePrefixes.CONFIG_DEFAULT_MANGA_SET_MANGA_INPUT)]
        public string URL { get; set; } = "";
    }

    public struct ConfirmState
    {
        public SeriesIdentifier series;
        /// <summary>
        /// channel ID. 0ul means we want the default to be the server instead.
        /// </summary>
        public ulong channelId;

        public ConfirmState()
        {
        }

        public ConfirmState(SeriesIdentifier series, ulong channelId)
        {
            this.series = series;
            this.channelId = channelId;
        }
    }

    public override Page Id => Page.DefaultManga;

    public override string Label => "Default manga";

    public override string Description => "Change the manga that opens when no URL is specified. Can be per-server and per-channel.";

    private readonly DbService dbService;

    public DefaultMangaPage(DbService db)
    {
        dbService = db;
    }

    // step 1 - help page/modal open
    public override async Task<MessageContents> GetMessageContents(ConfigCommandService.State state)
    {
        using var dbContext = dbService.GetDbContext();

        var guildId = (Context.Guild?.Id) ?? throw new NullReferenceException("wtf");

        var defaults = await dbContext.DefaultMangas.Where(x => x.GuildId == guildId).ToArrayAsync();

        var embed = new EmbedBuilder();

        if (defaults.Length > 0)
        {
            foreach (var def in defaults)
            {
                embed.AddField(def.ChannelId == 0ul ? "Server-wide" : $"<#{def.ChannelId}>", def.Manga);
            }
        }
        else
        {
            embed.WithDescription("No defaults set.");
        }

        // TODO
        var components = new ComponentBuilder()
            .WithButton(new ButtonBuilder()
                .WithLabel("Set")
                .WithCustomId($"{ModulePrefixes.CONFIG_DEFAULT_MANGA_SET}")
                .WithStyle(ButtonStyle.Primary))
            .WithSelectMenu(ConfigPageUtility.GetPageSelectDropdown(ConfigPages, Id));

        return new MessageContents("", embed.Build(), components.Build());
    }

    [ComponentInteraction(ModulePrefixes.CONFIG_DEFAULT_MANGA_SET)]
    public async Task OpenModal()
    {
        await RespondWithModalAsync<DefaultMangaSetModal>(ModulePrefixes.CONFIG_DEFAULT_MANGA_SET_MODAL);
    }

    // step 2 - confirm section
    [ModalInteraction($"{ModulePrefixes.CONFIG_DEFAULT_MANGA_SET_MODAL}")]
    public async Task OnModalResponse(DefaultMangaSetModal modal)
    {
        await DeferAsync();

        var parsedUrl = ParseUrl.ParseMangaUrl(modal.URL);

        if (parsedUrl == null || parsedUrl.Value.platform == null && parsedUrl.Value.series == null)
        {
            var errorEmbed = new EmbedBuilder()
                .WithDescription("Unsupported/invalid URL. Please make sure you're using a link that is supported by the bot."); // TODO: l18n

            await ModifyOriginalResponseAsync(new MessageContents(string.Empty, errorEmbed.Build(), null));
            return;
        }

        await ModifyOriginalResponseAsync(ConfirmPromptContents(new ConfirmState(parsedUrl.Value, 0ul)));
        return;
    }

    [ComponentInteraction(ModulePrefixes.CONFIG_DEFAULT_MANGA_SET_CHANNEL_INPUT + "*")]
    public async Task OnChannelSet(string id, IChannel[] channel)
    {
        // should be doing UpdateAsync but i have no clue how to get that kekw
        await DeferAsync();
        await ModifyOriginalResponseAsync(
            ConfirmPromptContents(
                new ConfirmState(StateSerializer.DeserializeObject<SeriesIdentifier>(id),
                channel.Length > 0 ? channel[0].Id : 0ul)));
    }

    private static MessageContents ConfirmPromptContents(ConfirmState confirmState)
    {
        var embed = new EmbedBuilder()
            .WithDescription($"Set the default manga for **{(confirmState.channelId == 0ul ? "the server" : $"<#{confirmState.channelId}>")}** as {confirmState.series}?");

        var components = new ComponentBuilder()
            .WithSelectMenu(new SelectMenuBuilder()
                .WithCustomId(ModulePrefixes.CONFIG_DEFAULT_MANGA_SET_CHANNEL_INPUT + StateSerializer.SerializeObject(confirmState.series))
                .WithType(ComponentType.ChannelSelect)
                .WithPlaceholder("(Optional) channel.")
                .WithMinValues(0)
                .WithMaxValues(1)
                .WithChannelTypes(
                    ChannelType.Text, ChannelType.News, ChannelType.NewsThread,
                    ChannelType.PublicThread, ChannelType.PrivateThread, ChannelType.Forum))
            .WithButton(new ButtonBuilder()
                .WithLabel("Yes!")
                .WithCustomId(ModulePrefixes.CONFIG_DEFAULT_MANGA_SET_SUBMIT_BUTTON + StateSerializer.SerializeObject(confirmState))
                .WithStyle(ButtonStyle.Success));

        return new MessageContents(string.Empty, embed.Build(), components.Build());
    }

    // step 3 - we've got a submit!!
    [ComponentInteraction(ModulePrefixes.CONFIG_DEFAULT_MANGA_SET_SUBMIT_BUTTON + "*")]
    public async Task OnConfirmed(string id)
    {
        await DeferAsync();

        var state = StateSerializer.DeserializeObject<ConfirmState>(id);

        var toAdd = new Core.Database.Models.DefaultManga()
        {
            GuildId = Context.Guild.Id,
            ChannelId = state.channelId,
            Manga = state.series.ToString()
        };

        // Why is this not a thing yet: https://github.com/dotnet/efcore/issues/4526
        using (var context = dbService.GetDbContext())
        {
            var exists = await context.DefaultMangas.FirstOrDefaultAsync(x => x.GuildId == toAdd.GuildId && x.ChannelId == toAdd.ChannelId);

            if (exists != null)
            {
                exists.Manga = toAdd.Manga;
            }
            else
            {
                context.DefaultMangas.Add(toAdd);
            }

            await context.SaveChangesAsync();
        }

        await ModifyOriginalResponseAsync(new MessageContents("saved!", embed: null, null));
    }
}
