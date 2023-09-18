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