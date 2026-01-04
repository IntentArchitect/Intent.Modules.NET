using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.Repositories.Tests.Domain.Common;
using AdvancedMappingCrud.Repositories.Tests.Domain.Common.Exceptions;
using AdvancedMappingCrud.Repositories.Tests.Domain.Repositories.ManyToMany;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.ManyToMany.UpdateProductItem
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class UpdateProductItemCommandHandler : IRequestHandler<UpdateProductItemCommand>
    {
        private readonly IProductItemRepository _productItemRepository;
        private readonly ITagRepository _tagRepository;
        private readonly ICategoryRepository _categoryRepository;

        [IntentManaged(Mode.Merge)]
        public UpdateProductItemCommandHandler(IProductItemRepository productItemRepository,
            ITagRepository tagRepository,
            ICategoryRepository categoryRepository)
        {
            _productItemRepository = productItemRepository;
            _tagRepository = tagRepository;
            _categoryRepository = categoryRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(UpdateProductItemCommand request, CancellationToken cancellationToken)
        {
            var productItem = await _productItemRepository.FindByIdAsync(request.Id, cancellationToken);
            if (productItem is null)
            {
                throw new NotFoundException($"Could not find ProductItem '{request.Id}'");
            }

            var existingTags = await _tagRepository.FindByIdsAsync(request.TagIds.ToArray(), cancellationToken);
            var existingCategories = await _categoryRepository.FindByIdsAsync(request.CategoryIds.ToArray(), cancellationToken);
            productItem.Name = request.Name;
            productItem.Categories = UpdateHelper.SynchronizeCollection(productItem.Categories, existingCategories, (e, d) => e.Id == d.Id);
            productItem.Tags = UpdateHelper.SynchronizeCollection(productItem.Tags, existingTags, (e, d) => e.Id == d.Id);
        }
    }
}