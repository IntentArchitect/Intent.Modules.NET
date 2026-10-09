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
description: Reverse-engineer an existing C# application layer (CQRS commands/queries, DTOs, services, controllers/endpoints, authorization, integration events/commands and their handlers) into the Intent Architect Services designer so the model mirrors the code and the Intent.Services.Guardrails.Contracts module adopts the existing files in place — including HTTP exposure, security and the intent of each command, query and service operation (entity actions, invocations, published events and sent commands), installing the WebApi/Security/DomainInteractions metadata modules when the code calls for them. Use when the user has installed the Services Guardrails module (or asks to "align / model / reverse-engineer the services") on a brownfield codebase.
short-description: Model existing commands, queries, DTOs and endpoints in the Services designer and adopt their files with the Services Guardrails module.
argument-hint: "[application name]"
---

# Reverse-engineer the Services

Goal: the Services designer describes the application layer **as it is** (requests, services, DTOs, endpoint surface, security, intent), and an SF run against the existing files changes **nothing but Intent file headers and the code's formatting**: what the code does stays exactly the same. Handlers, validators, mapping profiles and controllers stay hand-written.

Run **reverse-engineer-domain** first when the code has a domain layer. Commands, DTOs and entity actions reference Domain types. Load **scripting-the-model**, **exploring-the-model** and **changing-the-model** before modelling, and **generating-and-applying-code** / **resolve-destructive-changes** before the first SF run.

## 0. Preconditions

1. `Intent.Services.Guardrails.Contracts` must be installed. If it isn't, stop and ask.
2. **Detect what the code exposes and install the matching modules.** Running this skill is the user's authorization to install the metadata/modeler modules that apply. Detect from the code (controllers, `Program.cs`, csproj `PackageReference`s, attributes):

   | Evidence in the code | Install (if not already installed) |
   |---|---|
   | MediatR-style requests (`IRequest<T>`, `ICommand` / `IQuery`, `*Command` / `*Query` classes) | `Intent.Modelers.Services.CQRS` |
   | HTTP endpoints: `[ApiController]` / `ControllerBase`, minimal APIs (`app.MapGet/MapPost…`), FastEndpoints, Azure Functions HTTP triggers | `Intent.Metadata.WebApi` |
   | Authorization: `[Authorize(...)]`, `[AllowAnonymous]`, `RequireAuthorization(...)`, policies or roles | `Intent.Metadata.Security` (WebApi normally brings it as a dependency; install explicitly if missing) |
   | Handlers or service methods that load, create, update or delete Domain entities via repositories or a DbContext, and a Domain designer exists | `Intent.Modelers.Services.DomainInteractions` |
   | Pagination types (`PagedResult<T>`, `PagedList`) | none; reference the `Intent.Modelers.Services.Pagination` package (§3) |
   | Integration events / messaging: consumers or handlers (`IConsumer<T>`, `IHandleMessages<T>`, `IIntegrationEventHandler<T>`), publish or send calls (`IEventBus`, `IMessageBus`, `IPublishEndpoint`, `ISendEndpointProvider`), MassTransit, NServiceBus, Azure Service Bus | `Intent.Modelers.Eventing` + `Intent.Modelers.Services.EventInteractions` |
   | Domain event handlers: `INotificationHandler<…DomainEvent…>`, `IDomainEventHandler<T>`, `DomainEventNotification<T>` | `Intent.Modelers.Domain.Events` (run **reverse-engineer-domain** first, so the Domain Events exist to link to) |
   | Calls to another application's API through a client (Refit, NSwag or Kiota clients, typed `HttpClient` wrappers) | `Intent.Modelers.Types.ServiceProxies`, only if that application is modelled in Intent and its Services package can be referenced. Otherwise report the calls (§5). |

   - Get the exact current version with `searchAvailableModules(id)`, then call `install_or_update_modules` and state in one line what you installed and why.
   - After installing, a designer tab may open a dialog that blocks tools. Ask the user to close it, then continue.
   - Ask only when the evidence is ambiguous (e.g. gRPC or a custom transport).

3. Decompile the installed template (see **reverse-engineer-domain** §0) to confirm:
   - **Path:** `GetFolderPath()` → path = Template Output + designer folders.
   - **Name:** the class name is `Model.Name` verbatim.
   - **Type kind:** the DTO kind. Older builds always emit `class`. Newer builds honour the **`Type Configuration`** stereotype (`Type`: `record` | `class`), and its `Modifiers` together with the `Default Type Modifiers` module setting (§3).

