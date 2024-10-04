### Version 5.1.1

- Fixed: `TypeSchemaFilterExtension` class moved from `Intent.AspNetCore.Controllers` to `Intent.AspNetCore.Swashbuckle` module where it belongs as other hosting infrastructure using Swashbuckle can also benefit from it.

### Version 5.1.0

- Improvement: Default Model Rendering now uses the `Example` setting, offering a friendlier representation the payload structure.

### Version 5.0.4

- Improvement: Updated module NuGet packages infrastructure.

### Version 5.0.3

- Improvement: Updated NuGet packages to latest stables.

### Version 5.0.2

- Improvement: Updated Interoperable dependency versions.

### Version 5.0.1

- Improvement: Upgraded module to support new 4.1 SDK features.

### Version 5.0.0

- Improvement: Updated to support use of top-level statements and minimal hosting model improvements introduced in [`Intent.AspNetCore` version 6.0.0](https://github.com/IntentArchitect/Intent.Modules.NET/blob/development/Modules/Intent.Modules.AspNetCore/release-notes.md#version-600).

### Version 4.0.9

- Improvement: All non-nullable properties can now be marked as such and specified as mandatory in the generated `swagger.json` file by enabling the _Mark non-nullable fields as required_ application setting (thank you to [@shainegordon](https://github.com/shainegordon) for their [PR](https://github.com/IntentArchitect/Intent.Modules.NET/pull/4) for this).
- Improvement: It is now possible to opt-in to using simple schema identifiers as opposed to the current default of "fully qualified identifiers" by enabling the _Use simple schema identifiers_ application setting (thank you to [@shainegordon](https://github.com/shainegordon) for their [PR](https://github.com/IntentArchitect/Intent.Modules.NET/pull/4) for this).

### Version 4.0.8

- Improvement: XML comments if generated will be now applied, see [here](https://github.com/IntentArchitect/Intent.Modules.NET/tree/development/Modules/Intent.Modules.AspNetCore.Swashbuckle/README.md#xml-comments) for more information.

### Version 4.0.4

- Fixed: Multiple `options.DefaultModelsExpandDepth(...)` lines were being generated.
- Fixed: `app.UseSwagger()` would needlessly add the options parameter with an empty statement block.

### Version 4.0.3

- Change: Swagger Generation using full Type names in schema, to avoid naming conflicts.

### Version 4.0.1

- Fixed: Swagger UI misconfigured for JWT Bearer auth.

### Version 4.0.0

- New: Moved configuration from `appsettings.json` to code configuration.
- New: Upgraded Templates to use new Builder Pattern paradigm.
