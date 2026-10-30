using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: DefaultIntentManaged(Mode.Merge, Signature = Mode.Fully, Attributes = Mode.Merge, Comments = Mode.Merge, Targets = Targets.Constructors | Targets.Methods)]
[assembly: IntentTemplate("Intent.Services.Guardrails.Contracts.IntegrationEventHandlerContract", Version = "1.0")]

namespace eShop.Ordering.API.Application.IntegrationEvents.EventHandling;

public class GracePeriodConfirmedIntegrationEventHandler(
    IMediator mediator,
    ILogger<GracePeriodConfirmedIntegrationEventHandler> logger) : IIntegrationEventHandler<GracePeriodConfirmedIntegrationEvent>
{
    /// <summary>
    /// Event handler which confirms that the grace period
    /// has been completed and order will not initially be cancelled.
    /// Therefore, the order process continues for validation. 
    /// </summary>
    /// <param name="event">       
    /// </param>
    /// <returns></returns>
    public async Task Handle(GracePeriodConfirmedIntegrationEvent @event)
    {
        logger.LogInformation("Handling integration event: {IntegrationEventId} - ({@IntegrationEvent})", @event.Id, @event);

        var command = new SetAwaitingValidationOrderStatusCommand(@event.OrderId);

        logger.LogInformation(
            "Sending command: {CommandName} - {IdProperty}: {CommandId} ({@Command})",
            command.GetGenericTypeName(),
            nameof(command.OrderNumber),
            command.OrderNumber,
            command);

        await mediator.Send(command);
    }
}
