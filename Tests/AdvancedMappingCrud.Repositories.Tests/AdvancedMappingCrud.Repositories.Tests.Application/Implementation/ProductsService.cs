using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.Repositories.Tests.Application.Common.Pagination;
using AdvancedMappingCrud.Repositories.Tests.Application.Interfaces;
using AdvancedMappingCrud.Repositories.Tests.Application.Products;
using AdvancedMappingCrud.Repositories.Tests.Domain;
using AdvancedMappingCrud.Repositories.Tests.Domain.Common;
using AdvancedMappingCrud.Repositories.Tests.Domain.Common.Exceptions;
using AdvancedMappingCrud.Repositories.Tests.Domain.Entities;
using AdvancedMappingCrud.Repositories.Tests.Domain.Repositories;
using AdvancedMappingCrud.Repositories.Tests.Domain.Services;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.ServiceImplementations.ServiceImplementation", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.Implementation
{
    [IntentManaged(Mode.Merge)]
    public class ProductsService : IProductsService
    {
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;
        private readonly IPricingService _pricingService;

        [IntentManaged(Mode.Merge)]
        public ProductsService(IProductRepository productRepository, IMapper mapper, IPricingService pricingService)
        {
            _productRepository = productRepository;
            _mapper = mapper;
            _pricingService = pricingService;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<Guid> CreateProduct(ProductCreateDto dto, CancellationToken cancellationToken = default)
        {
            var product = new Product
            {
                Name = dto.Name,
                Tags = dto.Tags
                    .Select(t => new Tag(
                        name: t.Name,
                        value: t.Value))
                    .ToList()
            };

            _productRepository.Add(product);
            await _productRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return product.Id;
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
        public async Task<List<ProductDto>> FindProducts(CancellationToken cancellationToken = default)
        {
            var products = await _productRepository.FindAllAsync(cancellationToken);
            return products.MapToProductDtoList(_mapper);
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
            product.Tags = UpdateHelper.CreateOrUpdateCollection(product.Tags, dto.Tags, (e, d) => e.Equals(new Tag(
    name: d.Name,
    value: d.Value)), CreateOrUpdateTag);
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

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<decimal> GetProductPrice(
            Guid productId,
            decimal prices,
            CancellationToken cancellationToken = default)
        {
            var result = await _pricingService.GetProductPriceAsync(productId, cancellationToken);
            var sumPrice = _pricingService.SumPrices(prices);
            return result;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<Common.Pagination.PagedResult<ProductDto>> FindProductsPaged(
            int pageNo,
            int pageSize,
            string orderBy,
            CancellationToken cancellationToken = default)
        {
            var products = await _productRepository.FindAllAsync(pageNo, pageSize, queryOptions => queryOptions.OrderBy(orderBy), cancellationToken);
            return products.MapToPagedResult(x => x.MapToProductDto(_mapper));
        }

        public void Dispose()
        {
        }

        [IntentManaged(Mode.Fully)]
        private static Tag CreateOrUpdateTag(Tag? valueObject, UpdateProductTagDto dto)
        {
            if (valueObject is null)
            {
                return new Tag(
    name: dto.Name,
    value: dto.Value);
            }
            return valueObject;
        }
    }
}