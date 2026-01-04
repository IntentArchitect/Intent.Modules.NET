using System;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.Cosmos.Tests.Domain.Entities.ManyToMany;
using AdvancedMappingCrud.Cosmos.Tests.Domain.Repositories.ManyToMany;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.Application.ManyToMany.CreateProductItem
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateProductItemCommandHandler : IRequestHandler<CreateProductItemCommand>
    {
        private readonly IProductItemRepository _productItemRepository;

        [IntentManaged(Mode.Merge)]
        public CreateProductItemCommandHandler(IProductItemRepository productItemRepository)
        {
            _productItemRepository = productItemRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(CreateProductItemCommand request, CancellationToken cancellationToken)
        {
            var productItem = new ProductItem
            {
                Name = request.Name,
                CategoriesIds = request.CategoryIds,
                TagsIds = request.TagIds
            };

            _productItemRepository.Add(productItem);
        }
    }
}