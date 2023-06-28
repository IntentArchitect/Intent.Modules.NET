using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MassTransit.AzureServiceBus.Domain.Common;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.DomainEvents.DomainEventServiceInterface", Version = "1.0")]

namespace MassTransit.AzureServiceBus.Application.Common.Interfaces
{
    public interface IDomainEventService
    {
        Task Publish(DomainEvent domainEvent, CancellationToken cancellationToken = default);
    }
}