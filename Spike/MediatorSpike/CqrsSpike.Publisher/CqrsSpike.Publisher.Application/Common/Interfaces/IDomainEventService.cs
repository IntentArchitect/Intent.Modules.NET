using CqrsSpike.Publisher.Domain.Common;

namespace CqrsSpike.Publisher.Application.Common.Interfaces
{
    public interface IDomainEventService
    {
        Task Publish(DomainEvent domainEvent, CancellationToken cancellationToken = default);
    }
}