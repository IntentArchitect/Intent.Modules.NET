using System.Threading;
using System.Threading.Tasks;
using CosmosDB.MultiTenancy.SeperateDB.Domain.Entities;
using CosmosDB.MultiTenancy.SeperateDB.Domain.Repositories;
using CosmosDB.MultiTenancy.SeperateDB.Domain.Repositories.Documents;
using CosmosDB.MultiTenancy.SeperateDB.Infrastructure.Persistence;
using CosmosDB.MultiTenancy.SeperateDB.Infrastructure.Persistence.Documents;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Azure.CosmosRepository;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBRepository", Version = "1.0")]

namespace CosmosDB.MultiTenancy.SeperateDB.Infrastructure.Repositories
{
    internal class CustomerCosmosDBRepository : CosmosDBRepositoryBase<Customer, CustomerDocument, ICustomerDocument>, ICustomerRepository
    {
        public CustomerCosmosDBRepository(CosmosDBUnitOfWork unitOfWork,
            Microsoft.Azure.CosmosRepository.IRepository<CustomerDocument> cosmosRepository) : base(unitOfWork, cosmosRepository, "id")
        {
        }

        public async Task<Customer?> FindByIdAsync(string id, CancellationToken cancellationToken = default) => await base.FindByIdAsync(id: id, cancellationToken: cancellationToken);
    }
}