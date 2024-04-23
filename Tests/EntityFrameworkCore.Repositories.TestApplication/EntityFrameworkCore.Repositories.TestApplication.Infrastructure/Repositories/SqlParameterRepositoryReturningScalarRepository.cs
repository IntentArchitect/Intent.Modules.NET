using System.Data;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.Repositories.TestApplication.Domain.Repositories;
using EntityFrameworkCore.Repositories.TestApplication.Infrastructure.Persistence;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Data.SqlClient;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.Repositories.CustomRepository", Version = "1.0")]

namespace EntityFrameworkCore.Repositories.TestApplication.Infrastructure.Repositories
{
    public class SqlParameterRepositoryReturningScalarRepository : ISqlParameterRepositoryReturningScalarRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public SqlParameterRepositoryReturningScalarRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<int> Sp_params0_collection0_schemaName0(CancellationToken cancellationToken = default)
        {
            var result = await _dbContext.ExecuteScalarAsync<int>("EXECUTE sp_params0_collection0_schemaName0");

            return result;
        }

        public async Task<int> Sp_params0_collection0_schemaName1(CancellationToken cancellationToken = default)
        {
            var result = await _dbContext.ExecuteScalarAsync<int>("EXECUTE dbo.sp_custom");

            return result;
        }

        public async Task<int> Sp_params1_collection0_schemaName0(
            string param1,
            CancellationToken cancellationToken = default)
        {
            var param1Parameter = new SqlParameter
            {
                Direction = ParameterDirection.Input,
                SqlDbType = SqlDbType.VarChar,
                ParameterName = "@param1",
                Value = param1
            };

            var result = await _dbContext.ExecuteScalarAsync<int>("EXECUTE sp_params1_collection0_schemaName0 @param1", param1Parameter);

            return result;
        }

        public async Task<int> Sp_params1_collection0_schemaName1(
            string param1,
            CancellationToken cancellationToken = default)
        {
            var param1Parameter = new SqlParameter
            {
                Direction = ParameterDirection.Input,
                SqlDbType = SqlDbType.VarChar,
                ParameterName = "@param1",
                Value = param1
            };

            var result = await _dbContext.ExecuteScalarAsync<int>("EXECUTE dbo.sp_custom @param1", param1Parameter);

            return result;
        }

        public async Task<int> Sp_params2_collection0_schemaName0(
            string param1,
            string param2,
            CancellationToken cancellationToken = default)
        {
            var param1Parameter = new SqlParameter
            {
                Direction = ParameterDirection.Input,
                SqlDbType = SqlDbType.VarChar,
                ParameterName = "@param1",
                Value = param1
            };

            var param2Parameter = new SqlParameter
            {
                Direction = ParameterDirection.Input,
                SqlDbType = SqlDbType.VarChar,
                ParameterName = "@param2",
                Value = param2
            };

            var result = await _dbContext.ExecuteScalarAsync<int>("EXECUTE sp_params2_collection0_schemaName0 @param1, @param2", param1Parameter, param2Parameter);

            return result;
        }

        public async Task<int> Sp_params2_collection0_schemaName1(
            string param1,
            string param2,
            CancellationToken cancellationToken = default)
        {
            var param1Parameter = new SqlParameter
            {
                Direction = ParameterDirection.Input,
                SqlDbType = SqlDbType.VarChar,
                ParameterName = "@param1",
                Value = param1
            };

            var param2Parameter = new SqlParameter
            {
                Direction = ParameterDirection.Input,
                SqlDbType = SqlDbType.VarChar,
                ParameterName = "@param2",
                Value = param2
            };

            var result = await _dbContext.ExecuteScalarAsync<int>("EXECUTE dbo.sp_custom @param1, @param2", param1Parameter, param2Parameter);

            return result;
        }
    }
}