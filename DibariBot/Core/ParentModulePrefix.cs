namespace DibariBot;

[AttributeUsage(AttributeTargets.Method)]
public sealed class ParentModulePrefixAttribute(Type parentModule) : Attribute
{
    public Type ParentModule { get; } = parentModule;
}