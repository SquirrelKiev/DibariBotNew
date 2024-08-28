namespace DibariBot;

// Any service with this will be auto discovered and marked as a service.
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Struct)]
public sealed class InjectAttribute : Attribute
{
    public InjectAttribute(ServiceLifetime serviceLifetime)
    {
        ServiceLifetime = serviceLifetime;
    }

    public ServiceLifetime ServiceLifetime { get; }
}