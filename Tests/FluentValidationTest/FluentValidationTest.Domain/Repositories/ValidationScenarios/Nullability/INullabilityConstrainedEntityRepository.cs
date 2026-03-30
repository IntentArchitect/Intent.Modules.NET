using FluentValidationTest.Domain.Entities.ValidationScenarios.Nullability;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace FluentValidationTest.Domain.Repositories.ValidationScenarios.Nullability
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface INullabilityConstrainedEntityRepository : IEFRepository<NullabilityConstrainedEntity, NullabilityConstrainedEntity>
    {
        [IntentManaged(Mode.Fully)]
        Task<TProjection?> FindByIdProjectToAsync<TProjection>(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<NullabilityConstrainedEntity?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<NullabilityConstrainedEntity?> FindByIdAsync(Guid id, Func<IQueryable<NullabilityConstrainedEntity>, IQueryable<NullabilityConstrainedEntity>> queryOptions, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<NullabilityConstrainedEntity>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    }
}