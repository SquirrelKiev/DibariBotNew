namespace DibariBot;

public struct MessageContents
{
    /// <summary>
    /// Whether the red close button should be added when no component builder is specified.
    /// </summary>
    public static bool AddRedButtonDefault = true;

    public string body;
    public Embed[]? embeds;
    public MessageComponent? components;

    public MessageContents(string body, Embed[] embeds, ComponentBuilder? components)
    {
        this.body = body;
        this.embeds = embeds;

        if (AddRedButtonDefault)
            components ??= new ComponentBuilder().WithRedButton();
        this.components = components?.Build();
    }

    public MessageContents(string body = "", Embed? embed = null, ComponentBuilder? components = null)
    {
        this.body = body;
        embeds = embed == null ? null : [embed];

        if (AddRedButtonDefault)
            components ??= new ComponentBuilder().WithRedButton();

        if (components != null)
            this.components = components.Build();
    }

    public MessageContents(EmbedBuilder embed, ComponentBuilder? components = null, string body = "")
    {
        this.body = body;
        embeds = new[] { embed.Build() };

        if (AddRedButtonDefault)
            components ??= new ComponentBuilder().WithRedButton();

        if (components != null)
            this.components = components.Build();
    }

    public MessageContents SetEmbed(Embed embed)
    {
        embeds = new[] { embed };

        return this;
    }
}
