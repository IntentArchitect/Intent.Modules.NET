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
    public class DualGeneratedKeyEntityRepository : RepositoryBase, IDualGeneratedKeyEntityRepository
    {
        public DualGeneratedKeyEntityRepository(IConfiguration configuration) : base(configuration)
        {
        }

        public async Task AddAsync(DualGeneratedKeyEntity entity, CancellationToken cancellationToken = default)
        {
            using (var connection = GetConnection())
            {
                var sql = @"
INSERT INTO [DualGeneratedKeyEntity]
(Description)
OUTPUT Inserted.KeyPartA, Inserted.KeyPartB
VALUES
(@Description)
";

                var generatedResult = await connection.QuerySingleAsync(sql, entity);
                entity.KeyPartA = generatedResult.KeyPartA;
                entity.KeyPartB = generatedResult.KeyPartB;
            }
        }

        public async Task UpdateAsync(DualGeneratedKeyEntity entity, CancellationToken cancellationToken = default)
        {
            using (var connection = GetConnection())
            {
                var sql = @"
UPDATE [DualGeneratedKeyEntity] SET
    Description = @Description
WHERE KeyPartA = @KeyPartA AND KeyPartB = @KeyPartB
";

                await connection.ExecuteAsync(sql, entity);
            }
        }

        public async Task RemoveAsync(DualGeneratedKeyEntity entity, CancellationToken cancellationToken = default)
        {
            using (var connection = GetConnection())
            {
                var sql = "DELETE FROM [DualGeneratedKeyEntity] WHERE KeyPartA = @KeyPartA AND KeyPartB = @KeyPartB";

                await connection.ExecuteAsync(sql, new { KeyPartA = entity.KeyPartA, KeyPartB = entity.KeyPartB });
            }
        }

        public async Task<DualGeneratedKeyEntity?> FindByIdAsync(
            (Guid KeyPartA, Guid KeyPartB) id,
            CancellationToken cancellationToken = default)
        {
            using (var connection = GetConnection())
            {
                var sql = "SELECT * FROM [DualGeneratedKeyEntity] WHERE KeyPartA = @KeyPartA AND KeyPartB = @KeyPartB";

                return await connection.QuerySingleOrDefaultAsync<DualGeneratedKeyEntity>(sql, new { KeyPartA = id.KeyPartA, KeyPartB = id.KeyPartB });
            }
        }

        public async Task<List<DualGeneratedKeyEntity>> FindAllAsync(CancellationToken cancellationToken = default)
        {
            using (var connection = GetConnection())
            {
                var sql = "SELECT * FROM [DualGeneratedKeyEntity]";

                var result = await connection.QueryAsync<DualGeneratedKeyEntity>(sql);
                return result.ToList();
            }
        }
    }
}