using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using TrainingModel.Tests.Domain.Common.Exceptions;
using TrainingModel.Tests.Domain.Repositories;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace TrainingModel.Tests.Application.Products.AddPriceProduct
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class AddPriceProductCommandHandler : IRequestHandler<AddPriceProductCommand>
    {
        private readonly IProductRepository _productRepository;

        [IntentManaged(Mode.Merge)]
        public AddPriceProductCommandHandler(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(AddPriceProductCommand request, CancellationToken cancellationToken)
        {
            var product = await _productRepository.FindByIdAsync(request.Id, cancellationToken);
            if (product is null)
            {
                throw new NotFoundException($"Could not find Product '{request.Id}'");
            }

            product.AddPrice(request.ActiveFrom, request.Price);
        }
    }
}