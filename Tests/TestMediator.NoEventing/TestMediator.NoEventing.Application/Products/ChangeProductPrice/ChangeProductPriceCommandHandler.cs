using Intent.RoslynWeaver.Attributes;
using Mediator;
using TestMediator.NoEventing.Domain.Common.Exceptions;
using TestMediator.NoEventing.Domain.Repositories.Products;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Mediator.CommandHandler", Version = "1.0")]

namespace TestMediator.NoEventing.Application.Products.ChangeProductPrice
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class ChangeProductPriceCommandHandler : ICommandHandler<ChangeProductPriceCommand>
    {
        private readonly IProductRepository _productRepository;

        [IntentManaged(Mode.Merge)]
        public ChangeProductPriceCommandHandler(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async ValueTask<Unit> Handle(ChangeProductPriceCommand request, CancellationToken cancellationToken)
        {
            var product = await _productRepository.FindByIdAsync(request.Id, cancellationToken)
                ?? throw new NotFoundException($"Could not find Product '{request.Id}'");

            product.ChangePrice(request.NewPrice);
            return Unit.Value;
        }
    }
}
