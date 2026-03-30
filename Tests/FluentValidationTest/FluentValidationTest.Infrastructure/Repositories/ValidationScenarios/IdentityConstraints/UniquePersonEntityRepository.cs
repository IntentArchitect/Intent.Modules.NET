using AutoMapper;
using FluentValidationTest.Domain.Entities.ValidationScenarios.IdentityConstraints;
using FluentValidationTest.Domain.Repositories;
using FluentValidationTest.Domain.Repositories.ValidationScenarios.IdentityConstraints;
using FluentValidationTest.Infrastructure.Persistence;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.Repositories.Repository", Version = "1.0")]

namespace FluentValidationTest.Infrastructure.Repositories.ValidationScenarios.IdentityConstraints
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class UniquePersonEntityRepository : RepositoryBase<UniquePersonEntity, UniquePersonEntity, ApplicationDbContext>, IUniquePersonEntityRepository
    {
        public UniquePersonEntityRepository(ApplicationDbContext dbContext, IMapper mapper) : base(dbContext, mapper)
        {
        }

        public async Task<TProjection?> FindByIdProjectToAsync<TProjection>(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            return await FindProjectToAsync<TProjection>(x => x.Id == id, cancellationToken);
        }

        public async Task<UniquePersonEntity?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await FindAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<UniquePersonEntity?> FindByIdAsync(
            Guid id,
            Func<IQueryable<UniquePersonEntity>, IQueryable<UniquePersonEntity>> queryOptions,
            CancellationToken cancellationToken = default)
        {
            return await FindAsync(x => x.Id == id, queryOptions, cancellationToken);
        }

        public async Task<List<UniquePersonEntity>> FindByIdsAsync(
            Guid[] ids,
            CancellationToken cancellationToken = default)
        {
            // Force materialization - Some combinations of .net9 runtime and EF runtime crash with "Convert ReadOnlySpan to List since expression trees can't handle ref struct"
            var idList = ids.ToList();
            return await FindAllAsync(x => idList.Contains(x.Id), cancellationToken);
        }
    }
}