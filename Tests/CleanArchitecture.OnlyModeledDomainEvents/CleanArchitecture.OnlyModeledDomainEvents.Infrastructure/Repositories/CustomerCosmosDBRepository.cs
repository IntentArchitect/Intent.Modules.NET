using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.OnlyModeledDomainEvents.Domain.Entities;
using CleanArchitecture.OnlyModeledDomainEvents.Domain.Repositories;
using CleanArchitecture.OnlyModeledDomainEvents.Domain.Repositories.Documents;
using CleanArchitecture.OnlyModeledDomainEvents.Infrastructure.Persistence;
using CleanArchitecture.OnlyModeledDomainEvents.Infrastructure.Persistence.Documents;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Azure.CosmosRepository;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBRepository", Version = "1.0")]

namespace CleanArchitecture.OnlyModeledDomainEvents.Infrastructure.Repositories
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