### Version 1.1.4

- Improvement: Added support for DynamoDB unit of work.
- Fixed: EventBus didn't clear the messages buffer after being flushed.

### Version 1.1.3

- Improvement: SQL Lite ambient transaction suppression.

### Version 1.1.2

- Improvement: Handlers with no auto-implementation will now generate with an `IntentManaged` Body mode of Merge and their `throw new NotImplemented()` statements have been commented with [IntentInitialGen to prevent them from returning when deleted](https://docs.intentarchitect.com/articles/application-development/code-management/code-management-csharp/code-management-csharp.html#the--intentinitialgen-instruction). (Note: A side effect of this improvement is that entries may appear in the Customizations tab of the Software Factory or existing entries may no longer be approved and they will need to reviewed).

### Version 1.1.1

- Improvement: Updated NuGet package versions.
- Fixed: Issue with unit of work not being correctly injected into the constructor

### Version 1.1.0

- Improvement: Hosted Service introduced for .NET Hosts to receive inbound messages from Azure Service Bus.
- Improvement: Renamed `Options` classes to be more distinct to Azure Service Bus configuration.

### Version 1.0.4

- Improvement: Added stereotype descriptions in preperation for Intent Architect 4.5. 

### Version 1.0.3

- Fixed: Wrong Service Provider was supplied to Dispatcher resulting in EventBus flushing not to take place.

### Version 1.0.2

- Improvement: Updated NuGet package versions.
- Improvement: Added documentation.

### Version 1.0.1

- Fixed: Did not emit the `AzureServiceBus:ConnectionString` configuration section.

### Version 1.0.0

- New Feature: Model Queues and Topics in Intent Architect to generate code that directly integrates with Azure Service Bus.
