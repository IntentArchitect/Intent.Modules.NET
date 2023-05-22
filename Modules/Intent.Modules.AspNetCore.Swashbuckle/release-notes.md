### Version 4.1.4

- Update: Changed `DomainEventServiceTemplate` to be `Fully` managed from `Ignore`.

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