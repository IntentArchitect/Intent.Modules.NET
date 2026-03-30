using FluentValidationTest.Domain.Entities.ValidationScenarios.IdentityConstraints;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace FluentValidationTest.Domain.Repositories.ValidationScenarios.IdentityConstraints
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface IUniquePersonEntityRepository : IEFRepository<UniquePersonEntity, UniquePersonEntity>
    {
        [IntentManaged(Mode.Fully)]
        Task<TProjection?> FindByIdProjectToAsync<TProjection>(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<UniquePersonEntity?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<UniquePersonEntity?> FindByIdAsync(Guid id, Func<IQueryable<UniquePersonEntity>, IQueryable<UniquePersonEntity>> queryOptions, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<UniquePersonEntity>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    }
}