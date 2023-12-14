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

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBRepository", Version = "1.0")]

namespace CosmosDB.PrivateSetters.Infrastructure.Repositories
{
    internal class OrderCosmosDBRepository : CosmosDBRepositoryBase<Order, OrderDocument, IOrderDocument>, IOrderRepository
    {
        public OrderCosmosDBRepository(CosmosDBUnitOfWork unitOfWork,
            Microsoft.Azure.CosmosRepository.IRepository<OrderDocument> cosmosRepository,
            ICurrentUserService currentUserService) : base(unitOfWork, cosmosRepository, "id", currentUserService)
        {
        }

        public async Task<Order?> FindByIdAsync(
            (string Id, string WarehouseId) id,
            CancellationToken cancellationToken = default) => await base.FindByIdAsync(id: id.Id, partitionKey: id.WarehouseId, cancellationToken: cancellationToken);
    }
}