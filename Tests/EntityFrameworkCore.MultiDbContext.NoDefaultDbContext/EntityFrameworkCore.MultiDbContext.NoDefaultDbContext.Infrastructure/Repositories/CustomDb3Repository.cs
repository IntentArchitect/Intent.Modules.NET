using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.MultiDbContext.NoDefaultDbContext.Domain.Contracts;
using EntityFrameworkCore.MultiDbContext.NoDefaultDbContext.Domain.Repositories;
using EntityFrameworkCore.MultiDbContext.NoDefaultDbContext.Infrastructure.Persistence;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using NpgsqlTypes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.CustomRepository", Version = "1.0")]

namespace EntityFrameworkCore.MultiDbContext.NoDefaultDbContext.Infrastructure.Repositories
{
    public class CustomDb3Repository : ICustomDb3Repository
    {
        private readonly Db3DbContext _dbContext;

        public CustomDb3Repository(Db3DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task TestProc(CancellationToken cancellationToken = default)
        {
            await _dbContext.Database.ExecuteSqlInterpolatedAsync($"CALL TestProc()", cancellationToken);
        }

        public async Task<bool> Update_product_price(
            int product_id,
            decimal new_price,
            CancellationToken cancellationToken = default)
        {
            var successParameter = new NpgsqlParameter
            {
                Direction = ParameterDirection.InputOutput,
                NpgsqlDbType = NpgsqlDbType.Boolean,
                ParameterName = "success",
                Value = DBNull.Value
            };

            await _dbContext.Database.ExecuteSqlInterpolatedAsync($"CALL update_product_price({product_id}, {new_price}, {successParameter})", cancellationToken);

            return (bool)successParameter.Value;
        }

        public async Task Update_product_name(int product_id, string new_name, CancellationToken cancellationToken = default)
        {
            await _dbContext.Database.ExecuteSqlInterpolatedAsync($"CALL update_product_name({product_id}, {new_name})", cancellationToken);
        }

        public async Task<IReadOnlyCollection<Product>> Get_products_by_name(
            string search,
            CancellationToken cancellationToken = default)
        {
            var results = await _dbContext.Products
                .FromSqlInterpolated($"SELECT * FROM get_products_by_name({search})")
                .IgnoreQueryFilters()
                .ToArrayAsync(cancellationToken);

            return results;
        }
    }
}