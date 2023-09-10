using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DibariBot;

// Any service with this will be auto discovered and marked as a service.
[System.AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Struct, Inherited = true, AllowMultiple = false)]
sealed class InjectAttribute : Attribute
{
    public InjectAttribute(ServiceLifetime serviceLifetime)
    {
        ServiceLifetime = serviceLifetime;
    }

    public ServiceLifetime ServiceLifetime { get; set; }
}

enum ServiceLifetime
{
    Singleton,
    Transient,
}