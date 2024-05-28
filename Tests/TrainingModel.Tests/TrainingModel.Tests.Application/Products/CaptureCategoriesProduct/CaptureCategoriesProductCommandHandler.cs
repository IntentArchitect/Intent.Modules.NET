using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using TrainingModel.Tests.Domain.Common.Exceptions;
using TrainingModel.Tests.Domain.Repositories;
using TrainingModel.Tests.Domain.Services;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace TrainingModel.Tests.Application.Products.CaptureCategoriesProduct
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CaptureCategoriesProductCommandHandler : IRequestHandler<CaptureCategoriesProductCommand>
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryService _categoryService;

        [IntentManaged(Mode.Merge)]
        public CaptureCategoriesProductCommandHandler(IProductRepository productRepository,
            ICategoryService categoryService)
        {
            _productRepository = productRepository;
            _categoryService = categoryService;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(CaptureCategoriesProductCommand request, CancellationToken cancellationToken)
        {
            var product = await _productRepository.FindByIdAsync(request.Id, cancellationToken);
            if (product is null)
            {
                throw new NotFoundException($"Could not find Product '{request.Id}'");
            }

            await product.CaptureCategoriesAsync(request.CategoryNames, _categoryService, cancellationToken);
        }
    }
}