using Intent.RoslynWeaver.Attributes;
using Wolverine.AspNetCore.Controllers.Domain.Events;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Wolverine.DomainEvents.DefaultDomainEventHandler", Version = "1.0")]

namespace Wolverine.AspNetCore.Controllers.Application.EventHandlers
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class OrderPlacedDomainEventHandler
    {
        [IntentManaged(Mode.Merge)]
        public OrderPlacedDomainEventHandler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task Handle(OrderPlacedDomainEvent domainEvent, CancellationToken cancellationToken)
        {
            // Order placed — no side effects needed for this test.
            await Task.CompletedTask;
        }
    }
}
