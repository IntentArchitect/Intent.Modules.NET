using AdvancedMapping.Repositories.Mapperly.Tests.Domain.Entities;
using AdvancedMapping.Repositories.Mapperly.Tests.Domain.Repositories;
using AdvancedMapping.Repositories.Mapperly.Tests.Infrastructure.Persistence;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.Repositories.Repository", Version = "1.0")]

namespace AdvancedMapping.Repositories.Mapperly.Tests.Infrastructure.Repositories
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class ContainerRepository : RepositoryBase<Container, Container, ApplicationDbContext>, IContainerRepository
    {
        public ContainerRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<Container?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await FindAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<Container?> FindByIdAsync(
            Guid id,
            Func<IQueryable<Container>, IQueryable<Container>> queryOptions,
            CancellationToken cancellationToken = default)
        {
            return await FindAsync(x => x.Id == id, queryOptions, cancellationToken);
        }

        public async Task<List<Container>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default)
        {
            // Force materialization - Some combinations of .net9 runtime and EF runtime crash with "Convert ReadOnlySpan to List since expression trees can't handle ref struct"
            var idList = ids.ToList();
            return await FindAllAsync(x => idList.Contains(x.Id), cancellationToken);
        }
    }
}