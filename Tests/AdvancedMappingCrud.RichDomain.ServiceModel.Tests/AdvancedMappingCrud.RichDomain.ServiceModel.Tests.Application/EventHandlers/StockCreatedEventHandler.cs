using System;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.RichDomain.ServiceModel.Tests.Application.Common.Models;
using AdvancedMappingCrud.RichDomain.ServiceModel.Tests.Domain.Events;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.MediatR.DomainEvents.DefaultDomainEventHandler", Version = "1.0")]

namespace AdvancedMappingCrud.RichDomain.ServiceModel.Tests.Application.EventHandlers
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class StockCreatedEventHandler : INotificationHandler<DomainEventNotification<StockCreatedEvent>>
    {
        [IntentManaged(Mode.Merge)]
        public StockCreatedEventHandler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task Handle(
            DomainEventNotification<StockCreatedEvent> notification,
            CancellationToken cancellationToken)
        {
            // TODO: Implement Handle StockCreatedEventHandler) functionality
            throw new NotImplementedException("Implement your handler logic here...");
        }
    }
}