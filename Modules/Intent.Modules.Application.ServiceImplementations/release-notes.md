### Version 4.2.2

- Feature: It is now possible to specify that only contracts should be generated for a service by checking the `Contract Only` property on a Service's `Service Setting` sterotype. When checked the interface ("contract") for the service will still be generated, but no implementation and corresponding dependency injection registration.

### Version 4.2.1

- Asynchronous operations now have `CancellationToken cancellationToken = default` parameter.

### Version 4.2.0

- New: Comments on `Service`s and `Operation`s are added as XmlCodeDocs to generated classes.

### Version 4.1.4

- Updated dependencies and supported client versions to prevent warnings when used with Intent Architect 4.x.

### Version 4.1.0

- New: Implemented using the `CSharpFile` builder pattern.