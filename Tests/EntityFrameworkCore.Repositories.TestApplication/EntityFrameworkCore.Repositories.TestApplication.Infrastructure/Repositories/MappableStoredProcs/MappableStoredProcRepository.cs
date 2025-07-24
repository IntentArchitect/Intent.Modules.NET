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
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.CustomRepository", Version = "1.0")]

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

        public async Task<MappedSpResult> MappedOperation(
            string paramName2WithAltName,
            string paramSomething1WithAltName,
            CancellationToken cancellationToken = default)
        {
            var outputParam1Parameter = new SqlParameter
            {
                Direction = ParameterDirection.Output,
                SqlDbType = SqlDbType.VarChar,
                Size = 255,
                ParameterName = "@outputParam1"
            };

            var outputParam2Parameter = new SqlParameter
            {
                Direction = ParameterDirection.Output,
                SqlDbType = SqlDbType.VarChar,
                Size = 255,
                ParameterName = "@outputParam2"
            };

            var result = (await _dbContext.MappedSpResultItems
                .FromSqlInterpolated($"EXECUTE MappedSp {paramSomething1WithAltName}, {paramName2WithAltName}, {outputParam1Parameter} OUTPUT, {outputParam2Parameter} OUTPUT")
                .IgnoreQueryFilters()
                .ToArrayAsync(cancellationToken))
                .Single();

            return new MappedSpResult(result: result, simpleString: (string)outputParam2Parameter.Value);
        }

        public async Task<MappedSpResultCollection> MappedOperationWithCollection(
            string paramRandom2WithAltName,
            string paramElse1WithAltName,
            CancellationToken cancellationToken = default)
        {
            var outputParam2Parameter = new SqlParameter
            {
                Direction = ParameterDirection.Output,
                SqlDbType = SqlDbType.VarChar,
                Size = 255,
                ParameterName = "@outputParam2"
            };

            var outputParam1Parameter = new SqlParameter
            {
                Direction = ParameterDirection.Output,
                SqlDbType = SqlDbType.VarChar,
                Size = 255,
                ParameterName = "@outputParam1"
            };

            var results = await _dbContext.MappedSpResultItems
                .FromSqlInterpolated($"EXECUTE MappedSpCollection {paramRandom2WithAltName}, {paramElse1WithAltName}, {outputParam2Parameter} OUTPUT, {outputParam1Parameter} OUTPUT")
                .IgnoreQueryFilters()
                .ToArrayAsync(cancellationToken);

            return new MappedSpResultCollection(result: results, simpleString: (string)outputParam2Parameter.Value);
        }

        public async Task<int> MappedOperationCallingScalar(
            int param1WithDiffName,
            CancellationToken cancellationToken = default)
        {
            var param1Parameter = new SqlParameter
            {
                Direction = ParameterDirection.Input,
                SqlDbType = SqlDbType.Int,
                ParameterName = "@param1",
                Value = param1WithDiffName
            };

            var result = await _dbContext.ExecuteScalarAsync<int>("EXECUTE MappedSpReturningScalar @param1", param1Parameter);

            return result;
        }
    }
}