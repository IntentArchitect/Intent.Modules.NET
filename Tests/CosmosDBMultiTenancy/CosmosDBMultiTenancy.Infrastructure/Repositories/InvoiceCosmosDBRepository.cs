using System;
using System.Linq.Expressions;
using CosmosDBMultiTenancy.Application.Common.Interfaces;
using CosmosDBMultiTenancy.Domain.Entities;
using CosmosDBMultiTenancy.Domain.Repositories;
using CosmosDBMultiTenancy.Infrastructure.Persistence;
using CosmosDBMultiTenancy.Infrastructure.Persistence.Documents;
using Finbuckle.MultiTenant;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Azure.CosmosRepository;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBRepository", Version = "1.0")]

namespace CosmosDBMultiTenancy.Infrastructure.Repositories
{
    internal class InvoiceCosmosDBRepository : CosmosDBRepositoryBase<Invoice, Invoice, InvoiceDocument>, IInvoiceRepository
    {
        public InvoiceCosmosDBRepository(CosmosDBUnitOfWork unitOfWork,
            Microsoft.Azure.CosmosRepository.IRepository<InvoiceDocument> cosmosRepository,
            IMultiTenantContextAccessor<TenantInfo> multiTenantContextAccessor,
            ICurrentUserService currentUserService) : base(unitOfWork, cosmosRepository, "id", "tenantId", multiTenantContextAccessor, currentUserService)
        {
        }

        protected override Expression<Func<InvoiceDocument, bool>> GetHasPartitionKeyExpression(string? partitionKey)
        {
            return document => document.TenantId == partitionKey;
        }
    }
}