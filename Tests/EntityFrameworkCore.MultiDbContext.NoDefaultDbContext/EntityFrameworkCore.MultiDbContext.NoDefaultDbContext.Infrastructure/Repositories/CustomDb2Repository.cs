using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.MultiDbContext.NoDefaultDbContext.Domain.Contracts;
using EntityFrameworkCore.MultiDbContext.NoDefaultDbContext.Domain.Repositories;
using EntityFrameworkCore.MultiDbContext.NoDefaultDbContext.Infrastructure.Persistence;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.CustomRepository", Version = "1.0")]

namespace EntityFrameworkCore.MultiDbContext.NoDefaultDbContext.Infrastructure.Repositories
{
    public class CustomDb2Repository : ICustomDb2Repository
    {
        private readonly Db2DbContext _dbContext;

        public CustomDb2Repository(Db2DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task TestProc(CancellationToken cancellationToken = default)
        {
            await _dbContext.Database.ExecuteSqlInterpolatedAsync($"EXECUTE TestProc", cancellationToken);
        }

        public async Task UpdateProductName(int productId, string newName, CancellationToken cancellationToken = default)
        {
            await _dbContext.Database.ExecuteSqlInterpolatedAsync($"EXECUTE UpdateProductName {productId}, {newName}", cancellationToken);
        }

        public async Task<bool> UpdateProductPrice(
            int productId,
            decimal newPrice,
            CancellationToken cancellationToken = default)
        {
            var successParameter = new SqlParameter
            {
                Direction = ParameterDirection.Output,
                SqlDbType = SqlDbType.Bit,
                ParameterName = "@success"
            };

            await _dbContext.Database.ExecuteSqlInterpolatedAsync($"EXECUTE UpdateProductPrice {productId}, {newPrice}, {successParameter} OUTPUT", cancellationToken);

            return (bool)successParameter.Value;
        }

        public async Task<List<ProductInMemory>> GetProductsByName(
            string search,
            CancellationToken cancellationToken = default)
        {
            var results = await _dbContext.ProductInMemories
                .FromSqlInterpolated($"EXECUTE GetProductsByName {search}")
                .IgnoreQueryFilters()
                .ToListAsync(cancellationToken);

            return results;
        }
    }
}