### Version 1.0.1

- Improvement: `CommandQueryMappingResolver` no longer assumes that Commands and Queries always have parameterized constructors and can now also detect if object initialization is needed, for example for "DTO" versions of requests for Service Proxy invocations.

### Version 1.0.0

- Supports domain interactions via the new interaction strategy mechanism.