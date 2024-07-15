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
using Microsoft.Azure.CosmosRepository.Options;
using Microsoft.Azure.CosmosRepository.Providers;
using Microsoft.Extensions.Options;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBRepository", Version = "1.0")]

namespace CosmosDB.EntityInterfaces.Infrastructure.Repositories
{
    internal class OrderCosmosDBRepository : CosmosDBRepositoryBase<IOrder, Order, OrderDocument, IOrderDocument>, IOrderRepository
    {
        public OrderCosmosDBRepository(CosmosDBUnitOfWork unitOfWork,
            Microsoft.Azure.CosmosRepository.IRepository<OrderDocument> cosmosRepository,
            ICosmosContainerProvider<OrderDocument> containerProvider,
            IOptionsMonitor<RepositoryOptions> optionsMonitor,
            ICurrentUserService currentUserService) : base(unitOfWork, cosmosRepository, "id", containerProvider, optionsMonitor, currentUserService)
        {
        }

        public async Task<IOrder?> FindByIdAsync(
            (string Id, string WarehouseId) id,
            CancellationToken cancellationToken = default) => await FindByIdAsync(id: id.Id, partitionKey: id.WarehouseId, cancellationToken: cancellationToken);
    }
}