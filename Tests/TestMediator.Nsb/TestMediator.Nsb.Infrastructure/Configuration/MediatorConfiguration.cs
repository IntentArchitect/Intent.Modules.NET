using Intent.RoslynWeaver.Attributes;
using Mediator;
using Microsoft.Extensions.DependencyInjection;
using TestMediator.Nsb.Application;
using TestMediator.Nsb.Application.Common.Behaviours;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Mediator.MediatorConfiguration", Version = "1.0")]

namespace TestMediator.Nsb.Infrastructure.Configuration
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
                    typeof(ValidationBehaviour<,>),
                    typeof(UnitOfWorkBehaviour<,>)
                ];
            });
            return services;
        }
    }
}