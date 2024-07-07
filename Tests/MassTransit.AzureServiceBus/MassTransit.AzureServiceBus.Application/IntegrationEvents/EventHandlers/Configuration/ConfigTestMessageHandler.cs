using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MassTransit.AzureServiceBus.Application.Common.Eventing;
using MassTransit.AzureServiceBus.Services;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.MassTransit.IntegrationEventHandler", Version = "1.0")]

namespace MassTransit.AzureServiceBus.Application.IntegrationEvents.EventHandlers.Configuration
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class ConfigTestMessageHandler : IIntegrationEventHandler<ConfigTestMessageEvent>
    {
        [IntentManaged(Mode.Merge)]
        public ConfigTestMessageHandler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task HandleAsync(ConfigTestMessageEvent message, CancellationToken cancellationToken = default)
        {
        }
    }
}