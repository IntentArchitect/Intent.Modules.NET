using EntityFrameworkCore.SqlServer.EF11.Domain.ValueObjects;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace EntityFrameworkCore.SqlServer.EF11.Domain.Repositories.ValueObjects
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface IPersonWithAddressSerializedRepository : IEFRepository<PersonWithAddressSerialized, PersonWithAddressSerialized>
    {
        [IntentManaged(Mode.Fully)]
        Task<PersonWithAddressSerialized?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<PersonWithAddressSerialized?> FindByIdAsync(Guid id, Func<IQueryable<PersonWithAddressSerialized>, IQueryable<PersonWithAddressSerialized>> queryOptions, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<PersonWithAddressSerialized>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    }
}