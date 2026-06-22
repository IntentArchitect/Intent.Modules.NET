using Intent.RoslynWeaver.Attributes;
using Wolverine;
using Wolverine.AzureFunctions.Application.Common.Interfaces;
using Wolverine.AzureFunctions.Infrastructure.Dispatch.Middleware;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Wolverine.WolverineConfiguration", Version = "1.0")]

namespace Wolverine.AzureFunctions.Infrastructure.Configuration
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