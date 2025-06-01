using System.Reflection;
using Intent.Modules.NET.Tests.Application.Core.Common.Eventing;
using Intent.Modules.NET.Tests.Infrastructure.Core.Eventing;
using Intent.Modules.NET.Tests.Module2.Infrastructure.Eventing;
using Intent.RoslynWeaver.Attributes;
using MassTransit;
using MassTransit.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Module1.Eventing.Messages;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.MassTransit.MassTransitConfiguration", Version = "1.0")]

namespace Intent.Modules.NET.Tests.Module2.Infrastructure.Configuration
{
    public static class MassTransitConfiguration
    {
        public static void AddConsumers(this IRegistrationConfigurator cfg)
        {
            cfg.AddConsumer<IntegrationEventConsumer<IIntegrationEventHandler<CustomerCreatedIEEvent>, CustomerCreatedIEEvent>>(typeof(IntegrationEventConsumerDefinition<IIntegrationEventHandler<CustomerCreatedIEEvent>, CustomerCreatedIEEvent>)).Endpoint(config => config.InstanceId = "Module2");
        }
    }
}