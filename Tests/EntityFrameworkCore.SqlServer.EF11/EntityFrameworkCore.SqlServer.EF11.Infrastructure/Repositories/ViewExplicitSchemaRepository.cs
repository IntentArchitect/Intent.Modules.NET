using EntityFrameworkCore.SqlServer.EF11.Domain;
using EntityFrameworkCore.SqlServer.EF11.Domain.Repositories;
using EntityFrameworkCore.SqlServer.EF11.Infrastructure.Persistence;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.Repositories.Repository", Version = "1.0")]

namespace EntityFrameworkCore.SqlServer.EF11.Infrastructure.Repositories
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class ViewExplicitSchemaRepository : RepositoryBase<ViewExplicitSchema, ViewExplicitSchema, ApplicationDbContext>, IViewExplicitSchemaRepository
    {
        public ViewExplicitSchemaRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<ViewExplicitSchema?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await FindAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<ViewExplicitSchema?> FindByIdAsync(
            Guid id,
            Func<IQueryable<ViewExplicitSchema>, IQueryable<ViewExplicitSchema>> queryOptions,
            CancellationToken cancellationToken = default)
        {
            return await FindAsync(x => x.Id == id, queryOptions, cancellationToken);
        }

        public async Task<List<ViewExplicitSchema>> FindByIdsAsync(
            Guid[] ids,
            CancellationToken cancellationToken = default)
        {
            // Force materialization - Some combinations of .net9 runtime and EF runtime crash with "Convert ReadOnlySpan to List since expression trees can't handle ref struct"
            var idList = ids.ToList();
            return await FindAllAsync(x => idList.Contains(x.Id), cancellationToken);
        }
    }
}