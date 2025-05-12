using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.MultiDbContext.NoDefaultDbContext.Domain.Contracts;
using EntityFrameworkCore.MultiDbContext.NoDefaultDbContext.Domain.Repositories;
using EntityFrameworkCore.MultiDbContext.NoDefaultDbContext.Infrastructure.Persistence;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.CustomRepository", Version = "1.0")]

namespace EntityFrameworkCore.MultiDbContext.NoDefaultDbContext.Infrastructure.Repositories
{
    public class CustomDb2Repository : ICustomDb2Repository
    {
        private readonly Db2DbContext _dbContext;

        public CustomDb2Repository(Db2DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task TestProc(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public async Task UpdateProductName(int productId, string newName, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> UpdateProductPrice(
            int productId,
            decimal newPrice,
            CancellationToken cancellationToken = default)
        {

            throw new NotImplementedException();
        }

        public async Task<IReadOnlyCollection<ProductInMemory>> GetProductsByName(
            string search,
            CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}