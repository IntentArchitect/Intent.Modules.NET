using Intent.RoslynWeaver.Attributes;
using TestMediator.Consolidated.Domain.Customers;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace TestMediator.Consolidated.Domain.Repositories.Customers
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface ICustomerRepository : IEFRepository<Customer, Customer>
    {
        [IntentManaged(Mode.Fully)]
        Task<Customer?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<Customer?> FindByIdAsync(Guid id, Func<IQueryable<Customer>, IQueryable<Customer>> queryOptions, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<Customer>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    }
}