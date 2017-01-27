using System;

namespace Mechavian.Extensions.DependencyInjection
{
    [AttributeUsage(AttributeTargets.Constructor)]
    public sealed class ServiceConstructorAttribute : Attribute
    {
    }
}