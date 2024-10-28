using System;
using System.Threading;
using System.Threading.Tasks;
using FastEndpointsTest.Application.Common.Models;
using FastEndpointsTest.Domain.Events.DDD;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.MediatR.DomainEvents.AggregateManager", Version = "1.0")]

namespace FastEndpointsTest.Application.EventHandlers.DDD
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class AccountHolderManager : INotificationHandler<DomainEventNotification<AccountTransferStarted>>
    {
        public AccountHolderManager()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task Handle(DomainEventNotification<AccountTransferStarted> notification, CancellationToken cancellationToken)
        {
            // TODO: Implement Handle (AccountHolderManager) functionality
            throw new NotImplementedException("Implement your handler logic here...");
        }
    }
}