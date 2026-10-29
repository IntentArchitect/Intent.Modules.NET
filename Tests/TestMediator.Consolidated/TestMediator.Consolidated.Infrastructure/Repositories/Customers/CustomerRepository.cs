using Intent.RoslynWeaver.Attributes;
using TestMediator.Consolidated.Domain.Customers;
using TestMediator.Consolidated.Domain.Repositories;
using TestMediator.Consolidated.Domain.Repositories.Customers;
using TestMediator.Consolidated.Infrastructure.Persistence;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.Repositories.Repository", Version = "1.0")]

namespace TestMediator.Consolidated.Infrastructure.Repositories.Customers
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CustomerRepository : RepositoryBase<Customer, Customer, ApplicationDbContext>, ICustomerRepository
    {
        public CustomerRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<Customer?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await FindAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<Customer?> FindByIdAsync(
            Guid id,
            Func<IQueryable<Customer>, IQueryable<Customer>> queryOptions,
            CancellationToken cancellationToken = default)
        {
            return await FindAsync(x => x.Id == id, queryOptions, cancellationToken);
        }

        public async Task<List<Customer>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default)
        {
            // Force materialization - Some combinations of .net9 runtime and EF runtime crash with "Convert ReadOnlySpan to List since expression trees can't handle ref struct"
            var idList = ids.ToList();
            return await FindAllAsync(x => idList.Contains(x.Id), cancellationToken);
        }
    }
}