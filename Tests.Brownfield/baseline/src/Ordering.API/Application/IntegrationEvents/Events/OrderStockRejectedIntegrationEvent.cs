using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: DefaultIntentManaged(Mode.Merge, Signature = Mode.Fully, Attributes = Mode.Merge, Comments = Mode.Merge, Targets = Targets.Constructors | Targets.Methods)]
[assembly: IntentTemplate("Intent.Services.Guardrails.Contracts.IntegrationEventContract", Version = "1.0")]

namespace eShop.Ordering.API.Application.IntegrationEvents.Events;

public record OrderStockRejectedIntegrationEvent : IntegrationEvent
{
    public int OrderId { get; }

    public List<ConfirmedOrderStockItem> OrderStockItems { get; }

    public OrderStockRejectedIntegrationEvent(int orderId,
        List<ConfirmedOrderStockItem> orderStockItems)
    {
        OrderId = orderId;
        OrderStockItems = orderStockItems;
    }
}

public record ConfirmedOrderStockItem
{
    public int ProductId { get; }
    public bool HasStock { get; }

    public ConfirmedOrderStockItem(int productId, bool hasStock)
    {
        ProductId = productId;
        HasStock = hasStock;
    }
}
