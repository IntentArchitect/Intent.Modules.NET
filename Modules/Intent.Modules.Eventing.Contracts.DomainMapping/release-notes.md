### Version 4.1.6

- Improvement: Updated module NuGet packages infrastructure.

### Version 4.1.5

- New Feature: Added support for `Integration Command` domain mapping.


### Version 4.1.4

- Fixed: Mapping from Value Objects is now supported.

### Version 4.1.3

- Improvement : Upgraded CRUD Scripts to be forward compatible with Intent Architect v4.1.

### Version 4.1.2

- Fixed: DomainEvent to IntegrationEvent mapping are now being populated in the mappings.

### Version 4.1.1

- Fixed: Messages and DTOs will now prefer the generated Integration Enum above the Domain Enum for type referencing and will perform Enum casting where necessary.

### Version 4.1.0

- Improvement : Now supports mapping Messages from Domain Events and will automatically implement publishing in the default Domain Event Handler.
- Fixed: Nested and mapped Eventing DTOs cause Software Factory errors as field types are set incorrectly by mapping.

### Version 4.0.3

- Improvement : Updated dependencies and supported client versions to prevent warnings when used with Intent Architect 4.x.

### Version 4.0.2

- Fixed: When mapping from a collection, the `System.Linq` using directive was not added.

### Version 4.0.1

- Added mappings for Eventing DTOs.
- Fixed: Mappings would not work correctly for associations.
