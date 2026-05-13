using EventingSubscribers.Application.Common.Eventing;
using EventingSubscribers.Domain.Entities;
using EventingSubscribers.Domain.Repositories;
using EventingSubscribers.Eventing.Messages;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Contracts.IntegrationEventHandler", Version = "1.0")]

namespace EventingSubscribers.Application.IntegrationEvents.EventHandlers
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class ProductCreatedEventHandler : IIntegrationEventHandler<ProductCreatedEvent>
    {
        private readonly IProductRepository _productRepository;

        [IntentManaged(Mode.Merge)]
        public ProductCreatedEventHandler(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task HandleAsync(ProductCreatedEvent message, CancellationToken cancellationToken = default)
        {
            var product = new Product
            {
                Name = message.Name
            };

            _productRepository.Add(product);
        }
    }
}