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
                                       return serviceFactory.Create(sp, definedType.AsType());
                                   });
                    }
                    else
                    {
                        factory = (sp) => sp.Create(definedType.AsType());
                    }

                    serviceCollection.Add(new ServiceDescriptor(serviceAttribute.ServiceType, factory, serviceAttribute.ServiceLifetime));
                }
            }

            return serviceCollection;
        }
    }
}
