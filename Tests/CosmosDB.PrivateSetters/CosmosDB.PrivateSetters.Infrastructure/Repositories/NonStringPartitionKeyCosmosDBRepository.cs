using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using CosmosDB.PrivateSetters.Application.Common.Interfaces;
using CosmosDB.PrivateSetters.Domain.Entities;
using CosmosDB.PrivateSetters.Domain.Repositories;
using CosmosDB.PrivateSetters.Domain.Repositories.Documents;
using CosmosDB.PrivateSetters.Infrastructure.Persistence;
using CosmosDB.PrivateSetters.Infrastructure.Persistence.Documents;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Azure.CosmosRepository;
using Microsoft.Azure.CosmosRepository.Options;
using Microsoft.Azure.CosmosRepository.Providers;
using Microsoft.Extensions.Options;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBRepository", Version = "1.0")]

namespace CosmosDB.PrivateSetters.Infrastructure.Repositories
{
    internal class NonStringPartitionKeyCosmosDBRepository : CosmosDBRepositoryBase<NonStringPartitionKey, NonStringPartitionKeyDocument, INonStringPartitionKeyDocument>, INonStringPartitionKeyRepository
    {
        public NonStringPartitionKeyCosmosDBRepository(CosmosDBUnitOfWork unitOfWork,
            Microsoft.Azure.CosmosRepository.IRepository<NonStringPartitionKeyDocument> cosmosRepository,
            ICosmosContainerProvider<NonStringPartitionKeyDocument> containerProvider,
            IOptionsMonitor<RepositoryOptions> optionsMonitor,
            ICurrentUserService currentUserService) : base(unitOfWork, cosmosRepository, "id", containerProvider, optionsMonitor, currentUserService)
        {
        }

        public async Task<NonStringPartitionKey?> FindByIdAsync(
            (string Id, int PartInt) id,
            CancellationToken cancellationToken = default) => await FindByIdAsync(id: id.Id, partitionKey: id.PartInt.ToString(CultureInfo.InvariantCulture), cancellationToken: cancellationToken);
    }
}