using Intent.RoslynWeaver.Attributes;
using Wolverine;
using Wolverine.CQRS.TestApplication.Application.Common.Interfaces;
using Wolverine.Runtime.Handlers;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Wolverine.ApplicationHandlerPolicy", Version = "1.0")]

namespace Wolverine.CQRS.TestApplication.Infrastructure.Dispatch.Middleware
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
        }

        private static bool IsApplicationMessage(HandlerChain chain)
        {
            return typeof(ICommand).IsAssignableFrom(chain.MessageType) ||
                typeof(IQuery).IsAssignableFrom(chain.MessageType);
        }
    }
}
