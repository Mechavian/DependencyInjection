using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace Mechavian.Extensions.DependencyInjection
{
    public class ServiceCollectionExtensionsTests
    {
        [Fact]
        public void AddServicesFromAssembly_ArgumentNullException()
        {
            ArgumentNullException ex;

            var services = Mock.Of<IServiceCollection>();
            var assembly = GetType().Assembly;

            ex = Assert.Throws<ArgumentNullException>(() => ServiceCollectionExtensions.AddServicesFromAssembly(null, assembly));
            Assert.Equal("serviceCollection", ex.ParamName);

            ex = Assert.Throws<ArgumentNullException>(() => services.AddServicesFromAssembly(null));
            Assert.Equal("assembly", ex.ParamName);
        }

        [Fact]
        public void AddServicesFromAssembly_ServiceList()
        {
            List<ServiceDescriptor> descriptors = new List<ServiceDescriptor>();

            var servicesMock = new Mock<IServiceCollection>();
            servicesMock.Setup(s => s.Add(It.IsAny<ServiceDescriptor>()))
                        .Callback<ServiceDescriptor>((d) => descriptors.Add(d));
            var services = servicesMock.Object;
            var assembly = GetType().Assembly;

            var result = services.AddServicesFromAssembly(assembly);

            Assert.Same(services, result);
            Assert.Equal(5, descriptors.Count);
        }

        [Fact]
        public void AddServicesFromAssembly_ServiceList_FilterType()
        {
            List<ServiceDescriptor> descriptors = new List<ServiceDescriptor>();

            var servicesMock = new Mock<IServiceCollection>();
            servicesMock.Setup(s => s.Add(It.IsAny<ServiceDescriptor>()))
                        .Callback<ServiceDescriptor>((d) => descriptors.Add(d));
            var services = servicesMock.Object;
            var assembly = GetType().Assembly;
            var options = new ServiceLoadOptions()
                          {
                              TypeFilter = (t) => t.FullName == typeof(Service1Service).FullName
                          };

            var result = services.AddServicesFromAssembly(assembly, options);

            Assert.Same(services, result);
            Assert.Equal(3, descriptors.Count);

            Assert.Equal(typeof(IService1), descriptors[0].ServiceType);
            Assert.Equal(ServiceLifetime.Singleton, descriptors[0].Lifetime);

            Assert.Equal(typeof(IService1), descriptors[1].ServiceType);
            Assert.Equal(ServiceLifetime.Scoped, descriptors[1].Lifetime);

            Assert.Equal(typeof(IService1), descriptors[2].ServiceType);
            Assert.Equal(ServiceLifetime.Transient, descriptors[2].Lifetime);
        }

        [Fact]
        public void AddServicesFromAssembly_ServiceList_CustomFactory()
        {
            List<ServiceDescriptor> descriptors = new List<ServiceDescriptor>();

            var servicesMock = new Mock<IServiceCollection>();
            servicesMock.Setup(s => s.Add(It.IsAny<ServiceDescriptor>()))
                        .Callback<ServiceDescriptor>((d) => descriptors.Add(d));
            var services = servicesMock.Object;
            var assembly = GetType().Assembly;
            var options = new ServiceLoadOptions()
            {
                TypeFilter = (t) => t.FullName == typeof(Service2Service).FullName
            };

            var expectedService = new object();
            var mockFactory = Mock.Of<IServiceFactory>(s => s.Create(It.IsAny<IServiceProvider>(), typeof (Service2Service)) == expectedService);
            var mockProvider = Mock.Of<IServiceProvider>(s => s.GetService(typeof (Service2Factory)) == mockFactory);

            var result = services.AddServicesFromAssembly(assembly, options);

            Assert.Same(services, result);
            Assert.Single(descriptors);

            var serviceDescriptor = descriptors[0];
            var service = serviceDescriptor.ImplementationFactory(mockProvider);
            Assert.Same(expectedService, service);
        }

        [Fact]
        public void AddServicesFromAssembly_MultipleServices()
        {
            var serviceCollection = new ServiceCollection();
            var assembly = GetType().Assembly;
            var options = new ServiceLoadOptions()
            {
                TypeFilter = (t) => t.FullName == typeof(Service1Service).FullName
            };

            serviceCollection.AddServicesFromAssembly(assembly, options);

            var provider = serviceCollection.BuildServiceProvider();
            var services = provider.GetServices<IService1>();

            Assert.Equal(3, services.Count());
        }
    }

    public interface IService1
    {
    }

    [Service(typeof(Service2Factory))]
    public class Service2Factory : IServiceFactory
    {
        public object Create(IServiceProvider serviceProvider, Type serviceType)
        {
            throw new NotImplementedException("Note: this is overridden in a mock, so no need to implement");
        }
    }

    [Service(typeof(IService1), ServiceFactory = typeof(Service2Factory))]
    public class Service2Service
    {
        private readonly int _value;

        public Service2Service(int value)
        {
            _value = value;
        }
    }

    [Service(typeof(IService1), ServiceLifetime = ServiceLifetime.Singleton)]
    [Service(typeof(IService1), ServiceLifetime = ServiceLifetime.Scoped)]
    [Service(typeof(IService1), ServiceLifetime = ServiceLifetime.Transient)]
    public class Service1Service : IService1
    {
    }
}
