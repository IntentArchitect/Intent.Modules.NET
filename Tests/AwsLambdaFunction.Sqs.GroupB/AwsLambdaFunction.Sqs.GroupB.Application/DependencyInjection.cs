using System.Reflection;
using AwsLambdaFunction.Sqs.GroupA.Eventing.Messages;
using AwsLambdaFunction.Sqs.GroupB.Application.Common.Behaviours;
using AwsLambdaFunction.Sqs.GroupB.Application.Common.Eventing;
using AwsLambdaFunction.Sqs.GroupB.Application.Common.Validation;
using AwsLambdaFunction.Sqs.GroupB.Application.Implementation;
using AwsLambdaFunction.Sqs.GroupB.Application.IntegrationEvents.EventHandlers;
using AwsLambdaFunction.Sqs.GroupB.Application.Interfaces;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.DependencyInjection.DependencyInjection", Version = "1.0")]

namespace AwsLambdaFunction.Sqs.GroupB.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly(), lifetime: ServiceLifetime.Transient);
            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
                cfg.AddOpenBehavior(typeof(UnhandledExceptionBehaviour<,>));
                cfg.AddOpenBehavior(typeof(PerformanceBehaviour<,>));
                cfg.AddOpenBehavior(typeof(AuthorizationBehaviour<,>));
                cfg.AddOpenBehavior(typeof(MessageBusPublishBehaviour<,>));
                cfg.AddOpenBehavior(typeof(ValidationBehaviour<,>));
            });
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            services.AddScoped<IValidatorProvider, ValidatorProvider>();
            services.AddTransient<IValidationService, ValidationService>();
            services.AddTransient<ISpecificChannelService, SpecificChannelService>();
            services.AddTransient<IIntegrationEventHandler<ClientCreatedEvent>, ClientCreatedEventHandler>();
            services.AddTransient<IIntegrationEventHandler<CreateOrderCommand>, CreateOrderCommandHandler>();
            return services;
        }
    }
}