using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using CosmosDBMultiTenancy.Application.Common.Interfaces;
using CosmosDBMultiTenancy.Domain.Entities;
using CosmosDBMultiTenancy.Domain.Repositories;
using CosmosDBMultiTenancy.Domain.Repositories.Documents;
using CosmosDBMultiTenancy.Infrastructure.Persistence;
using CosmosDBMultiTenancy.Infrastructure.Persistence.Documents;
using Finbuckle.MultiTenant;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Azure.CosmosRepository;
using Microsoft.Azure.CosmosRepository.Options;
using Microsoft.Azure.CosmosRepository.Providers;
using Microsoft.Extensions.Options;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBRepository", Version = "1.0")]

namespace CosmosDBMultiTenancy.Infrastructure.Repositories
{
    internal class InvoiceCosmosDBRepository : CosmosDBRepositoryBase<Invoice, InvoiceDocument, IInvoiceDocument>, IInvoiceRepository
    {
        public InvoiceCosmosDBRepository(CosmosDBUnitOfWork unitOfWork,
            Microsoft.Azure.CosmosRepository.IRepository<InvoiceDocument> cosmosRepository,
            ICosmosContainerProvider<InvoiceDocument> containerProvider,
            IOptionsMonitor<RepositoryOptions> optionsMonitor,
            IMultiTenantContextAccessor<TenantInfo> multiTenantContextAccessor,
            ICurrentUserService currentUserService) : base(unitOfWork, cosmosRepository, "id", containerProvider, optionsMonitor, "tenantId", multiTenantContextAccessor, currentUserService)
        {
        }

        public async Task<Invoice?> FindByIdAsync(string id, CancellationToken cancellationToken = default) => await base.FindByIdAsync(id: id, cancellationToken: cancellationToken);

        protected override Expression<Func<InvoiceDocument, bool>> GetHasPartitionKeyExpression(string? partitionKey)
        {
            return document => document.TenantId == partitionKey;
        }
    }
}