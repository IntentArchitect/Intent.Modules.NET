using EventingSubscribers.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace EventingSubscribers.Domain.Repositories
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface IShipmentRepository : IEFRepository<Shipment, Shipment>
    {
        [IntentManaged(Mode.Fully)]
        Task<Shipment?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<Shipment?> FindByIdAsync(Guid id, Func<IQueryable<Shipment>, IQueryable<Shipment>> queryOptions, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<Shipment>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    }
}