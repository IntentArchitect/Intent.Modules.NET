using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.Repositories.TestApplication.Domain.Contracts;
using EntityFrameworkCore.Repositories.TestApplication.Domain.Repositories;
using EntityFrameworkCore.Repositories.TestApplication.Infrastructure.Persistence;
using EntityFrameworkCore.Repositories.TestApplication.Infrastructure.Repositories.ExtensionMethods;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.CustomRepository", Version = "1.0")]

namespace EntityFrameworkCore.Repositories.TestApplication.Infrastructure.Repositories
{
    public class SqlParameterRepositoryWithNameRepository : ISqlParameterRepositoryWithNameRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public SqlParameterRepositoryWithNameRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Input0_tableType0_output0_return0(CancellationToken cancellationToken = default)
        {
            await _dbContext.Database.ExecuteSqlInterpolatedAsync($"EXECUTE input0_tableType0_output0_return0", cancellationToken);
        }

        public async Task<SpResult> Input0_tableType0_output0_return1(CancellationToken cancellationToken = default)
        {
            var result = (await _dbContext.SpResults
                .FromSqlInterpolated($"EXECUTE input0_tableType0_output0_return1")
                .IgnoreQueryFilters()
                .ToListAsync(cancellationToken))
                .Single();

            return result;
        }

        public async Task<int> Input0_tableType0_output1_return0(CancellationToken cancellationToken = default)
        {
            var output0Parameter = new SqlParameter
            {
                Direction = ParameterDirection.Output,
                SqlDbType = SqlDbType.Int,
                ParameterName = "@output0"
            };

            await _dbContext.Database.ExecuteSqlInterpolatedAsync($"EXECUTE input0_tableType0_output1_return0 {output0Parameter} OUTPUT", cancellationToken);

            return (int)output0Parameter.Value;
        }

        public async Task<(SpResult Result, int Output0Output)> Input0_tableType0_output1_return1(CancellationToken cancellationToken = default)
        {
            var output0Parameter = new SqlParameter
            {
                Direction = ParameterDirection.Output,
                SqlDbType = SqlDbType.Int,
                ParameterName = "@output0"
            };

            var result = (await _dbContext.SpResults
                .FromSqlInterpolated($"EXECUTE input0_tableType0_output1_return1 {output0Parameter} OUTPUT")
                .IgnoreQueryFilters()
                .ToListAsync(cancellationToken))
                .Single();

            return (result, (int)output0Parameter.Value);
        }

        public async Task Input0_tableType1_output0_return0(
            IEnumerable<SpParameterWithName> tableType0,
            CancellationToken cancellationToken = default)
        {
            var tableType0Parameter = new SqlParameter
            {
                IsNullable = false,
                SqlDbType = SqlDbType.Structured,
                Value = tableType0.ToDataTable(),
                TypeName = "customName"
            };

            await _dbContext.Database.ExecuteSqlInterpolatedAsync($"EXECUTE input0_tableType1_output0_return0 {tableType0Parameter}", cancellationToken);
        }

        public async Task<SpResult> Input0_tableType1_output0_return1(
            IEnumerable<SpParameterWithName> tableType0,
            CancellationToken cancellationToken = default)
        {
            var tableType0Parameter = new SqlParameter
            {
                IsNullable = false,
                SqlDbType = SqlDbType.Structured,
                Value = tableType0.ToDataTable(),
                TypeName = "customName"
            };

            var result = (await _dbContext.SpResults
                .FromSqlInterpolated($"EXECUTE input0_tableType1_output0_return1 {tableType0Parameter}")
                .IgnoreQueryFilters()
                .ToListAsync(cancellationToken))
                .Single();

            return result;
        }

        public async Task<int> Input0_tableType1_output1_return0(
            IEnumerable<SpParameterWithName> tableType0,
            CancellationToken cancellationToken = default)
        {
            var output0Parameter = new SqlParameter
            {
                Direction = ParameterDirection.Output,
                SqlDbType = SqlDbType.Int,
                ParameterName = "@output0"
            };

            var tableType0Parameter = new SqlParameter
            {
                IsNullable = false,
                SqlDbType = SqlDbType.Structured,
                Value = tableType0.ToDataTable(),
                TypeName = "customName"
            };

            await _dbContext.Database.ExecuteSqlInterpolatedAsync($"EXECUTE input0_tableType1_output1_return0 {output0Parameter} OUTPUT, {tableType0Parameter}", cancellationToken);

            return (int)output0Parameter.Value;
        }

        public async Task<(SpResult Result, int Output0Output)> Input0_tableType1_output1_return1(
            IEnumerable<SpParameterWithName> tableType0,
            CancellationToken cancellationToken = default)
        {
            var output0Parameter = new SqlParameter
            {
                Direction = ParameterDirection.Output,
                SqlDbType = SqlDbType.Int,
                ParameterName = "@output0"
            };

            var tableType0Parameter = new SqlParameter
            {
                IsNullable = false,
                SqlDbType = SqlDbType.Structured,
                Value = tableType0.ToDataTable(),
                TypeName = "customName"
            };

            var result = (await _dbContext.SpResults
                .FromSqlInterpolated($"EXECUTE input0_tableType1_output1_return1 {output0Parameter} OUTPUT, {tableType0Parameter}")
                .IgnoreQueryFilters()
                .ToListAsync(cancellationToken))
                .Single();

            return (result, (int)output0Parameter.Value);
        }

