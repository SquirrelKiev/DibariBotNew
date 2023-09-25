namespace DibariBot.Modules.Core;

[AttributeUsage(AttributeTargets.Method)]
public class HelpPageDescriptionAttribute : Attribute
{
    public string Description { get; set; }

    public HelpPageDescriptionAttribute(string description)
    {
        Description = description;
    }
}
