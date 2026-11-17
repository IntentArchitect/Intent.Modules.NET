using Intent.RoslynWeaver.Attributes;
using Mediator;
using Microsoft.Extensions.DependencyInjection;
using TestMediator.Hangfire.Application;
using TestMediator.Hangfire.Infrastructure.Dispatch;
using TestMediator.Hangfire.Infrastructure.Dispatch.Behaviours;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Mediator.MediatorConfiguration", Version = "1.0")]

namespace TestMediator.Hangfire.Infrastructure.Configuration
{
    public static class MediatorConfiguration
    {
        public static IServiceCollection AddMediatorConfiguration(this IServiceCollection services)
        {
            services.AddMediator((MediatorOptions options) =>
            {
                options.ServiceLifetime = ServiceLifetime.Transient;
                options.Assemblies = [typeof(Application.DependencyInjection)];
                options.NotificationPublisherType = typeof(SequentialNotificationPublisher);
                options.PipelineBehaviors = [
                    typeof(LoggingBehaviour<,>),
                    typeof(UnhandledExceptionBehaviour<,>),
                    typeof(PerformanceBehaviour<,>),
                    typeof(AuthorizationBehaviour<,>),
                    typeof(ValidationBehaviour<,>)
                ];
            });
            return services;
        }
    }
}