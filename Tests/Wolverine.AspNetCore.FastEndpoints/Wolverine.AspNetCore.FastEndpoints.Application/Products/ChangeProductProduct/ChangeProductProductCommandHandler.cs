using Intent.RoslynWeaver.Attributes;
using Wolverine.AspNetCore.FastEndpoints.Domain.Common.Exceptions;
using Wolverine.AspNetCore.FastEndpoints.Domain.Repositories;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Wolverine.CommandHandler", Version = "1.0")]

namespace Wolverine.AspNetCore.FastEndpoints.Application.Products.ChangeProductProduct
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

        [IntentManaged(Mode.Merge, Signature = Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(ChangeProductProductCommand command, CancellationToken cancellationToken)
        {
            var product = await _productRepository.FindByIdAsync(command.Id, cancellationToken);
            if (product is null)
            {
                throw new NotFoundException($"Could not find Product '{command.Id}'");
            }

            product.ChangeProduct(command.Name, command.Price);
        }
    }
}