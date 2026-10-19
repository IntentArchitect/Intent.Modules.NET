using Intent.RoslynWeaver.Attributes;
using TestMediator.NoEventing.Domain.Products;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace TestMediator.NoEventing.Domain.Repositories.Products
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface IPriceHistoryEntryRepository : IEFRepository<PriceHistoryEntry, PriceHistoryEntry>
    {
        [IntentManaged(Mode.Fully)]
        Task<TProjection?> FindByIdProjectToAsync<TProjection>(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<PriceHistoryEntry?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<PriceHistoryEntry?> FindByIdAsync(Guid id, Func<IQueryable<PriceHistoryEntry>, IQueryable<PriceHistoryEntry>> queryOptions, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<PriceHistoryEntry>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    }
}