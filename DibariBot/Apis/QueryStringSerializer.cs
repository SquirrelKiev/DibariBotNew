using Newtonsoft.Json;

namespace DibariBot.Apis;

// why is there no existing stuff i can find that does this
// TODO: method to serialize into a string, string dictionary.
// nested types are serialized the same way, and are appended onto the main dict as fieldname[subfieldkey]
public static class QueryStringSerializer
{
    private static IEnumerable<KeyValuePair<string, string>> SerializeToDict(
        object obj, 
        NullValueHandling nullValueHandling = NullValueHandling.Ignore)
    {
        ArgumentNullException.ThrowIfNull(obj, nameof(obj));

        foreach(var field in obj.GetType().GetFields())
        {
            var val = field.GetValue(obj);

            if(val == null)
            {
                switch (nullValueHandling)
                {
                    case NullValueHandling.Ignore:
                        continue;

                    case NullValueHandling.Include:
                        yield return new(field.Name, "");
                        continue;
                    default:
                        throw new NotImplementedException();
                }
            }

            if(field.FieldType.IsPrimitive || field.FieldType == typeof(string))
            {
                yield return new(field.Name, val.ToString() ?? "");
            }
            else
            {
                foreach(var kvp in SerializeToDict(val, nullValueHandling))
                {
                    yield return new($"{field.Name}[{kvp.Key}]", kvp.Value);
                }
            }
        }
    }
}
