using System;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.RichDomain.SeparatedEntityState.Tests.Domain.Entities;
using AdvancedMappingCrud.RichDomain.SeparatedEntityState.Tests.Domain.Repositories;
using AdvancedMappingCrud.RichDomain.SeparatedEntityState.Tests.Domain.Services;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace AdvancedMappingCrud.RichDomain.SeparatedEntityState.Tests.Application.Products.CreateProduct
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Guid>
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoriesService _categoriesService;

        [IntentManaged(Mode.Merge)]
        public CreateProductCommandHandler(IProductRepository productRepository, ICategoriesService categoriesService)
        {
            _productRepository = productRepository;
            _categoriesService = categoriesService;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<Guid> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            var product = new Product(
                name: request.Name,
                categoryNames: request.CategoryNames,
                categoryService: _categoriesService);

            _productRepository.Add(product);
            await _productRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return product.Id;
        }
    }
}