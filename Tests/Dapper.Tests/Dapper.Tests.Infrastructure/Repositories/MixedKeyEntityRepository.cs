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
    public class MixedKeyEntityRepository : RepositoryBase, IMixedKeyEntityRepository
    {
        public MixedKeyEntityRepository(IConfiguration configuration) : base(configuration)
        {
        }

        public async Task AddAsync(MixedKeyEntity entity, CancellationToken cancellationToken = default)
        {
            using (var connection = GetConnection())
            {
                var sql = @"
INSERT INTO [MixedKeyEntity]
(TenantId, Name)
OUTPUT Inserted.RowId
VALUES
(@TenantId, @Name)
";

                var newId = await connection.QuerySingleAsync<Guid>(sql, entity);
                entity.RowId = newId;
            }
        }

        public async Task UpdateAsync(MixedKeyEntity entity, CancellationToken cancellationToken = default)
        {
            using (var connection = GetConnection())
            {
                var sql = @"
UPDATE [MixedKeyEntity] SET
    Name = @Name
WHERE TenantId = @TenantId AND RowId = @RowId
";

                await connection.ExecuteAsync(sql, entity);
            }
        }

        public async Task RemoveAsync(MixedKeyEntity entity, CancellationToken cancellationToken = default)
        {
            using (var connection = GetConnection())
            {
                var sql = "DELETE FROM [MixedKeyEntity] WHERE TenantId = @TenantId AND RowId = @RowId";

                await connection.ExecuteAsync(sql, new { TenantId = entity.TenantId, RowId = entity.RowId });
            }
        }

        public async Task<MixedKeyEntity?> FindByIdAsync(
            (Guid TenantId, Guid RowId) id,
            CancellationToken cancellationToken = default)
        {
            using (var connection = GetConnection())
            {
                var sql = "SELECT * FROM [MixedKeyEntity] WHERE TenantId = @TenantId AND RowId = @RowId";

                return await connection.QuerySingleOrDefaultAsync<MixedKeyEntity>(sql, new { TenantId = id.TenantId, RowId = id.RowId });
            }
        }

        public async Task<List<MixedKeyEntity>> FindAllAsync(CancellationToken cancellationToken = default)
        {
            using (var connection = GetConnection())
            {
                var sql = "SELECT * FROM [MixedKeyEntity]";

                var result = await connection.QueryAsync<MixedKeyEntity>(sql);
                return result.ToList();
            }
        }
    }
}