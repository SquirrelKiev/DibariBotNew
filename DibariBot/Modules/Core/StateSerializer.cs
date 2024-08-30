using System.ComponentModel;
using System.Reflection;

namespace DibariBot;

// > In .NET 6 and earlier versions, the GetFields method does not return fields in a particular order,
// > such as alphabetical or declaration order. Your code must not depend on the order in which fields are returned,
// > because that order varies. However, starting with .NET 7, the ordering is deterministic based upon the metadata ordering in the assembly.
// https://learn.microsoft.com/en-us/dotnet/api/system.type.getfields?view=net-7.0
// cant make this up, this code is only deterministic in net 7 lol. prefer this new system to alphabetically sorting it or whatever though
// TODO: Move this to a CSV-based implementation
public static class StateSerializer
{
    const char SEPARATOR = '|';

    public static string SerializeObject<T>(T obj, string prefix)
    {
        ArgumentNullException.ThrowIfNull(obj, nameof(obj));

        return prefix + SerializeObject(obj, typeof(T));
    }

    public static string SerializeObject<T>(T obj)
    {
        ArgumentNullException.ThrowIfNull(obj, nameof(obj));

        return SerializeObject(obj, typeof(T));
    }

    private static string SerializeObject(object? obj, Type type)
    {
        if (obj == null) return "";

        if (type == typeof(bool))
        {
            return (bool)obj ? "1" : "0";
        }
        if (type.IsPrimitive || type == typeof(string))
        {
            // https://github.com/dotnet/coreclr/pull/23466
            // tldr crazy people might be returning null from their ToString() overrides for whatever reason
            return obj.ToString() ?? "";
        }

        var nullableType = Nullable.GetUnderlyingType(type);
        if (nullableType != null)
        {
            return SerializeObject(obj, nullableType);
        }

        var serializedParts = new List<string>();

        foreach (var prop in type.GetFields(BindingFlags.Public | BindingFlags.Instance))
        {
            var value = prop.GetValue(obj);

            serializedParts.Add(SerializeObject(value, prop.FieldType));
        }

        return string.Join(SEPARATOR, serializedParts);
    }

    public static T? DeserializeObject<T>(string obj)
    {
        return (T?)DeserializeObject(obj, typeof(T));
    }

    private static object? DeserializeObject(string obj, Type type)
    {
        if (type == typeof(bool))
        {
            return Convert.ChangeType(int.Parse(obj), type);

        }
        if (type.IsPrimitive || type == typeof(string))
        {
            return Convert.ChangeType(obj, type);
        }

        var nullableType = Nullable.GetUnderlyingType(type);
        if (nullableType != null)
        {
            if (string.IsNullOrEmpty(obj))
                return null;

            return DeserializeObject(obj, nullableType);
        }

        var parts = obj.Split(SEPARATOR).ToList();

        object? instance = Activator.CreateInstance(type);

        foreach (var field in type.GetFields(BindingFlags.Public | BindingFlags.Instance))
        {
            if (!parts.Any())
            {
                var attr = field.GetCustomAttribute<DefaultValueAttribute>() ??
                           // field not in the serialized string, and there's no fallback value? must be error
                           throw new InvalidOperationException("Invalid serialized data");

                field.SetValue(instance, attr.Value);
                continue;

            }

            if (Nullable.GetUnderlyingType(field.FieldType) != null)
            {
                if (parts[0] == "")
                {
                    field.SetValue(instance, null);
                    parts.RemoveAt(0);
                    continue;
                }
            }

            string part;
            if (field.FieldType.IsPrimitive || field.FieldType == typeof(string))
            {
                part = parts[0];
                parts.RemoveAt(0);
            }
            else
            {
                var lookaheadType = Nullable.GetUnderlyingType(field.FieldType) ?? field.FieldType;

                int expectedFieldsCount = lookaheadType.GetFields(BindingFlags.Public | BindingFlags.Instance).Length;

                if (parts.Count < expectedFieldsCount)
                    throw new InvalidOperationException("Mismatched serialized data for nested types");

                part = string.Join(SEPARATOR, parts.Take(expectedFieldsCount));
                parts.RemoveRange(0, expectedFieldsCount);
            }

            var value = DeserializeObject(part, field.FieldType);
            field.SetValue(instance, value);
        }

        return instance;
    }
}
