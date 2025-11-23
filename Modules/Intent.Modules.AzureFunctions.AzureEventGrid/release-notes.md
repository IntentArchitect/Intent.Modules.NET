### Version 1.1.6

- Improvement: Updated dependencies due to changes made in `Intent.Eventing.AzureEventGrid`.

### Version 1.1.5

- Improvement: Updated NuGet package versions.
- Improvement: Updated Shared Module.

### Version 1.1.4

- Improvement: Updated Shared Module.

### Version 1.1.3

- Improvement: Added support for DynamoDB unit of work.

### Version 1.1.2

- Improvement: SQLite ambient transaction suppression.

### Version 1.1.1

- Improvement: Updated referenced packages versions
- Fixed: Issue with unit of work not being correctly injected into the constructor

### Version 1.1.0

- Improvement: Moved from `EventGridEvent` to `CloudEvent` schema as this adds the ability to add metadata via the Extension Attributes. This is the [recommended schema](https://learn.microsoft.com/en-us/azure/event-grid/event-schema) by Microsoft.

### Version 1.0.2

- Fixed: Adapted for newer way of doing Infrastructure as Code.

### Version 1.0.1

- Fixed: Wrong Service Provider was supplied to Dispatcher resulting in EventBus flushing not to take place.

### Version 1.0.0

Integration between Azure Event Grid and Azure Functions.
