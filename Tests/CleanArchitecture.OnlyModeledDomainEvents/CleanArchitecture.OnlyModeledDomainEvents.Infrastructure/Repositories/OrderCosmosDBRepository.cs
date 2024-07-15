using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.OnlyModeledDomainEvents.Domain.Entities;
using CleanArchitecture.OnlyModeledDomainEvents.Domain.Repositories;
using CleanArchitecture.OnlyModeledDomainEvents.Domain.Repositories.Documents;
using CleanArchitecture.OnlyModeledDomainEvents.Infrastructure.Persistence;
using CleanArchitecture.OnlyModeledDomainEvents.Infrastructure.Persistence.Documents;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Azure.CosmosRepository;
using Microsoft.Azure.CosmosRepository.Options;
using Microsoft.Azure.CosmosRepository.Providers;
using Microsoft.Extensions.Options;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBRepository", Version = "1.0")]

namespace CleanArchitecture.OnlyModeledDomainEvents.Infrastructure.Repositories
{
    internal class OrderCosmosDBRepository : CosmosDBRepositoryBase<Order, OrderDocument, IOrderDocument>, IOrderRepository
    {
        public OrderCosmosDBRepository(CosmosDBUnitOfWork unitOfWork,
            Microsoft.Azure.CosmosRepository.IRepository<OrderDocument> cosmosRepository,
            ICosmosContainerProvider<OrderDocument> containerProvider,
            IOptionsMonitor<RepositoryOptions> optionsMonitor) : base(unitOfWork, cosmosRepository, "id", containerProvider, optionsMonitor)
        {
        }

        public async Task<Order?> FindByIdAsync(string id, CancellationToken cancellationToken = default) => await base.FindByIdAsync(id: id, cancellationToken: cancellationToken);
    }
}