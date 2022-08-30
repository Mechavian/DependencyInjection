# Mechavian.Extensions.DependencyInjection

## Synopsis

Extensions on top of the [Microsoft.Extensions.DependencyInjection](https://github.com/aspnet/DependencyInjection) to provide more dynamic injections of services.

## Code Example

*Step 1*: Mark services with attributes

```C#
[Service(typeof(IMyService), ServiceLifetime = ServiceLifetime.Singleton)]
internal class MyServiceImpl : IMyService
{
    public MyServiceImpl(IMyDependentService otherService)
    {
    }
}

[Service(typeof(IMyDependentService), ServiceLifetime = ServiceLifetime.Singleton)]
internal class MyDependentServiceImpl : IMyDependentService
{
}
```

*Step 2*: Add attributed services to the IServiceCollection

```C#
services.AddServicesFromAssembly(GetType().Assembly);
```

*Step 3*: Get services as you normally would via the IServiceProvider...

```C#
var myService = serviceProvider.GetService<IMyService>();
```

## Motivation

These extensions remove the need for using a bunch of calls to `serviceCollection.AddService(...)` by using reflection to search for attributed types. 

## Installation

In NuGet package manager, run `Install-Package Mechavian.Extensions.DependencyInjection`.

## License

[MIT License](LICENSE) - Copyright (c) 2022
