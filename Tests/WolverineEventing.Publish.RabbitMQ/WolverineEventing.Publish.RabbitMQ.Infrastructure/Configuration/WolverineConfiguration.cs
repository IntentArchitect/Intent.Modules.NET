using Intent.RoslynWeaver.Attributes;
using Wolverine;
using WolverineEventing.Publish.RabbitMQ.Application.Common.Interfaces;
using WolverineEventing.Publish.RabbitMQ.Application.Orders.FailOrder;
using WolverineEventing.Publish.RabbitMQ.Application.Orders.RequestOrderProcessing;
using WolverineEventing.Publish.RabbitMQ.Application.Orders.ShipOrder;
using WolverineEventing.Publish.RabbitMQ.Infrastructure.Dispatch.Middleware;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Wolverine.WolverineConfiguration", Version = "1.0")]

namespace WolverineEventing.Publish.RabbitMQ.Infrastructure.Configuration
{
    public static class WolverineConfiguration
    {
        public static void Configure(WolverineOptions opts)
        {
            opts.Discovery.IncludeAssembly(typeof(ICommand).Assembly);
            opts.Discovery.IncludeType<FailOrderCommandHandler>();
            opts.Discovery.IncludeType<RequestOrderProcessingCommandHandler>();
            opts.Discovery.IncludeType<ShipOrderCommandHandler>();
            ApplicationHandlerPolicy.Apply(opts);
        }
    }
}