### Version 5.0.3

- Improvement: Updated module NuGet packages infrastructure.

### Version 5.0.2

- Improvement: Updated dependency versions.

### Version 5.0.1

- Improvement: Module dependency versions updated.

### Version 5.0.0

- New Feature: Updated target handlers based on new Advanced Mapping capabilities.

### Version 4.0.13

- Fixed: In some cases modeling messages being published that are correlated to Commands that have CRUD implementations would have thrown a `Sequence contains more than one matching element`.

### Version 4.0.9

- Improvement: Updated to ensure variable name of entity to publish event for matches that in generated method implmentation by the CRUD module.

### Version 4.0.6

- Improvement: Updated dependencies and supported client versions to prevent warnings when used with Intent Architect 4.x.

### Version 4.0.4

- Fixed: `IEventBus` wasn't being injected into handler constructors.

### Version 4.0.1

- Fixed: Null reference exception thrown when trying to detect the messages to use for injecting publishers. 

### Version 4.0.0

- New Feature: Injects the Eventing Publishers into the CRUD Command Handlers.
