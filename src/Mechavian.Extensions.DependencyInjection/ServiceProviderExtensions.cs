using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Mechavian.Extensions.DependencyInjection
{
    public static class ServiceProviderExtensions
    {
        public static T Create<T>(this IServiceProvider serviceProvider)
        {
            return (T)Create(serviceProvider, typeof(T));
        }

        public static object Create(this IServiceProvider serviceProvider, Type serviceType)
        {
            if (serviceProvider == null)
            {
                throw new ArgumentNullException(nameof(serviceProvider));
            }

            if (serviceType == null)
            {
                throw new ArgumentNullException(nameof(serviceType));
            }

            var constructor = FindConstructor(serviceType);
            var parameters = constructor.GetParameters();
            var args = parameters.Select(p => GetParameterValue(serviceProvider, p)).ToArray();
            return constructor.Invoke(args);
        }

        private static ConstructorInfo FindConstructor(Type serviceType)
        {
            var constructors = serviceType.GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (constructors.Length == 1)
            {
                return constructors[0];
            }

            constructors = constructors.Where(c => c.GetCustomAttributes<ServiceConstructorAttribute>().Any()).ToArray();
            if (constructors.Length == 1)
            {
                return constructors[0];
            }

            throw new InvalidOperationException($"No Constructor found for type {serviceType.Name}. Service must have only one constructor or must use the ServiceConstructorAttribute.");
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
