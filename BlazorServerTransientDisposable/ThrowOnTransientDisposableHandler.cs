using Microsoft.AspNetCore.Components.Server.Circuits;

namespace BlazorServerTransientDisposable
{
    internal class ThrowOnTransientDisposableHandler : CircuitHandler
    {
        public ThrowOnTransientDisposableHandler(ThrowOnTransientDisposable throwOnTransientDisposable)
        {
            throwOnTransientDisposable.ShouldThrow = true;
        }
    }
}