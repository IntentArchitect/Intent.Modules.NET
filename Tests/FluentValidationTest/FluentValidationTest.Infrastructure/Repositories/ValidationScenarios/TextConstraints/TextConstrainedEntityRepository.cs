using AutoMapper;
using FluentValidationTest.Domain.Entities.ValidationScenarios.TextConstraints;
using FluentValidationTest.Domain.Repositories;
using FluentValidationTest.Domain.Repositories.ValidationScenarios.TextConstraints;
using FluentValidationTest.Infrastructure.Persistence;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.Repositories.Repository", Version = "1.0")]

namespace FluentValidationTest.Infrastructure.Repositories.ValidationScenarios.TextConstraints
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class TextConstrainedEntityRepository : RepositoryBase<TextConstrainedEntity, TextConstrainedEntity, ApplicationDbContext>, ITextConstrainedEntityRepository
    {
        public TextConstrainedEntityRepository(ApplicationDbContext dbContext, IMapper mapper) : base(dbContext, mapper)
        {
        }

        public async Task<TProjection?> FindByIdProjectToAsync<TProjection>(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            return await FindProjectToAsync<TProjection>(x => x.Id == id, cancellationToken);
        }

        public async Task<TextConstrainedEntity?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await FindAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<TextConstrainedEntity?> FindByIdAsync(
            Guid id,
            Func<IQueryable<TextConstrainedEntity>, IQueryable<TextConstrainedEntity>> queryOptions,
            CancellationToken cancellationToken = default)
        {
            return await FindAsync(x => x.Id == id, queryOptions, cancellationToken);
        }

        public async Task<List<TextConstrainedEntity>> FindByIdsAsync(
            Guid[] ids,
            CancellationToken cancellationToken = default)
        {
            // Force materialization - Some combinations of .net9 runtime and EF runtime crash with "Convert ReadOnlySpan to List since expression trees can't handle ref struct"
            var idList = ids.ToList();
            return await FindAllAsync(x => idList.Contains(x.Id), cancellationToken);
        }
    }
}