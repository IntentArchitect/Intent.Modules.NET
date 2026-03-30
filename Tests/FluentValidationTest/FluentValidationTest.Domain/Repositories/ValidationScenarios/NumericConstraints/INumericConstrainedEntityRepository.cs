using FluentValidationTest.Domain.Entities.ValidationScenarios.NumericConstraints;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace FluentValidationTest.Domain.Repositories.ValidationScenarios.NumericConstraints
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface INumericConstrainedEntityRepository : IEFRepository<NumericConstrainedEntity, NumericConstrainedEntity>
    {
        [IntentManaged(Mode.Fully)]
        Task<TProjection?> FindByIdProjectToAsync<TProjection>(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<NumericConstrainedEntity?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<NumericConstrainedEntity?> FindByIdAsync(Guid id, Func<IQueryable<NumericConstrainedEntity>, IQueryable<NumericConstrainedEntity>> queryOptions, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<NumericConstrainedEntity>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    }
}