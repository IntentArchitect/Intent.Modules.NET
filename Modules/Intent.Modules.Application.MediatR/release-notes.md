### Version 4.2.8

- Improvement: Updated module NuGet packages infrastructure.

### Version 4.2.7

- Improvement: Updated NuGet packages to latest stables.

### Version 4.2.6

- Improvement: Improved `Authorized` modeling.

### Version 4.2.5

- Improvement: Added `TODO` comments on `NotImplementedException`.

### Version 4.2.4

- Improvement: Application Client Dto type using directives also to be resolved now in Command/Query handlers.

### Version 4.2.3

- Improvement: Updated Interoperable dependency versions.

### Version 4.2.2

- Improvement: Upgraded module to support new 4.1 SDK features.

### Version 4.2.0

- Improvement: Added Support for `+` in roles, to describe `and` relationships between roles e.g. `Admin,Manager` (or) vs `Admin+Manager` (and)

### Version 4.1.7

- Fixed: `QueryHander` and `CommandHandler` where incorrectly using `WithBaseType` instead of `InmplementsInterface` for `IRequestHandler`.

### Version 4.1.4

- Improvement: Introduced new "Consolidate Command/Query associated files into single file" setting. When enabled, commands/queries are no longer generated into sub-folders and their respective handlers and validators (as applicable) are now embedded within them as opposed to being generated separately. See the [README](https://github.com/IntentArchitect/Intent.Modules.NET/blob/development/Modules/Intent.Modules.Application.MediatR/README.md#cqrs-settings) for more details.
- Improvement: Internal model update for better identifying Command / Query Templates during Software Factory Execution.

### Version 4.1.3

- Fixed: For collections of a non-DTO return types, the handler's would incorrectly generate to return `IEnumerable<T>` instead of `List<T>` in alignment with the generic type argument of their respective `Query`s and `Command`s.

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
