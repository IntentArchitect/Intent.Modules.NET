using System;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.RichDomain.SeparatedEntityState.Tests.Domain.Common.Exceptions;
using AdvancedMappingCrud.RichDomain.SeparatedEntityState.Tests.Domain.Repositories;
using AdvancedMappingCrud.RichDomain.SeparatedEntityState.Tests.Domain.Services;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace AdvancedMappingCrud.RichDomain.SeparatedEntityState.Tests.Application.Products.ChangeCategoriesSyncProduct
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class ChangeCategoriesSyncProductCommandHandler : IRequestHandler<ChangeCategoriesSyncProductCommand>
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoriesService _categoriesService;

        [IntentManaged(Mode.Merge)]
        public ChangeCategoriesSyncProductCommandHandler(IProductRepository productRepository,
            ICategoriesService categoriesService)
        {
            _productRepository = productRepository;
            _categoriesService = categoriesService;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(ChangeCategoriesSyncProductCommand request, CancellationToken cancellationToken)
        {
            var product = await _productRepository.FindByIdAsync(request.Id, cancellationToken);
            if (product is null)
            {
                throw new NotFoundException($"Could not find Product '{request.Id}'");
            }

            product.ChangeCategoriesSync(request.CategoryNames, _categoriesService);
        }
    }
}