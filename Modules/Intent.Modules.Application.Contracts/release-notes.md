### Version 5.1.0

- Improvement: Upgraded to support service invocation interactions within the new interaction strategy mechanism.

### Version 5.0.13

- Improvement: Added stereotype descriptions in preperation for Intent Architect 4.5. 

### Version 5.0.12

- Improvement: Added support for `Domain` and `Services` naming conventions for `Entities`, `Attributes` and `Operations`.

### Version 5.0.11

- Improvement: Added Model metadata to Application Contract builder elements.

### Version 5.0.10

- Improvement: Updated module icon

### Version 5.0.9

- Improvement: Removed unnecessary implementation of IDisposable 

### Version 5.0.8

- Improvement: Updated module NuGet packages infrastructure.

### Version 5.0.7

- Fixed: Due to the nature of how GetTypeName resolves namespaces there are cases where ambiguous references still exist and causes compilation errors, this fix forces to re-evaluate a lot of types in this module.

### Version 5.0.6

- Fixed: Service Operations didn't generate generic type parameters.
- Fixed: Referencing Domain Enums will now add their respective using directives.

### Version 5.0.5

- Fixed: Enums using directives will now be resolved.

### Version 5.0.4

- Improvement: Making use of more sophisticated resolution systems when generating C# code.

### Version 5.0.3

- Fixed: Nullability related compiler warnings.

### Version 5.0.2

- New Feature: Support for XmlDocComments on Operation Parameters.

### Version 5.0.1

- Improvement: Asynchronous operations now have `CancellationToken cancellationToken = default` parameter.

### Version 5.0.0

- Improvement: Upgraded Templates to use new Builder Pattern paradigm.
- Fixed: Under certain circumstances `using System.Threading.Tasks;` was not being generated.

### Version 4.0.3

- Improvement: Updated supported client version to [3.4.0-pre.0, 5.0.0-a).
