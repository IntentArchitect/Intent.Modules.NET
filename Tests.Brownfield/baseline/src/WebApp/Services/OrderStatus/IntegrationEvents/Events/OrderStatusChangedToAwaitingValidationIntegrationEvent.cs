using eShop.EventBus.Events;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: DefaultIntentManaged(Mode.Merge, Signature = Mode.Fully, Attributes = Mode.Merge, Comments = Mode.Merge, Targets = Targets.Constructors | Targets.Methods)]
[assembly: IntentTemplate("Intent.Services.Guardrails.Contracts.IntegrationEventContract", Version = "1.0")]

namespace eShop.WebApp.Services.OrderStatus.IntegrationEvents;

public record OrderStatusChangedToAwaitingValidationIntegrationEvent : IntegrationEvent
{
    public int OrderId { get; }
    public string OrderStatus { get; }
    public string BuyerName { get; }
    public string BuyerIdentityGuid { get; }

    public OrderStatusChangedToAwaitingValidationIntegrationEvent(
        int orderId, string orderStatus, string buyerName, string buyerIdentityGuid)
    {
        OrderId = orderId;
        OrderStatus = orderStatus;
        BuyerName = buyerName;
        BuyerIdentityGuid = buyerIdentityGuid;
    }
}

