using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using SqlServerImporterTests.Domain.Contracts.Dbo;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.Repositories.CustomRepositoryInterface", Version = "1.0")]

namespace SqlServerImporterTests.Domain.Repositories.Dbo
{
    public interface IStoredProcedureRepository
    {
        Task<IReadOnlyCollection<GetCustomerOrdersResponse>> GetCustomerOrders(Guid customerId, CancellationToken cancellationToken = default);
        Task<IReadOnlyCollection<GetOrderItemDetailsResponse>> GetOrderItemDetails(Guid orderId, CancellationToken cancellationToken = default);
        Task InsertBrand(IEnumerable<BrandType> brand, CancellationToken cancellationToken = default);
    }
}