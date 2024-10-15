using System;
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
[assembly: IntentTemplate("Intent.EntityFrameworkCore.Repositories.CustomRepository", Version = "1.0")]

namespace EntityFrameworkCore.Repositories.TestApplication.Infrastructure.Repositories
{
    public class StoredProcOperationsRepository : IStoredProcOperationsRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public StoredProcOperationsRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<SpResult> MyProc(IEnumerable<SpParameter> param, CancellationToken cancellationToken = default)
        {
            var paramParameter = new SqlParameter
            {
                IsNullable = false,
                SqlDbType = SqlDbType.Structured,
                Value = param.ToDataTable(),
                TypeName = "SpParameter"
            };

            var result = (await _dbContext.SpResults
                .FromSqlInterpolated($"EXECUTE MyProc {paramParameter}")
                .IgnoreQueryFilters()
                .ToArrayAsync(cancellationToken))
                .Single();

            return result;
        }

        public async Task<string> WithBidrectionalParam(
            string birectionalTest,
            CancellationToken cancellationToken = default)
        {
            throw new NotSupportedException("One or more parameters have a direction of both which is not presently supported, please reach out to us at https://github.com/IntentArchitect/Support should you need support added.");
        }
    }
}