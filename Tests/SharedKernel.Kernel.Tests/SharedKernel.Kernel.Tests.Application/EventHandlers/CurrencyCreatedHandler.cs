using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using SharedKernel.Kernel.Tests.Application.Common.Models;
using SharedKernel.Kernel.Tests.Domain.Events;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.MediatR.DomainEvents.DomainEventHandler", Version = "1.0")]

namespace SharedKernel.Kernel.Tests.Application.EventHandlers
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CurrencyCreatedHandler : INotificationHandler<DomainEventNotification<CurrencyCreated>>
    {
        [IntentManaged(Mode.Merge)]
        public CurrencyCreatedHandler()
        {
        }

        [IntentIgnore]
        public Task Handle(
            DomainEventNotification<CurrencyCreated> notification,
            CancellationToken cancellationToken)
        {
            notification.DomainEvent.Currency.Description = "KernelEventSet";
            return Task.CompletedTask;
        }
    }
}