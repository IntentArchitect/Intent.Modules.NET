using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.RichDomain.ServiceModel.Tests.Application.Interfaces;
using AdvancedMappingCrud.RichDomain.ServiceModel.Tests.Application.Products;
using AdvancedMappingCrud.RichDomain.ServiceModel.Tests.Domain.Common.Exceptions;
using AdvancedMappingCrud.RichDomain.ServiceModel.Tests.Domain.Entities;
using AdvancedMappingCrud.RichDomain.ServiceModel.Tests.Domain.Repositories;
using AdvancedMappingCrud.RichDomain.ServiceModel.Tests.Domain.Services;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.ServiceImplementations.ServiceImplementation", Version = "1.0")]

namespace AdvancedMappingCrud.RichDomain.ServiceModel.Tests.Application.Implementation
{
    [IntentManaged(Mode.Merge)]
    public class ProductsService : IProductsService
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoriesService _categoriesService;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Merge)]
        public ProductsService(IProductRepository productRepository, ICategoriesService categoriesService, IMapper mapper)
        {
            _productRepository = productRepository;
            _categoriesService = categoriesService;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<Guid> CreateProduct(ProductCreateDto dto, CancellationToken cancellationToken = default)
        {
            var product = new Product(
                name: dto.Name,
                categoryNames: dto.CategoryNames,
                categoryService: _categoriesService);

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
        public async Task ChangeCategoriesAsync(
            Guid id,
            ChangeCategoriesAsyncDto dto,
            CancellationToken cancellationToken = default)
        {
            var product = await _productRepository.FindByIdAsync(id, cancellationToken);
            if (product is null)
            {
                throw new NotFoundException($"Could not find Product '{id}'");
            }

            await product.ChangeCategoriesAsync(dto.CategoryNames, _categoriesService, cancellationToken);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task ChangeCategoriesSync(
            Guid id,
            ChangeCategoriesSyncDto dto,
            CancellationToken cancellationToken = default)
        {
            var product = await _productRepository.FindByIdAsync(id, cancellationToken);
            if (product is null)
            {
                throw new NotFoundException($"Could not find Product '{id}'");
            }

            product.ChangeCategoriesSync(dto.CategoryNames, _categoriesService);
        }

        public void Dispose()
        {
        }
    }
}