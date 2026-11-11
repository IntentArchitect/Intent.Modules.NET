using EntityFrameworkCore.SqlServer.EF11.Domain.ValueObjects;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace EntityFrameworkCore.SqlServer.EF11.Domain.Repositories.ValueObjects
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface IDictionaryWithKvPSerializedRepository : IEFRepository<DictionaryWithKvPSerialized, DictionaryWithKvPSerialized>
    {
        [IntentManaged(Mode.Fully)]
        Task<DictionaryWithKvPSerialized?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<DictionaryWithKvPSerialized?> FindByIdAsync(Guid id, Func<IQueryable<DictionaryWithKvPSerialized>, IQueryable<DictionaryWithKvPSerialized>> queryOptions, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<DictionaryWithKvPSerialized>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    }
}