using AdvancedMapping.Repositories.Mapperly.Tests.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace AdvancedMapping.Repositories.Mapperly.Tests.Domain.Repositories
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface ICustomerSegmentsRepository : IEFRepository<CustomerSegments, CustomerSegments>
    {
        [IntentManaged(Mode.Fully)]
        Task<CustomerSegments?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<CustomerSegments?> FindByIdAsync(Guid id, Func<IQueryable<CustomerSegments>, IQueryable<CustomerSegments>> queryOptions, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<CustomerSegments>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    }
}