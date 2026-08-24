using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using Dapper.Tests.Domain.Entities;
using Dapper.Tests.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.Configuration;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Dapper.Repository", Version = "1.0")]

namespace Dapper.Tests.Infrastructure.Repositories
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class OrderLineRepository : RepositoryBase, IOrderLineRepository
    {
        public OrderLineRepository(IConfiguration configuration) : base(configuration)
        {
        }

        public async Task AddAsync(OrderLine entity, CancellationToken cancellationToken = default)
        {
            using (var connection = GetConnection())
            {
                var sql = @"
INSERT INTO [OrderLine]
(OrderId, ProductId, Quantity)
VALUES
(@OrderId, @ProductId, @Quantity)
";

                await connection.ExecuteAsync(sql, entity);
            }
        }

        public async Task UpdateAsync(OrderLine entity, CancellationToken cancellationToken = default)
        {
            using (var connection = GetConnection())
            {
                var sql = @"
UPDATE [OrderLine] SET
    Quantity = @Quantity
WHERE OrderId = @OrderId AND ProductId = @ProductId
";

                await connection.ExecuteAsync(sql, entity);
            }
        }

        public async Task RemoveAsync(OrderLine entity, CancellationToken cancellationToken = default)
        {
            using (var connection = GetConnection())
            {
                var sql = "DELETE FROM [OrderLine] WHERE OrderId = @OrderId AND ProductId = @ProductId";

                await connection.ExecuteAsync(sql, new { OrderId = entity.OrderId, ProductId = entity.ProductId });
            }
        }

        public async Task<OrderLine?> FindByIdAsync(
            (Guid OrderId, Guid ProductId) id,
            CancellationToken cancellationToken = default)
        {
            using (var connection = GetConnection())
            {
                var sql = "SELECT * FROM [OrderLine] WHERE OrderId = @OrderId AND ProductId = @ProductId";

                return await connection.QuerySingleOrDefaultAsync<OrderLine>(sql, new { OrderId = id.OrderId, ProductId = id.ProductId });
            }
        }

        public async Task<List<OrderLine>> FindAllAsync(CancellationToken cancellationToken = default)
        {
            using (var connection = GetConnection())
            {
                var sql = "SELECT * FROM [OrderLine]";

                var result = await connection.QueryAsync<OrderLine>(sql);
                return result.ToList();
            }
        }
    }
}