using Intent.RoslynWeaver.Attributes;
using JsonPatchRfc7396.Scalar.Domain.Entities.Configuration;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace JsonPatchRfc7396.Scalar.Domain.Repositories.Configuration
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface IConfigurationStoreRepository : IEFRepository<ConfigurationStore, ConfigurationStore>
    {
        [IntentManaged(Mode.Fully)]
        Task<TProjection?> FindByIdProjectToAsync<TProjection>(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<ConfigurationStore?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<ConfigurationStore?> FindByIdAsync(Guid id, Func<IQueryable<ConfigurationStore>, IQueryable<ConfigurationStore>> queryOptions, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<ConfigurationStore>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    }
}