using EntityFrameworkCore.SqlServer.EF11.Domain.Accounts;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace EntityFrameworkCore.SqlServer.EF11.Domain.Repositories.Accounts
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface IAccViewRepository : IEFRepository<AccView, AccView>
    {
        [IntentManaged(Mode.Fully)]
        Task<AccView?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<AccView?> FindByIdAsync(Guid id, Func<IQueryable<AccView>, IQueryable<AccView>> queryOptions, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<AccView>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    }
}