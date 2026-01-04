using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.Cosmos.Tests.Application.Interfaces.ManyToMany;
using AdvancedMappingCrud.Cosmos.Tests.Application.ManyToMany;
using AdvancedMappingCrud.Cosmos.Tests.Domain.Common.Exceptions;
using AdvancedMappingCrud.Cosmos.Tests.Domain.Entities.ManyToMany;
using AdvancedMappingCrud.Cosmos.Tests.Domain.Repositories.ManyToMany;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.ServiceImplementations.ServiceImplementation", Version = "1.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.Application.Implementation.ManyToMany
{
    [IntentManaged(Mode.Merge)]
    public class ProductItemService : IProductItemService
    {
        private readonly IProductItemRepository _productItemRepository;

        [IntentManaged(Mode.Merge)]
        public ProductItemService(IProductItemRepository productItemRepository)
        {
            _productItemRepository = productItemRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task CreateProductItem(CreateProductItemDto dto, CancellationToken cancellationToken = default)
        {
            var productItem = new ProductItem
            {
                Name = dto.Name,
                CategoriesIds = dto.CategoryIds,
                TagsIds = dto.TagIds
            };

            _productItemRepository.Add(productItem);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task UpdateProductItem(UpdateProductItemDto dto, Guid id, CancellationToken cancellationToken = default)
        {
            var productItem = await _productItemRepository.FindByIdAsync(id, cancellationToken);
            if (productItem is null)
            {
                throw new NotFoundException($"Could not find ProductItem '{id}'");
            }

            productItem.Name = dto.Name;
            productItem.CategoriesIds = dto.CategoryIds;
            productItem.TagsIds = dto.TagIds;

            _productItemRepository.Update(productItem);
        }
    }
}