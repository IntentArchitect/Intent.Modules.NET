using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using SecurityConfig.Tests.Application.Interfaces;
using SecurityConfig.Tests.Application.Products;
using SecurityConfig.Tests.Domain.Common.Exceptions;
using SecurityConfig.Tests.Domain.Entities;
using SecurityConfig.Tests.Domain.Repositories;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.ServiceImplementations.ServiceImplementation", Version = "1.0")]

namespace SecurityConfig.Tests.Application.Implementation
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
                Surname = dto.Surname
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
            product.Surname = dto.Surname;
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