### Version 4.0.9

- Updated to ensure variable name of entity to publish event for matches that in generated method implmentation by the CRUD module.

### Version 4.0.6

- Updated dependencies and supported client versions to prevent warnings when used with Intent Architect 4.x.

### Version 4.0.4

- Fixed: `IEventBus` wasn't being injected into handler constructors.

### Version 4.0.1

- Fixed: Null reference exception thrown when trying to detect the messages to use for injecting publishers. 

### Version 4.0.0

- New: Injects the Eventing Publishers into the CRUD Command Handlers.