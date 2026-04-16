using FluentValidationTest.Domain.Entities.ValidationScenarios.StressSuite;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace FluentValidationTest.Domain.Repositories.ValidationScenarios.StressSuite
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface IUserAccountRepository : IEFRepository<UserAccount, UserAccount>
    {
        [IntentManaged(Mode.Fully)]
        Task<TProjection?> FindByIdProjectToAsync<TProjection>(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<UserAccount?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<UserAccount?> FindByIdAsync(Guid id, Func<IQueryable<UserAccount>, IQueryable<UserAccount>> queryOptions, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<UserAccount>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    }
}