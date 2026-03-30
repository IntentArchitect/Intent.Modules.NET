using FluentValidationTest.Domain.Entities.ValidationScenarios.IdentityConstraints;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace FluentValidationTest.Domain.Repositories.ValidationScenarios.IdentityConstraints
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface IUniqueAccountEntityRepository : IEFRepository<UniqueAccountEntity, UniqueAccountEntity>
    {
        [IntentManaged(Mode.Fully)]
        Task<TProjection?> FindByIdProjectToAsync<TProjection>(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<UniqueAccountEntity?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<UniqueAccountEntity?> FindByIdAsync(Guid id, Func<IQueryable<UniqueAccountEntity>, IQueryable<UniqueAccountEntity>> queryOptions, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<UniqueAccountEntity>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    }
}