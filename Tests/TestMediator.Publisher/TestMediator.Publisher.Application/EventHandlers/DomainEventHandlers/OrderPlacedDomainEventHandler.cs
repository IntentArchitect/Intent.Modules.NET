using Intent.RoslynWeaver.Attributes;
using Mediator;
using TestMediator.Publisher.Application.Common.Models;
using TestMediator.Publisher.Application.Orders.CreateAuditEntry;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Mediator.DomainEvents.DomainEventHandler", Version = "1.0")]

namespace TestMediator.Publisher.Application.EventHandlers.DomainEventHandlers
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class OrderPlacedDomainEventHandler : INotificationHandler<OrderPlacedDomainEventNotification>
    {
        private readonly ISender _sender;

        // Test hook: fail after the nested command has saved, to show that both rows roll back together.
        public static volatile bool FailAfterAudit;

        [IntentManaged(Mode.Merge)]
        public OrderPlacedDomainEventHandler(ISender sender)
        {
            _sender = sender;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async ValueTask Handle(OrderPlacedDomainEventNotification notification, CancellationToken cancellationToken)
        {
            await _sender.Send(new CreateAuditEntryCommand(Guid.Empty, "Order placed"), cancellationToken);
            if (FailAfterAudit)
            {
                throw new InvalidOperationException("Injected failure after the nested command saved.");
            }
        }
    }
}