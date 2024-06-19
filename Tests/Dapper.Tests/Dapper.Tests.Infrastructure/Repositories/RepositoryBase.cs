using System;
using System.Data;
using System.Data.SqlClient;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.Configuration;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Dapper.RepositoryBase", Version = "1.0")]

namespace Dapper.Tests.Infrastructure.Repositories
{
    public abstract class RepositoryBase<TDomain>
        where TDomain : class
    {
        private readonly string _connectionString;

        public RepositoryBase(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection") ?? throw new Exception("No `DefaultConnection` connection string configured");
        }

        protected IDbConnection GetConnection()
        {
            var result = new SqlConnection(_connectionString);
            result.Open();
            return result;
        }
    }
}