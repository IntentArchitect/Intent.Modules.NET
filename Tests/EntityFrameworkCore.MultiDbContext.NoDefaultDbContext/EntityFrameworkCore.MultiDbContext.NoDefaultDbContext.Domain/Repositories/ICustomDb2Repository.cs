using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.MultiDbContext.NoDefaultDbContext.Domain.Contracts;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.CustomRepositoryInterface", Version = "1.0")]

namespace EntityFrameworkCore.MultiDbContext.NoDefaultDbContext.Domain.Repositories
{
    public interface ICustomDb2Repository
    {
        Task TestProc(CancellationToken cancellationToken = default);
        Task UpdateProductName(int productId, string newName, CancellationToken cancellationToken = default);
        Task<bool> UpdateProductPrice(int productId, decimal newPrice, CancellationToken cancellationToken = default);
        Task<List<ProductInMemory>> GetProductsByName(string search, CancellationToken cancellationToken = default);
    }
}