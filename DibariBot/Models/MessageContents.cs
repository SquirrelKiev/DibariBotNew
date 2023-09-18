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
        if (embed != null)
            embeds = new Embed[] { embed };
        else
            embeds = null;

        components ??= new ComponentBuilder().WithRedButton();

        this.components = components?.Build();
    }

    public MessageContents SetEmbed(Embed embed)
    {
        embeds = new Embed[] { embed };

        return this;
    }
}
