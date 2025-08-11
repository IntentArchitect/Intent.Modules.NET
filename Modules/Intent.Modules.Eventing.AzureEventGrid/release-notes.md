### Version 1.2.1

- Fixed: EventBus didn't clear the messages buffer after being flushed.

### Version 1.2.0

- Improvement: Updated NuGet package versions.
- Improvement: Event Domains support added. ❇️
- Improvement: `AzureEventGridEventBus` is enhanced to do more efficient message publishing using `SendEventsAsync`. ❇️

  > NOTE! ⚠️
  >
  > The Publish-configuration in `AzureEventGridConfiguration.cs` has been renamed from `Add` to `AddTopic` for Custom Topic Events.

- Improvement: Handlers with no auto-implementation will now generate with an `IntentManaged` Body mode of Merge and their `throw new NotImplemented()` statements have been commented with [IntentInitialGen to prevent them from returning when deleted](https://docs.intentarchitect.com/articles/application-development/code-management/code-management-csharp/code-management-csharp.html#the--intentinitialgen-instruction). (Note: A side effect of this improvement is that entries may appear in the Customizations tab of the Software Factory or existing entries may no longer be approved and they will need to reviewed).

### Version 1.1.1

- Improvement: Updated referenced packages versions.

### Version 1.1.0

- Improvement: Moved from `EventGridEvent` to `CloudEvent` schema as this adds the ability to add metadata via the Extension Attributes. This is the [recommended schema](https://learn.microsoft.com/en-us/azure/event-grid/event-schema) by Microsoft.
- Improvement: `Subject` is no longer present on the `Publish` method, but instead you can specify a dictionary of `Extension Attributes`. You can still specify the `Subject` that way.
- Improvement: Renamed `Options` classes to be more distinct to Azure Event Grid configuration.

> NOTE! ⚠️
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
