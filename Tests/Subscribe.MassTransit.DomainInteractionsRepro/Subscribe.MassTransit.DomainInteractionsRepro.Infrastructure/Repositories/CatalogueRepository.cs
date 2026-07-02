using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using Subscribe.MassTransit.DomainInteractionsRepro.Domain.Entities;
using Subscribe.MassTransit.DomainInteractionsRepro.Domain.Repositories;
using Subscribe.MassTransit.DomainInteractionsRepro.Infrastructure.Persistence;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.Repositories.Repository", Version = "1.0")]

namespace Subscribe.MassTransit.DomainInteractionsRepro.Infrastructure.Repositories
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CatalogueRepository : RepositoryBase<Catalogue, Catalogue, ApplicationDbContext>, ICatalogueRepository
    {
        public CatalogueRepository(ApplicationDbContext dbContext, IMapper mapper) : base(dbContext, mapper)
        {
        }

        public async Task<TProjection?> FindByIdProjectToAsync<TProjection>(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            return await FindProjectToAsync<TProjection>(x => x.Id == id, cancellationToken);
        }

        public async Task<Catalogue?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await FindAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<Catalogue?> FindByIdAsync(
            Guid id,
            Func<IQueryable<Catalogue>, IQueryable<Catalogue>> queryOptions,
            CancellationToken cancellationToken = default)
        {
            return await FindAsync(x => x.Id == id, queryOptions, cancellationToken);
        }

        public async Task<List<Catalogue>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default)
        {
            // Force materialization - Some combinations of .net9 runtime and EF runtime crash with "Convert ReadOnlySpan to List since expression trees can't handle ref struct"
            var idList = ids.ToList();
            return await FindAllAsync(x => idList.Contains(x.Id), cancellationToken);
        }
    }
}