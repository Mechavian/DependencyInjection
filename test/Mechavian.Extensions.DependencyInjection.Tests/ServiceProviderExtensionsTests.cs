using System;
using Moq;
using Xunit;

namespace Mechavian.Extensions.DependencyInjection
{
    public class ServiceProviderExtensionsTests
    {
        [Fact]
        public void Create_ArgumentNullException()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => ServiceProviderExtensions.Create<object>(null));
            Assert.Equal("serviceProvider", ex.ParamName);
        }

        [Fact]
        public void Create_NoPublicConstructor()
        {
            var serviceProvider = Mock.Of<IServiceProvider>();
            var result = serviceProvider.Create<PrivateConstructorService>();

            Assert.NotNull(result);
        }

        [Fact]
        public void Create_NoConstructorParameters()
        {
            var serviceProvider = Mock.Of<IServiceProvider>();
            var result = serviceProvider.Create<NoParametersService>();

            Assert.NotNull(result);
        }

        [Fact]
        public void Create_ParamServiceExists_NoDefault()
        {
            var service = new NoParametersService();
            var serviceProvider = Mock.Of<IServiceProvider>(s => s.GetService(typeof(NoParametersService)) == service);
            var result = serviceProvider.Create<DependantNoDefaultService>();

            Assert.NotNull(result);
            Assert.Same(service, result.Service);
        }

        [Fact]
        public void Create_ParamServiceExists_WithDefault()
        {
            var service = new NoParametersService();
            var serviceProvider = Mock.Of<IServiceProvider>(s => s.GetService(typeof(NoParametersService)) == service);

            var result = serviceProvider.Create<DependantWithDefaultService>();

            Assert.NotNull(result);
            Assert.Same(service, result.Service);
        }

        [Fact]
        public void Create_ParamServiceNotExists_NoDefault()
        {
            var serviceProvider = Mock.Of<IServiceProvider>();
            Assert.Throws<InvalidOperationException>(() => serviceProvider.Create<DependantNoDefaultService>());
        }

        [Fact]
        public void Create_ParamServiceNotExists_WithDefault()
        {
            var serviceProvider = Mock.Of<IServiceProvider>();
            var result = serviceProvider.Create<DependantWithDefaultService>();

            Assert.NotNull(result);
            Assert.Null(result.Service);
        }

        [Fact]
        public void Create_MultipleConstructors_NoAttribute()
        {
            var serviceProvider = Mock.Of<IServiceProvider>();
            Assert.Throws<InvalidOperationException>(() => serviceProvider.Create<MultipleConstructorsNoAttribService>());
        }

        [Fact]
        public void Create_MultipleConstructors_WithAttribute()
        {
            var service = new NoParametersService();
            var serviceProvider = Mock.Of<IServiceProvider>(s => s.GetService(typeof(NoParametersService)) == service);
            var result = serviceProvider.Create<MultipleConstructorsWithAttribService>();

            Assert.NotNull(result);
            Assert.Same(service, result.Service);
        }

        class PrivateConstructorService
        {
            private PrivateConstructorService()
            {
            }
        }

        class NoParametersService
        {
            public NoParametersService()
            {
            }
        }

        class DependantNoDefaultService
        {
            public NoParametersService Service { get; set; }

            public DependantNoDefaultService(NoParametersService service)
            {
                Service = service;
            }
        }

        class DependantWithDefaultService
        {
            public NoParametersService Service { get; set; }

            public DependantWithDefaultService(NoParametersService service = null)
            {
                Service = service;
            }
        }

        class MultipleConstructorsNoAttribService
        {
            public MultipleConstructorsNoAttribService(int value1)
            {
            }

            public MultipleConstructorsNoAttribService(IService1 service1)
            {
            }
        }

        class MultipleConstructorsWithAttribService
        {
            public NoParametersService Service { get; set; }

            [ServiceConstructor]
            public MultipleConstructorsWithAttribService(NoParametersService service)
            {
                Service = service;
            }

            public MultipleConstructorsWithAttribService(int value1)
            {
            }
        }
    }
}
