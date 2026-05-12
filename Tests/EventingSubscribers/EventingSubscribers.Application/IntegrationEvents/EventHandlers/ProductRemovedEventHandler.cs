using EventingSubscribers.Application.Common.Eventing;
using EventingSubscribers.Domain.Common.Exceptions;
using EventingSubscribers.Domain.Repositories;
using EventingSubscribers.Eventing.Messages;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Contracts.IntegrationEventHandler", Version = "1.0")]

namespace EventingSubscribers.Application.IntegrationEvents.EventHandlers
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class ProductRemovedEventHandler : IIntegrationEventHandler<ProductRemovedEvent>
    {
        private readonly IProductRepository _productRepository;

        [IntentManaged(Mode.Merge)]
        public ProductRemovedEventHandler(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task HandleAsync(ProductRemovedEvent message, CancellationToken cancellationToken = default)
        {
            var deleteProduct = await _productRepository.FindByIdAsync(message.ProductId, cancellationToken);
            if (deleteProduct is null)
            {
                throw new NotFoundException($"Could not find Product '{message.ProductId}'");
            }


            _productRepository.Remove(deleteProduct);
        }
    }
}