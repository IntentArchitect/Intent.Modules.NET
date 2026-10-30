using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: DefaultIntentManaged(Mode.Merge, Signature = Mode.Fully, Attributes = Mode.Merge, Comments = Mode.Merge, Targets = Targets.Constructors | Targets.Methods)]
[assembly: IntentTemplate("Intent.Services.Guardrails.Contracts.IntegrationEventHandlerContract", Version = "1.0")]

namespace Webhooks.API.IntegrationEvents;

public class OrderStatusChangedToPaidIntegrationEventHandler(
    IWebhooksRetriever retriever,
    IWebhooksSender sender,
    ILogger<OrderStatusChangedToShippedIntegrationEventHandler> logger) : IIntegrationEventHandler<OrderStatusChangedToPaidIntegrationEvent>
{
    public async Task Handle(OrderStatusChangedToPaidIntegrationEvent @event)
    {
        var subscriptions = await retriever.GetSubscriptionsOfType(WebhookType.OrderPaid);

        logger.LogInformation("Received OrderStatusChangedToShippedIntegrationEvent and got {SubscriptionsCount} subscriptions to process", subscriptions.Count());

        var whook = new WebhookData(WebhookType.OrderPaid, @event);

        await sender.SendAll(subscriptions, whook);
    }
}
