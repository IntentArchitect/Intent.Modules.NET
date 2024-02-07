# Intent.Eventing.MassTransit.Scheduling

This module extends the `Intent.Eventing.MassTransit` module, by adding the ability schedule messages for delivery.

For more info on MassTransit scheduling, check out their [documentation](https://masstransit.io/documentation/configuration/scheduling).

> **NOTE**
> Some Message Brokers (like RabbitMQ) requires plugins to be installed or configured for scheduled messages to work. Please see the [documentation](https://masstransit.io/documentation/configuration/scheduling) for more info.

## What's in this module?

This module enhances the `Intent.Eventing.MassTransit` module in the following ways:

* Extends the `IEventBus` interface to have scheduling methods.
* Configures scheduling infrastructure.
* Extends the `MassTransitEventBus` class to implement scheduling methods.

### Extends the `IEventBus` interface to have scheduling methods

```csharp
    public interface IEventBus
    {
        ...
        void SchedulePublish<T>(T message, DateTime scheduled)
            where T : class;
        void SchedulePublish<T>(T message, TimeSpan delay)
            where T : class;
    }
```
