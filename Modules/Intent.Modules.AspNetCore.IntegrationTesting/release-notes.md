### Version 1.0.5

- New Feature: Added container fixture support for MongoDb.
- Improvement: DbContext registrations for Integration tests, retain all application configuration options.
- Fixed: Integration Service proxies now respect the `ApiSetting` `Serialze Enums as Strings`.

### Version 1.0.4

- Improvement: Updated the logic which converts types to query parameters to be more standard in how it coverts types to string.

### Version 1.0.3

- Improvement: Added support for deserializing ProblemDetails for client HTTP calls.

### Version 1.0.2

- Fixed: Added missing using clause in generated CRUD tests.

### Version 1.0.1

- Improvement: File transfer ( upload / download ) support on proxies.

### Version 1.0.0

- New Feature: Asp.Net Core Integration Testing module.