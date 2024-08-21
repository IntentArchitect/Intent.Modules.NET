### Version 4.5.7

- Improvement: Updated module NuGet packages infrastructure.

### Version 4.5.6

- Improvement: Added `TODO` comments on `NotImplementedException`.

### Version 4.5.5

- Fixed: Due to the nature of how GetTypeName resolves namespaces there are cases where ambiguous references still exist and causes compilation errors, this fix forces to re-evaluate a lot of types in this module.

### Version 4.5.4

- Improvement: Application Client Dto type using directives also to be resolved now in Service implementations.

### Version 4.5.3

- Fixed: Service Operations didn't generate generic type parameters.
- Fixed: Referencing Domain Enums will now add their respective using directives.

### Version 4.5.2

- Fixed: Enums using directives will now be resolved.

### Version 4.5.1

- Improvement: Upgraded module to support new 4.1 SDK features.

### Version 4.4.0

- Update: Constructors will now be "Merge" by default. This will make user based injections and code-gen injections feasible.

### Version 4.3.1

- New: Support for XmlDocComments on Operation Parameters.

### Version 4.2.2

- Feature: It is now possible to specify that only contracts should be generated for a service by applying the `Contract Only` Stereotype to a Service. When applied, the interface ("contract") for the service will still be generated, but no implementation and corresponding dependency injection registration.

### Version 4.2.1

- Asynchronous operations now have `CancellationToken cancellationToken = default` parameter.

### Version 4.2.0

- New: Comments on `Service`s and `Operation`s are added as XmlCodeDocs to generated classes.

### Version 4.1.4

- Updated dependencies and supported client versions to prevent warnings when used with Intent Architect 4.x.

### Version 4.1.0

- New: Implemented using the `CSharpFile` builder pattern.
