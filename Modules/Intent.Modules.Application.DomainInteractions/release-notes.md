### Version 1.0.1

- Improvement: `CommandQueryMappingResolver` no longer assumes that Commands and Queries always have parameterized constructors and can now also detect if object initialization is needed, for example for "DTO" versions of requests for Service Proxy invocations.
- Improvement: Handlers with no auto-implementation will now generate with an `IntentManaged` Body mode of Merge and their `throw new NotImplemented()` statements have been commented with [IntentInitialGen to prevent them from returning when deleted](https://docs.intentarchitect.com/articles/application-development/code-management/code-management-csharp/code-management-csharp.html#the--intentinitialgen-instruction). (Note: A side effect of this improvement is that entries may appear in the Customizations tab of the Software Factory or existing entries may no longer be approved and they will need to reviewed).

### Version 1.0.0

- Supports domain interactions via the new interaction strategy mechanism.