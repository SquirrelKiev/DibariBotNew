using Microsoft.Extensions.DependencyInjection;

namespace DibariBot;

// Any service with this will be auto discovered and marked as a service.
[System.AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Struct, Inherited = true, AllowMultiple = false)]
public sealed class InjectAttribute : Attribute
{
    public InjectAttribute(ServiceLifetime serviceLifetime)
    {
        ServiceLifetime = serviceLifetime;
    }

    public ServiceLifetime ServiceLifetime { get; set; }
}