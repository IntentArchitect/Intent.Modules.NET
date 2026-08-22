using Intent.RoslynWeaver.Attributes;
using Wolverine.Publish.RabbitMQ.Domain.Common;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.DomainEvents.DomainEventServiceInterface", Version = "1.0")]

namespace Wolverine.Publish.RabbitMQ.Application.Common.Interfaces
{
    public interface IDomainEventService
    {
        Task Publish(DomainEvent domainEvent, CancellationToken cancellationToken = default);
    }
}