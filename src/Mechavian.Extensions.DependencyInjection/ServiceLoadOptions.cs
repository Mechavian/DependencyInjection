using System;
using System.Reflection;

namespace Mechavian.Extensions.DependencyInjection
{
    public sealed class ServiceLoadOptions
    {
        public Func<TypeInfo, bool> TypeFilter { get; set; }
    }
}