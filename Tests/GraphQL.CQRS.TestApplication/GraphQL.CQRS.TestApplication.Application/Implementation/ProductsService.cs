using System;
using System.Collections.Generic;
using System.Linq;
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
        public async Task<Guid> CreateProduct(ProductCreateDto dto)
        {
            var newProduct = new Product
            {
                Name = dto.Name,
                Description = dto.Description,
                IsActive = dto.IsActive,
            };
            _productRepository.Add(newProduct);
            await _productRepository.UnitOfWork.SaveChangesAsync();
            return newProduct.Id;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<ProductDto> FindProductById(Guid id)
        {
            var element = await _productRepository.FindByIdAsync(id);
            return element.MapToProductDto(_mapper);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<List<ProductDto>> FindProducts()
        {
            var elements = await _productRepository.FindAllAsync();
            return elements.MapToProductDtoList(_mapper);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task UpdateProduct(Guid id, ProductUpdateDto dto)
        {
            var existingProduct = await _productRepository.FindByIdAsync(id);
            existingProduct.Name = dto.Name;
            existingProduct.Description = dto.Description;
            existingProduct.IsActive = dto.IsActive;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task DeleteProduct(Guid id)
        {
            var existingProduct = await _productRepository.FindByIdAsync(id);
            _productRepository.Remove(existingProduct);
        }

        public void Dispose()
        {
        }
    }
}