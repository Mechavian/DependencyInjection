using System;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Mechavian.Extensions.DependencyInjection
{
    public static class ServiceProviderExtensions
    {
        public static T Create<T>(this IServiceProvider serviceProvider)
        {
            if (serviceProvider == null)
            {
                throw new ArgumentNullException(nameof(serviceProvider));
            }

            var serviceType = typeof(T);
            var constructor = serviceType.GetConstructors().SingleOrDefault();

            if (constructor == null)
            {
                throw new InvalidOperationException($"No Constructor found for type {serviceType.Name}");
            }

            var parameters = constructor.GetParameters();
            var args = parameters.Select(p => GetParameterValue(serviceProvider, p)).ToArray();
            return (T)constructor.Invoke(args);
        }

        private static object GetParameterValue(IServiceProvider serviceProvider, ParameterInfo parameter)
        {
            if (parameter.HasDefaultValue)
            {
                var value = serviceProvider.GetService(parameter.ParameterType);
                return value ?? parameter.DefaultValue;
            }

            return serviceProvider.GetRequiredService(parameter.ParameterType);
        }
    }
}
