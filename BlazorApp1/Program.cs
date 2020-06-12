using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace BlazorApp1
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.DetectIncorrectUsageOfTransientDisposables();
            builder.RootComponents.Add<App>("app");

            builder.Services.AddTransient<TransientDisposable>();
            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

            var host = builder.Build();
            host.EnableTransientDisposableDetection();
            await host.RunAsync();
        }
    }

    public class TransientDisposable : IDisposable
    {
        public void Dispose() => throw new NotImplementedException();
    }
}
