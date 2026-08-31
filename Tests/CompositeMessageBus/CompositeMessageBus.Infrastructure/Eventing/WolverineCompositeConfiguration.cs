using CompositeMessageBus.Eventing.Messages;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Wolverine.WolverineCompositeConfiguration", Version = "1.0")]

namespace CompositeMessageBus.Infrastructure.Eventing
{
    public static class WolverineCompositeConfiguration
    {
        public static IServiceCollection AddWolverineEventingConfiguration(
            this IServiceCollection services,
            IConfiguration configuration,
            MessageBrokerRegistry registry)
        {
            services.AddScoped<WolverineMessageBus>();

            registry.Register<MsgWolverineEvent, WolverineMessageBus>();

            return services;
        }
    }
}