using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using SqlDbProject.Domain.Entities.Accounts;
using SqlDbProject.Domain.Repositories;
using SqlDbProject.Domain.Repositories.Accounts;
using SqlDbProject.Infrastructure.Persistence;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.Repositories.Repository", Version = "1.0")]

namespace SqlDbProject.Infrastructure.Repositories.Accounts
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CurrencyRepository : RepositoryBase<Currency, Currency, ApplicationDbContext>, ICurrencyRepository
    {
        public CurrencyRepository(ApplicationDbContext dbContext, IMapper mapper) : base(dbContext, mapper)
        {
        }

        public async Task<TProjection?> FindByIdProjectToAsync<TProjection>(
            int currencyIso,
            CancellationToken cancellationToken = default)
        {
            return await FindProjectToAsync<TProjection>(x => x.CurrencyIso == currencyIso, cancellationToken);
        }

        public async Task<Currency?> FindByIdAsync(int currencyIso, CancellationToken cancellationToken = default)
        {
            return await FindAsync(x => x.CurrencyIso == currencyIso, cancellationToken);
        }

        public async Task<Currency?> FindByIdAsync(
            int currencyIso,
            Func<IQueryable<Currency>, IQueryable<Currency>> queryOptions,
            CancellationToken cancellationToken = default)
        {
            return await FindAsync(x => x.CurrencyIso == currencyIso, queryOptions, cancellationToken);
        }

        public async Task<List<Currency>> FindByIdsAsync(int[] currencyIsos, CancellationToken cancellationToken = default)
        {
            // Force materialization - Some combinations of .net9 runtime and EF runtime crash with "Convert ReadOnlySpan to List since expression trees can't handle ref struct"
            var idList = currencyIsos.ToList();
            return await FindAllAsync(x => idList.Contains(x.CurrencyIso), cancellationToken);
        }
    }
}