using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.FileBuilders.MarkdownFileBuilder;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.ProjectItemTemplate.Partial", Version = "1.0")]

namespace Intent.Modules.Services.Guardrails.Contracts.Templates.ReverseEngineerServicesSkill
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class ReverseEngineerServicesSkillTemplate : MarkdownBaseTemplate<object>, IMarkdownFileBuilderTemplate
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.Services.Guardrails.Contracts.ReverseEngineerServicesSkill";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public ReverseEngineerServicesSkillTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            WithContentHashing = true;
            MarkdownFile = new MarkdownFile("SKILL", relativeLocation: "reverse-engineer-services")
                .FromMarkdown("""
                    ---
                    description: Reverse-engineer an existing C# application layer (CQRS commands/queries, DTOs, services, controllers/endpoints, authorization, integration events/commands and their handlers, and API clients as Service Proxies) into the Intent Architect Services designer so the model mirrors the code and the Intent.Services.Guardrails.Contracts module adopts the existing files in place — including HTTP exposure, security and the intent of each command, query and service operation (entity actions, invocations, published events and sent commands), installing the WebApi/Security/DomainInteractions metadata modules when the code calls for them, and configures the drift check that compares the model with the code on every Software Factory run. Use when the user has installed the Services Guardrails module (or asks to "align / model / reverse-engineer the services") on a brownfield codebase.
                    short-description: Model existing commands, queries, DTOs and endpoints in the Services designer and adopt their files with the Services Guardrails module.
                    argument-hint: "[application name]"
                    ---

                    # Reverse-engineer the Services

                    Goal: the Services designer describes the application layer **as it is** (requests, services, DTOs, endpoint surface, security, intent), and an SF run against the existing files changes **nothing but Intent file headers and the code's formatting**: the code behaves exactly the same. Handlers, validators, mapping profiles and controllers stay hand-written. The last step configures the drift check, which compares the model with the code on every Software Factory run; the adoption ends with no drift.

                    Run **reverse-engineer-domain** first when the code has a domain layer. Commands, DTOs and entity actions reference Domain types. Load **scripting-the-model**, **exploring-the-model** and **changing-the-model** before modelling, and **generating-and-applying-code** / **resolve-destructive-changes** before the first SF run.

                    ## 0. Preconditions

                    1. `Intent.Services.Guardrails.Contracts` must be installed. If it isn't, stop and ask.
                    2. **Detect what the code exposes and install the matching modules.** Running this skill is the user's authorization to install the metadata/modeler modules that apply. Detect from the code (controllers, `Program.cs`, csproj `PackageReference`s, attributes):

                    | Evidence in the code | Install (if not already installed) |
                    |---|---|
                    | MediatR-style requests (`IRequest<T>`, `ICommand` / `IQuery`, `*Command` / `*Query` classes) | `Intent.Modelers.Services.CQRS` |
                    | HTTP endpoints: `[ApiController]` / `ControllerBase`, minimal APIs (`app.MapGet/MapPost…`), FastEndpoints, Azure Functions HTTP triggers | `Intent.Metadata.WebApi` 4.7.22-pre.1 or later, so include prereleases in the search (earlier versions add a duplicate route segment on every change for some route parameters) |
                    | Authorization: `[Authorize(...)]`, `[AllowAnonymous]`, `RequireAuthorization(...)`, policies or roles | `Intent.Metadata.Security` (WebApi normally brings it as a dependency; install explicitly if missing) |
                    | Handlers or service methods that load, create, update or delete Domain entities via repositories or a DbContext, and a Domain designer exists | `Intent.Modelers.Services.DomainInteractions` |
                    | Pagination types (`PagedResult<T>`, `PagedList`) | none; reference the `Intent.Modelers.Services.Pagination` package (§3) |
                    | Integration events / messaging: consumers or handlers (`IConsumer<T>`, `IHandleMessages<T>`, `IIntegrationEventHandler<T>`), publish or send calls (`IEventBus`, `IMessageBus`, `IPublishEndpoint`, `ISendEndpointProvider`), MassTransit, NServiceBus, Azure Service Bus | `Intent.Modelers.Services.EventInteractions` |
                    | Domain event handlers: `INotificationHandler<…DomainEvent…>`, `IDomainEventHandler<T>`, `DomainEventNotification<T>` | `Intent.Modelers.Domain.Events` (run **reverse-engineer-domain** first, so the Domain Events exist to link to) |
                    | API clients: typed `HttpClient` wrappers, Refit, NSwag or Kiota clients, or any other class whose job is calling an API | `Intent.Modelers.Types.ServiceProxies` 5.5.8-pre.1 or later, so include prereleases in the search (earlier versions block a scripted proxy creation, and reject overloads). Every client is modelled (§3), whatever API it calls. **Never** install `Intent.Application.Contracts.Clients` or `Intent.Integration.HttpClients` alongside it: they fail on the unmapped proxies this skill models. |

                    - Get the exact current version with `searchAvailableModules(id)`, then call `install_or_update_modules` and state in one line what you installed and why.
                    - Ask only when the evidence is ambiguous (e.g. gRPC or a custom transport).

                    3. **What the installed templates do.** This skill ships with them, so don't decompile the module to check:
                    - **Path:** Template Output location + the designer folder path. **Name:** the class name is the element's name verbatim.
                    - **Type kind:** `class`, unless the **`Type Configuration`** stereotype's `Type` is `record` or `positional record`. Its `Modifiers` and the `Default Type Modifiers` setting give the declaration's modifiers (§3).
                    - **Merge directives:** every file carries both (§7 step 3). **Async:** as §3 "Async operations" says.

                    ## 1. Extract the code (read-only, in parallel)

                    Dispatch `Explore` sub-agents in one message, one per feature folder group (e.g. Customers+Products, Orders+Payments+Wishlists). Each slice also reads its controllers. Ask for:

                    - **Each command / query:**
                    - Path, namespace, name, kind (`class`, `record`, or positional `record CancelOrderCommand(int OrderNumber)`) and declaration modifiers (`public`, `internal` or none, plus `sealed`, `abstract`, `static`, `partial`).
                    - Every property: exact type **as spelled** (`Nullable<T>` vs `T?`, `System.Guid` vs `Guid`, partly qualified names such as `DTO.Logging.LogDTO`), nullability, collection-ness, default value and whether it is `required`.
                    - Return type (`Guid`, a DTO, a list of DTOs, `PagedResult<…>`, or nothing).
                    - `[Authorize(Roles/Policy)]` / `[AllowAnonymous]`, and brief validator rules.
                    - Its handler's intent (§5): the Domain entities it creates, updates, deletes or reads; the service operations, commands, queries, domain services, repository operations, stored procedures or API client methods it calls; and the integration events it publishes or integration commands it sends.
                    - **Each DTO:** path, namespace, `class` or `record`, declaration modifiers, and every property as for commands.
                    - **Each service:**
                    - Path, namespace, class name, declaration modifiers, and every method signature, with parameter and return types as spelled and the method's modifiers as written (access, `static`, `virtual`, `override`, `sealed`, `new`).
                    - Whether it is **public** (a controller action or other endpoint calls its methods directly) or **internal** (only other application code calls it).
                    - Each method's intent, as for a command's handler.
                    - **Nullable context:** per project, whether the csproj sets `<Nullable>` (and to what); per file, any `#nullable` directives.
                    - **Each enum:** path, namespace, access modifier (`public`, `internal` or none), and literals with explicit values.
                    - **Each domain event handler:** path, namespace, class name, declaration modifiers, and the domain event(s) it handles.
                    - **Each integration event and integration command (message):**
                    - Path, namespace, name, kind (`class`, `record`, or positional `record OrderPlaced(Guid OrderId)`) and declaration modifiers.
                    - Whether it is published (an event) or sent to one receiver (a command), and from where.
                    - Whether it is this code's own copy of another application's message (handled, never published or sent here), and whose.
                    - Every property as for commands, plus its accessors as spelled (`{ get; set; }`, `{ get; init; }`, get-only).
                    - **Each nested type a message uses (an eventing DTO):** as for DTOs, plus its base type.
                    - **Each integration event handler or consumer:** path, namespace, class name, declaration modifiers, and the messages it handles (from `IConsumer<T>`, `IHandleMessages<T>`, `IIntegrationEventHandler<T>` or equivalent).
                    - **Each API client** (a typed `HttpClient` wrapper, a Refit, NSwag or Kiota client, or any other class whose job is calling an API):
                    - Which type to model (§3 "Service proxies"), its path, namespace, name, `class` or `interface`, and declaration modifiers.
                    - Every method as spelled: name, return type, each parameter's type and default, the CancellationToken's name and whether it has `= default`, whether it is `async`, and its modifiers and each parameter's (`ref`, `out`, `in`, `params`) as written; and anything §3 can't reproduce (explicit interface implementations, generic methods).
                    - Whether each request and response type is declared in the code (a local copy of the API's contract) or comes from outside it (e.g. a NuGet contracts package).
                    - **Each controller action:**
                    - HTTP verb, **absolute route** (and the controller's route prefix), and success status code.
                    - Which properties or parameters bind from the route, query or body.
                    - Which command or query it dispatches, or which service operation it calls.
                    - How the code states each of these facts, as written: where the route, verb, binding and authorization are declared (on the action, the controller class, a base controller, or `Program.cs`), and for FastEndpoints or minimal APIs the member that registers the endpoint (`Configure()`, `MapOrderEndpoints`): every style the code uses, with one verbatim example of each, for the recipes below.
                    - **Shared types:** `PagedResult<T>`, marker interfaces, permission constants.
                    - **The convention of each element type** you will model (DTO, command, query, service, enum, integration event, handler, API client): something all its code shares that a new one would have too (a folder with a name pattern such as every `*DTO` in the DTO project, a base class, an interface such as `IRequest<T>`, an attribute such as `[ApiController]`, or a member), with one verbatim example, or that there is none.

                    **Then write the drift check's recipes, before modelling anything,** while the reports are fresh: load **configure-drift-check** for the format and write `guardrails/services.drift.json` from the reports, without re-reading the code:
                    - a recipe for each HTTP and security fact the code states (route, verb, binding, security), with one `find` per style;
                    - `inherit` where an action takes a value from its controller, and `default` for what the code states by saying nothing (no `[Authorize]`);
                    - an `own` entry for each kind of endpoint the model must describe (every controller action, minimal API mapping or endpoint class);
                    - an `own` entry, with `kind` naming the element type, for each element type that follows a convention, finding every type that follows it. An element type without one gets none: its new types can't be told apart from helpers.

                    Ask a sub-agent only about a style the reports don't cover. The file turns the check on: every run now fails until the model matches the code, as expected during adoption, and its report lists what isn't modelled yet. §7 step 8 finishes the check.

                    ## 2. Align the Codebase Structure

                    Move the Services Guardrails Template Outputs (CommandContract, QueryContract, DtoContract, EnumContract, ServiceContract, and when present DomainEventHandlerContract, IntegrationEventContract, IntegrationCommandContract, EventingDtoContract, IntegrationEventHandlerContract, ServiceProxyContract) to the **Application project folder that contains the feature folders**, usually the project root. They often start in the Api project.

                    - **Messages in their own project.** Integration events, integration commands and their eventing DTOs often live in a separate Contracts project (e.g. `Acme.Contracts`) shared with other services. Put the IntegrationEventContract, IntegrationCommandContract and EventingDtoContract Template Outputs there, at the folder its namespaces start from; handlers usually stay with the Application project. If the code's enums sit with the messages, EnumContract needs a filtered copy there too (below).

                    - **API clients.** Concrete clients usually live in the Infrastructure project (e.g. `Acme.Infrastructure/HttpClients`). Put the ServiceProxyContract Template Output in the project that holds them, at the folder its namespaces start from. When clients sit in more than one project (e.g. Refit interfaces in Application, concrete clients in Infrastructure), duplicate it with mutually exclusive Registration Filters (below), filtering on the designer folder with `np(ParentElement.Name)` (its model is a raw element), e.g. `np(ParentElement.Name) == "HttpClients"`.

                    - **One template, several locations.** When a template's files live in more than one project or root folder (e.g. enums in both the DTO and Business projects), **duplicate its Template Output**, one per location, each with a **Registration Filter** (`Template Output Settings` stereotype) selecting the model instances generated there.
                    - The filter is a Dynamic LINQ `bool` expression over the template's model (`EnumModel`, `DTOModel`, …). `Folder.Name` is usually the simplest discriminator, with `np(...)` for anything nullable: `np(Folder.Name) == "BusinessObject"` on one copy, `!=` on the other.
                    - **Filters must be mutually exclusive:** an instance matching two outputs fails the SF, and an empty filter matches everything, so give the original the complementary filter.
                    - Script it: `project.addChild("Template Output", "<TemplateId>")`, then `ensureStereotype("Template Output Settings").setProperty("Registration Filter", "<expr>")`.
                    - Model those elements in a designer folder the filter selects on, with `File Config` (`Filename` / `Namespace`) to land each type in its existing file. `File Config` can't move a file to another project; a filtered Template Output can. Use Type-Definition placeholders only for types that can't be elements at all (nested types, `dynamic`, framework types).

                    Save the Codebase Structure designer (`saveOnSuccess: true` on a no-op script) once it is right.

                    ## 3. Model the services

                    1. **Ensure a package exists, then add references.**
                    - **Check first:** `getPackages()` on the Services designer.
                    - **If there are none,** create one named `{ApplicationName}.Services`, using the application name from the workspace context / `get_applications` (e.g. `Training.Ordering.Services`): `await createPackage("Training.Ordering.Services")`.
                    - **If one or more exist,** use the existing package and don't create another. If several exist, ask which one to use.
                    - **References:** add package references for the Domain package, `Intent.Common.Types` (primitive types), and `Intent.Modelers.Services.Pagination` (`PagedResult<TData>`) when the code pages.
                    - **Naming conventions, before creating anything.** Names must stay as the code spells them, but the designer renames elements to its `Service Settings` naming conventions (`Property`, `Entity` (Services too), `Operation`, `Parameter`; Parameter defaults to `Camel Case`, turning `ObjectID` into `objectID`). Read them with `getApplicationSettings({ search: "Naming Convention" })` (via `run_solution_query`), set each that isn't `Manual` to `Manual` with `update_application_settings`, and state in one line what you changed.
                    2. **Folders mirror the code exactly.** The Guardrails templates do **not** add a per-use-case sub-folder. If the code is `Customers/AddAddress/AddAddressCommand.cs`, model folder `Customers/AddAddress` and put the command inside. DTOs that sit in `Customers/` go in the `Customers` folder.
                    - **Files that don't follow the convention:** each element is written to `<ElementName>.cs` in its folder's namespace. Apply the **`File Config`** stereotype only where the code differs, setting only the property you need:
                    - **Namespace doesn't follow the folders:** set `Namespace` to the file's exact namespace. Every type that shares a file needs the same `Namespace`.
                    - **No namespace in the file** (types declared in the global namespace): tick `No Namespace` and leave `Namespace` empty.
                    - **File named differently from its type** (e.g. `psMembersDTO.cs` holds `PsMembersDTO`): set `Filename = psMembersDTO` (the file name only, with or without `.cs`; the folder stays the element's own). If the name starts or ends with a space (e.g. `abc .cs`), include `.cs`: without it, surrounding spaces are trimmed.
                    - **File holding several types:** model every type as its own element in that file's folder, and set `Filename` to the file's name on each type the file is *not* named after. Elements that end up at the same path are generated into one file, written by the element the file is named after.
                    - **Set it before the first SF run.** Pointing a type whose own file was already generated at another file makes the SF offer to delete the old file: a destructive change, not to apply unless the old file is a duplicate.
                    3. **DTOs first** (dependency order), then commands, queries and services:
                    - **A type name used in more than one package** (each project's own copy of an event, DTO or enum): set the type by the element's id (`setType(el.getId())`). A name that matches several elements fails the script.
                    - **Fields:** one `DTO-Field` per property, with exact names and types. `isNullable` for nullable types (see "Nullability and type spelling" below), `isCollection` for lists (never `List<…>` as a type name). A property initializer (`= "Basislink"`) is the field's Value (see "Default values" below).
                    - **Services:** one `Service` per application service class, named as the concrete class (no interface is generated), with one `Operation` per method it exposes ("Which members to model" below) and one `Parameter` per parameter, typed as for fields. Both kinds are modelled the same way here:
                    - **Public:** an endpoint calls its operations directly, so each is that endpoint's request, like a command or query, with HTTP settings and security (§4) and intent (§5).
                    - **Internal:** only other application code calls it: intent (§5), but no HTTP settings.
                    - **Default values and initializers:** where a service operation parameter has a default (`string area = "Basislink"`, `bool forceCreate = false`), or a DTO, command or query property an initializer, set the element's Value (`el.setValue("Basislink")`), or Merge mode drops it from the existing signature.
                    - Strings are quoted for you (an already quoted value stays); `null`, `default`, numbers, `true` and `false` are used as written.
                    - An enum literal gets its enum type (`Info` → `EventTypeEnum.Info`), and a bare `DateTime` member its type (`Now` → `DateTime.Now`).
                    - Anything that looks like an expression (contains `.` or `(`, or starts with `_`) is used as written; prefix any other expression with `@` to pass it through verbatim (`@DefaultArea` → `DefaultArea`).
                    - A `ref`, `out` or `params` parameter can't have a default; that combination is an error on the element.
                    - **Collections:** the template emits `List<T>`. Wherever the code uses anything else (`IEnumerable<T>`, `IList<T>`, `IQueryable<T>`, `T[]`, …), apply **`Collection Type`** to that field, parameter or operation (for its return type) with a matching `Type` (`Custom` + `Custom Format` with `{0}` for anything else), or Merge mode rewrites the signature.
                    - **Nullability and type spelling.** Model the type, not its spelling:
                    - **Nullable value types:** set `isNullable` for `T?` **and** for `Nullable<T>` / `System.Nullable<T>`. They are the same type.
                    - **`string?` (or any reference type with `?`) with nullable reference types off** (the csproj has no `<Nullable>enable</Nullable>` or `annotations`, and the file no `#nullable enable`): the `?` does nothing but warn CS8632. Model a nullable `string` anyway; the template emits `string`, a change beyond the headers handled under §7 step 4. Never drop the member.
                    - **Nullable reference types on:** make the project's Nullable option in the Codebase Structure designer match the csproj, so the template emits `string?`. A `string?` → `string` diff then is real drift: fix the project setting, not the member.
                    - **Other spellings of the same type** (`System.Guid`, `Int32`, `String`, or a partly or fully qualified name such as `DTO.Logging.LogDTO`): model the type itself (`guid`, `int`, `string`, `LogDTO`); the SF may rewrite the spelling (§7 step 4).
                    - **Parameter modifiers:** where a service operation parameter is declared `ref`, `out`, `in`, `ref readonly` or `params`, apply the **`Parameter Modifier`** stereotype and set `Modifier` to match (for `params`, also set `Collection Type = Array`). Otherwise Merge mode drops the modifier from the existing signature. An extension method (`this` parameter) can't be reproduced; leave it unmodelled and report it.
                    - **Parameter attributes** (`[FromBody]`, `[FromHeader(Name = "x-requestid")]`, `[AsParameters]`, `[FromServices]`): nothing to model; Merge keeps a parameter's attributes on a fully managed signature.
                    - **Async operations:** settle each operation's async shape before the first SF run, as **reverse-engineer-domain** §3 "Async operations" describes: this module's `Async Configuration` has the same properties (`Async Mode`, `Task Type`, `Include Cancellation Token`, `Cancellation Token Name`, `Cancellation Token Default`) and rules, and an operation without it is async when its name ends in `Async`.
                    - **Operation modifiers:** method signatures are fully managed, so an operation's modifiers must match the code. It is `public` unless **`Operation Configuration`** sets `Access Modifier` (`public`, `protected`, `internal`, `private`, `protected internal`, or `none` for none declared) and ticks `Static`, `Virtual`, `Override`, `Sealed` (only with `Override`) or `New` as the code declares; `async` stays on `Async Configuration`. A combination C# rejects is an error on the element. `private protected` can't be reproduced; leave that operation unmodelled and report it.
                    - **Required properties:** for a property declared `required`, apply **`Property Configuration`** and tick `Required`: a property's first merge replaces its modifiers with the generated ones, so an unmodelled `required` is dropped. Accessors need nothing: Merge keeps `init`, `private set` or get-only as written. Ignored on a positional record's fields (constructor parameters).
                    - **Property access modifiers:** a property is `public` unless **`Property Configuration`** sets `Access Modifier` (`public`, `protected`, `internal`, `private`, `protected internal`, or `none`). Model the code's own, or the first merge makes it `public`. Ignored on a positional record's fields.
                    - **Return type:**
                    - Plain type: `setType("guid")` or `setType("CustomerDetailDto")`.
                    - Paged: `setType("PagedResult<CustomerSummaryDto>")`.
                    - List: `setType({ name: "WishlistSummaryDto", isCollection: true })`.
                    - Nothing: leave it unset (void).
                    - **Domain enums:** use them directly as field types.
                    - **`Type Configuration`.** Apply it **only when required**: set `Type = record` on a DTO or request that is a `record` in code. Leave the default for classes.
                    - **Positional record** (`public record CancelOrderCommand(int OrderNumber) : IRequest<bool>;`, nothing but a parameter list): set `Type = positional record`. The fields, in model order, are the positional parameters (model them PascalCase, as declared), and a field's Value is the parameter's default. Base types and interfaces stay in the code. A record with a parameter list **and** body properties can't be reproduced yet; leave it unmodelled and report it.
                    - **Positional inheritance:** a DTO whose base DTO is a positional record must be one too, and its own fields must include the base's (passed on by name: `record ChildDto(int Id, string Name) : BaseDto(Id)`). A Service can't be a positional record.
                    - **Type modifiers:** Merge mode replaces each class, record or enum declaration's modifiers with the generated ones, so they must match the code.
                    - **Set the convention once, before the first SF run.** If most declarations aren't plain `public` (e.g. `internal sealed`), set `Default Type Modifiers` (group `Services Guardrails Contracts Settings`; the Domain module has its own) to the dominant combination: `public` or `internal`, optionally followed by `sealed`, `partial` or `sealed partial`; one of those three alone; or `none`. Read it with `getApplicationSettings({ search: "Default Type Modifiers" })` (via `run_solution_query`), change it with `update_application_settings`, and state in one line what you set and why.
                    - **The setting's `sealed` is left off automatically** for an abstract DTO and one another DTO inherits from.
                    - **Exceptions:** where a type differs from the setting, set `Type Configuration` → `Modifiers` to the code's keywords (e.g. `sealed`, `internal static`): at most one of `public` / `internal`, plus any of `sealed`, `abstract`, `static`, `partial`. Omit the access modifier when the code has none (never write `internal` instead); `none` alone means no modifiers, an empty value the setting. `Is Abstract` still adds `abstract`. An invalid or conflicting value (`sealed` on an abstract DTO, `none` with another keyword) is an error on the element.
                    - **Enums** take only the setting's access modifier (`internal sealed` gives `internal enum`; `sealed` or `none` gives `enum`). Where an enum differs, set `Enum Configuration` → `Access Modifier` to `public`, `internal` or `none`.
                    - **Can't be reproduced:** `file`-scoped and `unsafe` types. Leave them unmodelled and report them.
                    - **Which members to model.** Each type's public members; a protected, internal or private one only when it's part of the design (a protected method subclasses override), never just because it exists. The drift check reports only unmodelled public members, and Merge keeps one left out as written: no `[IntentIgnore]`.
                    - **Leaving members out.** This section's "leave it unmodelled and report it" rules (extension methods, `file`-scoped and `unsafe` types, the eventing and service proxy shapes below) cover only shapes the model can't express. A type merely spelled differently is never a reason to leave a member out (§7 step 4).
                    - **Domain event handlers** (when `Intent.Modelers.Domain.Events` is installed): one `Domain Event Handler` per handler class, in the folder matching its code folder, named verbatim, with an association to each Domain Event it handles. Class-only (the class, its file and namespace): Merge keeps the `Handle` methods and handler interfaces. `File Config` applies, and handlers have their own `Type Configuration` with only `Modifiers` (often `internal sealed`).
                    - **Integration eventing** (handlers need `Intent.Modelers.Services.EventInteractions`):
                    - **Package and folders:** integration events, integration commands and eventing DTOs live in an `Eventing Package` in the Services designer. Use the existing one, or create one named after the application: `await createPackage("Training.Ordering.Eventing", "Eventing Package")`. Folders mirror the code, as for DTOs.
                    - **Event or command:** a type published to any subscriber is an Integration Event (element type `Message`); one sent to one receiver is an `Integration Command`. Name it verbatim (no `Event` suffix is added), with one `Property` per property, typed and flagged as for DTO fields. A `Property` has no Value, so a message property initializer can't be modelled: report it.
                    - **Eventing DTOs:** one `Eventing DTO` per nested type a message uses, with one `Eventing DTO-Field` per property (an initializer is the field's Value), `Is Abstract`, its generic types, and a Generalization (`New Inheritance`) to its base Eventing DTO. **Enums** used by messages go in the eventing package too; EnumContract generates them like any other enum.
                    - **Handlers:** one `Integration Event Handler` per consumer or handler class, in the Services package folder matching its code folder, named verbatim, with a `Subscribe Integration Event` (or `Subscribe Integration Command`) association to each message it handles. Class-only, as for domain event handlers: Merge keeps the handle methods (`Consume`, `Handle`, `HandleAsync`) and handler interfaces. The associations document the subscription; the template doesn't read them.
                    - **Stereotypes,** working as above: `File Config` on all four element types; `Type Configuration` (`Type` and `Modifiers`) on integration events, integration commands and eventing DTOs, and the handler one (`Modifiers` only) on integration event handlers; `Collection Type` and `Property Configuration` on message properties and eventing DTO fields. The setting's `sealed` is left off an abstract eventing DTO and one another inherits from, and an eventing DTO must have its base's `Type`.
                    - **Only this application's own packages are generated.** A message or enum modelled in another application's eventing package that this one references (the usual subscriber setup) is never written here.
                    - **This code's own copy of another application's message** (§1): model it in this application's eventing package, then `Link To Published Integration Event` (or `Link To Sent Integration Command`) to the original, first adding a reference to the original's eventing package if missing.
                    - **Positional records** (`record OrderPlaced(Guid OrderId)`): `Type Configuration` → `Type` = `positional record`, as for commands and DTOs.
                    - **Accessors:** the template always emits `{ get; set; }`, and what Merge does to an existing `init` or get-only accessor hasn't been verified. If the SF diff changes an accessor on any property (messages, DTOs, commands or queries), don't apply that file; list it in the final report.
                    - **Service proxies** (when `Intent.Modelers.Types.ServiceProxies` is installed): one `Service Proxy` per API client the code declares (§1), purely as a picture of the code.
                    - **Which type:** the concrete client class, named verbatim (e.g. `PaymentsClient`). The interface it implements (`IPaymentsClient`) isn't modelled: it stays hand-written, and Merge keeps the class's `: IPaymentsClient`. Only when the code has no concrete class (the implementation is generated at runtime, e.g. Refit), model the interface, named verbatim (`IGitHubApi`), with `Service Proxy Configuration` → `Type` = `interface`. Never model both for one client.
                    - **Where:** in the Services package folder matching the client's code folder, or at the package root for a client in the Template Output's own folder.
                    - **By script, never with a mapping:** `createElementUnder(folder, "Service Proxy", "PaymentsClient")` (or under the package), one `Operation` per method (`proxy.addChild("Operation", "GetPaymentAsync")`; an overload is another `Operation` with the same name) and one `Parameter` per parameter (`operation.addChild("Parameter", "id")`, though the designer hides "Add Parameter" there). Type them as for service operations: a Parameter must have a type, and an Operation without one returns `void`. Never map a proxy or its operations to another application's endpoints, or model the remote API: the template ignores mappings.
                    - **Request and response types:** a type the client code declares itself (a local copy of the API's contract) is a `DTO`, `Command`, `Query` or `Enum` in this package, modelled and guarded like the others. A type from outside the code (e.g. a NuGet contracts package) is a `Type-Definition`, which nothing generates.
                    - **Async, collections and defaults:** a proxy Operation has its own `Async Configuration` (a service operation's properties and rules), and is async without it when its name ends in `Async`; only a class method gets the `async` keyword, per `Async Mode`. `Collection Type` applies to a proxy Operation (its return type) and Parameter. A proxy Parameter has no Value, so for a code default (`int page = 1`) apply **`Default Value`** with `Value` formatted as for service operation parameters, or Merge drops it. The CancellationToken's default stays on `Async Configuration`. `Parameter Modifier` applies to a proxy Parameter too.
                    - **Declaration:** a class takes the `Default Type Modifiers` setting, or `Service Proxy Configuration` → `Modifiers` (`Type Configuration`'s rules); an interface only the setting's access modifier, or its own `Modifiers`: at most one of `public` / `internal`, plus `partial`, or `none`. A class method's modifiers come from its own **`Operation Configuration`**, as for a service operation (`public` without it); an interface method takes none. `File Config` applies too.
                    - **Attributes and base types stay in the code.** The template generates none, and Merge keeps the code's own (a class's base list, Refit's `[Get("/users/{user}")]`).
                    - **Can't be reproduced yet:** an explicit interface implementation (`Task<PaymentDto> IPaymentsClient.GetPaymentAsync(...)`) and a generic method. Leave that method unmodelled (Merge keeps it) and report it.
                    - **Only this application's own packages are generated,** as for integration eventing.
                    4. **Validate** (`getDesignerValidationErrors`), then save the designer (`saveOnSuccess: true`). A script-created package is invisible to the SF until saved.

                    ## 4. Endpoint surface and security (HTTP solution; WebApi/Security metadata installed in §0)

                    For each command, query and **public** service operation (§3), set the following from the controller action or endpoint that calls it.

                    - **`Http Settings`:**
                    - `Verb` and `Route`. A command or query takes the **absolute** route (e.g. `api/customers/{customerId}/addresses`). A service operation's route is relative to its service: put the controller's route prefix on the Service as `Http Service Settings` → `Route` (e.g. `api/customers`), and give each operation the rest (`{customerId}/addresses`). A `Route` starting with `~/` ignores the prefix. If a service's operations are called from controllers with different prefixes, leave `Http Service Settings` off and give each operation its absolute route.
                    - **API versions read from the query string** (`HasApiVersion(...)` with Asp.Versioning's default `?api-version=` reader): API versions make the designer add a `{version}` segment to a route that has none. Put it in the Service's route as the query string instead (`api/orders?api-version={version}`), and have the `service.route` recipe read the code's group route that way.
                    - `Success Response Code` from the actual result: `Created(...)` → `201 (Created)`, `NoContent()` → `204 (No Content)`, `Ok(...)` → `200 (Ok)`.
                    - **`Parameter Settings.Source`:** `From Route` for every field or operation parameter whose camelCase name is a `{routeParam}`; `From Query` for the remaining fields or parameters of GET and DELETE actions. Leave body fields at the default.
                    - **Security:**
                    - `Secured` with `Roles` (or `Policy`) mirroring `[Authorize(...)]`; resolve constants, so `Permissions.RoleAdmin` becomes `Admin`.
                    - `Unsecured` for `[AllowAnonymous]`.
                    - Apply them per element: command, query, service operation, or the Service when all its operations are secured the same way. Use the package level only if the code secures everything uniformly.

                    Internal services get none of this. If the code has no HTTP surface, skip this section.

                    ## 5. Intent (DomainInteractions and EventInteractions installed in §0)

                    Model the **intent** of each command, query and service operation, for public **and** internal services alike: what it does, as interaction associations from it. Follow the designer's own rules; read them in `getDesignerSchema()`.

                    | What the code does | Association (context menu) | Target |
                    |---|---|---|
                    | Creates, updates, deletes or reads Domain entities | `Create Entity Action` / `Update Entity Action` / `Delete Entity Action` / `Query Entity Action` (`Create Entity`, `Update Entity`, …) | The Domain class (see "Entity Actions" below) |
                    | Calls another service operation, or dispatches a command or query (`_ordersService.Approve(…)`, `_mediator.Send(new ApproveOrderCommand(…))`) | `Perform Invocation` (`Invoke Service`) | That service `Operation`, `Command` or `Query` |
                    | Calls a domain service, a custom repository operation or a stored procedure | `Perform Invocation` | That Domain Service or Repository `Operation`, or the `Stored Procedure` |
                    | Calls an API through one of the code's API clients (`_paymentsClient.GetPaymentAsync(…)`) | `Perform Invocation` | That client's `Operation` on its `Service Proxy` (§3) |
                    | Publishes an integration event | `Publish Integration Event` | The integration event (`Message`) |
                    | Sends an integration command | `Send Integration Command` | The `Integration Command` |

                    - **Model every intent, not every step.** Everything the code does on purpose gets its association: an operation that updates an order, invokes `NotificationService.SendConfirmation` and publishes `OrderUpdated` gets all three. Don't add what an intent implies: updating an entity reads it first, so `Update Entity Action` alone is enough, and cancelling an order that also fails its payment inside the aggregate is one `Update Entity Action` → `Order`.
                    - **Invoke, don't repeat.** When an operation delegates to another service operation, command or query, model only the `Perform Invocation`; the callee carries its own intent.
                    - **The target must be in the model.** If what the code calls isn't modelled (a private helper, or a client method §3 couldn't reproduce), add no association; say what it calls in the element's comment, and report it.
                    - **No intent, no association.** An operation that only calculates or formats gets none.
                    - **When unsure, ask** the user, in one `ask_user_question` for all the uncertain elements, rather than skipping them.
                    - **Projections.** If a handler or operation projects one entity through another (e.g. payments via orders), target the conceptually primary entity and mention the projection in the comment.
                    - **Mappings.** Create the associations without field mappings (the designer's default); add mappings only if the user asks.

                    **Entity Actions** target the entity the code acts on:

                    | Purpose | Entity Action |
                    |---|---|
                    | Create aggregate root | `Create Entity Action` → root |
                    | Add a compositional child | `Create Entity Action` → child (`Address`, `OrderItem`, …), not the root |
                    | Change root state (rename, deactivate, confirm, cancel, advance) | `Update Entity Action` → root |
                    | Update or set-default a child; update an owned 1:1 child | `Update Entity Action` → child (e.g. `Preferences`, `Payment`) |
                    | Delete or remove a root or child | `Delete Entity Action` → that entity |
                    | Get by id / reference | `Query Entity Action` → entity, target `"1"` |
                    | Search / list | `Query Entity Action` → entity, target `"0..*"` |

                    ## 6. Diagram

                    Create a `Diagram` in the package if none exists:

                    - All commands, queries and services in **one vertical column on the left**, grouped by feature folder, commands and queries exactly **100px apart top-to-top**, and the element after a service **100px below its bottom edge**.
                    - The related Domain entities **200px to the right**, grouped by feature, with any domain services, repositories and stored procedures the left column invokes.
                    - The integration events, integration commands and service proxies the left column publishes, sends or invokes, if the diagram can show them, in a further column **200px to the right**.
                    - Route every intent association (§5). Invocations between commands, queries and services stay within the left column. DTOs can't be placed on this diagram.

                    ## 7. Generate, verify and adopt

                    Same as **reverse-engineer-domain** §6:

                    1. **Every change must be a header or formatting change:** `using` + `IntentTemplate`, the two merge directives, the csproj package reference, and Auto Format's formatting anywhere in the file (step 5), or an equivalent type rewrite the user accepts (step 4). While there is drift, a run also lists the drift report, `<<DriftReport>>` (in the solution's git-ignored `.cache` folder), as a new file needing no review. Until step 8 its drift lines fail the run, so nothing is applied by itself (step 6 applies by path anyway). Its "modelled but not in the code" and "isn't modelled" rows are a second view of what is still mismatched or missing.
                    2. **Diagnose anything else:** `create` means a path or name mismatch, or a type in another file without `File Config` (§3). Member inserts mean model drift. Deletes, and changed lines that aren't just formatting (step 5), mean destructive drift (**resolve-destructive-changes**). A duplicate type means the record/class kind is wrong. A rewritten type declaration (`internal sealed class` → `public class`) means wrong modifiers: fix `Default Type Modifiers`, or that element's `Type Configuration` → `Modifiers` (`Enum Configuration` → `Access Modifier` for an enum; §3). **A type spelled differently on an existing member** (`Nullable<DateTime>` → `DateTime?`, `string?` → `string`): see step 4. Don't remove the member.
                    3. **Merge directives:** every adopted file needs both of these lines, in this order:

                    ```csharp
                    [assembly: DefaultIntentManaged(Mode.Merge)]
                    [assembly: DefaultIntentManaged(Mode.Merge, Signature = Mode.Fully, Attributes = Mode.Merge, Comments = Mode.Merge, Targets = Targets.Constructors | Targets.Methods)]
                    ```

                    The current templates write both. Add them if the template didn't, **before** the next model change. Without the first, the SF strips `[Authorize]`, `IRequest<T>, ICommand` base types, XML docs and constructors. The second fully manages constructor and method signatures, and merges their attributes and comments.
                    4. **Equivalent type rewrites — ask, never drop.** An SF diff whose only change to an existing member is how a type is spelled, both spellings meaning the same type:

                    | Code | Template |
                    |---|---|
                    | `Nullable<DateTime>`, `System.Nullable<int>` | `DateTime?`, `int?` |
                    | `System.Guid`, `Int32`, `String` | `Guid`, `int`, `string` |
                    | `DTO.Logging.LogDTO` (partly or fully qualified) | `LogDTO` plus a `using` |
                    | `string?` with nullable reference types off | `string` |

                    - **Never remove a member (property, field, parameter or operation) from the model, or leave one out, to avoid an equivalent rewrite.** The model must mirror the code.
                    - **Confirm it is really equivalent:** the same type after resolving usings and enclosing namespaces; `Nullable<T>` vs `T?`; for `string?`, nullable reference types off. Anything else (a different type, nullability added or removed on a value type, a changed collection type) is real drift: fix the model.
                    - **Ask the user once, for all of them,** with `ask_user_question`, **before** applying any affected file, listing each as `File:line — before → after`. Offer:
                    1. **Accept the rewrites (recommended).** Keep the members and apply the files; only the spelling changes. For `string?` with nullable reference types off, this also clears CS8632.
                    2. **Keep the code as written; leave these members out of the model,** with a comment on each affected element saying what is missing and why.
                    3. **Model them, but don't adopt the affected files yet,** until a RoslynWeaver build keeps the existing spelling. Until then the SF keeps proposing them, so those files fail the zero-change check.
                    - **Follow the answer, and state it in the final report** with the affected members.
                    5. **Formatting changes — accept, never suppress.** RoslynWeaver's Auto Format (on by default) lays out each adopted file to the C# style rules, often across hundreds of lines and inside hand-written bodies (`GetFileName(link): null` → `GetFileName(link) : null`). It changes the layout, never what the code does.
                    - **Confirm it is formatting only:** the same tokens in the same order, only whitespace changed, string literals and comments unchanged. Anything else is not formatting; diagnose it under step 2.
                    - **Apply formatting changes with the rest of the file,** without asking.
                    - **Never turn off Auto Format** (`Auto Format by Default`, under `RoslynWeaver Settings`) or suppress it any other way to shrink a diff: that hides harmless fixes, and members the SF rewrites lose their indentation. If it is already off, ask the user whether to turn it back on (recommended).
                    - **Reviewing a large diff:** what is left past the whitespace must be step 1's headers or step 4's rewrites. For files under git with no other uncommitted changes, `git diff -w -- <file>` after applying shows it; revert any file showing more, and diagnose it under step 2.
                    - **In the final report,** say how many files were reformatted.
                    6. **Apply** the reviewed files only (`apply_staged_file_changes` with explicit paths): files whose only changes are the headers and formatting, plus files whose equivalent rewrites the user accepted (step 4).
                    7. **Finish with:** `dotnet build`, tests, and a final SF run proposing **zero** changes to the code, apart from files the user chose not to adopt yet (step 4, option 3). Its drift lines are step 8's.
                    - **An ambiguous name after adoption** (CS0104, e.g. `Order` from two namespaces the global usings import): add a using alias at the top of the file for the type it means (`using Order = eShop.Ordering.Domain.AggregatesModel.OrderAggregate.Order;`). An alias outranks every namespace import, global ones included, and later runs keep it. Don't leave the file unadopted.
                    8. **Finish the drift check (required).** §1 wrote `guardrails/services.drift.json`, so every run already compares the model's HTTP and security facts, and the types the Guardrails templates generate, with the code. Save the designer (the check reads the saved model), load **configure-drift-check** and do its steps 4 to 9 for the Services designer: fix the recipes until no fact is reported as can't be verified (a kind of fact with no recipe, or a style §1 missed) and validate-drift-recipes shows every recipe reading the code, then resolve the rest.
                    - No recipe file (the adoption began before §1 wrote one): do all of configure-drift-check's steps.
                    - The adoption isn't finished until a run finds no drift and no warnings, so it writes no `<<DriftReport>>`, and the user has approved the model changes (configure-drift-check's step 9). Skip that approval when an orchestrating skill's **User decision** says so.
                    - A contradiction is a recipe misreading the code, a modelling slip or real drift: check the code at its location first (configure-drift-check's step 6), fix a recipe that misreads it, and otherwise fix the model, never the code. Where the code looks wrong, say so in the final report.
                    - Code the model doesn't describe: model what §3 should have covered; add an exception for helpers and plumbing, asking the user about anything that isn't clearly one.

                    ## Gotchas

                    - **Concurrent edits.** Elements, associations or stereotypes you didn't create are the user's in-progress work. Leave them, don't apply their output, and report them.
                    - **Disabling templates.** Disabling a Template Output after its files were applied makes the SF propose deleting them all. Don't use it as a workaround after adoption.
                    - **Check a merge risk on the staged diff first.** Before excluding a file from generation or asking the user about something the merge might lose, stage it and read its diff. Ask only about a loss the diff shows.
                    - **DTO inheritance.** With `Intent.Modelers.Eventing` installed, `createAssociation("Generalization", …)` resolves to the Eventing DTO's Generalization, not the DTO's. Create a DTO's Generalization by its type id instead: `5ba12bbf-122f-4c3e-af3c-4a88dc554597`.
                    - **A concrete client's interface isn't guarded.** When a `Service Proxy` is the class, the interface it implements stays hand-written. A later signature change in the model changes only the class, so edit the interface by hand to match.
                    - **Proxy designer errors (not verified).** The Service Proxies modeler re-syncs proxy parameters from their mappings, so it may show an error on these unmapped proxies; it stops before touching any parameter. Report any such error; never add a mapping to silence it.
                    """.Replace("<<DriftReport>>", ModelSyncGuidance.ReportPath(outputTarget.ExecutionContext, "Services")));
        }

        [IntentManaged(Mode.Fully)]
        public override IMarkdownFile MarkdownFile { get; }

        [IntentManaged(Mode.Fully)]
        public override ITemplateFileConfig GetTemplateFileConfig() => MarkdownFile.GetConfig();

    }
}
