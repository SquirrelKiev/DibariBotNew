#pragma warning disable CS8618 // shhh json will take care of it
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System.Runtime.Serialization;

namespace DibariBot.Apis;

// TODO: these are very incomplete. should ideally get all the schemas in here

public class MangaListQueryParams
{
    public int? limit;
    public int? offset;
    public string? title;
    public MangaListQueryOrder? order;
    public ReferenceExpansionMangaSchema[] includes;
}

// no clue if this can be [Flags] or not
[JsonConverter(typeof(StringEnumConverter), typeof(SnakeCaseNamingStrategy))]
public enum ReferenceExpansionMangaSchema
{
    Manga,
    CoverArt,
    Author,
    Artist,
    Tag,
    Creator
}

public class MangaListQueryOrder
{
    [JsonConverter(typeof(StringEnumConverter), typeof(SnakeCaseNamingStrategy))]
    public enum QueryOrderSchema
    {
        [EnumMember(Value = "asc")]
        Ascending,
        [EnumMember(Value = "desc")]
        Descending
    }

    public QueryOrderSchema? relevance;
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
    public TypeSchema type;
    public MangaAttributesSchema attributes;
    public RelationshipSchema[] relationships;
}

public class MangaAttributesSchema
{
    [JsonConverter(typeof(StringEnumConverter), typeof(SnakeCaseNamingStrategy))]
    public enum PublicationDemographic
    {
        Shounen,
        Shoujo,
        Josei,
        Seinen
    }

    [JsonConverter(typeof(StringEnumConverter), typeof(SnakeCaseNamingStrategy))]
    public enum Status
    {
        Completed,
        Ongoing,
        Cancelled,
        Hiatus
    }

    [JsonConverter(typeof(StringEnumConverter), typeof(SnakeCaseNamingStrategy))]
    public enum ContentRating
    {
        Safe,
        Suggestive,
        Erotica,
        Pornographic
    }

    [JsonConverter(typeof(StringEnumConverter), typeof(SnakeCaseNamingStrategy))]
    public enum State
    {
        Draft,
        Submitted,
        Published,
        Rejected
    }

    public LocalizedStringSchema title;
    public LocalizedStringSchema[] altTitles;
    public LocalizedStringSchema description;
    public bool isLocked;
    public Dictionary<string, string> links;
    public string originalLanguage;
    public string? lastVolume;
    public string? lastChapter;
    public PublicationDemographic? publicationDemographic;
    public Status status;
    public int? year;
    public ContentRating contentRating;
    public bool chapterNumbersResetOnNewVolume;
    public string[] availableTranslatedLanguages;
    public string latestUploadedChapter;
    public TagSchema[] tags;
    public State state;
    public int version;
    public DateTime createdAt;
    public DateTime updatedAt;

}

public class TagSchema
{
    public string id;
    public TypeSchema type;
    public TagAttributesSchema attributes;
    public RelationshipSchema[] relationships;
}

[JsonConverter(typeof(StringEnumConverter), typeof(SnakeCaseNamingStrategy))]
public enum TypeSchema
{
    Tag,
    Manga
}

public class TagAttributesSchema
{
    [JsonConverter(typeof(StringEnumConverter), typeof(SnakeCaseNamingStrategy))]
    public enum GroupSchema
    {
        Content,
        Format,
        Genre,
        Theme
    }

    public LocalizedStringSchema name;
    public LocalizedStringSchema description;
    public string group;
    public int version;
}

public class RelationshipSchema
{
    [JsonConverter(typeof(StringEnumConverter), typeof(SnakeCaseNamingStrategy))]
    public enum RelatedSchema
    {
        Monochrome,
        MainStory,
        AdaptedFrom,
        BasedOn,
        Prequel,
        SideStory,
        Doujinshi,
        SameFranchise,
        SharedUniverse,
        Sequel,
        SpinOff,
        AlternateStory,
        AlternateVersion,
        Preserialization,
        Colored,
        Serialization
    }

    public string id;
    public string type;
    public RelatedSchema related;

    /// <remarks>
    /// If Reference Expansion is applied, contains objects attributes
    /// </remarks>
    public object? attributes;
}

public class LocalizedStringSchema : Dictionary<string, string>
{
    public override string ToString()
    {
        return this.FirstOrDefault().Value;
    }
}