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
    public interface IOrderRepository2Repository
    {
        Task<IReadOnlyCollection<CustomerOrder>> GetCustomerOrders(Guid customerId, CancellationToken cancellationToken = default);
    }
}