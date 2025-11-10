using System.Collections.Generic;
using System.Data;
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
    public class BespokeRepository : IBespokeRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public BespokeRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task SpUpdateDataEntries(
            int? supplierID,
            IEnumerable<UdttDataEntityModel>? dataEntity,
            int? userID,
            CancellationToken cancellationToken = default)
        {
            var dataEntityParameter = new SqlParameter
            {
                IsNullable = true,
                SqlDbType = SqlDbType.Structured,
                Value = dataEntity.ToDataTable(),
                TypeName = "UdttDataEntityModel"
            };

            await _dbContext.Database.ExecuteSqlInterpolatedAsync($"EXECUTE spCacheSupplierPrice {supplierID}, {dataEntityParameter}, {userID}", cancellationToken);
        }
    }
}