using FluentValidationTest.Domain.Entities.ValidationScenarios.ConstructorOperationConstraints;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace FluentValidationTest.Domain.Repositories.ValidationScenarios.ConstructorOperationConstraints
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface IConstructedConstrainedEntityRepository : IEFRepository<ConstructedConstrainedEntity, ConstructedConstrainedEntity>
    {
        [IntentManaged(Mode.Fully)]
        Task<TProjection?> FindByIdProjectToAsync<TProjection>(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<ConstructedConstrainedEntity?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<ConstructedConstrainedEntity?> FindByIdAsync(Guid id, Func<IQueryable<ConstructedConstrainedEntity>, IQueryable<ConstructedConstrainedEntity>> queryOptions, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<ConstructedConstrainedEntity>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    }
}