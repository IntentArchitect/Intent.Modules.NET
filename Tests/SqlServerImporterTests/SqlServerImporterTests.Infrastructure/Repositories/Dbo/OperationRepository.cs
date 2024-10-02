using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;
using SqlServerImporterTests.Domain.Contracts.Dbo;
using SqlServerImporterTests.Domain.Repositories.Dbo;
using SqlServerImporterTests.Infrastructure.Persistence;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.Repositories.CustomRepository", Version = "1.0")]

namespace SqlServerImporterTests.Infrastructure.Repositories.Dbo
{
    public class OperationRepository : IOperationRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public OperationRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public List<GetCustomerOrdersResponse> GetCustomerOrders(Guid customerId)
        {
            // TODO: Implement GetCustomerOrders (OperationRepository) functionality
            throw new NotImplementedException("Your implementation here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public List<GetOrderItemDetailsResponse> GetOrderItemDetails(Guid orderId)
        {
            // TODO: Implement GetOrderItemDetails (OperationRepository) functionality
            throw new NotImplementedException("Your implementation here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public void InsertBrand(IEnumerable<BrandType> brand)
        {
            // TODO: Implement InsertBrand (OperationRepository) functionality
            throw new NotImplementedException("Your implementation here...");
        }
    }
}