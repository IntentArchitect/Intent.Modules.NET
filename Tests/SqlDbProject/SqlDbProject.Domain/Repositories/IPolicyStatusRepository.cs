using Intent.RoslynWeaver.Attributes;
using SqlDbProject.Domain.Entities;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace SqlDbProject.Domain.Repositories
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface IPolicyStatusRepository : IEFRepository<PolicyStatus, PolicyStatus>
    {
        [IntentManaged(Mode.Fully)]
        Task<TProjection?> FindByIdProjectToAsync<TProjection>(Guid policyStatusId, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<PolicyStatus?> FindByIdAsync(Guid policyStatusId, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<PolicyStatus>> FindByIdsAsync(Guid[] policyStatusIds, CancellationToken cancellationToken = default);
    }
}