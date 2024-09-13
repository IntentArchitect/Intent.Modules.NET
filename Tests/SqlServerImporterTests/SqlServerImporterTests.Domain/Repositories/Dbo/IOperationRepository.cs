using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;
using SqlServerImporterTests.Domain.Contracts.Dbo;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.Repositories.CustomRepositoryInterface", Version = "1.0")]

namespace SqlServerImporterTests.Domain.Repositories.Dbo
{
    public interface IOperationRepository
    {
        List<GetCustomerOrdersResponse> GetCustomerOrders(Guid customerId);
        List<GetOrderItemDetailsResponse> GetOrderItemDetails(Guid orderId);
        void InsertBrand(IEnumerable<BrandType> brand);
    }
}