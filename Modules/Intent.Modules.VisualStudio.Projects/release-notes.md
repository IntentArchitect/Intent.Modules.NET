### Version 3.3.13

- Fixed: local.settings.json doesn't support object values and better measures have been applied. 

### Version 3.3.12

- Fixed: Folder Options stereotype not applicable to normal folders in other designers.
- Fixed: An exception would occur when running the software factory after renaming a project in the Visual Studio designer.
- Fixed: When `WithBuildAction("None")` was specified for a template targetting an `.sqlproj` file, instead of it having an `ItemType` of `None`, it would exclude it completely.
- Fixed: An exception would occur in `LocalSettingsJsonTemplate` when an `AppSettingRegistrationRequest` had an anonymous object for its `Value`.
