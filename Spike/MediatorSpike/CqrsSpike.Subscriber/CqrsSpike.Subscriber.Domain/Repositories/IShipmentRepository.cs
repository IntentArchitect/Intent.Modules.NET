using CqrsSpike.Subscriber.Domain.Entities;

namespace CqrsSpike.Subscriber.Domain.Repositories
{
    public interface IShipmentRepository : IEFRepository<Shipment, Shipment>
    {
        Task<TProjection?> FindByIdProjectToAsync<TProjection>(Guid id, CancellationToken cancellationToken = default);
        Task<Shipment?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<Shipment?> FindByIdAsync(Guid id, Func<IQueryable<Shipment>, IQueryable<Shipment>> queryOptions, CancellationToken cancellationToken = default);
        Task<List<Shipment>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    }
}