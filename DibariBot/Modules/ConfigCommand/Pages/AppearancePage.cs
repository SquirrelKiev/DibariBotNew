using System.Diagnostics.Contracts;
using DibariBot.Database;
using Discord.Interactions;

namespace DibariBot.Modules.ConfigCommand.Pages;

[ConfigPage(Id, "Appearance", "How Bot messages will look.", Conditions.NotInDm)]
public partial class AppearancePage(ConfigCommandService configCommandService, DbService dbService, ColorProvider colorProvider, BotConfig config)
    : BotModule,
        IConfigPage
{
    public class SetColorModal : IModal
    {
        public string Title => "Set Embed Color";

        [ModalTextInput(
            customId: ModulePrefixes.CONFIG_APPEARANCE_CHANGE_COLOR_MODAL_COLOR_TEXTBOX,
            minLength: 0,
            maxLength: 20,
            placeholder: "#FFFFFF"
        )]
        public string Color { get; set; } = "";
    }

    public const Page Id = Page.Appearance;

    public Task<MessageContents> GetMessageContents(ConfigCommandService.State state) =>
        GetMessageContents();

    public async Task<MessageContents> GetMessageContents()
    {
        await using var context = dbService.GetDbContext();

        var guildConfig = await context.GetGuildConfig(Context.Guild.Id);

        var eb = new EmbedBuilder()
            .WithFields(
                new EmbedFieldBuilder()
                    .WithName("Embed Color")
                    .WithValue(guildConfig.EmbedColor.HasValue ? $"`#{guildConfig.EmbedColor:X6}`" : $"Default (`#{config.DefaultEmbedColor:X6}`)")
            )
            .WithColor(colorProvider.GetEmbedColor(guildConfig));

        var components = new ComponentBuilder()
            .WithSelectMenu(configCommandService.GetPageSelectDropdown(Id, IsUserInstallInteraction()))
            .WithButton(
                "Embed Color",
                ModulePrefixes.CONFIG_APPEARANCE_CHANGE_COLOR_BUTTON,
                ButtonStyle.Secondary
            )
            .WithRedButton();

        return new MessageContents(eb, components);
    }

    [ComponentInteraction(ModulePrefixes.CONFIG_APPEARANCE_CHANGE_COLOR_BUTTON)]
    public async Task OpenModalButton()
    {
        await using var context = dbService.GetDbContext();

        var guildConfig = await context.GetGuildConfig(Context.Guild.Id);

        await RespondWithModalAsync<SetColorModal>(
            ModulePrefixes.CONFIG_APPEARANCE_CHANGE_COLOR_MODAL,
            modifyModal: x =>
            {
                x.UpdateTextInput(
                    ModulePrefixes.CONFIG_APPEARANCE_CHANGE_COLOR_MODAL_COLOR_TEXTBOX,
                    i =>
                    {
                        i.Value = guildConfig.EmbedColor == null ? "" : $"#{guildConfig.EmbedColor:X6}";
                        i.Placeholder = $"#{config.DefaultEmbedColor}";
                        i.Required = false;
                    }
                );
            }
        );
    }

    [ModalInteraction(ModulePrefixes.CONFIG_APPEARANCE_CHANGE_COLOR_MODAL)]
    public async Task ModalResponse(SetColorModal modal)
    {
        await DeferAsync();

        await using var context = dbService.GetDbContext();

        var guildConfig = await context.GetGuildConfig(Context.Guild.Id);

        // nullable structs are a PITA that i CBA to deal with in small stuff like this
        var color = -1;
        if(modal.Color != "")
        {
            try
            {
                var value = modal.Color;

                color = (int)ParseColorInput(value).RawValue;
            }
            catch
            {
                // we don't care why it failed to parse the color we just know it did, and in that case color will stay -1
            }
        }

        if (color == -1)
        {
            guildConfig.EmbedColor = null;

            await context.SaveChangesAsync();
        }
        else
        {
            guildConfig.EmbedColor = color;

            await context.SaveChangesAsync();
        }

        await ModifyOriginalResponseAsync(await GetMessageContents());
    }

    [Pure]
    private static Color ParseColorInput(string input)
    {
        var color = System.Drawing.ColorTranslator.FromHtml(input);

        return new Color(color.R, color.G, color.B);
    }
}
