using Intent.RoslynWeaver.Attributes;
using SqlDbProject.Domain.Contracts;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.CustomRepositoryInterface", Version = "1.0")]

namespace SqlDbProject.Domain.Repositories
{
    public interface IShareholderRepository
    {
        Task<ShareholderPerson> GetStakeholderPerson(long stakeholderId, CancellationToken cancellationToken = default);
    }
}