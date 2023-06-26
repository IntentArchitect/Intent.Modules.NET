### Version 4.0.10

- Fixed: Namespaces would not be added for enums used on `QueryHandler`s and `CommandHandler`s.

### Version 4.0.9

- Fixed: DTOs will now be properly referenced with using directives for Command / Query models.

### Version 4.0.8

- Fixed: Namespaces would not be added for enums used on `Query`s and `Command`s.

### Version 4.0.7

- Update: `Command ModelTemplate` and `QueryModelTemplate` upgraded to CSharpFileBuilder.
- Version bump for dependencies.

### Version 4.0.5

- Update: NuGet dependency on MediatR.Contracts
- Update: Changed default `IntentManged` on `ServiceCallHandler` template to be Merge.

### Version 4.0.4

- Update: Included Service Designer comments on `Query`s and `Command`s as documentation on `Query`s, `Command`s and `CommandHandlers` .

### Version 4.0.2

- Update: Updated Module interoperability for integrating with AspNetCore.

### Version 4.0.0

- New: Upgraded Templates to use new Builder Pattern paradigm.