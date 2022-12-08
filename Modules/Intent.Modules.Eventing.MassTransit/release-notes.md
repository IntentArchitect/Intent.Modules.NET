### Version 4.0.1

- Update: Extracted Interfaces into separate Contracts module.
- Update: Updated Module version of .NET to 6.

### Version 3.3.3

- Fixed: EventBus didn't clear the messages buffer after being flushed.
- Fixed: Multiple subscribers for the same events will now be differentiated by App name.

### Version 3.3.1

- Update: Merged Outbox pattern into this module (separate Outbox pattern module is no more).

### Version 3.3.0

- New: This introduces [MassTransit](https://masstransit-project.com/) as a framework for modeling Publisher/Subscriber communication between applications.
- Note: To publish messages, inject `IEventBus` into your business logic.
- Note: Subscriber business logic can be implemented in the Event Handlers located in the `Application` layer under `IntegrationEventHandlers`.