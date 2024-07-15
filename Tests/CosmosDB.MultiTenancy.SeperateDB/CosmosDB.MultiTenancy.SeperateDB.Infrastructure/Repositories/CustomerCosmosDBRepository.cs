using System.Threading;
using System.Threading.Tasks;
using CosmosDB.MultiTenancy.SeperateDB.Domain.Entities;
using CosmosDB.MultiTenancy.SeperateDB.Domain.Repositories;
using CosmosDB.MultiTenancy.SeperateDB.Domain.Repositories.Documents;
using CosmosDB.MultiTenancy.SeperateDB.Infrastructure.Persistence;
using CosmosDB.MultiTenancy.SeperateDB.Infrastructure.Persistence.Documents;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Azure.CosmosRepository;
using Microsoft.Azure.CosmosRepository.Options;
using Microsoft.Azure.CosmosRepository.Providers;
using Microsoft.Extensions.Options;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBRepository", Version = "1.0")]

namespace CosmosDB.MultiTenancy.SeperateDB.Infrastructure.Repositories
{
    internal class CustomerCosmosDBRepository : CosmosDBRepositoryBase<Customer, CustomerDocument, ICustomerDocument>, ICustomerRepository
    {
        public CustomerCosmosDBRepository(CosmosDBUnitOfWork unitOfWork,
            Microsoft.Azure.CosmosRepository.IRepository<CustomerDocument> cosmosRepository,
            ICosmosContainerProvider<CustomerDocument> containerProvider,
            IOptionsMonitor<RepositoryOptions> optionsMonitor) : base(unitOfWork, cosmosRepository, "id", containerProvider, optionsMonitor)
        {
        }

        public async Task<Customer?> FindByIdAsync(string id, CancellationToken cancellationToken = default) => await base.FindByIdAsync(id: id, cancellationToken: cancellationToken);
    }
}