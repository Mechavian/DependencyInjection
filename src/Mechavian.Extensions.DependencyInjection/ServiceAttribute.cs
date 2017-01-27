using System;
using Microsoft.Extensions.DependencyInjection;

namespace Mechavian.Extensions.DependencyInjection
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public sealed class ServiceAttribute : Attribute
    {
        public Type ServiceType { get; }

        public ServiceLifetime ServiceLifetime { get; set; } = ServiceLifetime.Singleton;

        public Type ServiceFactory { get; set; }

        public ServiceAttribute(Type serviceType)
        {
            if (serviceType == null)
            {
                throw new ArgumentNullException(nameof(serviceType));
            }

            ServiceType = serviceType;
        }
    }
}