using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.Repositories.TestApplication.Domain.Contracts.MappableStoredProcs;
using EntityFrameworkCore.Repositories.TestApplication.Domain.Repositories.MappableStoredProcs;
using EntityFrameworkCore.Repositories.TestApplication.Infrastructure.Persistence;
using EntityFrameworkCore.Repositories.TestApplication.Infrastructure.Repositories.ExtensionMethods.MappableStoredProcs;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.Repositories.CustomRepository", Version = "1.0")]

namespace EntityFrameworkCore.Repositories.TestApplication.Infrastructure.Repositories.MappableStoredProcs
{
    public class MappableStoredProcRepository : IMappableStoredProcRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public MappableStoredProcRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<EntityRecord> GetEntityById(Guid id, CancellationToken cancellationToken = default)
        {
            var result = (await _dbContext.EntityRecords
                .FromSqlInterpolated($"EXECUTE GetEntityById {id}")
                .IgnoreQueryFilters()
                .ToArrayAsync(cancellationToken))
                .Single();

            return result;
        }

        public async Task CreateEntities(IEnumerable<EntityRecord> entities, CancellationToken cancellationToken = default)
        {
            var entitiesParameter = new SqlParameter
            {
                IsNullable = false,
                SqlDbType = SqlDbType.Structured,
                Value = entities.ToDataTable(),
                TypeName = "EntityRecord"
            };

            await _dbContext.Database.ExecuteSqlInterpolatedAsync($"EXECUTE CreateEntities {entitiesParameter}", cancellationToken);
        }

        public async Task<string> GetEntityName(Guid id, CancellationToken cancellationToken = default)
        {
            var idParameter = new SqlParameter
            {
                Direction = ParameterDirection.Input,
                SqlDbType = SqlDbType.UniqueIdentifier,
                ParameterName = "@id",
                Value = id
            };

            var result = await _dbContext.ExecuteScalarAsync<string>("EXECUTE GetEntityName @id", idParameter);

            return result;
        }

        public async Task DoSomething(CancellationToken cancellationToken = default)
        {
            await _dbContext.Database.ExecuteSqlInterpolatedAsync($"EXECUTE DoSomething", cancellationToken);
        }

        public async Task CreateEntity(Guid id, string name, CancellationToken cancellationToken = default)
        {
            await _dbContext.Database.ExecuteSqlInterpolatedAsync($"EXECUTE CreateEntity {id}, {name}", cancellationToken);
        }

        public async Task<IReadOnlyCollection<EntityRecord>> GetEntities(CancellationToken cancellationToken = default)
        {
            var results = await _dbContext.EntityRecords
                .FromSqlInterpolated($"EXECUTE GetEntities")
                .IgnoreQueryFilters()
                .ToArrayAsync(cancellationToken);

            return results;
        }
    }
}