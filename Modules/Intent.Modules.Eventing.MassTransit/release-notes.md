### Version 5.0.0

- New: Configure subscription based concerns for MessageBrokers with Stereotypes for both Azure Service Bus as well as RabbitMQ.
- New: Message Retry configuration introduced.
- New: Upgraded Templates to use new Builder Pattern paradigm.
- Update: Bumped MassTransit nuget package versions from `8.0.6` to `8.0.16`.

### Version 4.0.7

- Default Retry Intervals for Rabbit MQ were added so that it will retry to deliver a message after every 30 seconds for 10 attempts before sending the message to the error queue.

### Version 4.0.6

- Updated dependencies and supported client versions to prevent warnings when used with Intent Architect 4.x.

### Version 4.0.4

- New: Interoperability with Open Telemetry Module.
- Update: Event Messages will now contain Correlation Ids which may be optionally set.

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