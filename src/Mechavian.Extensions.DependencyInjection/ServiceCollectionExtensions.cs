using System;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Mechavian.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddServicesFromAssembly(this IServiceCollection serviceCollection, Assembly assembly, ServiceLoadOptions options = null)
        {
            if (serviceCollection == null)
            {
                throw new ArgumentNullException(nameof(serviceCollection));
            }

            if (assembly == null)
            {
                throw new ArgumentNullException(nameof(assembly));
            }

            options = options ?? new ServiceLoadOptions();

            Func<TypeInfo, bool> filterType = options.TypeFilter ?? ((t) => true);

            foreach (var definedType in assembly.DefinedTypes.Where(filterType))
            {
                foreach (var serviceAttribute in definedType.GetCustomAttributes<ServiceAttribute>())
                {
                    Func<IServiceProvider, object> factory;

                    if (serviceAttribute.ServiceFactory != null)
                    {
                        factory = (sp =>
                                   {
                                       // Get factory in the same scope as the service
                                       var serviceFactory = (IServiceFactory)sp.GetService(serviceAttribute.ServiceFactory);
                                       return serviceFactory.Create(sp, serviceAttribute.ServiceType);
                                   });
                    }
                    else
                    {
                        factory = (sp) => sp.Create(serviceAttribute.ServiceType);
                    }

                    serviceCollection.Add(new ServiceDescriptor(serviceAttribute.ServiceType, factory, serviceAttribute.ServiceLifetime));
                }
            }

            return serviceCollection;
        }
    }

    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = true)]
    public class ServiceAttribute : Attribute
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

    public interface IServiceFactory
    {
        object Create(IServiceProvider serviceProvider, Type serviceType);
    }

    public sealed class ServiceLoadOptions
    {
        public Func<TypeInfo, bool> TypeFilter { get; set; }
    }
}
