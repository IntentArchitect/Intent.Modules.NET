### Version 1.1.2

- Improvement: SQL Lite ambient transaction suppression.

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