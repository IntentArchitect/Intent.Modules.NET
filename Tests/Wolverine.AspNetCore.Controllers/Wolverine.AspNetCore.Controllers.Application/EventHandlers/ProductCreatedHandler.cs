using Intent.RoslynWeaver.Attributes;
using Wolverine.AspNetCore.Controllers.Domain.Events;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Wolverine.DomainEvents.DefaultDomainEventHandler", Version = "1.0")]

namespace Wolverine.AspNetCore.Controllers.Application.EventHandlers
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class ProductCreatedHandler
    {
        [IntentManaged(Mode.Merge)]
        public ProductCreatedHandler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task Handle(ProductCreated domainEvent, CancellationToken cancellationToken)
        {
            // TODO: Implement Handle (ProductCreatedHandler) functionality
            throw new NotImplementedException("Implement your handler logic here...");
        }
    }
}