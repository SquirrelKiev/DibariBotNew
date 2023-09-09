namespace DibariBot;

public struct MessageContents
{
    public string body;
    public Embed[]? embeds;
    public MessageComponent? components;

    public MessageContents(string body, Embed[] embeds, MessageComponent? components)
    {
        this.body = body;
        this.embeds = embeds;
        this.components = components;
    }

    public MessageContents(string body, Embed? embed, MessageComponent? components)
    {
        this.body = body;
        if (embed != null)
            embeds = new Embed[] { embed };
        else
            embeds = null;
        this.components = components;
    }

    public MessageContents SetEmbed(Embed embed)
    {
        embeds = new Embed[] { embed };

        return this;
    }
}
