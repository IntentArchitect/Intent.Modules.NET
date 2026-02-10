using AdvancedMapping.Repositories.Mapperly.Tests.Domain.Entities.Sales;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace AdvancedMapping.Repositories.Mapperly.Tests.Domain.Repositories.Sales
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface IProductCategoryRepository : IEFRepository<ProductCategory, ProductCategory>
    {
        [IntentManaged(Mode.Fully)]
        Task<ProductCategory?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<ProductCategory?> FindByIdAsync(Guid id, Func<IQueryable<ProductCategory>, IQueryable<ProductCategory>> queryOptions, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<ProductCategory>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    }
}