using EntityFrameworkCore.SqlServer.EF11.Domain.PkDataSources;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace EntityFrameworkCore.SqlServer.EF11.Domain.Repositories.PkDataSources
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface IDefaultDataSourceEntityRepository : IEFRepository<DefaultDataSourceEntity, DefaultDataSourceEntity>
    {
        [IntentManaged(Mode.Fully)]
        Task<DefaultDataSourceEntity?> FindByIdAsync(long id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<DefaultDataSourceEntity?> FindByIdAsync(long id, Func<IQueryable<DefaultDataSourceEntity>, IQueryable<DefaultDataSourceEntity>> queryOptions, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<DefaultDataSourceEntity>> FindByIdsAsync(long[] ids, CancellationToken cancellationToken = default);
    }
}