### Version 4.1.0

- Upgrade - Breaking Changes: Upgrades MediatR package to 12.1.1, updated patterns accordingly. 

NB there are breaking changes in MediatR 12.1.1, if you have custom `IPipelineBehavior`s you will need to update these, of particular importance is the change to the generic constraints 
i.e. `where TRequest : IRequest<TResponse>` changes to `where TRequest : notnull` 

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