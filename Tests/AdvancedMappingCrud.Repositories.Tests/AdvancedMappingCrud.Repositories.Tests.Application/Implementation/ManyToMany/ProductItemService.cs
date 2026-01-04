using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.Repositories.Tests.Application.Interfaces.ManyToMany;
using AdvancedMappingCrud.Repositories.Tests.Application.ManyToMany;
using AdvancedMappingCrud.Repositories.Tests.Domain.Common;
using AdvancedMappingCrud.Repositories.Tests.Domain.Common.Exceptions;
using AdvancedMappingCrud.Repositories.Tests.Domain.Entities.ManyToMany;
using AdvancedMappingCrud.Repositories.Tests.Domain.Repositories.ManyToMany;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.ServiceImplementations.ServiceImplementation", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.Implementation.ManyToMany
{
    [IntentManaged(Mode.Merge)]
    public class ProductItemService : IProductItemService
    {
        private readonly IProductItemRepository _productItemRepository;
        private readonly ITagRepository _tagRepository;
        private readonly ICategoryRepository _categoryRepository;

        [IntentManaged(Mode.Merge)]
        public ProductItemService(IProductItemRepository productItemRepository,
            ITagRepository tagRepository,
            ICategoryRepository categoryRepository)
        {
            _productItemRepository = productItemRepository;
            _tagRepository = tagRepository;
            _categoryRepository = categoryRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task CreateProductItem(CreateProductItemDto dto, CancellationToken cancellationToken = default)
        {
            var existingTags = await _tagRepository.FindByIdsAsync(dto.TagIds.ToArray(), cancellationToken);
            var existingCategories = await _categoryRepository.FindByIdsAsync(dto.CategoryIds.ToArray(), cancellationToken);
            var productItem = new ProductItem
            {
                Name = dto.Name,
                Categories = existingCategories,
                Tags = existingTags
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

            var existingTags = await _tagRepository.FindByIdsAsync(dto.TagIds.ToArray(), cancellationToken);
            var existingCategories = await _categoryRepository.FindByIdsAsync(dto.CategoryIds.ToArray(), cancellationToken);
            productItem.Name = dto.Name;
            productItem.Categories = UpdateHelper.SynchronizeCollection(productItem.Categories, existingCategories, (e, d) => e.Id == d.Id);
            productItem.Tags = UpdateHelper.SynchronizeCollection(productItem.Tags, existingTags, (e, d) => e.Id == d.Id);
        }
    }
}