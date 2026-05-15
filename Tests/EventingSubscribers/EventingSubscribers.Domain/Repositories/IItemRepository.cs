using EventingSubscribers.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace EventingSubscribers.Domain.Repositories
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface IItemRepository : IEFRepository<Item, Item>
    {
        [IntentManaged(Mode.Fully)]
        Task<Item?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<Item?> FindByIdAsync(Guid id, Func<IQueryable<Item>, IQueryable<Item>> queryOptions, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<Item>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    }
}