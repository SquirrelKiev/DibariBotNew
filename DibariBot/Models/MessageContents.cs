namespace DibariBot;

public struct MessageContents
{
    public string body;
    public Embed[]? embeds;
    public MessageComponent? components;

    public MessageContents(string body, Embed[] embeds, ComponentBuilder? components)
    {
        this.body = body;
        this.embeds = embeds;

        components ??= new ComponentBuilder().WithRedButton();
        this.components = components?.Build();
    }

    public MessageContents(string body, Embed? embed, ComponentBuilder? components)
    {
        this.body = body;
        embeds = embed == null ? null : new[] { embed };

        components ??= new ComponentBuilder().WithRedButton();

        this.components = components?.Build();
    }

    public MessageContents SetEmbed(Embed embed)
    {
        embeds = new[] { embed };

        return this;
    }
}
