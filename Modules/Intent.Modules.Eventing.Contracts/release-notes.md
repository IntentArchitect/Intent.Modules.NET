### Version 5.0.0

- Added support for the Advanced Mapping functionality and explicit modeling of actions available in Intent Archtiect 4.1.x.

### Version 4.0.9

- Improvement: Description Attributes can be applied to `Enum` literals through the usage of the Description Stereotype.

### Version 4.0.8

- Fixed: Enums generated for Integration purposes will only be done when the corresponding Domain Enum is not present in the application which is publishing / subscribing. This will ensure that if an application subscribes to its own Enums or other applications that share domain elements via packages won't have Integration Enums generated.

### Version 4.0.7

- Fixed: Enums generated for Integration purposes will only be done with Subscriber messages and no longer for Publisher messages.

### Version 4.0.6

- Fixed: Messages and DTOs will now prefer the generated Integration Enum above the Domain Enum for type referencing.

### Version 4.0.5

- Default collection type set to `List<T>`.
- Fixed: Messages and DTOs not automatically resolving domain Enum using clauses.

### Version 4.0.4

- Updated dependencies and supported client versions to prevent warnings when used with Intent Architect 4.x.

### Version 4.0.3

- Updated supported client version to [3.4.0-pre.0, 5.0.0).

### Version 4.0.2

- Enums will now generate comments captured in designers.

### Version 4.0.1

- Enums used on messages or their DTOs will now also be generated.

### Version 4.0.0

- New: Extracted Interfaces into separate Contracts module.