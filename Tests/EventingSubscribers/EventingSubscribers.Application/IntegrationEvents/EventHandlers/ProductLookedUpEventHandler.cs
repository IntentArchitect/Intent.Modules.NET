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
    public class ProductLookedUpEventHandler : IIntegrationEventHandler<ProductLookedUpEvent>
    {
        private readonly IProductRepository _productRepository;

        [IntentManaged(Mode.Merge)]
        public ProductLookedUpEventHandler(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task HandleAsync(ProductLookedUpEvent message, CancellationToken cancellationToken = default)
        {
            var queryProduct = await _productRepository.FindAsync(x => x.Name == message.Name, cancellationToken);
            if (queryProduct is null)
            {
                throw new NotFoundException($"Could not find Product '{message.Name}'");
            }
        }
    }
}