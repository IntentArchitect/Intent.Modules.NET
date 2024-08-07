using System.Threading;
using System.Threading.Tasks;
using BasicAuditing.CustomUserId.Tests.Application.Common.Interfaces;
using BasicAuditing.CustomUserId.Tests.Domain.Entities;
using BasicAuditing.CustomUserId.Tests.Domain.Repositories;
using BasicAuditing.CustomUserId.Tests.Domain.Repositories.Documents;
using BasicAuditing.CustomUserId.Tests.Infrastructure.Persistence;
using BasicAuditing.CustomUserId.Tests.Infrastructure.Persistence.Documents;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Azure.CosmosRepository;
using Microsoft.Azure.CosmosRepository.Options;
using Microsoft.Azure.CosmosRepository.Providers;
using Microsoft.Extensions.Options;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBRepository", Version = "1.0")]

namespace BasicAuditing.CustomUserId.Tests.Infrastructure.Repositories
{
    internal class AccountCosmosDBRepository : CosmosDBRepositoryBase<Account, AccountDocument, IAccountDocument>, IAccountRepository
    {
        public AccountCosmosDBRepository(CosmosDBUnitOfWork unitOfWork,
            Microsoft.Azure.CosmosRepository.IRepository<AccountDocument> cosmosRepository,
            ICosmosContainerProvider<AccountDocument> containerProvider,
            IOptionsMonitor<RepositoryOptions> optionsMonitor,
            ICurrentUserService currentUserService) : base(unitOfWork, cosmosRepository, "id", containerProvider, optionsMonitor, currentUserService)
        {
        }

        public async Task<Account?> FindByIdAsync(string id, CancellationToken cancellationToken = default) => await base.FindByIdAsync(id: id, cancellationToken: cancellationToken);
    }
}