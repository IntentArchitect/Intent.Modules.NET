using System.Threading;
using System.Threading.Tasks;
using CosmosDB.Application.Common.Interfaces;
using CosmosDB.Domain.Entities.Throughput;
using CosmosDB.Domain.Repositories.Documents.Throughput;
using CosmosDB.Domain.Repositories.Throughput;
using CosmosDB.Infrastructure.Persistence;
using CosmosDB.Infrastructure.Persistence.Documents.Throughput;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Azure.CosmosRepository;
using Microsoft.Azure.CosmosRepository.Options;
using Microsoft.Azure.CosmosRepository.Providers;
using Microsoft.Extensions.Options;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBRepository", Version = "1.0")]

namespace CosmosDB.Infrastructure.Repositories.Throughput
{
    internal class AutoscaleWithMaxValueCosmosDBRepository : CosmosDBRepositoryBase<AutoscaleWithMaxValue, AutoscaleWithMaxValueDocument, IAutoscaleWithMaxValueDocument>, IAutoscaleWithMaxValueRepository
    {
        public AutoscaleWithMaxValueCosmosDBRepository(CosmosDBUnitOfWork unitOfWork,
            IRepository<AutoscaleWithMaxValueDocument> cosmosRepository,
            ICosmosContainerProvider<AutoscaleWithMaxValueDocument> containerProvider,
            IOptionsMonitor<RepositoryOptions> optionsMonitor,
            ICurrentUserService currentUserService) : base(unitOfWork, cosmosRepository, "id", containerProvider, optionsMonitor, currentUserService)
        {
        }

        public async Task<AutoscaleWithMaxValue?> FindByIdAsync(string id, CancellationToken cancellationToken = default) => await base.FindByIdAsync(id: id, cancellationToken: cancellationToken);
    }
}