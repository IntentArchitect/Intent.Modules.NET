using Intent.RoslynWeaver.Attributes;
using Wolverine;
using WolverineEventing.Coexist.Cqrs.Application.Common.Interfaces;
using WolverineEventing.Coexist.Cqrs.Application.Orders.CreateOrder;
using WolverineEventing.Coexist.Cqrs.Infrastructure.Dispatch.Middleware;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Wolverine.WolverineConfiguration", Version = "1.0")]

namespace WolverineEventing.Coexist.Cqrs.Infrastructure.Configuration
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