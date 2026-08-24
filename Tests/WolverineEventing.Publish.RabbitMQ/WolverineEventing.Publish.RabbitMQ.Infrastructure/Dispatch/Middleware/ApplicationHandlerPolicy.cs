using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.DependencyInjection;
using Wolverine;
using Wolverine.Runtime.Handlers;
using WolverineEventing.Publish.RabbitMQ.Application.Common.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Wolverine.ApplicationHandlerPolicy", Version = "1.0")]

namespace WolverineEventing.Publish.RabbitMQ.Infrastructure.Dispatch.Middleware
{
    internal static class ApplicationHandlerPolicy
    {
        internal static void Apply(WolverineOptions opts)
        {
            opts.Policies.AddMiddleware<AuthorizationMiddleware>(IsApplicationMessage);
            opts.Policies.AddMiddleware<ValidationMiddleware>(IsApplicationMessage);
            opts.Policies.AddMiddleware<LoggingMiddleware>(IsApplicationMessage);
            opts.Policies.AddMiddleware<PerformanceMiddleware>(IsApplicationMessage);
            opts.Policies.AddMiddleware<UnhandledExceptionMiddleware>(IsApplicationMessage);
            opts.Policies.AddMiddleware<UnitOfWorkMiddleware>(c => typeof(ICommand).IsAssignableFrom(c.MessageType));

            opts.Services.AddTransient<AuthorizationMiddleware>();
            opts.Services.AddTransient<ValidationMiddleware>();
            opts.Services.AddTransient<LoggingMiddleware>();
            opts.Services.AddTransient<PerformanceMiddleware>();
            opts.Services.AddTransient<UnhandledExceptionMiddleware>();
            opts.Services.AddTransient<UnitOfWorkMiddleware>();
        }

        private static bool IsApplicationMessage(HandlerChain chain)
        {
            return typeof(ICommand).IsAssignableFrom(chain.MessageType) ||
            typeof(IQuery).IsAssignableFrom(chain.MessageType);
        }
    }
}