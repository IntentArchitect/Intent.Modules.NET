using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using TrainingModel.Tests.Domain.Common.Exceptions;
using TrainingModel.Tests.Domain.Repositories;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace TrainingModel.Tests.Application.Products.ActivateProduct
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class ActivateProductCommandHandler : IRequestHandler<ActivateProductCommand>
    {
        private readonly IProductRepository _productRepository;

        [IntentManaged(Mode.Merge)]
        public ActivateProductCommandHandler(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(ActivateProductCommand request, CancellationToken cancellationToken)
        {
            var product = await _productRepository.FindByIdAsync(request.Id, cancellationToken);
            if (product is null)
            {
                throw new NotFoundException($"Could not find Product '{request.Id}'");
            }

            product.Activate();
        }
    }
}