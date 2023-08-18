### Version 5.2.0

- New Feature: Finbuckle multi-tenancy support for MassTransit messaging.
- Improvement: Choose which default retry strategy to use from the Module settings.

### Version 5.1.0

- Improvement: upgraded to MediatR 12.

### Version 5.0.0

- New Feature: Configure subscription based concerns for MessageBrokers with Stereotypes for both Azure Service Bus as well as RabbitMQ.
- New Feature: Message Retry configuration introduced.
- Improvement: Upgraded Templates to use new Builder Pattern paradigm.
- Improvement: Bumped MassTransit nuget package versions from `8.0.6` to `8.0.16`.
- Improvement: `In Memory` outbox is now the default setting.
- Improvement: Integration with Open Telemetry will no longer add CorrelationId's with each Message type. This is redundant and it is encouraged to use the Operation Id (found in the structured logs) instead.
- Fixed: In Memory Outbox is now properly setup so that it will perform idempotent checks.

### Version 4.0.7

- Improvement: Default Retry Intervals for Rabbit MQ were added so that it will retry to deliver a message after every 30 seconds for 10 attempts before sending the message to the error queue.

### Version 4.0.6

- Improvement: Updated dependencies and supported client versions to prevent warnings when used with Intent Architect 4.x.

### Version 4.0.4

- New Feature: Interoperability with Open Telemetry Module.
- Improvement: Event Messages will now contain Correlation Ids which may be optionally set.

### Version 4.0.1

- Improvement: Extracted Interfaces into separate Contracts module.
- Improvement: Updated Module version of .NET to 6.

### Version 3.3.3

- Fixed: EventBus didn't clear the messages buffer after being flushed.
- Fixed: Multiple subscribers for the same events will now be differentiated by App name.

### Version 3.3.1

- Improvement: Merged Outbox pattern into this module (separate Outbox pattern module is no more).

### Version 3.3.0

- New Feature: This introduces [MassTransit](https://masstransit-project.com/) as a framework for modeling Publisher/Subscriber communication between applications.
- Note: To publish messages, inject `IEventBus` into your business logic.
- Note: Subscriber business logic can be implemented in the Event Handlers located in the `Application` layer under `IntegrationEventHandlers`.