using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.DbContext.ProjectTo.Tests.Application.Common.Interfaces;
using AdvancedMappingCrud.DbContext.ProjectTo.Tests.Application.Interfaces.ManyToMany;
using AdvancedMappingCrud.DbContext.ProjectTo.Tests.Application.ManyToMany;
using AdvancedMappingCrud.DbContext.ProjectTo.Tests.Domain.Common;
using AdvancedMappingCrud.DbContext.ProjectTo.Tests.Domain.Common.Exceptions;
using AdvancedMappingCrud.DbContext.ProjectTo.Tests.Domain.Entities.ManyToMany;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.ServiceImplementations.ServiceImplementation", Version = "1.0")]

namespace AdvancedMappingCrud.DbContext.ProjectTo.Tests.Application.Implementation.ManyToMany
{
    [IntentManaged(Mode.Merge)]
    public class ProductItemService : IProductItemService
    {
        private readonly IApplicationDbContext _dbContext;

        [IntentManaged(Mode.Merge)]
        public ProductItemService(IApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task CreateProductItem(CreateProductItemDto dto, CancellationToken cancellationToken = default)
        {
            var existingTags = await _dbContext.Tag.Where(x => dto.TagIds.Contains(x.Id)).ToListAsync(cancellationToken);
            var existingCategories = await _dbContext.Category.Where(x => dto.CategoryIds.Contains(x.Id)).ToListAsync(cancellationToken);
            var productItem = new ProductItem
            {
                Name = dto.Name,
                Categories = existingCategories,
                Tags = existingTags
            };

            _dbContext.ProductItem.Add(productItem);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task UpdateProductItem(UpdateProductItemDto dto, Guid id, CancellationToken cancellationToken = default)
        {
            var productItem = await _dbContext.ProductItem.SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
            if (productItem is null)
            {
                throw new NotFoundException($"Could not find ProductItem '{id}'");
            }

            var existingTags = await _dbContext.Tag.Where(x => dto.TagIds.Contains(x.Id)).ToListAsync(cancellationToken);
            var existingCategories = await _dbContext.Category.Where(x => dto.CategoryIds.Contains(x.Id)).ToListAsync(cancellationToken);
            productItem.Name = dto.Name;
            productItem.Categories = UpdateHelper.SynchronizeCollection(productItem.Categories, existingCategories, (e, d) => e.Id == d.Id);
            productItem.Tags = UpdateHelper.SynchronizeCollection(productItem.Tags, existingTags, (e, d) => e.Id == d.Id);
        }
    }
}