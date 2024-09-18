using System.Threading;
using System.Threading.Tasks;
using CosmosDB.EnumStrings.Domain.Entities;
using CosmosDB.EnumStrings.Domain.Repositories;
using CosmosDB.EnumStrings.Domain.Repositories.Documents;
using CosmosDB.EnumStrings.Infrastructure.Persistence;
using CosmosDB.EnumStrings.Infrastructure.Persistence.Documents;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Azure.CosmosRepository;
using Microsoft.Azure.CosmosRepository.Options;
using Microsoft.Azure.CosmosRepository.Providers;
using Microsoft.Extensions.Options;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBRepository", Version = "1.0")]

namespace CosmosDB.EnumStrings.Infrastructure.Repositories
{
    internal class RootEntityCosmosDBRepository : CosmosDBRepositoryBase<RootEntity, RootEntityDocument, IRootEntityDocument>, IRootEntityRepository
    {
        public RootEntityCosmosDBRepository(CosmosDBUnitOfWork unitOfWork,
            Microsoft.Azure.CosmosRepository.IRepository<RootEntityDocument> cosmosRepository,
            ICosmosContainerProvider<RootEntityDocument> containerProvider,
            IOptionsMonitor<RepositoryOptions> optionsMonitor) : base(unitOfWork, cosmosRepository, "id", containerProvider, optionsMonitor)
        {
        }

        public async Task<RootEntity?> FindByIdAsync(string id, CancellationToken cancellationToken = default) => await base.FindByIdAsync(id: id, cancellationToken: cancellationToken);
    }
}