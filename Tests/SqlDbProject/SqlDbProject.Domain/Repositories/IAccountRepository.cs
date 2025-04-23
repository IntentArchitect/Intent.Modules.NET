using Intent.RoslynWeaver.Attributes;
using SqlDbProject.Domain.Contracts;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.CustomRepositoryInterface", Version = "1.0")]

namespace SqlDbProject.Domain.Repositories
{
    public interface IAccountRepository
    {
        Task<AccountHolderPerson> GetAccountHolderPerson(long stakeholderId, CancellationToken cancellationToken = default);
    }
}