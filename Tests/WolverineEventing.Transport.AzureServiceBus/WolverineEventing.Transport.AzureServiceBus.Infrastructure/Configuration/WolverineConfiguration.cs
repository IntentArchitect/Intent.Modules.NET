using Intent.RoslynWeaver.Attributes;
using Wolverine;
using WolverineEventing.Transport.AzureServiceBus.Application.Common.Interfaces;
using WolverineEventing.Transport.AzureServiceBus.Application.Orders.CreateOrder;
using WolverineEventing.Transport.AzureServiceBus.Infrastructure.Dispatch.Middleware;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Wolverine.WolverineConfiguration", Version = "1.0")]

namespace WolverineEventing.Transport.AzureServiceBus.Infrastructure.Configuration
{
    public static class WolverineConfiguration
    {
        public static void Configure(WolverineOptions opts)
        {
            opts.Discovery.IncludeAssembly(typeof(ICommand).Assembly);
            opts.Discovery.IncludeType<CreateOrderCommandHandler>();
            ApplicationHandlerPolicy.Apply(opts);
        }
    }
}