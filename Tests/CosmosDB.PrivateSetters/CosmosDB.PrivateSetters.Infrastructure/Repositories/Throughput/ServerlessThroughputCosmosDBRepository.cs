using System.Threading;
using System.Threading.Tasks;
using CosmosDB.PrivateSetters.Application.Common.Interfaces;
using CosmosDB.PrivateSetters.Domain.Entities.Throughput;
using CosmosDB.PrivateSetters.Domain.Repositories.Documents.Throughput;
using CosmosDB.PrivateSetters.Domain.Repositories.Throughput;
using CosmosDB.PrivateSetters.Infrastructure.Persistence;
using CosmosDB.PrivateSetters.Infrastructure.Persistence.Documents.Throughput;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Azure.CosmosRepository;
using Microsoft.Azure.CosmosRepository.Options;
using Microsoft.Azure.CosmosRepository.Providers;
using Microsoft.Extensions.Options;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBRepository", Version = "1.0")]

namespace CosmosDB.PrivateSetters.Infrastructure.Repositories.Throughput
{
    internal class ServerlessThroughputCosmosDBRepository : CosmosDBRepositoryBase<ServerlessThroughput, ServerlessThroughputDocument, IServerlessThroughputDocument>, IServerlessThroughputRepository
    {
        public ServerlessThroughputCosmosDBRepository(CosmosDBUnitOfWork unitOfWork,
            IRepository<ServerlessThroughputDocument> cosmosRepository,
            ICosmosContainerProvider<ServerlessThroughputDocument> containerProvider,
            IOptionsMonitor<RepositoryOptions> optionsMonitor,
            ICurrentUserService currentUserService) : base(unitOfWork, cosmosRepository, "id", containerProvider, optionsMonitor, currentUserService)
        {
        }

        public async Task<ServerlessThroughput?> FindByIdAsync(string id, CancellationToken cancellationToken = default) => await base.FindByIdAsync(id: id, cancellationToken: cancellationToken);
    }
}