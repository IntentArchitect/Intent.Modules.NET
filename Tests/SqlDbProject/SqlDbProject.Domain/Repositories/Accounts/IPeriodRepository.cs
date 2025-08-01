using Intent.RoslynWeaver.Attributes;
using SqlDbProject.Domain.Entities.Accounts;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace SqlDbProject.Domain.Repositories.Accounts
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface IPeriodRepository : IEFRepository<Period, Period>
    {
        [IntentManaged(Mode.Fully)]
        Task<TProjection?> FindByIdProjectToAsync<TProjection>(int periodId, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<Period?> FindByIdAsync(int periodId, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<Period?> FindByIdAsync(int periodId, Func<IQueryable<Period>, IQueryable<Period>> queryOptions, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<Period>> FindByIdsAsync(int[] periodIds, CancellationToken cancellationToken = default);
    }
}