using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.Cosmos.Tests.Domain.Entities;
using AdvancedMappingCrud.Cosmos.Tests.Domain.Repositories;
using AdvancedMappingCrud.Cosmos.Tests.Domain.Repositories.Documents;
using AdvancedMappingCrud.Cosmos.Tests.Infrastructure.Persistence;
using AdvancedMappingCrud.Cosmos.Tests.Infrastructure.Persistence.Documents;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Azure.CosmosRepository;
using Microsoft.Azure.CosmosRepository.Options;
using Microsoft.Azure.CosmosRepository.Providers;
using Microsoft.Extensions.Options;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBRepository", Version = "1.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.Infrastructure.Repositories
{
    internal class StockCosmosDBRepository : CosmosDBRepositoryBase<Stock, Persistence.Documents.StockDocument, IStockDocument>, IStockRepository
    {
        public StockCosmosDBRepository(CosmosDBUnitOfWork unitOfWork,
            Microsoft.Azure.CosmosRepository.IRepository<Persistence.Documents.StockDocument> cosmosRepository,
            ICosmosContainerProvider<Persistence.Documents.StockDocument> containerProvider,
            IOptionsMonitor<RepositoryOptions> optionsMonitor,
            IMapper mapper) : base(unitOfWork, cosmosRepository, "id", containerProvider, optionsMonitor, mapper)
        {
        }

        public async Task<Stock?> FindByIdAsync(string id, CancellationToken cancellationToken = default) => await base.FindByIdAsync(id: id, cancellationToken: cancellationToken);
    }
}