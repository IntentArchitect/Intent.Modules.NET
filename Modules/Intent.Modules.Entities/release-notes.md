### Version 4.0.2

* Support for `[Flags]` attribute on enums. To add this attribute, apply the `Flags` stereotype to the enum.

### Version 4.0.1

* Fixed: `Intent.Modules.Entities` nuget package has incorrect dependency on `Intent.Modules.Constants`.

### Version 4.0.0

This version allows users to select their preferences for the Entities pattern from the Domain Settings (see Application Settings). 
These include the following options:

* Support for private setters - useful for ensuring a rich domain model and preventing anaemic access to properties.
* Support for separating state from behaviour - for when keeping rich domain behaviours separate from the state properties is preferred.
* Support for entity interfaces - recommended only when very strict control to the domain is necessary.

> This verion is implemented using the `CSharpFile` builder pattern for the various templates.
### Version 3.3.13

* Fixed: Entity Interface template now generates the `?` symbol when a nullable Enum is specified on the operation.
