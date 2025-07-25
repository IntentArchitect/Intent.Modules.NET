using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.MultiDbContext.NoDefaultDbContext.Domain.Contracts;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.CustomRepositoryInterface", Version = "1.0")]

namespace EntityFrameworkCore.MultiDbContext.NoDefaultDbContext.Domain.Repositories
{
    public interface ICustomDb3Repository
    {
        Task TestProc(CancellationToken cancellationToken = default);
        Task<bool> Update_product_price(int product_id, decimal new_price, CancellationToken cancellationToken = default);
        Task Update_product_name(int product_id, string new_name, CancellationToken cancellationToken = default);
        Task<List<Product>> Get_products_by_name(string search, CancellationToken cancellationToken = default);
    }
}