# Intent.Eventing.MassTransit.Scheduling

This module extends the `Intent.Eventing.MassTransit` module by adding the ability to schedule messages for delivery at a specific time or after a delay.

For more info on MassTransit scheduling, check out their [documentation](https://masstransit.io/documentation/configuration/scheduling).

> [!NOTE]
> Some Message Brokers (like RabbitMQ) require plugins to be installed or configured for scheduled messages to work. Please see the [MassTransit documentation](https://masstransit.io/documentation/configuration/scheduling) for more info on your specific broker.

## What's in this module?

This module enhances the `Intent.Eventing.MassTransit` module in the following ways:

* Extends the `IMessageBus` interface with scheduling methods
* Configures scheduling infrastructure for your selected message broker
* Implements scheduling methods in the `MassTransitEventBus` class

## Scheduling Methods

This module adds two scheduling methods to the `IMessageBus` interface:

```csharp
public interface IMessageBus
{
    // ... existing methods ...
    
    /// <summary>
    /// Schedules a message to be published at a specific UTC time.
    /// </summary>
    /// <typeparam name="TMessage">The message type.</typeparam>
    /// <param name="message">The message instance to schedule.</param>
    /// <param name="scheduled">The UTC date/time when the message should be dispatched.</param>
    void SchedulePublish<TMessage>(TMessage message, DateTime scheduled) where TMessage : class;
    
    /// <summary>
    /// Schedules a message to be published after a delay relative to the current UTC time.
    /// </summary>
    /// <typeparam name="TMessage">The message type.</typeparam>
    /// <param name="message">The message instance to schedule.</param>
    /// <param name="delay">The time span to add to the current UTC time for scheduling.</param>
    void SchedulePublish<TMessage>(TMessage message, TimeSpan delay) where TMessage : class;
}
```

### Usage Example

```csharp
public class OrderService
{
    private readonly IMessageBus _messageBus;

    public OrderService(IMessageBus messageBus)
    {
        _messageBus = messageBus;
    }

    public async Task ScheduleOrderReminder(Order order)
    {
        // Schedule an order reminder to be sent 7 days from now
        _messageBus.SchedulePublish(
            new OrderReminderEvent { OrderId = order.Id },
            TimeSpan.FromDays(7));
        
        await _messageBus.FlushAllAsync();
    }

    public async Task ScheduleExpiration(Order order)
    {
        // Schedule an expiration check at a specific time
        var expirationTime = DateTime.UtcNow.AddHours(24);
        _messageBus.SchedulePublish(
            new OrderExpirationEvent { OrderId = order.Id },
            expirationTime);
        
        await _messageBus.FlushAllAsync();
    }
}
```

## Multi-Provider Scheduling

When this module is installed alongside other message bus providers, the scheduling methods operate across **all** installed providers that have been designated to handle the message type.

For example, if you have both MassTransit and Kafka configured:
- Messages designated for MassTransit will be scheduled using MassTransit's scheduling capabilities
- Messages designated for Kafka will be handled by Kafka's implementation
- Messages can be scheduled for multiple providers simultaneously if they're marked for both

The scheduling methods ensure consistent behavior across different message bus implementations, allowing you to use the same code regardless of which provider(s) are handling your messages.

For more details on the multi-provider architecture and message designation, see the [Intent.Eventing.Contracts documentation](https://docs.intentarchitect.com/articles/modules-dotnet/intent-eventing-contracts/intent-eventing-contracts.html).
