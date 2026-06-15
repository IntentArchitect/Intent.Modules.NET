using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using Wolverine.CQRS.TestApplication.Domain.Entities.Items;
using Wolverine.CQRS.TestApplication.Domain.Repositories;
using Wolverine.CQRS.TestApplication.Domain.Repositories.Items;
using Wolverine.CQRS.TestApplication.Infrastructure.Persistence;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.Repositories.Repository", Version = "1.0")]

namespace Wolverine.CQRS.TestApplication.Infrastructure.Repositories.Items
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class ItemRepository : RepositoryBase<Item, Item, ApplicationDbContext>, IItemRepository
    {
        public ItemRepository(ApplicationDbContext dbContext, IMapper mapper) : base(dbContext, mapper)
        {
        }

        public async Task<TProjection?> FindByIdProjectToAsync<TProjection>(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            return await FindProjectToAsync<TProjection>(x => x.Id == id, cancellationToken);
        }

        public async Task<Item?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await FindAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<Item?> FindByIdAsync(
            Guid id,
            Func<IQueryable<Item>, IQueryable<Item>> queryOptions,
            CancellationToken cancellationToken = default)
        {
            return await FindAsync(x => x.Id == id, queryOptions, cancellationToken);
        }

        public async Task<List<Item>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default)
        {
            // Force materialization - Some combinations of .net9 runtime and EF runtime crash with "Convert ReadOnlySpan to List since expression trees can't handle ref struct"
            var idList = ids.ToList();
            return await FindAllAsync(x => idList.Contains(x.Id), cancellationToken);
        }
    }
}