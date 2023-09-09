using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DibariBot.Modules.Manga;

public class SortedGroupsDictionaryConverter<TValue> : JsonConverter
{
    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(SortedDictionary<string, TValue>);
    }

    public override object ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
    {
        var obj = JToken.ReadFrom(reader);
        if (objectType == typeof(Dictionary<string, TValue>))
        {
            var sortedDict = new SortedDictionary<string, TValue>(new ChapterNameComparer());

            serializer.Populate(reader, sortedDict);

            return sortedDict;
        }

        var typedObj = obj.ToObject(objectType);

        // y'never know
        if (typedObj == null)
            throw new NullReferenceException(nameof(typedObj));

        return typedObj;
    }

    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
    {
        serializer.Serialize(writer, value);
    }
}