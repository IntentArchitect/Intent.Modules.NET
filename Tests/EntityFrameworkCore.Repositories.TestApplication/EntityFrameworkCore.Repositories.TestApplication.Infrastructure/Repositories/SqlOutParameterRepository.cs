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
    }
}