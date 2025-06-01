using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Contracts.IntegrationEventHandlerInterface", Version = "1.0")]

namespace AzureFunctions.AzureServiceBus.Application.Common.Eventing
{
    public interface IIntegrationEventHandler<in TMessage>
        where TMessage : class
    {
        Task HandleAsync(TMessage message, CancellationToken cancellationToken = default);
    }
}