        public async Task Input1_tableType0_output0_return0(int input0, CancellationToken cancellationToken = default)
        {
            await _dbContext.Database.ExecuteSqlInterpolatedAsync($"EXECUTE input1_tableType0_output0_return0 {input0}", cancellationToken);
        }

        public async Task<SpResult> Input1_tableType0_output0_return1(
            int input0,
            CancellationToken cancellationToken = default)
        {
            var result = (await _dbContext.SpResults
                .FromSqlInterpolated($"EXECUTE input1_tableType0_output0_return1 {input0}")
                .IgnoreQueryFilters()
                .ToListAsync(cancellationToken))
                .Single();

            return result;
        }

        public async Task<int> Input1_tableType0_output1_return0(int input0, CancellationToken cancellationToken = default)
        {
            var output0Parameter = new SqlParameter
            {
                Direction = ParameterDirection.Output,
                SqlDbType = SqlDbType.Int,
                ParameterName = "@output0"
            };

            await _dbContext.Database.ExecuteSqlInterpolatedAsync($"EXECUTE input1_tableType0_output1_return0 {output0Parameter} OUTPUT, {input0}", cancellationToken);

            return (int)output0Parameter.Value;
        }

        public async Task<(SpResult Result, int Output0Output)> Input1_tableType0_output1_return1(
            int input0,
            CancellationToken cancellationToken = default)
        {
            var output0Parameter = new SqlParameter
            {
                Direction = ParameterDirection.Output,
                SqlDbType = SqlDbType.Int,
                ParameterName = "@output0"
            };

            var result = (await _dbContext.SpResults
                .FromSqlInterpolated($"EXECUTE input1_tableType0_output1_return1 {output0Parameter} OUTPUT, {input0}")
                .IgnoreQueryFilters()
                .ToListAsync(cancellationToken))
                .Single();

            return (result, (int)output0Parameter.Value);
        }

        public async Task Input1_tableType1_output0_return0(
            IEnumerable<SpParameterWithName> tableType0,
            int input0,
            CancellationToken cancellationToken = default)
        {
            var tableType0Parameter = new SqlParameter
            {
                IsNullable = false,
                SqlDbType = SqlDbType.Structured,
                Value = tableType0.ToDataTable(),
                TypeName = "customName"
            };

            await _dbContext.Database.ExecuteSqlInterpolatedAsync($"EXECUTE input1_tableType1_output0_return0 {tableType0Parameter}, {input0}", cancellationToken);
        }

        public async Task<SpResult> Input1_tableType1_output0_return1(
            IEnumerable<SpParameterWithName> tableType0,
            int input0,
            CancellationToken cancellationToken = default)
        {
            var tableType0Parameter = new SqlParameter
            {
                IsNullable = false,
                SqlDbType = SqlDbType.Structured,
                Value = tableType0.ToDataTable(),
                TypeName = "customName"
            };

            var result = (await _dbContext.SpResults
                .FromSqlInterpolated($"EXECUTE input1_tableType1_output0_return1 {tableType0Parameter}, {input0}")
                .IgnoreQueryFilters()
                .ToListAsync(cancellationToken))
                .Single();

            return result;
        }

        public async Task<int> Input1_tableType1_output1_return0(
            IEnumerable<SpParameterWithName> tableType0,
            int input0,
            CancellationToken cancellationToken = default)
        {
            var output0Parameter = new SqlParameter
            {
                Direction = ParameterDirection.Output,
                SqlDbType = SqlDbType.Int,
                ParameterName = "@output0"
            };

            var tableType0Parameter = new SqlParameter
            {
                IsNullable = false,
                SqlDbType = SqlDbType.Structured,
                Value = tableType0.ToDataTable(),
                TypeName = "customName"
            };

            await _dbContext.Database.ExecuteSqlInterpolatedAsync($"EXECUTE input1_tableType1_output1_return0 {output0Parameter} OUTPUT, {tableType0Parameter}, {input0}", cancellationToken);

            return (int)output0Parameter.Value;
        }

        public async Task<(SpResult Result, int Output0Output)> Input1_tableType1_output1_return1(
            IEnumerable<SpParameterWithName> tableType0,
            int input0,
            CancellationToken cancellationToken = default)
        {
            var output0Parameter = new SqlParameter
            {
                Direction = ParameterDirection.Output,
                SqlDbType = SqlDbType.Int,
                ParameterName = "@output0"
            };

            var tableType0Parameter = new SqlParameter
            {
                IsNullable = false,
                SqlDbType = SqlDbType.Structured,
                Value = tableType0.ToDataTable(),
                TypeName = "customName"
            };

            var result = (await _dbContext.SpResults
                .FromSqlInterpolated($"EXECUTE input1_tableType1_output1_return1 {output0Parameter} OUTPUT, {tableType0Parameter}, {input0}")
                .IgnoreQueryFilters()
                .ToListAsync(cancellationToken))
                .Single();

            return (result, (int)output0Parameter.Value);
        }
    }
}