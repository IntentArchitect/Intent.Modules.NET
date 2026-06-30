using Intent.RoslynWeaver.Attributes;
using ObjectMappingTest.Domain.Entities;
using ObjectMappingTest.Domain.Repositories;
using ObjectMappingTest.Infrastructure.Persistence;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.Repositories.Repository", Version = "1.0")]

namespace ObjectMappingTest.Infrastructure.Repositories
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class DigitalProductRepository : RepositoryBase<DigitalProduct, DigitalProduct, ApplicationDbContext>, IDigitalProductRepository
    {
        public DigitalProductRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<DigitalProduct?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await FindAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<DigitalProduct?> FindByIdAsync(
            Guid id,
            Func<IQueryable<DigitalProduct>, IQueryable<DigitalProduct>> queryOptions,
            CancellationToken cancellationToken = default)
        {
            return await FindAsync(x => x.Id == id, queryOptions, cancellationToken);
        }

        public async Task<List<DigitalProduct>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default)
        {
            // Force materialization - Some combinations of .net9 runtime and EF runtime crash with "Convert ReadOnlySpan to List since expression trees can't handle ref struct"
            var idList = ids.ToList();
            return await FindAllAsync(x => idList.Contains(x.Id), cancellationToken);
        }
    }
}