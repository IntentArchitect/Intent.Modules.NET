using Intent.RoslynWeaver.Attributes;
using ObjectMapping.Lenient.Domain.Entities;
using ObjectMapping.Lenient.Domain.Repositories;
using ObjectMapping.Lenient.Infrastructure.Persistence;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.Repositories.Repository", Version = "1.0")]

namespace ObjectMapping.Lenient.Infrastructure.Repositories
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CardPaymentRepository : RepositoryBase<CardPayment, CardPayment, ApplicationDbContext>, ICardPaymentRepository
    {
        public CardPaymentRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<CardPayment?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await FindAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<CardPayment?> FindByIdAsync(
            Guid id,
            Func<IQueryable<CardPayment>, IQueryable<CardPayment>> queryOptions,
            CancellationToken cancellationToken = default)
        {
            return await FindAsync(x => x.Id == id, queryOptions, cancellationToken);
        }

        public async Task<List<CardPayment>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default)
        {
            // Force materialization - Some combinations of .net9 runtime and EF runtime crash with "Convert ReadOnlySpan to List since expression trees can't handle ref struct"
            var idList = ids.ToList();
            return await FindAllAsync(x => idList.Contains(x.Id), cancellationToken);
        }
    }
}