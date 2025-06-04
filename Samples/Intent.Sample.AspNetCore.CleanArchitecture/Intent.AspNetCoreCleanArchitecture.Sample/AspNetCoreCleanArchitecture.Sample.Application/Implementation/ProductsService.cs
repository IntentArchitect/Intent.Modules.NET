using AspNetCoreCleanArchitecture.Sample.Application.Common.Pagination;
using AspNetCoreCleanArchitecture.Sample.Application.Interfaces;
using AspNetCoreCleanArchitecture.Sample.Application.Products;
using AspNetCoreCleanArchitecture.Sample.Domain.Common.Exceptions;
using AspNetCoreCleanArchitecture.Sample.Domain.Entities;
using AspNetCoreCleanArchitecture.Sample.Domain.Repositories;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using static System.Linq.Dynamic.Core.DynamicQueryableExtensions;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.ServiceImplementations.ServiceImplementation", Version = "1.0")]

namespace AspNetCoreCleanArchitecture.Sample.Application.Implementation
{
    [IntentManaged(Mode.Merge)]
    public class ProductsService : IProductsService
    {
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Merge)]
        public ProductsService(IProductRepository productRepository, IMapper mapper)
        {
            _productRepository = productRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<Guid> CreateProduct(ProductCreateDto dto, CancellationToken cancellationToken = default)
        {
            var product = new Product
            {
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price,
                ImageUrl = dto.ImageUrl
            };

            _productRepository.Add(product);
            await _productRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return product.Id;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task UpdateProduct(Guid id, ProductUpdateDto dto, CancellationToken cancellationToken = default)
        {
            var product = await _productRepository.FindByIdAsync(id, cancellationToken);
            if (product is null)
            {
                throw new NotFoundException($"Could not find Product '{id}'");
            }

            product.Name = dto.Name;
            product.Description = dto.Description;
            product.Price = dto.Price;
            product.ImageUrl = dto.ImageUrl;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<ProductDto> FindProductById(Guid id, CancellationToken cancellationToken = default)
        {
            var product = await _productRepository.FindByIdAsync(id, cancellationToken);
            if (product is null)
            {
                throw new NotFoundException($"Could not find Product '{id}'");
            }
            return product.MapToProductDto(_mapper);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<PagedResult<ProductDto>> FindProducts(
            int pageNo,
            int pageSize,
            string? orderBy,
            CancellationToken cancellationToken = default)
        {
            var products = await _productRepository.FindAllAsync(pageNo, pageSize, queryOptions => queryOptions.OrderBy(orderBy ?? "Id"), cancellationToken);
            return products.MapToPagedResult(x => x.MapToProductDto(_mapper));
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task DeleteProduct(Guid id, CancellationToken cancellationToken = default)
        {
            var product = await _productRepository.FindByIdAsync(id, cancellationToken);
            if (product is null)
            {
                throw new NotFoundException($"Could not find Product '{id}'");
            }

            _productRepository.Remove(product);
        }
    }
}