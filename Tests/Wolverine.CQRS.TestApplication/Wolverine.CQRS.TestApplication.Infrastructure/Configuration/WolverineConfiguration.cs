using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.DependencyInjection;
using Wolverine;
using Wolverine.CQRS.TestApplication.Application.Common.Interfaces;
using Wolverine.CQRS.TestApplication.Infrastructure.Dispatch.Middleware;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Wolverine.WolverineConfiguration", Version = "1.0")]

namespace Wolverine.CQRS.TestApplication.Infrastructure.Configuration
{
    public static class WolverineConfiguration
    {
        public static void Configure(WolverineOptions opts)
        {
            opts.Discovery.IncludeAssembly(typeof(ICommand).Assembly);
            ApplicationHandlerPolicy.Apply(opts);
            opts.Services.AddTransient<AuthorizationMiddleware>();
            opts.Services.AddTransient<ValidationMiddleware>();
            opts.Services.AddTransient<LoggingMiddleware>();
            opts.Services.AddTransient<PerformanceMiddleware>();
            opts.Services.AddTransient<UnhandledExceptionMiddleware>();
            opts.Services.AddTransient<UnitOfWorkMiddleware>();
        }
    }
}