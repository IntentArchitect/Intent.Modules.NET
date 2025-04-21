using Intent.RoslynWeaver.Attributes;
using SqlDbProject.Domain.Entities.Accounts;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace SqlDbProject.Domain.Repositories.Accounts
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface ICurrencyRepository : IEFRepository<Currency, Currency>
    {
        [IntentManaged(Mode.Fully)]
        Task<TProjection?> FindByIdProjectToAsync<TProjection>(int currencyIso, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<Currency?> FindByIdAsync(int currencyIso, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<Currency>> FindByIdsAsync(int[] currencyIsos, CancellationToken cancellationToken = default);
    }
}