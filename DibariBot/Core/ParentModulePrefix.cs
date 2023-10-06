using Microsoft.Extensions.DependencyInjection;

namespace DibariBot;

[AttributeUsage(AttributeTargets.Method)]
public sealed class ParentModulePrefixAttribute : Attribute
{
    public ParentModulePrefixAttribute(Type parentModule)
    {
        ParentModule = parentModule;
    }

    public Type ParentModule { get; }
}