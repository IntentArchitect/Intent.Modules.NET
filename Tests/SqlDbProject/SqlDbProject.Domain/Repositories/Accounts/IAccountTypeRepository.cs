using Intent.RoslynWeaver.Attributes;
using SqlDbProject.Domain.Entities.Accounts;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace SqlDbProject.Domain.Repositories.Accounts
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface IAccountTypeRepository : IEFRepository<AccountType, AccountType>
    {
        [IntentManaged(Mode.Fully)]
        Task<TProjection?> FindByIdProjectToAsync<TProjection>(int accountTypeId, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<AccountType?> FindByIdAsync(int accountTypeId, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<AccountType?> FindByIdAsync(int accountTypeId, Func<IQueryable<AccountType>, IQueryable<AccountType>> queryOptions, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<AccountType>> FindByIdsAsync(int[] accountTypeIds, CancellationToken cancellationToken = default);
    }
}