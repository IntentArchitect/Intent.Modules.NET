using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using JsonPatchRfc7396.Scalar.Domain.Entities.Configuration;
using JsonPatchRfc7396.Scalar.Domain.Repositories;
using JsonPatchRfc7396.Scalar.Domain.Repositories.Configuration;
using JsonPatchRfc7396.Scalar.Infrastructure.Persistence;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.Repositories.Repository", Version = "1.0")]

namespace JsonPatchRfc7396.Scalar.Infrastructure.Repositories.Configuration
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class ConfigurationStoreRepository : RepositoryBase<ConfigurationStore, ConfigurationStore, ApplicationDbContext>, IConfigurationStoreRepository
    {
        public ConfigurationStoreRepository(ApplicationDbContext dbContext, IMapper mapper) : base(dbContext, mapper)
        {
        }

        public async Task<TProjection?> FindByIdProjectToAsync<TProjection>(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            return await FindProjectToAsync<TProjection>(x => x.Id == id, cancellationToken);
        }

        public async Task<ConfigurationStore?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await FindAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<ConfigurationStore?> FindByIdAsync(
            Guid id,
            Func<IQueryable<ConfigurationStore>, IQueryable<ConfigurationStore>> queryOptions,
            CancellationToken cancellationToken = default)
        {
            return await FindAsync(x => x.Id == id, queryOptions, cancellationToken);
        }

        public async Task<List<ConfigurationStore>> FindByIdsAsync(
            Guid[] ids,
            CancellationToken cancellationToken = default)
        {
            // Force materialization - Some combinations of .net9 runtime and EF runtime crash with "Convert ReadOnlySpan to List since expression trees can't handle ref struct"
            var idList = ids.ToList();
            return await FindAllAsync(x => idList.Contains(x.Id), cancellationToken);
        }
    }
}