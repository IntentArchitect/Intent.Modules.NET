using Intent.RoslynWeaver.Attributes;
using Wolverine.AspNetCore.Controllers.Application.Common.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Wolverine.CommandModels", Version = "1.0")]

namespace Wolverine.AspNetCore.Controllers.Application.PlaceOrder
{
    /// <summary>
    /// Confirms/places an order by invoking the Order.PlaceOrder domain operation, which raises OrderPlacedDomainEvent (handled by the default domain-event handler).
    /// </summary>
    public class PlaceOrderCommand : ICommand
    {
        public PlaceOrderCommand(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; set; }
    }
}