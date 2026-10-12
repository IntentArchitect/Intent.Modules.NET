using CqrsSpike.Publisher.Application.Common.Behaviours;
using Mediator;
using Microsoft.Extensions.DependencyInjection;

namespace CqrsSpike.Publisher.Infrastructure.Configuration
{
    public static class MediatorConfiguration
    {
        public static IServiceCollection AddMediatorConfiguration(this IServiceCollection services)
        {
            // The source generator reads these options from source at compile time, so every value
            // must stay a literal (D10): a variable or helper silently drops what it holds (P5).
            services.AddMediator((MediatorOptions options) =>
            {
                // A fresh handler and behaviour per dispatch, as in MediatR and Wolverine (D23).
                options.ServiceLifetime = ServiceLifetime.Transient;
                options.Assemblies = [typeof(Application.DependencyInjection)];
                // One handler at a time, stopping at the first failure (D22).
                options.NotificationPublisherType = typeof(SequentialNotificationPublisher);
                options.PipelineBehaviors =
                [
                    typeof(LoggingBehaviour<,>),
                    typeof(UnhandledExceptionBehaviour<,>),
                    typeof(PerformanceBehaviour<,>),
                    typeof(AuthorizationBehaviour<,>),
                    typeof(MessageBusPublishBehaviour<,>),
                    typeof(ValidationBehaviour<,>),
                    typeof(UnitOfWorkBehaviour<,>)
                ];
            });
            return services;
        }
    }
}
