### Version 1.1.1

- Improvement: Updated referenced packages versions

### Version 1.1.0

- Improvement: Moved from `EventGridEvent` to `CloudEvent` schema as this adds the ability to add metadata via the Extension Attributes. This is the [recommended schema](https://learn.microsoft.com/en-us/azure/event-grid/event-schema) by Microsoft.
- Improvement: `Subject` is no longer present on the `Publish` method, but instead you can specify a dictionary of `Extension Attributes`. You can still specify the `Subject` that way.
- Improvement: Renamed `Options` classes to be more distinct to Azure Event Grid configuration.

> [!NOTE]
> 
> Ensure that your Event Grid Topics are set to use `CloudEvent` schema (v1.0).

### Version 1.0.3

- Improvement: Added stereotype descriptions in preparation for Intent Architect 4.5. 
- Improvement: Updated NuGet package versions.

### Version 1.0.2

- Fixed: Wrong Service Provider was supplied to Dispatcher resulting in EventBus flushing not to take place.

### Version 1.0.1

- Improvement: Added documentation.

### Version 1.0.0

- New Feature: Model Topics in Intent Architect to generate code that directly integrates with Azure Event Grid.
