using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.SingleFiles.Domain.Entities;
using CleanArchitecture.SingleFiles.Domain.Repositories;
using CleanArchitecture.SingleFiles.Domain.Repositories.Documents;
using CleanArchitecture.SingleFiles.Infrastructure.Persistence;
using CleanArchitecture.SingleFiles.Infrastructure.Persistence.Documents;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Azure.CosmosRepository;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBRepository", Version = "1.0")]

namespace CleanArchitecture.SingleFiles.Infrastructure.Repositories
{
    internal class CosmosInvoiceCosmosDBRepository : CosmosDBRepositoryBase<CosmosInvoice, CosmosInvoiceDocument, ICosmosInvoiceDocument>, ICosmosInvoiceRepository
    {
        public CosmosInvoiceCosmosDBRepository(CosmosDBUnitOfWork unitOfWork,
            Microsoft.Azure.CosmosRepository.IRepository<CosmosInvoiceDocument> cosmosRepository) : base(unitOfWork, cosmosRepository, "id")
        {
        }

        public async Task<CosmosInvoice?> FindByIdAsync(string id, CancellationToken cancellationToken = default) => await base.FindByIdAsync(id: id, cancellationToken: cancellationToken);
    }
}