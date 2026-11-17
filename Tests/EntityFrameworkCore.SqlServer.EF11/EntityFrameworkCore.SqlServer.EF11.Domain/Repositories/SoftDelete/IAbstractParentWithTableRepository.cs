using EntityFrameworkCore.SqlServer.EF11.Domain.SoftDelete;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace EntityFrameworkCore.SqlServer.EF11.Domain.Repositories.SoftDelete
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface IAbstractParentWithTableRepository : IEFRepository<AbstractParentWithTable, AbstractParentWithTable>
    {
        [IntentManaged(Mode.Fully)]
        Task<AbstractParentWithTable?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<AbstractParentWithTable?> FindByIdAsync(Guid id, Func<IQueryable<AbstractParentWithTable>, IQueryable<AbstractParentWithTable>> queryOptions, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<AbstractParentWithTable>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    }
}