using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using GraphQL.CQRS.TestApplication.Application.Interfaces;
using GraphQL.CQRS.TestApplication.Application.Products;
using GraphQL.CQRS.TestApplication.Domain.Entities;
using GraphQL.CQRS.TestApplication.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.ServiceImplementations.ServiceImplementation", Version = "1.0")]

namespace GraphQL.CQRS.TestApplication.Application.Implementation
{
    [IntentManaged(Mode.Merge)]
    public class ProductsService : IProductsService
    {
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public ProductsService(IProductRepository productRepository, IMapper mapper)
        {
            _productRepository = productRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<ProductDto> CreateProduct(ProductCreateDto dto, CancellationToken cancellationToken = default)
        {
            var newProduct = new Product
            {
                Name = dto.Name,
                Description = dto.Description,
                IsActive = dto.IsActive,
            };
            _productRepository.Add(newProduct);
            await _productRepository.UnitOfWork.SaveChangesAsync();
            return newProduct.MapToProductDto(_mapper);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<ProductDto> FindProductById(Guid id, CancellationToken cancellationToken = default)
        {
            var element = await _productRepository.FindByIdAsync(id);
            return element.MapToProductDto(_mapper);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<List<ProductDto>> FindProducts(CancellationToken cancellationToken = default)
        {
            var elements = await _productRepository.FindAllAsync();
            return elements.MapToProductDtoList(_mapper);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<ProductDto> UpdateProduct(
            Guid id,
            ProductUpdateDto dto,
            CancellationToken cancellationToken = default)
        {
            var existingProduct = await _productRepository.FindByIdAsync(id);
            existingProduct.Name = dto.Name;
            existingProduct.Description = dto.Description;
            existingProduct.IsActive = dto.IsActive;
            await _productRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return existingProduct.MapToProductDto(_mapper);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<ProductDto> DeleteProduct(Guid id, CancellationToken cancellationToken = default)
        {
            var existingProduct = await _productRepository.FindByIdAsync(id);
            _productRepository.Remove(existingProduct);
            return existingProduct.MapToProductDto(_mapper);
        }

        public void Dispose()
        {
        }
    }
}