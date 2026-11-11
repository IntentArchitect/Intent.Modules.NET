using EntityFrameworkCore.SqlServer.EF11.Domain.Accounts;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace EntityFrameworkCore.SqlServer.EF11.Domain.Repositories.Accounts
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface IAccTableRepository : IEFRepository<AccTable, AccTable>
    {
        [IntentManaged(Mode.Fully)]
        Task<AccTable?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<AccTable?> FindByIdAsync(Guid id, Func<IQueryable<AccTable>, IQueryable<AccTable>> queryOptions, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<AccTable>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    }
}