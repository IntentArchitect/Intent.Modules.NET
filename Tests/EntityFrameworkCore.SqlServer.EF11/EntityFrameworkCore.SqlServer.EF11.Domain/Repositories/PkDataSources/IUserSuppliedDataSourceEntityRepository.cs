using EntityFrameworkCore.SqlServer.EF11.Domain.PkDataSources;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace EntityFrameworkCore.SqlServer.EF11.Domain.Repositories.PkDataSources
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface IUserSuppliedDataSourceEntityRepository : IEFRepository<UserSuppliedDataSourceEntity, UserSuppliedDataSourceEntity>
    {
        [IntentManaged(Mode.Fully)]
        Task<UserSuppliedDataSourceEntity?> FindByIdAsync(long id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<UserSuppliedDataSourceEntity?> FindByIdAsync(long id, Func<IQueryable<UserSuppliedDataSourceEntity>, IQueryable<UserSuppliedDataSourceEntity>> queryOptions, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<UserSuppliedDataSourceEntity>> FindByIdsAsync(long[] ids, CancellationToken cancellationToken = default);
    }
}