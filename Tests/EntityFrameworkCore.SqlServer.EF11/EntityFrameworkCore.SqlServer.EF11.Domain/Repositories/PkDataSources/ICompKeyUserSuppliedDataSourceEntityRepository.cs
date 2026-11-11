using EntityFrameworkCore.SqlServer.EF11.Domain.PkDataSources;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace EntityFrameworkCore.SqlServer.EF11.Domain.Repositories.PkDataSources
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface ICompKeyUserSuppliedDataSourceEntityRepository : IEFRepository<CompKeyUserSuppliedDataSourceEntity, CompKeyUserSuppliedDataSourceEntity>
    {
        [IntentManaged(Mode.Fully)]
        Task<CompKeyUserSuppliedDataSourceEntity?> FindByIdAsync((long Id1, long Id2) id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<CompKeyUserSuppliedDataSourceEntity?> FindByIdAsync((long Id1, long Id2) id, Func<IQueryable<CompKeyUserSuppliedDataSourceEntity>, IQueryable<CompKeyUserSuppliedDataSourceEntity>> queryOptions, CancellationToken cancellationToken = default);
    }
}