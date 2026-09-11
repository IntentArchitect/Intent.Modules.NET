using Intent.RoslynWeaver.Attributes;
using WolverineEventing.Subscribe.RabbitMQ.Domain.Entities;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace WolverineEventing.Subscribe.RabbitMQ.Domain.Repositories
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface IShippedOrderRecordRepository : IEFRepository<ShippedOrderRecord, ShippedOrderRecord>
    {
        [IntentManaged(Mode.Fully)]
        Task<TProjection?> FindByIdProjectToAsync<TProjection>(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<ShippedOrderRecord?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<ShippedOrderRecord?> FindByIdAsync(Guid id, Func<IQueryable<ShippedOrderRecord>, IQueryable<ShippedOrderRecord>> queryOptions, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<ShippedOrderRecord>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    }
}