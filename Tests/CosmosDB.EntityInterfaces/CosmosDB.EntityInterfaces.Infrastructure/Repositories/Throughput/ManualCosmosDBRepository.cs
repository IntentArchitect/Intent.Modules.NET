using System.Threading;
using System.Threading.Tasks;
using CosmosDB.EntityInterfaces.Application.Common.Interfaces;
using CosmosDB.EntityInterfaces.Domain.Entities.Throughput;
using CosmosDB.EntityInterfaces.Domain.Repositories.Documents.Throughput;
using CosmosDB.EntityInterfaces.Domain.Repositories.Throughput;
using CosmosDB.EntityInterfaces.Infrastructure.Persistence;
using CosmosDB.EntityInterfaces.Infrastructure.Persistence.Documents.Throughput;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Azure.CosmosRepository;
using Microsoft.Azure.CosmosRepository.Options;
using Microsoft.Azure.CosmosRepository.Providers;
using Microsoft.Extensions.Options;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBRepository", Version = "1.0")]

namespace CosmosDB.EntityInterfaces.Infrastructure.Repositories.Throughput
{
    internal class ManualCosmosDBRepository : CosmosDBRepositoryBase<IManual, Manual, ManualDocument, IManualDocument>, IManualRepository
    {
        public ManualCosmosDBRepository(CosmosDBUnitOfWork unitOfWork,
            IRepository<ManualDocument> cosmosRepository,
            ICosmosContainerProvider<ManualDocument> containerProvider,
            IOptionsMonitor<RepositoryOptions> optionsMonitor,
            ICurrentUserService currentUserService) : base(unitOfWork, cosmosRepository, "id", containerProvider, optionsMonitor, currentUserService)
        {
        }

        public async Task<IManual?> FindByIdAsync(string id, CancellationToken cancellationToken = default) => await base.FindByIdAsync(id: id, cancellationToken: cancellationToken);
    }
}