## 1. Extract the code (read-only, in parallel)

Dispatch `Explore` sub-agents in one message, one per feature folder group (e.g. Customers+Products, Orders+Payments+Wishlists). Each slice also reads its controllers. Ask for:

- **Each command / query:**
  - Path, namespace, name, kind (`class` or `record`) and declaration modifiers (`public`, `internal` or none, plus `sealed`, `abstract`, `static`, `partial`).
  - Every property: exact type **as spelled** (e.g. `Nullable<T>` vs `T?`, `System.Guid` vs `Guid`, partly qualified names such as `DTO.Logging.LogDTO`), nullability, collection-ness and default value.
  - Return type (`Guid`, a DTO, a list of DTOs, `PagedResult<…>`, or nothing).
  - `[Authorize(Roles/Policy)]` / `[AllowAnonymous]`.
  - Brief validator rules.
  - Its handler's intent (§5): the Domain entities it creates, updates, deletes or reads; the service operations, commands, queries, domain services, repository operations, stored procedures or other applications' API clients it calls; and the integration events it publishes or integration commands it sends.
- **Each DTO:** path, namespace, `class` or `record`, declaration modifiers, and every property, reported as for commands (type as spelled, nullability, collection-ness, default value).
- **Each service:**
  - Path, namespace, class name, declaration modifiers, and every method signature, with parameter and return types as spelled.
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
- **Each controller action:**
  - HTTP verb, **absolute route** (and the controller's route prefix), and success status code.
  - Which properties or parameters bind from the route, query or body.
  - Which command or query it dispatches, or which service operation it calls.
- **Shared types:** `PagedResult<T>`, marker interfaces, permission constants.

## 2. Align the Codebase Structure

Move the Services Guardrails Template Outputs (CommandContract, QueryContract, DtoContract, EnumContract, ServiceContract, and when present DomainEventHandlerContract, IntegrationEventContract, IntegrationCommandContract, EventingDtoContract, IntegrationEventHandlerContract) to the **Application project folder that contains the feature folders**, usually the project root. They often start in the Api project.

- **Messages in their own project.** Integration events, integration commands and the eventing DTOs they use often live in a separate Contracts project (e.g. `Acme.Contracts`), shared with other services. Put the IntegrationEventContract, IntegrationCommandContract and EventingDtoContract Template Outputs in that project, at the folder its namespaces start from. Handlers usually stay with the Application project. If the code's enums sit with the messages, EnumContract needs a filtered copy there too (below).

- **One template, several locations.** A template is not limited to one Template Output. When a template's existing files live in more than one project or root folder (e.g. enums in both the DTO and Business projects), **duplicate the Template Output**, one copy per location. Give each copy a **Registration Filter** (on its `Template Output Settings` stereotype) that selects which model instances generate there.
  - The filter is a Dynamic LINQ expression over the template's model (e.g. `EnumModel`, `DTOModel`, `ServiceModel`, `ClassModel`) and must return `bool`. `Folder.Name` is usually the simplest discriminator. Use `np(...)` for anything that can be null. Example: `np(Folder.Name) == "BusinessObject"` on the Business copy and `np(Folder.Name) != "BusinessObject"` on the DTO copy.
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
2. **Folders mirror the code exactly.** The Guardrails templates do **not** add a per-use-case sub-folder. If the code is `Customers/AddAddress/AddAddressCommand.cs`, model folder `Customers/AddAddress` and put the command inside. DTOs that sit in `Customers/` go in the `Customers` folder.
   - **Files that don't follow the convention:** each element is written to `<ElementName>.cs` in its folder's namespace. Apply the **`File Config`** stereotype only where the code differs, and set only the property you need; both are optional.
     - **Namespace doesn't follow the folders:** set `Namespace` to the file's exact namespace. Every type that shares a file needs the same `Namespace`.
     - **No namespace in the file** (types declared in the global namespace): tick `No Namespace` and leave `Namespace` empty.
     - **File name differs, or the file holds several types:** set `Filename` (the file name only, with or without `.cs`; the folder stays the element's own), as below.
     - **File named differently from its type** (e.g. `psMembersDTO.cs` holds `PsMembersDTO`): apply `File Config` with `Filename = psMembersDTO`.
     - **File holding several types:** model every type as its own element in that file's folder, and apply `File Config` with the file's name to each type the file is *not* named after. Elements that end up at the same path are generated into one file, written by the element the file is named after.
     - **Set it before the first SF run.** If a type already had its own file generated and you then point it at another file, the SF offers to delete the old file. Treat that as a destructive change: don't apply it unless the old file is a duplicate.
3. **DTOs first** (dependency order), then commands, queries and services:
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
   - **Return type:**
     - Plain type: `setType("guid")` or `setType("CustomerDetailDto")`.
     - Paged: `setType("PagedResult<CustomerSummaryDto>")`.
     - List: `setType({ name: "WishlistSummaryDto", isCollection: true })`.
     - Nothing: leave it unset (void).
   - **Domain enums:** use them directly as field types.
   - **`Type Configuration`** (newer module only). Apply it **only when required**: set `Type = record` on a DTO or request that is a `record` in code. Leave the default for classes. If the stereotype doesn't exist, the code has records, and the template emits `class`, **ask the user**: convert the records to classes, or wait for the module. Merging a `class` into a `record` file adds a duplicate type.
   - **Type modifiers:** Merge mode replaces each class, record or enum declaration's modifiers with the generated ones, so they must match the code.
     - **Set the convention once, before the first SF run.** If most of the code's declarations aren't plain `public` (e.g. `internal sealed`), set the application setting `Default Type Modifiers` (group `Services Guardrails Contracts Settings`) to the dominant combination. Its options are `public` or `internal`, each optionally followed by `sealed`, `partial` or `sealed partial`. Read it with `getApplicationSettings({ search: "Default Type Modifiers" })` (via `run_solution_query`), change it with `update_application_settings`, and state in one line what you set and why. The Domain module has its own setting of the same name. A type declared without an access modifier is `internal`.
     - **The setting's `sealed` is left off automatically** for an abstract DTO and for a DTO another DTO inherits from.
     - **Exceptions:** wherever a type differs from the setting, apply `Type Configuration` and set `Modifiers` to the code's keywords, e.g. `public` or `internal static`: one of `public` / `internal`, plus any of `sealed`, `abstract`, `static`, `partial`, in any order. `Is Abstract` on a DTO still adds `abstract`. An invalid or conflicting value (e.g. `sealed` on an abstract DTO) is an error on the element.
     - **Enums** take only the setting's access modifier (`internal sealed` gives `internal enum`). Where an enum differs, apply `Enum Configuration` and set `Access Modifier` to `public` or `internal`.
     - **Can't be reproduced:** `file`-scoped and `unsafe` types. Leave them unmodelled and report them.
   - **Leaving members out.** The "leave it unmodelled and report it" rules in this section (extension methods, `file`-scoped and `unsafe` types, and the integration eventing shapes below) apply only to shapes the model can't express at all. A type that is merely spelled differently is never a reason to leave a member out (§7, "Equivalent type rewrites").
   - **Domain event handlers** (when `Intent.Modelers.Domain.Events` is installed): one `Domain Event Handler` element per handler class, in the folder that matches its code folder, named verbatim, with an association to each Domain Event it handles. The contract is class-only: it guards the class, its file and its namespace. Merge mode keeps the `Handle` methods and handler interfaces. `File Config` applies to handlers too. Handlers have their own `Type Configuration` with only `Modifiers` (handlers are often `internal sealed`).
   - **Integration eventing** (when `Intent.Modelers.Eventing` and `Intent.Modelers.Services.EventInteractions` are installed):
     - **Package and folders:** integration events, integration commands and eventing DTOs live in an `Eventing Package` in the Services designer. Use the existing one; if there is none, create one: `await createPackage("Training.Ordering.Eventing", "Eventing Package")`, named after the application. Folders mirror the code, as for DTOs.
     - **Event or command:** a type published to any subscriber is an Integration Event (element type `Message`); a type sent to one receiver is an `Integration Command`. Name it verbatim; no `Event` suffix is added. Add one `Property` per property, typed and flagged as for DTO fields. A `Property` has no Value, so a message property initializer can't be modelled: report it.
     - **Eventing DTOs:** one `Eventing DTO` per nested type a message uses, with one `Eventing DTO-Field` per property (an initializer is the field's Value), `Is Abstract`, its generic types, and a Generalization (`New Inheritance`) to its base Eventing DTO.
     - **Enums** used by messages go in the eventing package too. EnumContract generates them like any other enum.
     - **Handlers:** one `Integration Event Handler` per consumer or handler class, in the Services package folder that matches its code folder, named verbatim, with a `Subscribe Integration Event` (or `Subscribe Integration Command`) association to each message it handles. The contract is class-only, as for domain event handlers: Merge mode keeps the handle methods (`Consume`, `Handle`, `HandleAsync`) and the handler interfaces. The associations document the subscription; the template doesn't read them.
     - **Stereotypes:** `File Config` applies to all four element types. Integration events, integration commands and eventing DTOs have their own `Type Configuration` (`Type` and `Modifiers`), and integration event handlers share the handler `Type Configuration` (`Modifiers` only). `Collection Type` applies to message properties and eventing DTO fields. They work as described above; the setting's `sealed` is left off an abstract eventing DTO and one another eventing DTO inherits from, and an eventing DTO must have the same `Type` as its base.
     - **Only this application's own packages are generated.** A message or enum modelled in another application's eventing package that this application references (the usual subscriber setup) is never written here. If this code keeps its own copy of such a message, it can't be guarded yet: leave it unmodelled and report it.
     - **Positional records** (`record OrderPlaced(Guid OrderId)`) can't be reproduced yet: the template emits property-style members, which would be merged as a second set. Leave them unmodelled and report them.
     - **Accessors:** the template always emits `{ get; set; }`. What Merge mode does to an existing `init` or get-only accessor hasn't been verified. If the SF diff changes an accessor on any property (messages, DTOs, commands or queries), don't apply that file; list it in the final report.
4. **Validate** (`getDesignerValidationErrors`), then save the designer (`saveOnSuccess: true`). A script-created package is invisible to the SF until saved.

## 4. Endpoint surface and security (HTTP solution; WebApi/Security metadata installed in §0)

For each command, query and **public** service operation (§3), set the following from the controller action or endpoint that calls it. A public service operation is an endpoint's request in the same way a command or query is.

- **`Http Settings`:**
  - `Verb` and `Route`. A command or query takes the **absolute** route (e.g. `api/customers/{customerId}/addresses`). A service operation's route is relative to its service: put the controller's route prefix on the Service as `Http Service Settings` → `Route` (e.g. `api/customers`), and give each operation the rest (`{customerId}/addresses`). A `Route` starting with `~/` ignores the prefix. If a service's operations are called from controllers with different prefixes, leave `Http Service Settings` off and give each operation its absolute route.
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
| Calls another application's API through a client | `Perform Invocation` | The `Operation` of the matching `Service Proxy` |
| Publishes an integration event | `Publish Integration Event` | The integration event (`Message`) |
| Sends an integration command | `Send Integration Command` | The `Integration Command` |

- **Model every intent, not every step.** Everything the code does on purpose gets its association: an operation that updates an order, invokes `NotificationService.SendConfirmation` and publishes `OrderUpdated` gets all three. Don't add what an intent already implies: updating an entity reads it first, so `Update Entity Action` alone is enough, and cancelling an order that also fails its payment inside the aggregate is one `Update Entity Action` → `Order`.
- **Invoke, don't repeat.** When an operation delegates to another service operation, command or query, model the `Perform Invocation` only. The callee carries its own intent; don't copy it onto the caller.
- **The target must be in the model.** If what the code calls isn't modelled (a private helper, an external client with no `Service Proxy`), add no association; say what it calls in the element's comment, and report it.
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

1. **Every change must be a header or formatting change.** Expect `using` + `IntentTemplate`, the two merge directives, the csproj package reference, and formatting from RoslynWeaver's Auto Format anywhere in the file (step 5). The only other exception is an equivalent type rewrite the user accepts (step 4).
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
6. **Apply** the reviewed files only: files whose only changes are the headers and formatting, plus files whose equivalent rewrites the user accepted (step 4).
7. **Finish with:** `dotnet build`, tests, and a final SF run proposing **zero** changes, apart from files the user chose not to adopt yet (step 4, option 3).

## Gotchas

- **Concurrent edits.** Elements, associations or stereotypes you didn't create are the user's in-progress work. Leave them, don't apply their output, and report them.
- **Blocking dialogs.** A dialog in a designer tab blocks all tools. Ask the user to close it.
- **Disabling templates.** Disabling a Template Output after its files were applied makes the SF propose deleting them all. Don't use it as a workaround after adoption.
- **Auto Format.** Never turn it off to make adoption diffs smaller (§7 step 5).
""");
        }

        [IntentManaged(Mode.Fully)]
        public override IMarkdownFile MarkdownFile { get; }

        [IntentManaged(Mode.Fully)]
        public override ITemplateFileConfig GetTemplateFileConfig() => MarkdownFile.GetConfig();

    }
}
