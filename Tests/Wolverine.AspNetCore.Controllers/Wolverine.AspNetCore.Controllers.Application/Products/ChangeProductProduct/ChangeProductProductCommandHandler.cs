using Intent.RoslynWeaver.Attributes;
using Wolverine.AspNetCore.Controllers.Domain.Common.Exceptions;
using Wolverine.AspNetCore.Controllers.Domain.Repositories;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Wolverine.CommandHandler", Version = "1.0")]

namespace Wolverine.AspNetCore.Controllers.Application.ChangeProductProduct
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class ChangeProductProductCommandHandler
    {
        private readonly IProductRepository _productRepository;

        [IntentManaged(Mode.Merge)]
        public ChangeProductProductCommandHandler(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(ChangeProductProductCommand request, CancellationToken cancellationToken)
        {
            var product = await _productRepository.FindByIdAsync(request.Id, cancellationToken);
            if (product is null)
            {
                throw new NotFoundException($"Could not find Product '{request.Id}'");
            }

            product.ChangeProduct(request.Name, request.Price);
        }
    }
}