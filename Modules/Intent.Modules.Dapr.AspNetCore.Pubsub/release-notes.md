### Version 3.0.9

- Improvement: EventBus now implements `IMessageBus` and supports composite message bus architecture for multi-provider scenarios.

### Version 3.0.8

- Improvement: Updated Shared Module.

### Version 3.0.7

- Fixed: A software factory exception would occur when references were used in the Visual Studio designer.

### Version 3.0.6

- Improvement: Added support for DynamoDB unit of work.

### Version 3.0.5

- Improvement: SQLite ambient transaction suppression.

### Version 3.0.4

- Improvement: Handlers with no auto-implementation will now generate with an `IntentManaged` Body mode of Merge and their `throw new NotImplemented()` statements have been commented with [IntentInitialGen to prevent them from returning when deleted](https://docs.intentarchitect.com/articles/application-development/code-management/code-management-csharp/code-management-csharp.html#the--intentinitialgen-instruction). (Note: A side effect of this improvement is that entries may appear in the Customizations tab of the Software Factory or existing entries may no longer be approved and they will need to reviewed).

### Version 3.0.3

- Fixed: Issue with unit of work not being correctly injected into the constructor

### Version 3.0.2

- Improvement: Added stereotype descriptions in preperation for Intent Architect 4.5. 

### Version 3.0.1

- Fixed: The Dapr controllers for receiving PubSub events did not perform saving persistence changes or flush event bus events.

### Version 3.0.0

- Updated dependency version of the `Intent.Modelers.Eventing` module from 5.0.2 to 6.0.1.

  > ⚠️ **NOTE**
  >
  > This major version change of `Intent.Modelers.Eventing` is a possibly breaking change, please refer to [its release notes](https://github.com/IntentArchitect/Intent.Modules/blob/master/Modules/Intent.Modules.Modelers.Eventing/release-notes.md#version-600) for additional details.

- Improvement: TopicName will now contain the full `Message Event` name (namespace and class name).

  > ⚠️ **NOTE**
  >
  > The changing of the `Topic Name` is potentially a breaking change for existing applications. When updating to this version, existing messages will me migrated to have the `Dapr Settings -> Topic Name` set to the existing topic name. Any new messages created will default to the full name.

### Version 2.2.2

- Fixed: `DaprEventHandlerController` would not be generated when there was no Event Designer installed.

### Version 2.2.1

- Improvement: Updated NuGet package versions.

### Version 2.2.0

- Improvement: Updated Event subscriptions which are modeled in the Services Designer to implement the subscription using the `IIntegrationEventHandler` similar to our other Eventing models.
    If you had any Event subscriptions modeled in the Services Designer prior to this version you simply need to move the implementation logic from the `IReqyuestHandler` implementation to the `IIntegrationEventHandler` implementation.

### Version 2.1.4

- Improvement: Updated module icon.

### Version 2.1.3

- Fixed: Subscribing to Events can now be modeled in the Services designer. 

### Version 2.1.2

- Improvement: Updated module NuGet packages infrastructure.

### Version 2.1.1

- Improvement: Updated NuGet packages to latest stables.

### Version 2.1.0

- Fixed: Subscribing to Message events through the Advanced mapping system is fixed.

### Version 2.0.5

- Fixed: `UseCloudEvents` will no longer be generated in the wrong location when using Minimal Hosting Model.

### Version 2.0.4

- Improvement: Updated Interoperable dependency versions.

### Version 2.0.3

- Improvement: Upgraded module to support new 4.1 SDK features.

### Version 2.0.0

- Improvement: Updated to support use of top-level statements and minimal hosting model improvements introduced in [`Intent.AspNetCore` version 6.0.0](https://github.com/IntentArchitect/Intent.Modules.NET/blob/development/Modules/Intent.Modules.AspNetCore/release-notes.md#version-600).

### Version 1.1.1

- Fixed: Nullability related compiler warnings.

### Version 1.1.0

- Improvement : Upgraded to MediatR 12.

### Version 1.0.5

- Fixed: Software Factory executions on case-sensitive file systems would fail due to file not found errors.
- Fixed: Under certain circumstances the `EventHandlerController` file would not be generated.
- Fixed: `EventBusImplementation` was incorrectly being generated into the `Application` project instead of the `Infrastructure` project.

### Version 1.0.4

- Improvement: Updated dependencies and supported client versions to prevent warnings when used with Intent Architect 4.x.

### Version 1.0.2

- Fixed: Wasn't automatically installing `Intent.Application.MediatR.CRUD.Eventing` and `Intent.Application.ServiceImplementations.CRUD.Eventing` when appropriate.
- Fixed: Updated dependencies.

### Version 1.0.1

- Fixed: Event handlers would be generated into a sub-folder with the same name as the message which depending on the message name would cause conflicts between the fully qualified message type and the event handler's namespace.
- Fixed: `IntentManaged` for the `Body` of event handlers classes was set to `Fully` instead of `Merge`.
- Fixed: Event's were being skipped over by `IUnitOfWork` implementations due to not implementing `ICommand`.
