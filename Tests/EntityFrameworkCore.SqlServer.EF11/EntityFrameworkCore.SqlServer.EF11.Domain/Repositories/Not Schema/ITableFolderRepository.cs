using EntityFrameworkCore.SqlServer.EF11.Domain.NotSchema;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace EntityFrameworkCore.SqlServer.EF11.Domain.Repositories.NotSchema
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface ITableFolderRepository : IEFRepository<TableFolder, TableFolder>
    {
        [IntentManaged(Mode.Fully)]
        Task<TableFolder?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<TableFolder?> FindByIdAsync(Guid id, Func<IQueryable<TableFolder>, IQueryable<TableFolder>> queryOptions, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<TableFolder>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    }
}