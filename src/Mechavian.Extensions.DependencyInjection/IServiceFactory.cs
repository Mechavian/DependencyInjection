using System;

namespace Mechavian.Extensions.DependencyInjection
{
    public interface IServiceFactory
    {
        object Create(IServiceProvider serviceProvider, Type serviceType);
    }
}