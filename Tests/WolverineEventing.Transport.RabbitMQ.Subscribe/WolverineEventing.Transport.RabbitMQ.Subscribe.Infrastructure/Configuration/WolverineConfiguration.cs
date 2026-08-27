using Intent.RoslynWeaver.Attributes;
using Wolverine;
using WolverineEventing.Transport.RabbitMQ.Subscribe.Application.Common.Interfaces;
using WolverineEventing.Transport.RabbitMQ.Subscribe.Infrastructure.Dispatch.Middleware;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Wolverine.WolverineConfiguration", Version = "1.0")]

namespace WolverineEventing.Transport.RabbitMQ.Subscribe.Infrastructure.Configuration
{
    public static class WolverineConfiguration
    {
        public static void Configure(WolverineOptions opts)
        {
            opts.Discovery.IncludeAssembly(typeof(ICommand).Assembly);
            ApplicationHandlerPolicy.Apply(opts);
        }
    }
}