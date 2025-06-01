using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using SqlDbProject.Domain.Entities;
using SqlDbProject.Domain.Repositories;
using SqlDbProject.Infrastructure.Persistence;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.Repositories.Repository", Version = "1.0")]

namespace SqlDbProject.Infrastructure.Repositories
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class ProductRepository : RepositoryBase<Product, Product, ApplicationDbContext>, IProductRepository
    {
        public ProductRepository(ApplicationDbContext dbContext, IMapper mapper) : base(dbContext, mapper)
        {
        }

        public async Task<TProjection?> FindByIdProjectToAsync<TProjection>(
            int productId,
            CancellationToken cancellationToken = default)
        {
            return await FindProjectToAsync<TProjection>(x => x.ProductId == productId, cancellationToken);
        }

        public async Task<Product?> FindByIdAsync(int productId, CancellationToken cancellationToken = default)
        {
            return await FindAsync(x => x.ProductId == productId, cancellationToken);
        }

        public async Task<Product?> FindByIdAsync(
            int productId,
            Func<IQueryable<Product>, IQueryable<Product>> queryOptions,
            CancellationToken cancellationToken = default)
        {
            return await FindAsync(x => x.ProductId == productId, queryOptions, cancellationToken);
        }

        public async Task<List<Product>> FindByIdsAsync(int[] productIds, CancellationToken cancellationToken = default)
        {
            // Force materialization - Some combinations of .net9 runtime and EF runtime crash with "Convert ReadOnlySpan to List since expression trees can't handle ref struct"
            var idList = productIds.ToList();
            return await FindAllAsync(x => idList.Contains(x.ProductId), cancellationToken);
        }
    }
}