using AdvancedMapping.Repositories.Mapperly.Tests.Domain.Entities.Sales;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace AdvancedMapping.Repositories.Mapperly.Tests.Domain.Repositories.Sales
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface IDiscountRepository : IEFRepository<Discount, Discount>
    {
        [IntentManaged(Mode.Fully)]
        Task<Discount?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<Discount?> FindByIdAsync(Guid id, Func<IQueryable<Discount>, IQueryable<Discount>> queryOptions, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<Discount>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    }
}