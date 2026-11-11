using EntityFrameworkCore.SqlServer.EF11.Domain.Associations;
using EntityFrameworkCore.SqlServer.EF11.Domain.Repositories;
using EntityFrameworkCore.SqlServer.EF11.Domain.Repositories.Associations;
using EntityFrameworkCore.SqlServer.EF11.Infrastructure.Persistence;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.Repositories.Repository", Version = "1.0")]

namespace EntityFrameworkCore.SqlServer.EF11.Infrastructure.Repositories.Associations
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class M_SelfReferenceBiNavRepository : RepositoryBase<M_SelfReferenceBiNav, M_SelfReferenceBiNav, ApplicationDbContext>, IM_SelfReferenceBiNavRepository
    {
        public M_SelfReferenceBiNavRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<M_SelfReferenceBiNav?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await FindAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<M_SelfReferenceBiNav?> FindByIdAsync(
            Guid id,
            Func<IQueryable<M_SelfReferenceBiNav>, IQueryable<M_SelfReferenceBiNav>> queryOptions,
            CancellationToken cancellationToken = default)
        {
            return await FindAsync(x => x.Id == id, queryOptions, cancellationToken);
        }

        public async Task<List<M_SelfReferenceBiNav>> FindByIdsAsync(
            Guid[] ids,
            CancellationToken cancellationToken = default)
        {
            // Force materialization - Some combinations of .net9 runtime and EF runtime crash with "Convert ReadOnlySpan to List since expression trees can't handle ref struct"
            var idList = ids.ToList();
            return await FindAllAsync(x => idList.Contains(x.Id), cancellationToken);
        }
    }
}