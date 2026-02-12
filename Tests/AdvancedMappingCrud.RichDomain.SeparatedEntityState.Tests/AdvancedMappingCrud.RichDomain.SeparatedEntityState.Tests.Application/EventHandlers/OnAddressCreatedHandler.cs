using System;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.RichDomain.SeparatedEntityState.Tests.Application.Common.Models;
using AdvancedMappingCrud.RichDomain.SeparatedEntityState.Tests.Domain.Events;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.MediatR.DomainEvents.DefaultDomainEventHandler", Version = "1.0")]

namespace AdvancedMappingCrud.RichDomain.SeparatedEntityState.Tests.Application.EventHandlers
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class OnAddressCreatedHandler : INotificationHandler<DomainEventNotification<OnAddressCreated>>
    {
        [IntentManaged(Mode.Merge)]
        public OnAddressCreatedHandler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task Handle(
            DomainEventNotification<OnAddressCreated> notification,
            CancellationToken cancellationToken)
        {
            // TODO: Implement Handle OnAddressCreatedHandler) functionality
            throw new NotImplementedException("Implement your handler logic here...");
        }
    }
}