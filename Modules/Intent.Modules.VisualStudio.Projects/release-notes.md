### Version 3.3.19

- Fixed: Exceptions would get thrown when certain aspects of a `.csproj` file were declared in or used properties defined in a `Directory.Build.props` file:
    - When `<TargetFramework(s) />` is not defined in the `.csproj` file, a warning will now occur with advice to use the `(unspecified)` option in the Visual Studio designer.
    - When a `<PackageReference />`'s `Version` is unparseable as a semantic version (for example due to being an MSBuild variable), it is now ignored.

### Version 3.3.18

- Added .NET 7.0 version option.
- Added support for generating `Remove` element types for file items in .NET `.csproj` files.
- The `Root Namespace` value of a project in the Visual Studio designer is now made available on `IOutputTarget.Metadata` under the `Root Namespace` key.

### Version 3.3.17

Updated description of module.

### Version 3.3.16

 - Fixed: Nullable types not being indicated when `Nullable` is set to `enable`.

### Version 3.3.15

- Updated `Nullable` stereotype property for the `C# Project Options` stereotype to have all available options for `.csproj` files.

### Version 3.3.14

- If a `.csproj` file has a defined value when its corresponding property in the `.NET Core Settings` stereotype is blank, it will no longer be removed from the `.csproj` file.
- `IntentIgnore="true"` can now be applied to file item sub-elements in `.csproj` files.
- Fixed: `No project found for id "{projectId}"` exception would be thrown when a `Template Output` was in a solution folder and not a project.

### Version 3.3.13

- Fixed: local.settings.json doesn't support object values and better measures have been applied. 

### Version 3.3.12

- Fixed: Folder Options stereotype not applicable to normal folders in other designers.
- Fixed: An exception would occur when running the software factory after renaming a project in the Visual Studio designer.
- Fixed: When `WithBuildAction("None")` was specified for a template targetting an `.sqlproj` file, instead of it having an `ItemType` of `None`, it would exclude it completely.
- Fixed: An exception would occur in `LocalSettingsJsonTemplate` when an `AppSettingRegistrationRequest` had an anonymous object for its `Value`.
