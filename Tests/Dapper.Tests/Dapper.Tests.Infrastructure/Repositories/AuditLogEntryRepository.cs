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
    public class AuditLogEntryRepository : RepositoryBase, IAuditLogEntryRepository
    {
        public AuditLogEntryRepository(IConfiguration configuration) : base(configuration)
        {
        }

        public async Task AddAsync(AuditLogEntry entity, CancellationToken cancellationToken = default)
        {
            using (var connection = GetConnection())
            {
                var sql = @"
INSERT INTO [AuditLogEntry]
(Message, IsProcessed)
VALUES
(@Message, @IsProcessed)
";

                await connection.ExecuteAsync(sql, entity);
            }
        }

        public async Task<List<AuditLogEntry>> FindAllAsync(CancellationToken cancellationToken = default)
        {
            using (var connection = GetConnection())
            {
                var sql = "SELECT * FROM [AuditLogEntry]";

                var result = await connection.QueryAsync<AuditLogEntry>(sql);
                return result.ToList();
            }
        }
    }
}