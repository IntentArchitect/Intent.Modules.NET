using Intent.RoslynWeaver.Attributes;
using MediatR;
using MudBlazor.ExampleApp.Domain.Common.Exceptions;
using MudBlazor.ExampleApp.Domain.Repositories;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace MudBlazor.ExampleApp.Application.Products.DeleteProduct
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand>
    {
        private readonly IProductRepository _productRepository;

        [IntentManaged(Mode.Merge)]
        public DeleteProductCommandHandler(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(DeleteProductCommand request, CancellationToken cancellationToken)
        {
            var product = await _productRepository.FindByIdAsync(request.Id, cancellationToken);
            if (product is null)
            {
                throw new NotFoundException($"Could not find Product '{request.Id}'");
            }

            _productRepository.Remove(product);
        }
    }
}