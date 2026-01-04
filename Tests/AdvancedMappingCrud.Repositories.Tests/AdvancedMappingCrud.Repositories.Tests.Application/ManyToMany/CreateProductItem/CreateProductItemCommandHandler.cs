using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.Repositories.Tests.Domain.Entities.ManyToMany;
using AdvancedMappingCrud.Repositories.Tests.Domain.Repositories.ManyToMany;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.ManyToMany.CreateProductItem
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateProductItemCommandHandler : IRequestHandler<CreateProductItemCommand>
    {
        private readonly IProductItemRepository _productItemRepository;
        private readonly ITagRepository _tagRepository;
        private readonly ICategoryRepository _categoryRepository;

        [IntentManaged(Mode.Merge)]
        public CreateProductItemCommandHandler(IProductItemRepository productItemRepository,
            ITagRepository tagRepository,
            ICategoryRepository categoryRepository)
        {
            _productItemRepository = productItemRepository;
            _tagRepository = tagRepository;
            _categoryRepository = categoryRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(CreateProductItemCommand request, CancellationToken cancellationToken)
        {
            var existingTags = await _tagRepository.FindByIdsAsync(request.TagIds.ToArray(), cancellationToken);
            var existingCategories = await _categoryRepository.FindByIdsAsync(request.CategoryIds.ToArray(), cancellationToken);
            var productItem = new ProductItem
            {
                Name = request.Name,
                Categories = existingCategories,
                Tags = existingTags
            };

            _productItemRepository.Add(productItem);
        }
    }
}