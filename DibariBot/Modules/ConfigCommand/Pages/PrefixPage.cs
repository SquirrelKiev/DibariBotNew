using DibariBot.Database;
using Discord.Interactions;

namespace DibariBot.Modules.ConfigCommand.Pages;

public class SetPrefixModal : IModal
{
    public string Title => "Set Prefix";

    [ModalTextInput(customId: ModulePrefixes.CONFIG_PREFIX_MODAL_PREFIX_TEXTBOX, minLength: 1, maxLength: 10)]
    public string Prefix { get; set; } = "";
}

public class PrefixPage(ConfigCommandService configCommandService, DbService dbService) : ConfigPage
{
    public override Page Id => Page.Prefix;
    
    public override string Label => "Prefix";

    public override string Description => "What prefix to use for prefix commands.";

    public override bool EnabledInDMs => false;

    public override async Task<MessageContents> GetMessageContents(ConfigCommandService.State state)
    {
        await using var dbContext = dbService.GetDbContext();

        var config = await dbContext.GetGuildConfig(Context.Guild.Id);
        var prefix = config.Prefix;

        var embed = new EmbedBuilder()
            .WithFields(new EmbedFieldBuilder()
                .WithName("Prefix")
                .WithValue($"`{prefix}`"))
            .WithColor(CommandResult.Default);

        var components = new ComponentBuilder()
            .WithSelectMenu(GetPageSelectDropdown(configCommandService.ConfigPages, Id, IsDm()))
            .WithButton("Change Prefix", ModulePrefixes.CONFIG_PREFIX_MODAL_BUTTON, ButtonStyle.Secondary)
            .WithRedButton();

        return new MessageContents(string.Empty, embed.Build(), components);
    }

    [ComponentInteraction(ModulePrefixes.CONFIG_PREFIX_MODAL_BUTTON)]
    [DefaultMemberPermissions(GuildPermission.ManageGuild)]
    public async Task OnChangeButton()
    {
        await using var dbContext = dbService.GetDbContext();

        var config = await dbContext.GetGuildConfig(Context.Guild.Id);
        var prefix = config.Prefix;

        await Context.Interaction.RespondWithModalAsync<SetPrefixModal>(ModulePrefixes.CONFIG_PREFIX_MODAL, modifyModal:
            builder =>
            {
                builder.UpdateTextInput(ModulePrefixes.CONFIG_PREFIX_MODAL_PREFIX_TEXTBOX,
                    input => input.Value = prefix);
            });
    }

    [ModalInteraction(ModulePrefixes.CONFIG_PREFIX_MODAL)]
    [DefaultMemberPermissions(GuildPermission.ManageGuild)]
    public async Task OnModal(SetPrefixModal modal)
    {
        await DeferAsync();

        await using var dbContext = dbService.GetDbContext();

        var config = await dbContext.GetGuildConfig(Context.Guild.Id);

        config.Prefix = modal.Prefix;

        await dbContext.SaveChangesAsync();

        await ModifyOriginalResponseAsync(
            await configCommandService.GetMessageContents(new ConfigCommandService.State(page: Id), Context));
    }
}