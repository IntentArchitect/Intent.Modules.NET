using FluentValidationTest.Domain.Entities.ValidationScenarios.StressSuite;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace FluentValidationTest.Domain.Repositories.ValidationScenarios.StressSuite
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface IPersistencePrecedenceRepository : IEFRepository<PersistencePrecedence, PersistencePrecedence>
    {
        [IntentManaged(Mode.Fully)]
        Task<TProjection?> FindByIdProjectToAsync<TProjection>(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<PersistencePrecedence?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<PersistencePrecedence?> FindByIdAsync(Guid id, Func<IQueryable<PersistencePrecedence>, IQueryable<PersistencePrecedence>> queryOptions, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<PersistencePrecedence>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    }
}