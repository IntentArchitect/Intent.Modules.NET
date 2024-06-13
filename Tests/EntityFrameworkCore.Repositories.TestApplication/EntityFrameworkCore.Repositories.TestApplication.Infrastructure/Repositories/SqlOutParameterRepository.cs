using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.Repositories.TestApplication.Domain.Repositories;
using EntityFrameworkCore.Repositories.TestApplication.Infrastructure.Persistence;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.Repositories.CustomRepository", Version = "1.0")]

namespace EntityFrameworkCore.Repositories.TestApplication.Infrastructure.Repositories
{
    public class SqlOutParameterRepository : ISqlOutParameterRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public SqlOutParameterRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<int> Sp_out_params_int(CancellationToken cancellationToken = default)
        {
            var outputParameter = new SqlParameter
            {
                Direction = ParameterDirection.Output,
                SqlDbType = SqlDbType.Int,
                ParameterName = "@output"
            };

            await _dbContext.Database.ExecuteSqlInterpolatedAsync($"EXECUTE sp_out_params_int {outputParameter} OUTPUT", cancellationToken);

            return (int)outputParameter.Value;
        }

        public async Task<string> Sp_out_params_string(CancellationToken cancellationToken = default)
        {
            var outputParameter = new SqlParameter
            {
                Direction = ParameterDirection.Output,
                SqlDbType = SqlDbType.NVarChar,
                Size = 255,
                ParameterName = "@output"
            };

            await _dbContext.Database.ExecuteSqlInterpolatedAsync($"EXECUTE sp_out_params_string {outputParameter} OUTPUT", cancellationToken);

            return (string)outputParameter.Value;
        }

        public async Task<decimal> Sp_out_params_decimal_default(CancellationToken cancellationToken = default)
        {
            var outputParameter = new SqlParameter
            {
                Direction = ParameterDirection.Output,
                SqlDbType = SqlDbType.Decimal,
                Precision = 18,
                Scale = 2,
                ParameterName = "@output"
            };

            await _dbContext.Database.ExecuteSqlInterpolatedAsync($"EXECUTE sp_out_params_decimal_default {outputParameter} OUTPUT", cancellationToken);

            return (decimal)outputParameter.Value;
        }

        public async Task<decimal> Sp_out_params_decimal_specific(CancellationToken cancellationToken = default)
        {
            var outputParameter = new SqlParameter
            {
                Direction = ParameterDirection.Output,
                SqlDbType = SqlDbType.Decimal,
                Precision = 16,
                Scale = 4,
                ParameterName = "@output"
            };

            await _dbContext.Database.ExecuteSqlInterpolatedAsync($"EXECUTE sp_out_params_decimal_specific {outputParameter} OUTPUT", cancellationToken);

            return (decimal)outputParameter.Value;
        }

        public async Task<(string Param1Output, DateTime Param2Output, bool Param3Output)> Sp_out_params_multiple(CancellationToken cancellationToken = default)
        {
            var param1Parameter = new SqlParameter
            {
                Direction = ParameterDirection.Output,
                SqlDbType = SqlDbType.VarChar,
                Size = 255,
                ParameterName = "@param1"
            };

            var param2Parameter = new SqlParameter
            {
                Direction = ParameterDirection.Output,
                SqlDbType = SqlDbType.DateTime2,
                ParameterName = "@param2"
            };

            var param3Parameter = new SqlParameter
            {
                Direction = ParameterDirection.Output,
                SqlDbType = SqlDbType.Bit,
                ParameterName = "@param3"
            };

            await _dbContext.Database.ExecuteSqlInterpolatedAsync($"EXECUTE sp_out_params_multiple {param1Parameter} OUTPUT, {param2Parameter} OUTPUT, {param3Parameter} OUTPUT", cancellationToken);

            return ((string)param1Parameter.Value, (DateTime)param2Parameter.Value, (bool)param3Parameter.Value);
        }
    }
}