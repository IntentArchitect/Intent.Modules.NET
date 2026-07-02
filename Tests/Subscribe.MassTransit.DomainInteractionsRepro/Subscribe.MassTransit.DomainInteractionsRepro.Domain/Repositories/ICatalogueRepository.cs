using Intent.RoslynWeaver.Attributes;
using Subscribe.MassTransit.DomainInteractionsRepro.Domain.Entities;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace Subscribe.MassTransit.DomainInteractionsRepro.Domain.Repositories
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface ICatalogueRepository : IEFRepository<Catalogue, Catalogue>
    {
        [IntentManaged(Mode.Fully)]
        Task<TProjection?> FindByIdProjectToAsync<TProjection>(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<Catalogue?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<Catalogue?> FindByIdAsync(Guid id, Func<IQueryable<Catalogue>, IQueryable<Catalogue>> queryOptions, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<Catalogue>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    }
}