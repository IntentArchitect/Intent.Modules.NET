### Version 6.2.6

- Fixed: Disabled TelemetryConfigurationExtension due to new CSharpInvocationStatements representable as method chaining statements. Quick fix applied in `Intent.Eventing.MassTransit` module.

### Version 6.2.5

- Improvement: Updated module NuGet packages infrastructure.

### Version 6.2.4

- Improvement: Updated NuGet packages to latest stables.

### Version 6.2.3

- Fixed: Azure Service Bus settings accepting a time span (Default time to live, Lock duration, duplicate detection time window) had an issue where the generated time was different to the specified time in some scenarios.

### Version 6.2.2

- Improvement: Added `TODO` comments on `NotImplementedException`.
- Improvement: Bumped `Intent.Eventing.Contracts` dependency so that Service Designer event package gets added when installing this module.

### Version 6.2.1

- Fixed: Updated important dependency version.

### Version 6.2.0

- Improvement: Concurrent Message Limit setting was introduced for Azure Service Bus and RabbitMQ.
- Improvement: Override the subscription endpoint names through the Consumer Settings stereotypes.

> ⚠️ **NOTE**
>
> There generated code that introduces the `AddCustomConsumerEndpoint` method in the `MassTransitConfiguration` class has been updated in case you've manually overridden for custom configuration.

### Version 6.1.3

- Fixed: EventBus flush all would not have been invoked if there wasn't a DbContext present.

### Version 6.1.2

- Fixed: Duplicate `IDistributedCacheWithUnitOfWork` would be generated into classes under certain circumstances.

### Version 6.1.1

- Improvement: Added support for `IDistributedCacheWithUnitOfWork` to unit of work implementation.

### Version 6.1.0

- Improvement: Renamed `ConsumerWrapper` to `IntegrationEventConsumer`.
- Improvement: Bumped MassTransit nuget versions from `8.0.16` to `8.1.3`.
- Improvement: `MassTransitConfigurationTemplate` has been revamped to allow for `Consumers` and `Producers` to be registered through `Decorators` to simplify config setup.

### Version 6.0.2

- Feature: Support for Sending Commands.
- Improvement: Added `ArgumentNullException` check on `IUnitOfWork` dependency injection.
- Improvement: Allow support for Redis OM Unit of Work.

### Version 6.0.0

- Feature: Added support for explicitly modeled Integration Event Handler and the Advance Mapping systems available in Intent Architect 4.1.x

### Version 5.2.4

- Fixed: Issue around Event Bus Messages not publishing when published in Domain Events, with the context of consuming Integration Events.
- Improvement: When using EF Outbox pattern with.  

### Version 5.2.4

- Improvement: `ConsumerWrapper` will now universally save changes for all of the following modules:
  - `Intent.EntityFrameworkCore`
  - `Intent.CosmosDB`
  - `Intent.Dapr.AspNetCore.StateManagement`
  - `Intent.MongoDb`
- Improvement: Integration Event Handler's constructor will now be in Intent Managed Merge mode.  

### Version 5.2.2

- Fixed: `Identifier` was incorrectly spelled as `Indentifier` in a couple of templates.

### Version 5.2.0

- New Feature: Finbuckle multi-tenancy support for MassTransit messaging.
- Improvement: Choose which default retry strategy to use from the Module settings.
- Improvement: Override the Entity Name for a Message (i.e. change the `topic` name) by applying the `Message Settings` stereotype.
- Improvement: EventBus Container registration code moved to `MassTransitConfiguration.cs` instead.

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
