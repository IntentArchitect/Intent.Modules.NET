using AutoMapper;
using FluentValidationTest.Domain.Entities.ValidationScenarios.StressSuite;
using FluentValidationTest.Domain.Repositories;
using FluentValidationTest.Domain.Repositories.ValidationScenarios.StressSuite;
using FluentValidationTest.Infrastructure.Persistence;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.Repositories.Repository", Version = "1.0")]

namespace FluentValidationTest.Infrastructure.Repositories.ValidationScenarios.StressSuite
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class PersistencePrecedenceRepository : RepositoryBase<PersistencePrecedence, PersistencePrecedence, ApplicationDbContext>, IPersistencePrecedenceRepository
    {
        public PersistencePrecedenceRepository(ApplicationDbContext dbContext, IMapper mapper) : base(dbContext, mapper)
        {
        }

        public async Task<TProjection?> FindByIdProjectToAsync<TProjection>(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            return await FindProjectToAsync<TProjection>(x => x.Id == id, cancellationToken);
        }

        public async Task<PersistencePrecedence?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await FindAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<PersistencePrecedence?> FindByIdAsync(
            Guid id,
            Func<IQueryable<PersistencePrecedence>, IQueryable<PersistencePrecedence>> queryOptions,
            CancellationToken cancellationToken = default)
        {
            return await FindAsync(x => x.Id == id, queryOptions, cancellationToken);
        }

        public async Task<List<PersistencePrecedence>> FindByIdsAsync(
            Guid[] ids,
            CancellationToken cancellationToken = default)
        {
            // Force materialization - Some combinations of .net9 runtime and EF runtime crash with "Convert ReadOnlySpan to List since expression trees can't handle ref struct"
            var idList = ids.ToList();
            return await FindAllAsync(x => idList.Contains(x.Id), cancellationToken);
        }
    }
}