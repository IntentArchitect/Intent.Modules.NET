using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using SqlServerImporterTests.Domain.Contracts.Dbo;
using SqlServerImporterTests.Domain.Repositories.Dbo;
using SqlServerImporterTests.Infrastructure.Persistence;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.Repositories.CustomRepository", Version = "1.0")]

namespace SqlServerImporterTests.Infrastructure.Repositories.Dbo
{
    public class OrderRepository2Repository : IOrderRepository2Repository
    {
        private readonly ApplicationDbContext _dbContext;

        public OrderRepository2Repository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IReadOnlyCollection<CustomerOrder>> GetCustomerOrders(
            Guid customerId,
            CancellationToken cancellationToken = default)
        {
            var results = await _dbContext.CustomerOrders
                .FromSqlInterpolated($"EXECUTE GetCustomerOrders {customerId}")
                .IgnoreQueryFilters()
                .ToArrayAsync(cancellationToken);

            return results;
        }
    }
}