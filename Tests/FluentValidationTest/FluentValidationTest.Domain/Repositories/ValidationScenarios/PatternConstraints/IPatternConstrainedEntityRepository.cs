using FluentValidationTest.Domain.Entities.ValidationScenarios.PatternConstraints;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace FluentValidationTest.Domain.Repositories.ValidationScenarios.PatternConstraints
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface IPatternConstrainedEntityRepository : IEFRepository<PatternConstrainedEntity, PatternConstrainedEntity>
    {
        [IntentManaged(Mode.Fully)]
        Task<TProjection?> FindByIdProjectToAsync<TProjection>(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<PatternConstrainedEntity?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<PatternConstrainedEntity?> FindByIdAsync(Guid id, Func<IQueryable<PatternConstrainedEntity>, IQueryable<PatternConstrainedEntity>> queryOptions, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<PatternConstrainedEntity>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    }
}