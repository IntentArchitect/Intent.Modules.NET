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

                    Goal: the Services designer describes the application layer **as it is** (requests, services, DTOs, endpoint surface, security, intent), and an SF run against the existing files changes **nothing but Intent file headers and the code's formatting**: what the code does stays exactly the same. Handlers, validators, mapping profiles and controllers stay hand-written. The last step configures the drift check, so every Software Factory run compares the model with the code, and the adoption ends with the run reporting no drift.

                    Run **reverse-engineer-domain** first when the code has a domain layer. Commands, DTOs and entity actions reference Domain types. Load **scripting-the-model**, **exploring-the-model** and **changing-the-model** before modelling, and **generating-and-applying-code** / **resolve-destructive-changes** before the first SF run.

                    ## 0. Preconditions

                    1. `Intent.Services.Guardrails.Contracts` must be installed. If it isn't, stop and ask.
                    2. **Detect what the code exposes and install the matching modules.** Running this skill is the user's authorization to install the metadata/modeler modules that apply. Detect from the code (controllers, `Program.cs`, csproj `PackageReference`s, attributes):

                    | Evidence in the code | Install (if not already installed) |
                    |---|---|
                    | MediatR-style requests (`IRequest<T>`, `ICommand` / `IQuery`, `*Command` / `*Query` classes) | `Intent.Modelers.Services.CQRS` |
                    | HTTP endpoints: `[ApiController]` / `ControllerBase`, minimal APIs (`app.MapGet/MapPost…`), FastEndpoints, Azure Functions HTTP triggers | `Intent.Metadata.WebApi`, version 4.7.22-pre.1 or later, so include prereleases in the search (earlier versions append a duplicate segment to a Route on every change when a route parameter is cased differently from its parameter, as in `{CacheKey}`, or is optional, defaulted or catch-all) |
                    | Authorization: `[Authorize(...)]`, `[AllowAnonymous]`, `RequireAuthorization(...)`, policies or roles | `Intent.Metadata.Security` (WebApi normally brings it as a dependency; install explicitly if missing) |
                    | Handlers or service methods that load, create, update or delete Domain entities via repositories or a DbContext, and a Domain designer exists | `Intent.Modelers.Services.DomainInteractions` |
                    | Pagination types (`PagedResult<T>`, `PagedList`) | none; reference the `Intent.Modelers.Services.Pagination` package (§3) |
                    | Integration events / messaging: consumers or handlers (`IConsumer<T>`, `IHandleMessages<T>`, `IIntegrationEventHandler<T>`), publish or send calls (`IEventBus`, `IMessageBus`, `IPublishEndpoint`, `ISendEndpointProvider`), MassTransit, NServiceBus, Azure Service Bus | `Intent.Modelers.Eventing` + `Intent.Modelers.Services.EventInteractions` |
                    | Domain event handlers: `INotificationHandler<…DomainEvent…>`, `IDomainEventHandler<T>`, `DomainEventNotification<T>` | `Intent.Modelers.Domain.Events` (run **reverse-engineer-domain** first, so the Domain Events exist to link to) |
                    | API clients: typed `HttpClient` wrappers, Refit, NSwag or Kiota clients, or any other class whose job is calling an API | `Intent.Modelers.Types.ServiceProxies`, version 5.5.8-pre.0 or later, so include prereleases in the search (earlier versions open a mapping dialog when a proxy is created, which blocks the tools, and can't create a proxy at the Services package root). Every client is modelled (§3), whatever API it calls. **Never** install `Intent.Application.Contracts.Clients` or `Intent.Integration.HttpClients` alongside it: they fail on the unmapped proxies this skill models. |

                    - Get the exact current version with `searchAvailableModules(id)`, then call `install_or_update_modules` and state in one line what you installed and why.
                    - Ask only when the evidence is ambiguous (e.g. gRPC or a custom transport).

                    3. Decompile the installed template (see **reverse-engineer-domain** §0) to confirm:
                    - **Path:** `GetFolderPath()` → path = Template Output + designer folders.
                    - **Name:** the class name is `Model.Name` verbatim.
                    - **Type kind:** the DTO kind. Older builds always emit `class`. Newer builds honour the **`Type Configuration`** stereotype (`Type`: `record` | `class` | `positional record`), and its `Modifiers` together with the `Default Type Modifiers` module setting (§3).

                    ## 1. Extract the code (read-only, in parallel)

                    Dispatch `Explore` sub-agents in one message, one per feature folder group (e.g. Customers+Products, Orders+Payments+Wishlists). Each slice also reads its controllers. Ask for:

                    - **Each command / query:**
                    - Path, namespace, name, kind (`class`, `record`, or positional `record CancelOrderCommand(int OrderNumber)`) and declaration modifiers (`public`, `internal` or none, plus `sealed`, `abstract`, `static`, `partial`).
                    - Every property: exact type **as spelled** (e.g. `Nullable<T>` vs `T?`, `System.Guid` vs `Guid`, partly qualified names such as `DTO.Logging.LogDTO`), nullability, collection-ness, default value and whether it is `required`.
                    - Return type (`Guid`, a DTO, a list of DTOs, `PagedResult<…>`, or nothing).
                    - `[Authorize(Roles/Policy)]` / `[AllowAnonymous]`.
                    - Brief validator rules.
                    - Its handler's intent (§5): the Domain entities it creates, updates, deletes or reads; the service operations, commands, queries, domain services, repository operations, stored procedures or API client methods it calls; and the integration events it publishes or integration commands it sends.
                    - **Each DTO:** path, namespace, `class` or `record`, declaration modifiers, and every property, reported as for commands (type as spelled, nullability, collection-ness, default value, `required`).
                    - **Each service:**
                    - Path, namespace, class name, declaration modifiers, and every method signature, with parameter and return types as spelled and the method's modifiers exactly as written (access, `static`, `virtual`, `override`, `sealed`, `new`).
                    - Whether it is **public** (a controller action or other endpoint calls its methods directly) or **internal** (only other application code calls it).
                    - Each method's intent, reported as for a command's handler.
                    - **Nullable context:** per project, whether the csproj sets `<Nullable>` (and to what); per file, any `#nullable` directives.
                    - **Each enum:** path, namespace, access modifier (`public`, `internal` or none), and literals with explicit values.
                    - **Each domain event handler:** path, namespace, class name, declaration modifiers, and the domain event(s) it handles.
                    - **Each integration event and integration command (message):**
                    - Path, namespace, name, kind (`class`, `record`, or positional `record OrderPlaced(Guid OrderId)`) and declaration modifiers.
                    - Whether it is published (an event) or sent to one receiver (a command), and from where.
                    - Every property, reported as for commands, plus its accessors as spelled (`{ get; set; }`, `{ get; init; }`, get-only).
                    - **Each nested type a message uses (an eventing DTO):** reported as for DTOs, plus its base type.
                    - **Each integration event handler or consumer:** path, namespace, class name, declaration modifiers, and the messages it handles (from `IConsumer<T>`, `IHandleMessages<T>`, `IIntegrationEventHandler<T>` or equivalent).
                    - **Each API client** (a typed `HttpClient` wrapper, a Refit, NSwag or Kiota client, or any other class whose job is calling an API):
                    - Which type to model: the **concrete client class**, or the **interface** only when the code has no concrete class because the implementation is generated at runtime (e.g. a Refit interface). Never the interface a concrete client implements.
                    - Path, namespace, name, `class` or `interface`, and declaration modifiers.
                    - Every method as spelled: name, return type, each parameter's type and default value, the CancellationToken parameter (its name, and whether it has `= default`), and whether the method is `async`.
                    - Anything §3's "Service proxies" can't reproduce: method modifiers other than `public` and `async`, explicit interface implementations, generic methods, and `ref`, `out` or `params` parameters.
                    - Where each request and response type comes from: declared in the code (a local copy of the API's contract), or from outside it (e.g. a NuGet contracts package).
                    - **Each controller action:**
                    - HTTP verb, **absolute route** (and the controller's route prefix), and success status code.
                    - Which properties or parameters bind from the route, query or body.
                    - Which command or query it dispatches, or which service operation it calls.
                    - How the code states each of these facts, as written: where the route, verb, binding and authorization are declared (on the action, on the controller class, on a base controller, or in `Program.cs`), and for FastEndpoints or minimal APIs the member that registers the endpoint (`Configure()`, `MapOrderEndpoints`). Note every style the code uses, with one verbatim example of each. The recipes below are written from this.
                    - **Shared types:** `PagedResult<T>`, marker interfaces, permission constants.
                    - **The convention of each element type:** for each type of element you will model (DTO, command, query, service, enum, integration event, handler, API client), whether all of its code shares something a new one would have too: a folder with a name pattern (every `*DTO` in the DTO project), a base class (`: BaseBusinessObject`), an interface (`IRequest<T>`), an attribute (`[ApiController]`), or a member. Report it with one verbatim example, or report that there is none.

                    **Then write the drift check's recipes, before modelling anything.** The sub-agents have just read every endpoint, and a file outlives this conversation's context, so don't leave this to the end. Load **configure-drift-check** for the format and write `guardrails/services.drift.json` from the sub-agents' reports, without re-reading the code:
                    - a recipe for each HTTP and security fact the code states (route, verb, binding, security), with one `find` per style;
                    - `inherit` where an action takes a value from its controller, and `default` for what the code states by saying nothing (no `[Authorize]`);
                    - an `own` entry for each kind of endpoint the model must describe (every controller action, minimal API mapping or endpoint class);
                    - an `own` entry for each element type that follows a convention, which finds every type following it, with `kind` naming the element type. An element type without a convention gets none: its new types can't be told apart from helpers, so they aren't detected.

                    Ask a sub-agent only about a style the reports don't cover. Writing the file turns the check on: from now on every Software Factory run compares the model with the code, and fails until they match. During adoption that is expected, and the report lists what isn't modelled yet. §7 step 8 finishes the check.

                    ## 2. Align the Codebase Structure

                    Move the Services Guardrails Template Outputs (CommandContract, QueryContract, DtoContract, EnumContract, ServiceContract, and when present DomainEventHandlerContract, IntegrationEventContract, IntegrationCommandContract, EventingDtoContract, IntegrationEventHandlerContract, ServiceProxyContract) to the **Application project folder that contains the feature folders**, usually the project root. They often start in the Api project.

                    - **Messages in their own project.** Integration events, integration commands and the eventing DTOs they use often live in a separate Contracts project (e.g. `Acme.Contracts`), shared with other services. Put the IntegrationEventContract, IntegrationCommandContract and EventingDtoContract Template Outputs in that project, at the folder its namespaces start from. Handlers usually stay with the Application project. If the code's enums sit with the messages, EnumContract needs a filtered copy there too (below).

                    - **API clients.** Concrete clients usually live in the Infrastructure project (e.g. `Acme.Infrastructure/HttpClients`). Put the ServiceProxyContract Template Output in the project that holds them, at the folder its namespaces start from. When clients sit in more than one project (e.g. Refit interfaces in Application, concrete clients in Infrastructure), duplicate it with mutually exclusive Registration Filters (below). Its model is a raw element, so filter on the designer folder with `np(ParentElement.Name)`, e.g. `np(ParentElement.Name) == "HttpClients"`.

                    - **One template, several locations.** A template is not limited to one Template Output. When a template's existing files live in more than one project or root folder (e.g. enums in both the DTO and Business projects), **duplicate the Template Output**, one copy per location. Give each copy a **Registration Filter** (on its `Template Output Settings` stereotype) that selects which model instances generate there.
                    - The filter is a Dynamic LINQ expression over the template's model (e.g. `EnumModel`, `DTOModel`, `ServiceModel`, `ClassModel`) and must return `bool`. `Folder.Name` is usually the simplest discriminator. Use `np(...)` for anything that can be null. Example: `np(Folder.Name) == "BusinessObject"` on the Business copy and `np(Folder.Name) != "BusinessObject"` on the DTO copy.
                    - When each service has its own package, filter on the package instead: `np(InternalElement.Package.Name) == "Ordering.Services"` for a template with a typed model (e.g. `DtoContract`, `ServiceContract`), and `np(Package.Name) == "Ordering.Eventing"` for one whose model is a raw element (e.g. `IntegrationEventContract`, `EventingDtoContract`).
                    - **Filters must be mutually exclusive.** An instance matching two outputs fails the SF. An empty filter matches everything, so when you add a filtered copy, also give the original the complementary filter.
                    - Script it: `project.addChild("Template Output", "<TemplateId>")`, then `ensureStereotype("Template Output Settings").setProperty("Registration Filter", "<expr>")`.
                    - Model those elements in a designer folder that the filter selects on, and combine with `File Config` (`Filename` / `Namespace`) to land each type in its exact existing file.
                    - `File Config` can't move a file to another project; a filtered Template Output can. Use Type-Definition placeholders only for types that can't be elements at all (nested types, `dynamic`, framework types).

                    Save the Codebase Structure designer (`saveOnSuccess: true` on a no-op script) once it is right.

                    ## 3. Model the services

                    1. **Ensure a package exists, then add references.**
                    - **Check first:** `getPackages()` on the Services designer.
                    - **If there are none,** create one named `{ApplicationName}.Services`, using the application name from the workspace context / `get_applications` (e.g. `Training.Ordering.Services`): `await createPackage("Training.Ordering.Services")`.
                    - **If one or more exist,** use the existing package and don't create another. If several exist, ask which one to use.
                    - **References:** add package references for the Domain package, `Intent.Common.Types` (primitive types), and `Intent.Modelers.Services.Pagination` (`PagedResult<TData>`) when the code pages.
                    - **Naming conventions, before creating anything.** Every name must stay exactly as the code spells it, but the Services designer renames elements to its `Service Settings` naming conventions: `Property`, `Entity` (Services too), `Operation` and `Parameter Naming Convention`. Parameter defaults to `Camel Case`, so a parameter declared `ObjectID` would become `objectID`. Read them with `getApplicationSettings({ search: "Naming Convention" })` (via `run_solution_query`), set every one that isn't `Manual` to `Manual` with `update_application_settings`, and state in one line what you changed. `Parameter Naming Convention` arrived in Intent.Modelers.Services 4.0.17, which this module's dependency installs; before it, every parameter was camel-cased whenever its operation changed.
                    2. **Folders mirror the code exactly.** The Guardrails templates do **not** add a per-use-case sub-folder. If the code is `Customers/AddAddress/AddAddressCommand.cs`, model folder `Customers/AddAddress` and put the command inside. DTOs that sit in `Customers/` go in the `Customers` folder.
                    - **Files that don't follow the convention:** each element is written to `<ElementName>.cs` in its folder's namespace. Apply the **`File Config`** stereotype only where the code differs, and set only the property you need; both are optional.
                    - **Namespace doesn't follow the folders:** set `Namespace` to the file's exact namespace. Every type that shares a file needs the same `Namespace`.
                    - **No namespace in the file** (types declared in the global namespace): tick `No Namespace` and leave `Namespace` empty.
                    - **File name differs, or the file holds several types:** set `Filename` (the file name only, with or without `.cs`; the folder stays the element's own), as below.
                    - **File named differently from its type** (e.g. `psMembersDTO.cs` holds `PsMembersDTO`): apply `File Config` with `Filename = psMembersDTO`. If the name starts or ends with a space (e.g. `abc .cs`), include `.cs`: with it the name is used exactly as written, without it surrounding spaces are trimmed.
                    - **File holding several types:** model every type as its own element in that file's folder, and apply `File Config` with the file's name to each type the file is *not* named after. Elements that end up at the same path are generated into one file, written by the element the file is named after.
                    - **Set it before the first SF run.** If a type already had its own file generated and you then point it at another file, the SF offers to delete the old file. Treat that as a destructive change: don't apply it unless the old file is a duplicate.
                    3. **DTOs first** (dependency order), then commands, queries and services:
                    - **A type name used in more than one package** (each project's own copy of an event, DTO or enum): set the type by the element's id (`setType(el.getId())`). A name that matches several elements fails the script.
                    - **Fields:** one `DTO-Field` per property, with exact names and types. `isNullable` for nullable types (see "Nullability and type spelling" below), `isCollection` for lists (never `List<…>` as a type name). A property initializer (`= "Basislink"`) is the field's Value (see "Default values" below).
                    - **Services:** one `Service` per application service class, named as the concrete class (no interface is generated), with one `Operation` per method and one `Parameter` per parameter, typed as for fields. A service is one of two kinds, and both are modelled the same way here:
                    - **Public:** a controller action or other endpoint calls its operations directly, so each operation is that endpoint's request, exactly like a command or query. It gets HTTP settings and security (§4) and its intent (§5).
                    - **Internal:** only other application code calls it. It gets no HTTP settings, but still gets its intent (§5).
                    - **Default values and initializers:** where a service operation parameter has a default (`string area = "Basislink"`, `bool forceCreate = false`, `Exception ex = null`), or a DTO, command or query property has an initializer, set the element's Value (`el.setValue("Basislink")`). Otherwise Merge mode drops the default from the existing signature.
                    - Strings are quoted for you, and an already quoted value stays as it is. `null`, `default`, numbers, `true` and `false` are used as written.
                    - An enum literal name gets its enum type (`Info` becomes `EventTypeEnum.Info`), and a bare `DateTime` member gets its type (`Now` becomes `DateTime.Now`).
                    - Anything that looks like an expression (contains `.` or `(`, or starts with `_`) is used as written. Prefix any other expression with `@` to pass it through verbatim (`@DefaultArea` gives `DefaultArea`).
                    - A `ref`, `out` or `params` parameter can't have a default; that combination is an error on the element.
                    - **Collections:** the template emits `List<T>`. Wherever the code uses anything else (`IEnumerable<T>`, `IList<T>`, `IQueryable<T>`, `T[]`, …), apply the **`Collection Type`** stereotype to that field, service operation parameter or service operation (for its return type) and set `Type` to match (`Custom` + `Custom Format` with `{0}` for anything else). Otherwise Merge mode rewrites the existing signature.
                    - **Nullability and type spelling.** Model the type, not its spelling:
                    - **Nullable value types:** set `isNullable` for `T?` **and** for `Nullable<T>` / `System.Nullable<T>`. They are the same type.
                    - **`string?` (or any reference type with `?`) with nullable reference types off:** nullable reference types are off when the csproj has no `<Nullable>enable</Nullable>` (or `annotations`) and the file has no `#nullable enable`. The `?` then does nothing, and the compiler warns CS8632. Model the member as a nullable `string` anyway. The template emits `string`, which removes the meaningless `?`. That fixes the code, but it is a change beyond the headers, so handle it under "Equivalent type rewrites" (§7). Never drop the member.
                    - **Nullable reference types on:** when the csproj enables `<Nullable>`, make sure the project's Nullable option in the Codebase Structure designer matches, so the template emits `string?`. A `string?` → `string` diff in that case is real drift: fix the project setting, not the member.
                    - **Other spellings of the same type** (`System.Guid`, `Int32`, `String`, or a partly or fully qualified name such as `DTO.Logging.LogDTO`): model the type itself (`guid`, `int`, `string`, `LogDTO`). Depending on the RoslynWeaver version, the SF may rewrite the code's spelling to the template's. Handle that under "Equivalent type rewrites" (§7).
                    - **Parameter modifiers:** where a service operation parameter is declared `ref`, `out`, `in`, `ref readonly` or `params`, apply the **`Parameter Modifier`** stereotype and set `Modifier` to match (for `params`, also set `Collection Type = Array`). Otherwise Merge mode drops the modifier from the existing signature. An extension method (`this` parameter) can't be reproduced; leave it unmodelled and report it.
                    - **Parameter attributes** (`[FromBody]`, `[FromHeader(Name = "x-requestid")]`, `[AsParameters]`, `[FromServices]`): nothing to model. Merge keeps a parameter's attributes on a fully managed signature, so an endpoint class whose parameters have them is adopted like any other.
                    - **Async operations:** settle every service operation's async shape before the first SF run, exactly as **reverse-engineer-domain** §3 "Async operations" describes. This module's `Async Configuration` has the same properties (`Async Mode`, `Task Type`, `Include Cancellation Token`, `Cancellation Token Name`, `Cancellation Token Default`) and the same rules, and an operation without it is async when its name ends in `Async`.
                    - **Operation modifiers:** method signatures are fully managed, so a service operation's modifiers must match the code exactly. An operation is `public` unless the **`Operation Configuration`** stereotype says otherwise: set `Access Modifier` (`public`, `protected`, `internal`, `private`, `protected internal`, or `none` for a method declared without one) and tick `Static`, `Virtual`, `Override`, `Sealed` (only with `Override`) or `New` as the code declares; `async` stays on `Async Configuration`. A combination C# rejects is an error on the element. `private protected` can't be reproduced; leave that operation unmodelled and report it.
                    - **Required properties:** where a field's property is declared `required`, apply the **`Property Configuration`** stereotype and tick `Required`. On its first merge, Merge mode replaces a property's modifiers with the generated ones, so an unmodelled `required` is dropped. Accessors need nothing: Merge keeps an existing `init`, `private set` or get-only accessor as written. A positional record's fields are constructor parameters, so `Required` is ignored on them.
                    - **Property access modifiers:** a field's property is `public` unless the **`Property Configuration`** stereotype sets `Access Modifier` (`public`, `protected`, `internal`, `private`, `protected internal`, or `none` for a property declared without one). Model the code's own modifier: on its first merge, Merge mode replaces a property's modifiers with the generated ones, so an unmodelled `protected` or `internal` becomes `public`. Ignored on a positional record's fields.
                    - **Return type:**
                    - Plain type: `setType("guid")` or `setType("CustomerDetailDto")`.
                    - Paged: `setType("PagedResult<CustomerSummaryDto>")`.
                    - List: `setType({ name: "WishlistSummaryDto", isCollection: true })`.
                    - Nothing: leave it unset (void).
                    - **Domain enums:** use them directly as field types.
                    - **`Type Configuration`** (newer module only). Apply it **only when required**: set `Type = record` on a DTO or request that is a `record` in code. Leave the default for classes.
                    - **Positional record** (`public record CancelOrderCommand(int OrderNumber) : IRequest<bool>;`, nothing but a parameter list): set `Type = positional record`. The fields, in model order, are the positional parameters (model them PascalCase, as declared), and a field's Value is the parameter's default. Base types and interfaces stay in the code. A record with a parameter list **and** body properties can't be reproduced yet; leave it unmodelled and report it.
                    - **Positional inheritance:** a DTO whose base DTO is a positional record must be one too, and its own fields must include the base's (passed on by name: `record ChildDto(int Id, string Name) : BaseDto(Id)`). A Service can't be a positional record. If the stereotype doesn't exist, the code has records, and the template emits `class`, **ask the user**: convert the records to classes, or wait for the module. Merging a `class` into a `record` file adds a duplicate type.
                    - **Type modifiers:** Merge mode replaces each class, record or enum declaration's modifiers with the generated ones, so they must match the code.
                    - **Set the convention once, before the first SF run.** If most of the code's declarations aren't plain `public` (e.g. `internal sealed`, or `sealed` with no access modifier), set the application setting `Default Type Modifiers` (group `Services Guardrails Contracts Settings`) to the dominant combination. Its options are `public` or `internal`, each optionally followed by `sealed`, `partial` or `sealed partial`; `sealed`, `partial` or `sealed partial` with no access modifier; and `none` for no modifiers at all. Read it with `getApplicationSettings({ search: "Default Type Modifiers" })` (via `run_solution_query`), change it with `update_application_settings`, and state in one line what you set and why. The Domain module has its own setting of the same name.
                    - **The setting's `sealed` is left off automatically** for an abstract DTO and for a DTO another DTO inherits from.
                    - **Exceptions:** wherever a type differs from the setting, apply `Type Configuration` and set `Modifiers` to the code's keywords exactly as written, e.g. `public`, `sealed` or `internal static`: at most one of `public` / `internal`, plus any of `sealed`, `abstract`, `static`, `partial`, in any order. Omit the access modifier when the code has none (never write `internal` in its place), and use `none` on its own when the declaration has no modifiers at all, because an empty `Modifiers` means the setting applies. `Is Abstract` on a DTO still adds `abstract`. An invalid or conflicting value (e.g. `sealed` on an abstract DTO, or `none` with another keyword) is an error on the element.
                    - **Enums** take only the setting's access modifier (`internal sealed` gives `internal enum`, and a setting without one, such as `sealed` or `none`, gives `enum`). Where an enum differs, apply `Enum Configuration` and set `Access Modifier` to `public`, `internal`, or `none` for an enum declared without one.
                    - **Can't be reproduced:** `file`-scoped and `unsafe` types. Leave them unmodelled and report them.
                    - **Leaving members out.** The "leave it unmodelled and report it" rules in this section (extension methods, `file`-scoped and `unsafe` types, and the integration eventing and service proxy shapes below) apply only to shapes the model can't express at all. A type that is merely spelled differently is never a reason to leave a member out (§7, "Equivalent type rewrites").
                    - **Domain event handlers** (when `Intent.Modelers.Domain.Events` is installed): one `Domain Event Handler` element per handler class, in the folder that matches its code folder, named verbatim, with an association to each Domain Event it handles. The contract is class-only: it guards the class, its file and its namespace. Merge mode keeps the `Handle` methods and handler interfaces. `File Config` applies to handlers too. Handlers have their own `Type Configuration` with only `Modifiers` (handlers are often `internal sealed`).
                    - **Integration eventing** (when `Intent.Modelers.Eventing` and `Intent.Modelers.Services.EventInteractions` are installed):
                    - **Package and folders:** integration events, integration commands and eventing DTOs live in an `Eventing Package` in the Services designer. Use the existing one; if there is none, create one: `await createPackage("Training.Ordering.Eventing", "Eventing Package")`, named after the application. Folders mirror the code, as for DTOs.
                    - **Event or command:** a type published to any subscriber is an Integration Event (element type `Message`); a type sent to one receiver is an `Integration Command`. Name it verbatim; no `Event` suffix is added. Add one `Property` per property, typed and flagged as for DTO fields. A `Property` has no Value, so a message property initializer can't be modelled: report it.
                    - **Eventing DTOs:** one `Eventing DTO` per nested type a message uses, with one `Eventing DTO-Field` per property (an initializer is the field's Value), `Is Abstract`, its generic types, and a Generalization (`New Inheritance`) to its base Eventing DTO.
                    - **Enums** used by messages go in the eventing package too. EnumContract generates them like any other enum.
                    - **Handlers:** one `Integration Event Handler` per consumer or handler class, in the Services package folder that matches its code folder, named verbatim, with a `Subscribe Integration Event` (or `Subscribe Integration Command`) association to each message it handles. The contract is class-only, as for domain event handlers: Merge mode keeps the handle methods (`Consume`, `Handle`, `HandleAsync`) and the handler interfaces. The associations document the subscription; the template doesn't read them.
                    - **Stereotypes:** `File Config` applies to all four element types. Integration events, integration commands and eventing DTOs have their own `Type Configuration` (`Type` and `Modifiers`), and integration event handlers share the handler `Type Configuration` (`Modifiers` only). `Collection Type` and `Property Configuration` apply to message properties and eventing DTO fields. They work as described above; the setting's `sealed` is left off an abstract eventing DTO and one another eventing DTO inherits from, and an eventing DTO must have the same `Type` as its base.
                    - **Only this application's own packages are generated.** A message or enum modelled in another application's eventing package that this application references (the usual subscriber setup) is never written here. If this code keeps its own copy of such a message, it can't be guarded yet: leave it unmodelled and report it.
                    - **Positional records** (`record OrderPlaced(Guid OrderId)`): set `Type Configuration` → `Type` to `positional record`, as for commands and DTOs (above).
                    - **Accessors:** the template always emits `{ get; set; }`. What Merge mode does to an existing `init` or get-only accessor hasn't been verified. If the SF diff changes an accessor on any property (messages, DTOs, commands or queries), don't apply that file; list it in the final report.
                    - **Service proxies** (when `Intent.Modelers.Types.ServiceProxies` is installed): one `Service Proxy` per API client the code declares (§1), purely as a picture of the code.
                    - **Which type:** the concrete client class, named verbatim (e.g. `PaymentsClient`). The interface it implements (e.g. `IPaymentsClient`) isn't modelled: it stays hand-written, and Merge keeps the class's `: IPaymentsClient`. Only when the code has no concrete class, because the implementation is generated at runtime (e.g. a Refit interface), model the interface, named verbatim (`IGitHubApi`), and set `Service Proxy Configuration` → `Type` to `interface`. Never model both for one client.
                    - **Where:** in the Services package, in the folder that matches the client's code folder, or at the package root for a client in the Template Output's own folder.
                    - **By script, and never with a mapping.** Create the proxy with `createElementUnder(folder, "Service Proxy", "PaymentsClient")` (or `createElementUnder(servicesPackage, …)` at the root), then one `Operation` per method (`proxy.addChild("Operation", "GetPaymentAsync")`) and one `Parameter` per parameter (`operation.addChild("Parameter", "id")`; the designer hides "Add Parameter" on proxy operations, but a script can add one). Type them as for service operations: a Parameter must have a type, and an Operation without one returns `void`. Never map a proxy or its operations to another application's endpoints, and don't model the remote API: the proxy represents the client code only, and the template ignores mappings.
                    - **Request and response types:** a type the client code declares itself (a local copy of the API's contract) is a `DTO`, `Command`, `Query` or `Enum` in this application's package, modelled and guarded like the others. A type from outside the code (e.g. a NuGet contracts package) is a `Type-Definition`, which nothing generates.
                    - **Async, collections and defaults:** a proxy Operation has its own `Async Configuration`, with the same properties and rules as a service operation's ("Async operations" above), and an operation without it is async when its name ends in `Async`. A class method gets the `async` keyword per `Async Mode`; an interface method never does. `Collection Type` applies to a proxy Operation (for its return type) and Parameter. A proxy Parameter has no Value, so where the code declares a default (`int page = 1`), apply **`Default Value`** and set `Value`, formatted as for service operation parameters ("Default values and initializers" above); otherwise Merge mode drops the default. The CancellationToken's default stays on `Async Configuration`.
                    - **Declaration:** a class takes the `Default Type Modifiers` setting, or `Service Proxy Configuration` → `Modifiers`, with the same rules as `Type Configuration` → `Modifiers`. An interface takes only the setting's access modifier, or its own `Modifiers`: at most one of `public` / `internal`, plus `partial`, or `none`. Class methods are written `public`, interface methods with no access modifier. `File Config` applies to proxies too.
                    - **Attributes and base types stay in the code.** The template generates none, and Merge keeps the code's own: a class's base list and Refit's `[Get("/users/{user}")]` on an interface method.
                    - **Can't be reproduced yet:** a concrete client's method declared with modifiers other than `public` and `async` (`virtual`, `override`, `sealed`, `new`, or another access modifier), an explicit interface implementation (`Task<PaymentDto> IPaymentsClient.GetPaymentAsync(...)`), a generic method (a proxy Operation can't have generic parameters), and a method with `ref`, `out` or `params` parameters. Leave that method unmodelled (Merge keeps it as written) and report it.
                    - **Only this application's own packages are generated,** as for integration eventing.
                    4. **Validate** (`getDesignerValidationErrors`), then save the designer (`saveOnSuccess: true`). A script-created package is invisible to the SF until saved.

                    ## 4. Endpoint surface and security (HTTP solution; WebApi/Security metadata installed in §0)

                    For each command, query and **public** service operation (§3), set the following from the controller action or endpoint that calls it. A public service operation is an endpoint's request in the same way a command or query is.

                    - **`Http Settings`:**
                    - `Verb` and `Route`. A command or query takes the **absolute** route (e.g. `api/customers/{customerId}/addresses`). A service operation's route is relative to its service: put the controller's route prefix on the Service as `Http Service Settings` → `Route` (e.g. `api/customers`), and give each operation the rest (`{customerId}/addresses`). A `Route` starting with `~/` ignores the prefix. If a service's operations are called from controllers with different prefixes, leave `Http Service Settings` off and give each operation its absolute route.
                    - **API versions read from the query string** (`HasApiVersion(...)` with Asp.Versioning's default `?api-version=` reader): API versions make the designer add a `{version}` segment to a route that has none. Put it in the Service's route as the query string instead (`api/orders?api-version={version}`), and have the `service.route` recipe read the code's group route that way.
                    - `Success Response Code` from the actual result: `Created(...)` → `201 (Created)`, `NoContent()` → `204 (No Content)`, `Ok(...)` → `200 (Ok)`.
                    - **`Parameter Settings.Source`:**
                    - `From Route` for every field or operation parameter whose camelCase name is a `{routeParam}`.
                    - `From Query` for the remaining fields or parameters of GET and DELETE actions.
                    - Leave body fields at the default.
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

                    - **Model every intent, not every step.** Everything the code does on purpose gets its association: an operation that updates an order, invokes `NotificationService.SendConfirmation` and publishes `OrderUpdated` gets all three. Don't add what an intent already implies: updating an entity reads it first, so `Update Entity Action` alone is enough, and cancelling an order that also fails its payment inside the aggregate is one `Update Entity Action` → `Order`.
                    - **Invoke, don't repeat.** When an operation delegates to another service operation, command or query, model the `Perform Invocation` only. The callee carries its own intent; don't copy it onto the caller.
                    - **The target must be in the model.** If what the code calls isn't modelled (a private helper, or a client method §3 couldn't reproduce), add no association; say what it calls in the element's comment, and report it.
                    - **No intent, no association.** An operation that only calculates or formats gets none.
                    - **When unsure, ask** the user, in one `ask_user_question` for all the uncertain elements, rather than skipping them.
                    - **Projections.** If a handler or service operation projects one entity through another (e.g. listing payments via orders), still target the conceptually primary entity and mention the projection in the comment.
                    - **Mappings.** Create the associations without field mappings (the designer's default). Add mappings only if the user asks.

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

                    - All commands, queries and services in **one vertical column on the left**, grouped by feature folder. Commands and queries go exactly **100px apart top-to-top**. A service grows with its operations, so start the next element **100px below the service's bottom edge**.
                    - The related Domain entities **200px to the right**, grouped by feature, together with any domain services, repositories and stored procedures the left column invokes.
                    - If the diagram can show them, the integration events, integration commands and service proxies the left column publishes, sends or invokes go in a further column **200px to the right** of the entities.
                    - Route every intent association (§5). Invocations between commands, queries and services stay within the left column. DTOs can't be placed on this diagram.

                    ## 7. Generate, verify and adopt

                    Same as **reverse-engineer-domain** §6:

                    1. **Every change must be a header or formatting change.** Expect `using` + `IntentTemplate`, the two merge directives, the csproj package reference, and formatting from RoslynWeaver's Auto Format anywhere in the file (step 5). The only other exception is an equivalent type rewrite the user accepts (step 4). While there is drift, a run also lists the drift report, `<<DriftReport>>` (in the solution's git-ignored `.cache` folder), as a new file; it isn't code and needs no review. Until step 8, its drift lines fail the run, so the Software Factory applies nothing by itself (step 6 applies by path anyway). They aren't for step 2; read its rows that say something is modelled but not in the code, or isn't modelled, as a second view of what is still mismatched or missing.
                    2. **Diagnose anything else:** `create` means a path or name mismatch, or a type that lives in another file without `File Config` (§3). Member inserts mean model drift. Deletes, and changed lines that aren't just formatting (step 5), mean destructive drift (use **resolve-destructive-changes**). A duplicate type means the record/class kind is wrong. A changed type declaration (e.g. `internal sealed class` rewritten to `public class`) means the modifiers are wrong: fix `Default Type Modifiers`, or that element's `Type Configuration` → `Modifiers` (`Enum Configuration` → `Access Modifier` for an enum; §3). **A type spelled differently on an existing member** (`Nullable<DateTime>` → `DateTime?`, `DTO.Logging.LogDTO` → `LogDTO`, `string?` → `string`): see "Equivalent type rewrites" (step 4). Don't remove the member.
                    3. **Merge directives:** every adopted file needs both of these lines, in this order:

                    ```csharp
                    [assembly: DefaultIntentManaged(Mode.Merge)]
                    [assembly: DefaultIntentManaged(Mode.Merge, Signature = Mode.Fully, Attributes = Mode.Merge, Comments = Mode.Merge, Targets = Targets.Constructors | Targets.Methods)]
                    ```

                    The current templates write both. Add them if the template didn't, **before** the next model change. Without the first, the SF strips `[Authorize]`, `IRequest<T>, ICommand` base types, XML docs and constructors. The second fully manages constructor and method signatures, and merges their attributes and comments.
                    4. **Equivalent type rewrites — ask, never drop.** An equivalent type rewrite is an SF diff where the only change to an existing member is how a type is spelled, and both spellings mean the same type:

                    | Code | Template |
                    |---|---|
                    | `Nullable<DateTime>`, `System.Nullable<int>` | `DateTime?`, `int?` |
                    | `System.Guid`, `Int32`, `String` | `Guid`, `int`, `string` |
                    | `DTO.Logging.LogDTO` (partly or fully qualified) | `LogDTO` plus a `using` |
                    | `string?` with nullable reference types off | `string` |

                    - **Never remove a member from the model, or leave one out, to avoid an equivalent rewrite.** This covers properties, fields, parameters and operations. The model must mirror the code; a smaller model that keeps the diff free of type rewrites defeats the purpose.
                    - **Confirm it is really equivalent:**
                    - the same type after resolving usings and enclosing namespaces;
                    - `Nullable<T>` vs `T?`;
                    - for `string?`, that nullable reference types are off.

                    Anything else is real drift, so fix the model. That includes a different type, nullability added or removed on a value type, or a changed collection type.
                    - **Ask the user once, for all of them together,** with `ask_user_question`, **before** applying any affected file. List each as `File:line — before → after`. Offer:
                    1. **Accept the rewrites (recommended).** Keep the members in the model and apply the files. Only the spelling changes; behaviour is identical. For `string?` with nullable reference types off, this also clears warning CS8632.
                    2. **Keep the code as written; leave these members out of the model.** The files keep their spelling, but the model is incomplete. Put a comment on each affected element saying what is missing and why.
                    3. **Model them, but don't adopt the affected files yet.** Apply every other file, and leave these until a RoslynWeaver build keeps the existing spelling. Until then, the SF keeps proposing these changes, so the zero-change check doesn't pass for those files.
                    - **Follow the answer, and state it in the final report** with the list of affected members.
                    5. **Formatting changes — accept, never suppress.** RoslynWeaver's Auto Format is on by default and lays out each adopted file to the C# style rules: indentation, spaces around operators, commas and colons, trailing spaces, blank lines and line breaks. It can touch hundreds of lines, including inside hand-written method bodies (`GetFileName(link): null` → `GetFileName(link) : null`). That is expected: it changes the layout, never what the code does, and brings the code in line with its coding standards.
                    - **Confirm it is formatting only:** the same tokens in the same order, with only the whitespace between them changed. String literal contents and comment text are unchanged. Anything else (a token added, removed, reordered or renamed) is not formatting; diagnose it under step 2.
                    - **Apply formatting changes with the rest of the file.** They need no question to the user.
                    - **Never turn off Auto Format** (the `Auto Format by Default` application setting, under `RoslynWeaver Settings`), and never suppress it any other way, to make a diff smaller or header-only. That hides harmless fixes, and with it off, the members the SF rewrites (declarations with attributes, for example) lose their indentation. If it is already off, ask the user whether to turn it back on (recommended); don't change it without asking.
                    - **Reviewing a large diff:** read past the whitespace. What is left must be the header changes from step 1 or equivalent type rewrites (step 4). If the files are under git and had no other uncommitted changes, `git diff -w -- <file>` after applying shows exactly what is left; revert any file that shows more, and diagnose it under step 2.
                    - **In the final report,** say how many files were reformatted, so the user expects the whitespace changes in their commit.
                    6. **Apply** the reviewed files only (`apply_staged_file_changes` with explicit paths): files whose only changes are the headers and formatting, plus files whose equivalent rewrites the user accepted (step 4).
                    7. **Finish with:** `dotnet build`, tests, and a final SF run proposing **zero** changes to the code, apart from files the user chose not to adopt yet (step 4, option 3). Its drift lines are step 8's.
                    - **An ambiguous name after adoption** (CS0104, e.g. `Order` from two namespaces the global usings import): add a using alias for the type the file means at the top of the file (`using Order = eShop.Ordering.Domain.AggregatesModel.OrderAggregate.Order;`). An alias outranks every namespace import, global ones included, and later runs keep it. Don't leave the file unadopted.
                    8. **Finish the drift check (required).** §1 wrote `guardrails/services.drift.json`, so every run already compares the Services model's HTTP and security facts, and the types the Guardrails templates generate, with the code. Load **configure-drift-check** and do its steps 4 to 9 for the Services designer: fix the recipes until no fact is reported as can't be verified (a kind of fact with no recipe, or a style §1 missed) and validate-drift-recipes shows every recipe reading the code, then resolve the rest. Save the designer first: the check reads the saved model.
                    - No recipe file (the adoption began before §1 wrote one): do all of configure-drift-check's steps.
                    - The adoption isn't finished until a run finds no drift and no warnings, so it writes no `<<DriftReport>>`, and the user has approved the model changes (configure-drift-check's step 9). The exception is an orchestrating skill's **User decision** to skip that approval: then list the model changes in your final report instead.
                    - A contradiction it finds is a recipe misreading the code, a modelling slip or real drift. Check the code at its location first (configure-drift-check's step 6): fix a recipe that misreads it, and otherwise fix the model to match the code, never the code. Where the code looks wrong, say so in the final report; setting up the check never changes the code.
                    - Code the model doesn't describe: model what §3 should have covered; add an exception for helpers and plumbing, asking the user about anything that isn't clearly one.

                    ## Gotchas

                    - **Concurrent edits.** Elements, associations or stereotypes you didn't create are the user's in-progress work. Leave them, don't apply their output, and report them.
                    - **Disabling templates.** Disabling a Template Output after its files were applied makes the SF propose deleting them all. Don't use it as a workaround after adoption.
                    - **Check a merge risk on the staged diff first.** Before excluding a file from generation or asking the user about something the merge might lose, stage it and read its diff. Ask only about a loss the diff shows.
                    - **Auto Format.** Never turn it off to make adoption diffs smaller (§7 step 5).
                    - **A concrete client's interface isn't guarded.** When a `Service Proxy` is the class, the interface it implements stays hand-written. A later signature change in the model changes only the class, so edit the interface by hand to match.
                    - **Service proxy limits.** Methods with modifiers other than `public` and `async`, explicit interface implementations, generic methods and `ref` / `out` / `params` parameters can't be reproduced yet; leave those methods unmodelled and report them (§3).
                    - **Intent's own client generators.** Never install `Intent.Application.Contracts.Clients` or `Intent.Integration.HttpClients` with this module: they fail on unmapped proxies.
                    - **Proxy designer errors (not verified).** The Service Proxies modeler re-syncs proxy parameters from their mappings when the Services package loads and when a proxy changes, and its script assumes every proxy operation is mapped. On these unmapped proxies it may show an error; it stops before touching any parameter, so nothing is deleted. Report any such error to the user; never add a mapping to silence it.
                    """.Replace("<<DriftReport>>", ModelSyncGuidance.ReportPath(outputTarget.ExecutionContext, "Services")));
        }

        [IntentManaged(Mode.Fully)]
        public override IMarkdownFile MarkdownFile { get; }

        [IntentManaged(Mode.Fully)]
        public override ITemplateFileConfig GetTemplateFileConfig() => MarkdownFile.GetConfig();

    }
}
