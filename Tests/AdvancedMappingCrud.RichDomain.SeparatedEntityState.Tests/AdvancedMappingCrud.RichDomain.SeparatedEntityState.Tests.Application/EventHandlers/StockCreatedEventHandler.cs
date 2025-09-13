using System;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.RichDomain.SeparatedEntityState.Tests.Application.Common.Models;
using AdvancedMappingCrud.RichDomain.SeparatedEntityState.Tests.Domain.Common.Exceptions;
using AdvancedMappingCrud.RichDomain.SeparatedEntityState.Tests.Domain.Events;
using AdvancedMappingCrud.RichDomain.SeparatedEntityState.Tests.Domain.Repositories;
using AdvancedMappingCrud.RichDomain.SeparatedEntityState.Tests.Domain.Services;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.MediatR.DomainEvents.DomainEventHandler", Version = "1.0")]

namespace AdvancedMappingCrud.RichDomain.SeparatedEntityState.Tests.Application.EventHandlers
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class StockCreatedEventHandler : INotificationHandler<DomainEventNotification<StockCreatedEvent>>
    {
        private readonly IProductRepository _productRepository;
        private readonly ISecondService _secondService;

        [IntentManaged(Mode.Merge)]
        public StockCreatedEventHandler(IProductRepository productRepository, ISecondService secondService)
        {
            _productRepository = productRepository;
            _secondService = secondService;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task Handle(
            DomainEventNotification<StockCreatedEvent> notification,
            CancellationToken cancellationToken)
        {
            var domainEvent = notification.DomainEvent;

            var product = await _productRepository.FindAsync(x => x.Name == domainEvent.Name, cancellationToken);
            if (product is null)
            {
                throw new NotFoundException($"Could not find Product '{domainEvent.Name}'");
            }

            product.Another(
    _secondService);
        }
    }
}