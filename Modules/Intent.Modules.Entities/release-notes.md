### Version 5.1.4

- Fixed: Domain Behaviours not discoverable by a specific role.

### Version 5.1.3

- Improvement: Updated module NuGet packages infrastructure.

### Version 5.1.2

- Improvement: Added `TODO` comments on `NotImplementedException`.

### Version 5.1.1

- Improvement: Updated `Entity` template metadata that the c# class represents the model, so it can be inferred by the Advanced mapping system.

### Version 5.1.0

- Improvement: `DataContractTemplate` update to include `RepresentsModel` to aid in advanced mapping detection scenarios.
- Improvement: `DataContractTemplate` constructor will accept parameters to initialize base class first before accepting parameters to populate its own attributes.
- Improvement: `DataContractTemplate` checks the base type now through its own Generalization association instead of the `Base Type` (Type References).

### Version 5.0.3

- Fixed: Data Contract does not apply Base Type when it is set in the designer.

### Version 5.0.2

- Improvement: Made small changes to UpdateHelper to make it more extensible.

### Version 4.4.2

- Improvement: Primary domain entity interface qualified properties now also have a `model` metadata populated for identification in other modules.
- Fixed: Updated explicit implementations on domain entities to use actual explicit implementation methods on CSharpFileBuilder.

### Version 4.4.0

- Improvement: Description Attributes can be applied to `Enum` literals through the usage of the Description Stereotype.
- Improvement: Added code documentation for all helper code.
- Improvement: Entity classes have the Intent Tag Mode set to `Implicit` to reduce the noise caused by the Intent Managed Attributes.

### Version 4.3.7

- Improvement: Added support for `abstract` operations, and override implementations in derived classes. 
- Fixed: Parameterless `public` constructor with `<Property> = null!` statements will no longer be generated in order to avoid impacting already existing files with unmanaged constructors.

### Version 4.3.6

- Update: Adding explicit parameterless constructors, when there are no constructors to remove nullabilty warning's on `Properties` which should be dealt with through constructor usage.

### Version 4.3.5

- Update: Automatically add `async` modifier to Async domain operations.

### Version 4.3.4

- Update: Base classes not have `protected` setters, rather than `private` setters, to make them more accessible in the inheritance hierarchy.
- Update: Generic type parameters specified for `Class`es in the Domain Designer are now respected during generation of entities.

### Version 4.3.3

- Fixed: Constructor and Operation mappings don't respect Pascal-casing of Properties when camel-casing is used in the designer.

### Version 4.3.2

- Updated code-management instructions to ignore constructor body by default.

### Version 4.3.1

- Update: Removed various compiler warnings.
- Domain entity operations will now also be detected as `async` if their name is suffixed with `Async`.
- Asynchronous domain entity operations now have an additional `CancellationToken cancellationToken = default` parameter.

### Version 4.3.0

- Default Values are now populated on Operation Parameters and Constructor Parameters.
- Update: When `Ensure Private Property Setters` is enabled, association collections are now generated as `IReadOnlyCollection`s.
- Update: Comments on `Entity`s and their members are now added as XmlDocComments to generated code. 

### Version 4.2.2

- Updated dependencies and supported client versions to prevent warnings when used with Intent Architect 4.x.

### Version 4.2.1

- Fixed unresolved namespaces for Domain types like Data Contracts.

### Version 4.2.0

- Sets default value on properties as per Domain designer

### Version 4.1.0

- Auto-implements operations that are mapped (supported in Intent.Modelers.Domain <= 3.4.0). 

### Version 4.0.5

- Enums will now generate comments captured in designers.

### Version 4.0.4

- Fixed: `UpdateHelper` would not correctly handle sources with null values.
- Fixed: Domain Entity collection property initializers would not have generic type arguments included.

### Version 4.0.3

- Fixed: When "Separate State from Behaviour" was selected, `using System;` would not be added for `NotImplementedException`s innew operations causing a compilation error.
- Updated `UpdateHelper` so that if `changedCollection` is `null`, it will only ignore the update request.

### Version 4.0.2

- Support for `[Flags]` attribute on enums. To add this attribute, apply the `Flags` stereotype to the enum.

### Version 4.0.1

- Fixed: `Intent.Modules.Entities` nuget package has incorrect dependency on `Intent.Modules.Constants`.

### Version 4.0.0

This version allows users to select their preferences for the Entities pattern from the Domain Settings (see Application Settings). 
These include the following options:

- Support for private setters - useful for ensuring a rich domain model and preventing anaemic access to properties.
- Support for separating state from behaviour - for when keeping rich domain behaviours separate from the state properties is preferred.
- Support for entity interfaces - recommended only when very strict control to the domain is necessary.

> This verion is implemented using the `CSharpFile` builder pattern for the various templates.

### Version 3.3.13

- Fixed: Entity Interface template now generates the `?` symbol when a nullable Enum is specified on the operation.
