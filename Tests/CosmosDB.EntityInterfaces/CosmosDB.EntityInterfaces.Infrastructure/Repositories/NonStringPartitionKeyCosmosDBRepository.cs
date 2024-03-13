using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using CosmosDB.EntityInterfaces.Application.Common.Interfaces;
using CosmosDB.EntityInterfaces.Domain.Entities;
using CosmosDB.EntityInterfaces.Domain.Repositories;
using CosmosDB.EntityInterfaces.Domain.Repositories.Documents;
using CosmosDB.EntityInterfaces.Infrastructure.Persistence;
using CosmosDB.EntityInterfaces.Infrastructure.Persistence.Documents;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Azure.CosmosRepository;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBRepository", Version = "1.0")]

namespace CosmosDB.EntityInterfaces.Infrastructure.Repositories
{
    internal class NonStringPartitionKeyCosmosDBRepository : CosmosDBRepositoryBase<INonStringPartitionKey, NonStringPartitionKey, NonStringPartitionKeyDocument, INonStringPartitionKeyDocument>, INonStringPartitionKeyRepository
    {
        public NonStringPartitionKeyCosmosDBRepository(CosmosDBUnitOfWork unitOfWork,
            Microsoft.Azure.CosmosRepository.IRepository<NonStringPartitionKeyDocument> cosmosRepository,
            ICurrentUserService currentUserService) : base(unitOfWork, cosmosRepository, "id", currentUserService)
        {
        }

        public async Task<INonStringPartitionKey?> FindByIdAsync(
            (string Id, int PartInt) id,
            CancellationToken cancellationToken = default) => await FindByIdAsync(id: id.Id, partitionKey: id.PartInt.ToString(CultureInfo.InvariantCulture), cancellationToken: cancellationToken);
    }
}