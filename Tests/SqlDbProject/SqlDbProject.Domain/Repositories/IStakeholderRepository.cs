using Intent.RoslynWeaver.Attributes;
using SqlDbProject.Domain.Entities;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace SqlDbProject.Domain.Repositories
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface IStakeholderRepository : IEFRepository<Stakeholder, Stakeholder>
    {
        [IntentManaged(Mode.Fully)]
        Task<TProjection?> FindByIdProjectToAsync<TProjection>(long stakeholderId, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<Stakeholder?> FindByIdAsync(long stakeholderId, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<Stakeholder>> FindByIdsAsync(long[] stakeholderIds, CancellationToken cancellationToken = default);
    }
}