### Version 4.1.0

- Now supports mapping Messages from Domain Events and will automatically implement publishing in the default Domain Event Handler.
- Fixed: Nested and mapped Eventing DTOs cause Software Factory errors as field types are set incorrectly by mapping.

### Version 4.0.3

- Updated dependencies and supported client versions to prevent warnings when used with Intent Architect 4.x.

### Version 4.0.2

- Fixed: When mapping from a collection, the `System.Linq` using directive was not added.

### Version 4.0.1

- Added mappings for Eventing DTOs.
- Fixed: Mappings would not work correctly for associations.
