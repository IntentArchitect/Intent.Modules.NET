using Intent.RoslynWeaver.Attributes;
using ObjectMappingTest.Domain.Entities;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace ObjectMappingTest.Domain.Repositories
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface IDigitalProductRepository : IEFRepository<DigitalProduct, DigitalProduct>
    {
        [IntentManaged(Mode.Fully)]
        Task<DigitalProduct?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<DigitalProduct?> FindByIdAsync(Guid id, Func<IQueryable<DigitalProduct>, IQueryable<DigitalProduct>> queryOptions, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<DigitalProduct>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    }
}