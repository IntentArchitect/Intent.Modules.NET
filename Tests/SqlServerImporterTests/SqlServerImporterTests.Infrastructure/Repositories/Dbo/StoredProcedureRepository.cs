using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SqlServerImporterTests.Domain.Contracts.Dbo;
using SqlServerImporterTests.Domain.Repositories.Dbo;
using SqlServerImporterTests.Infrastructure.Persistence;
using SqlServerImporterTests.Infrastructure.Repositories.ExtensionMethods.Dbo;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.Repositories.CustomRepository", Version = "1.0")]

namespace SqlServerImporterTests.Infrastructure.Repositories.Dbo
{
    public class StoredProcedureRepository : IStoredProcedureRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public StoredProcedureRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IReadOnlyCollection<GetCustomerOrdersResponse>> GetCustomerOrders(
            Guid customerId,
            CancellationToken cancellationToken = default)
        {
            var results = await _dbContext.GetCustomerOrdersResponses
                .FromSqlInterpolated($"EXECUTE GetCustomerOrders {customerId}")
                .IgnoreQueryFilters()
                .ToArrayAsync(cancellationToken);

            return results;
        }

        public async Task<IReadOnlyCollection<GetOrderItemDetailsResponse>> GetOrderItemDetails(
            Guid orderId,
            CancellationToken cancellationToken = default)
        {
            var results = await _dbContext.GetOrderItemDetailsResponses
                .FromSqlInterpolated($"EXECUTE GetOrderItemDetails {orderId}")
                .IgnoreQueryFilters()
                .ToArrayAsync(cancellationToken);

            return results;
        }

        public async Task InsertBrand(IEnumerable<BrandType> brand, CancellationToken cancellationToken = default)
        {
            var brandParameter = new SqlParameter
            {
                IsNullable = false,
                SqlDbType = SqlDbType.Structured,
                Value = brand.ToDataTable(),
                TypeName = "BrandType"
            };

            await _dbContext.Database.ExecuteSqlInterpolatedAsync($"EXECUTE InsertBrand {brandParameter}", cancellationToken);
        }
    }
}