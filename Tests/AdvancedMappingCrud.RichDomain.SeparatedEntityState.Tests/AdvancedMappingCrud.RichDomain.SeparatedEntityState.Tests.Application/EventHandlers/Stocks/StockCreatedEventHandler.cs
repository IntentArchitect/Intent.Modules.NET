using System;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.RichDomain.SeparatedEntityState.Tests.Application.Common.Models;
using AdvancedMappingCrud.RichDomain.SeparatedEntityState.Tests.Domain.Common.Exceptions;
using AdvancedMappingCrud.RichDomain.SeparatedEntityState.Tests.Domain.Entities;
using AdvancedMappingCrud.RichDomain.SeparatedEntityState.Tests.Domain.Events;
using AdvancedMappingCrud.RichDomain.SeparatedEntityState.Tests.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.MediatR.DomainEvents.DomainEventHandler", Version = "1.0")]

namespace AdvancedMappingCrud.RichDomain.SeparatedEntityState.Tests.Application.EventHandlers.Stocks
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class StockCreatedEventHandler : INotificationHandler<DomainEventNotification<StockCreatedEvent>>, INotificationHandler<DomainEventNotification<StockLevelUpdatedEvent>>
    {
        private readonly IStockRepository _stockRepository;

        [IntentManaged(Mode.Merge)]
        public StockCreatedEventHandler(IStockRepository stockRepository)
        {
            _stockRepository = stockRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task Handle(
            DomainEventNotification<StockCreatedEvent> notification,
            CancellationToken cancellationToken)
        {
            var domainEvent = notification.DomainEvent;
            var stock = new Stock(
                name: domainEvent.Name,
                total: domainEvent.Total,
                addedUser: domainEvent.AddedUser);

            _stockRepository.Add(stock);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task Handle(
            DomainEventNotification<StockLevelUpdatedEvent> notification,
            CancellationToken cancellationToken)
        {
            var domainEvent = notification.DomainEvent;

            var stock = await _stockRepository.FindByIdAsync(domainEvent.Id, cancellationToken);
            if (stock is null)
            {
                throw new NotFoundException($"Could not find Stock '{domainEvent.Id}'");
            }

            stock.UpdateStockLevel(domainEvent.Id, domainEvent.Total, domainEvent.DateUpdated);
        }
    }
}