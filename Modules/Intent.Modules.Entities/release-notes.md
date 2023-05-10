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
