using Intent.RoslynWeaver.Attributes;
using Wolverine;
using Wolverine.AspNetCore.FastEndpoints.Application.Common.Interfaces;
using Wolverine.AspNetCore.FastEndpoints.Infrastructure.Dispatch.Middleware;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Wolverine.WolverineConfiguration", Version = "1.0")]

namespace Wolverine.AspNetCore.FastEndpoints.Infrastructure.Configuration
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