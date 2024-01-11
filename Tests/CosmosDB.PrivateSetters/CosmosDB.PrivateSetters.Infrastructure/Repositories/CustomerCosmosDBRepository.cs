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
    internal class CustomerCosmosDBRepository : CosmosDBRepositoryBase<Customer, CustomerDocument, ICustomerDocument>, ICustomerRepository
    {
        public CustomerCosmosDBRepository(CosmosDBUnitOfWork unitOfWork,
            Microsoft.Azure.CosmosRepository.IRepository<CustomerDocument> cosmosRepository,
            ICurrentUserService currentUserService) : base(unitOfWork, cosmosRepository, "id", currentUserService)
        {
        }

        public async Task<Customer?> FindByIdAsync(string id, CancellationToken cancellationToken = default) => await base.FindByIdAsync(id: id, cancellationToken: cancellationToken);
    }
}