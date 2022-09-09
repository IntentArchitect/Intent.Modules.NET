### Version 3.3.0

- New: This introduces [MassTransit](https://masstransit-project.com/) as a framework for modeling Publisher/Subscriber communication between applications.
- Note: To publish messages, inject `IEventBus` into your business logic.
- Note: Subscriber business logic can be implemented in the Event Handlers located in the `Application` layer under `IntegrationEventHandlers`.