namespace DibariBot.Apis;

// TODO: these are very incomplete. should ideally get all the schemas in here
public class MangaListQueryParams
{
    public int? limit;
    public int? offset;
    public string? title;
    public MangaListQueryOrder? order;
}

public class MangaListQueryOrder
{
    public string? relevance;
}

public class MangaListSchema
{
    public string result;
    public string response;
    public MangaSchema[] data;
        public int limit;
        public int offset;
        public int total;
}

public class MangaSchema
{
    public string id;
    public 
}