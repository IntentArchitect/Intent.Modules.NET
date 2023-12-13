using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Azure.CosmosRepository;
using MultipleDocumentStores.Domain.Entities;
using MultipleDocumentStores.Domain.Repositories;
using MultipleDocumentStores.Domain.Repositories.Documents;
using MultipleDocumentStores.Infrastructure.Persistence;
using MultipleDocumentStores.Infrastructure.Persistence.Documents;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBRepository", Version = "1.0")]

namespace MultipleDocumentStores.Infrastructure.Repositories
{
    internal class CustomerCosmosCosmosDBRepository : CosmosDBRepositoryBase<CustomerCosmos, CustomerCosmosDocument, ICustomerCosmosDocument>, ICustomerCosmosRepository
    {
        public CustomerCosmosCosmosDBRepository(CosmosDBUnitOfWork unitOfWork,
            Microsoft.Azure.CosmosRepository.IRepository<CustomerCosmosDocument> cosmosRepository) : base(unitOfWork, cosmosRepository, "id")
        {
        }

        public async Task<CustomerCosmos?> FindByIdAsync(string id, CancellationToken cancellationToken = default) => await base.FindByIdAsync(id: id, cancellationToken: cancellationToken);
    }
}