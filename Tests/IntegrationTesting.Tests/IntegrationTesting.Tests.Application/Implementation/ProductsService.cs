using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using IntegrationTesting.Tests.Application.Interfaces;
using IntegrationTesting.Tests.Application.Products;
using IntegrationTesting.Tests.Domain.Common.Exceptions;
using IntegrationTesting.Tests.Domain.Entities;
using IntegrationTesting.Tests.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.ServiceImplementations.ServiceImplementation", Version = "1.0")]

namespace IntegrationTesting.Tests.Application.Implementation
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
                Name = dto.Name
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

        public void Dispose()
        {
        }
    }
}