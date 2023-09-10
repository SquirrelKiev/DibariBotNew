using Tomlyn.Model;

namespace DibariBot;

[AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
sealed class TomlMetadataAttribute : Attribute
{
    public string Comment { get; set; } = string.Empty;
    public bool NewlineBefore { get; set; } = false;
    public bool NewlineAfter { get; set; } = false;
    public TomlPropertyDisplayKind DisplayKind { get; set; } = TomlPropertyDisplayKind.Default;
}