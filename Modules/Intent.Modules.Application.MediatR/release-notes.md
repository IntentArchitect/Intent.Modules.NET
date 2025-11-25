### Version 4.5.3

- Improvement: XML Documentation improvements for Command and Query models.

### Version 4.5.2

- Improvement: Updated projectUrl to reference external documentation instead of local README.

### Version 4.5.1

- Improvement: Detects existence and name of `CancellationToken` parameter before adding to the `Send` invocation.

### Version 4.5.0

- Improvement: Select whether to lock the version of the MediatR Nuget package to the one prior to the commercial version or proceed to use the commercial version accepting its license. Read the article [here](https://www.jimmybogard.com/automapper-and-mediatr-commercial-editions-launch-today/).

> ⚠️ NOTE
>
> If you decide to go with the commercial version you will need to obtain and specify the license key.
> This can be done by requesting one as indicated in the article above and then inserting it into the `appsettings.json` under `MediatR:LicenseKey` (or as an environment variable `MediatR__LicenseKey`).

### Version 4.4.2

- Improvement: `Query` not represent `QueryModels`.

### Version 4.4.1

- Improvement: Locked MediatR NuGet package version
- Improvement: Handlers with no auto-implementation will now generate with an `IntentManaged` Body mode of Merge and their `throw new NotImplemented()` statements have been commented with [IntentInitialGen to prevent them from returning when deleted](https://docs.intentarchitect.com/articles/application-development/code-management/code-management-csharp/code-management-csharp.html#the--intentinitialgen-instruction). (Note: A side effect of this improvement is that entries may appear in the Customizations tab of the Software Factory or existing entries may no longer be approved and they will need to reviewed).

### Version 4.4.0

- Improvement: Provides a `_mediator.Send(...)` implmentation strategy for the new interactions system.

### Version 4.3.4

- Improvement: Default values on Query properties will now be output.

### Version 4.3.3

- Improvement: Updated NuGet package versions.

### Version 4.3.2

- Improvement: `Roles` and `Policies` will now used constants instead of strings, if they are available.

### Version 4.3.1

- Improvement: Included module help topic.

### Version 4.3.0

- Improvement: The `Authorize` stereotype has been removed and its usage has been replaced with the `Security` stereotype. A module migration will automatically convert existing `Authorize` stereotypes to `Secured` stereotypes which should allow everything to continue working without any additional intervention required.

  > ⚠️ **NOTE**
  >
  > A consequence of this change is that `Security` stereotype will now also cause `[Authorization]` to be added to generated `Command` and `Query` classes. If prior to upgrading this module you had only `Security` stereotypes applied to `Command` and `Query` element types, generated classes from them will now have `[Authorize]` attributes added to them.

- Improvement: The `Security` Stereotype can now be applied multiple times to an element to represent an `AND` security requirement.
- Improvement: An `OpenAPI Settings` Stereotype can now be applied to the types `DTO Field` and `Parameter`, allowing for example values to be set to reflect in the OpenApi spec.
- Improvement: Updated NuGet package versions.

### Version 4.2.10

- Improvement: `IQuery` and `ICommand` using CSharpFileBuilder paradigm.

### Version 4.2.9

- Improvement: Default values on Command properties are now used in the Command constructor, provided there are no properties which proceed it without a default value.


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
