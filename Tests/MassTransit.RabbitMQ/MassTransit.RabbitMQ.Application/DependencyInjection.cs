using System.Reflection;
using AutoMapper;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;
using MassTransit.RabbitMQ.Application.Common.Behaviours;
using MassTransit.RabbitMQ.Application.Common.Eventing;
using MassTransit.RabbitMQ.Application.IntegrationEventHandlers;
using MassTransit.RabbitMQ.Eventing.Messages;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.DependencyInjection.DependencyInjection", Version = "1.0")]

namespace MassTransit.RabbitMQ.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            services.AddMediatR(Assembly.GetExecutingAssembly());
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(UnhandledExceptionBehaviour<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(PerformanceBehaviour<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(AuthorizationBehaviour<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(EventBusPublishBehaviour<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(UnitOfWorkBehaviour<,>));
            services.AddTransient<IIntegrationEventHandler<TestMessageEvent>, TestMessageEventHandler>();
            return services;
        }
    }
}