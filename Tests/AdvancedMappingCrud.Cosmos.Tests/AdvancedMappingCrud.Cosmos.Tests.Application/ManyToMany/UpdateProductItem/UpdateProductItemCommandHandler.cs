using System;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.Cosmos.Tests.Domain.Common.Exceptions;
using AdvancedMappingCrud.Cosmos.Tests.Domain.Repositories.ManyToMany;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.Application.ManyToMany.UpdateProductItem
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class UpdateProductItemCommandHandler : IRequestHandler<UpdateProductItemCommand>
    {
        private readonly IProductItemRepository _productItemRepository;

        [IntentManaged(Mode.Merge)]
        public UpdateProductItemCommandHandler(IProductItemRepository productItemRepository)
        {
            _productItemRepository = productItemRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(UpdateProductItemCommand request, CancellationToken cancellationToken)
        {
            var productItem = await _productItemRepository.FindByIdAsync(request.Id, cancellationToken);
            if (productItem is null)
            {
                throw new NotFoundException($"Could not find ProductItem '{request.Id}'");
            }

            productItem.Name = request.Name;
            productItem.CategoriesIds = request.CategoryIds;
            productItem.TagsIds = request.TagIds;

            _productItemRepository.Update(productItem);
        }
    }
}