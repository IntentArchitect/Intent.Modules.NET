using FluentValidationTest.Domain.Entities.ValidationScenarios.TextConstraints;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace FluentValidationTest.Domain.Repositories.ValidationScenarios.TextConstraints
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface ITextConstrainedEntityRepository : IEFRepository<TextConstrainedEntity, TextConstrainedEntity>
    {
        [IntentManaged(Mode.Fully)]
        Task<TProjection?> FindByIdProjectToAsync<TProjection>(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<TextConstrainedEntity?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<TextConstrainedEntity?> FindByIdAsync(Guid id, Func<IQueryable<TextConstrainedEntity>, IQueryable<TextConstrainedEntity>> queryOptions, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<TextConstrainedEntity>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    }
}