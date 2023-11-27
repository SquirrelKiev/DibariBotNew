using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Globalization;
using System.Text;
using System.Web;

namespace DibariBot.Apis;

// gotta be a faster way of doing this, like,, why is this not built in in some form
// if perf was a concern this wouldn't exist
public static class QueryStringSerializer
{
    public static string ToQueryParams(object obj)
    {
        var dict = ToUrlEncodedKeyValue(obj);

        if (dict is null)
            return "";

        return string.Join("&", dict.Select(kvp => $"{kvp.Key}={kvp.Value}"));
    }

    public static Dictionary<string, string>? ToUrlEncodedKeyValue(object obj)
    {
        if (obj == null)
        {
            return null;
        }

        var serializer = new JsonSerializer
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            NullValueHandling = NullValueHandling.Ignore
        };

        return ToUrlEncodedKeyValue(JObject.FromObject(obj, serializer));
    }

    public static Dictionary<string, string>? ToUrlEncodedKeyValue(JToken? token)
    {
        if (token == null)
        {
            return null;
        }

        var contentData = new Dictionary<string, string>();
        if (token.HasValues)
        {
            foreach (var child in token.Children())
            {
                var childContent = ToUrlEncodedKeyValue(child);
                if (childContent != null)
                {
                    foreach (var kv in childContent)
                    {
                        contentData[kv.Key] = kv.Value;
                    }
                }
            }

            return contentData;
        }

        var jValue = token as JValue;
        if (jValue?.Value == null)
        {
            return null;
        }

        var value = jValue.Type == JTokenType.Date
                    ? jValue.ToString("o", CultureInfo.InvariantCulture)
                    : jValue.ToString(CultureInfo.InvariantCulture);

        StringBuilder customPath = new();
        var split = token.Path.Split('.');
        for (int i = 0; i < split.Length; i++)
        {
            if (i == 0)
                customPath.Append(split[i]);
            else
            {
                customPath.Append('[');
                customPath.Append(HttpUtility.UrlEncode(split[i]));
                customPath.Append(']');
            }
        }

        contentData[customPath.ToString()] = HttpUtility.UrlEncode(value) ?? "";
        return contentData;
    }
}
