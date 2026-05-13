using EventingSubscribers.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace EventingSubscribers.Domain.Repositories
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface IShipTaskRepository : IEFRepository<ShipTask, ShipTask>
    {
        [IntentManaged(Mode.Fully)]
        Task<ShipTask?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<ShipTask?> FindByIdAsync(Guid id, Func<IQueryable<ShipTask>, IQueryable<ShipTask>> queryOptions, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<ShipTask>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    }
}