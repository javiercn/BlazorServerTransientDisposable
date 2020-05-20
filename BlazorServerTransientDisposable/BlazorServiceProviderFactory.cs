using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorServerTransientDisposable
{
    public class BlazorServiceProviderFactory : IServiceProviderFactory<IServiceCollection>
    {
        public IServiceCollection CreateBuilder(IServiceCollection services) => services;
        
        public IServiceProvider CreateServiceProvider(IServiceCollection containerBuilder)
        {
            var collection = new ServiceCollection();
            foreach (var descriptor in containerBuilder)
            {
                collection.Add(descriptor switch
                {
                    { ImplementationType: var implementation, Lifetime: ServiceLifetime.Transient } when typeof(IDisposable).IsAssignableFrom(implementation) => CreatePatchedDescriptor(descriptor),
                    _ => descriptor
                });
            }
            collection.AddScoped<ThrowOnTransientDisposable>();

            return collection.BuildServiceProvider();
        }

        private ServiceDescriptor CreatePatchedDescriptor(ServiceDescriptor descriptor)
        {
            var newDescriptor = new ServiceDescriptor(
                descriptor.ServiceType,
                (sp) => {
                    var throwOnTransientDisposable = sp.GetRequiredService<ThrowOnTransientDisposable>();
                    if (throwOnTransientDisposable.ShouldThrow)
                    {
                        throw new InvalidOperationException("Trying to resolve transient disposable service in the wrong scope. Use an 'OwningComponentBase<T>' component base class for the service 'T' you are trying to resolve.");
                    }

                    return descriptor switch
                    {
                        { ImplementationFactory: null, ImplementationType: var type } => ActivatorUtilities.CreateInstance(sp, type),
                        { ImplementationFactory: var factory } => factory(sp)
                    };
                },
                ServiceLifetime.Transient);
            return newDescriptor;
        }
    }

    internal class ThrowOnTransientDisposable
    {
        public bool ShouldThrow { get; set; }
    }
}
