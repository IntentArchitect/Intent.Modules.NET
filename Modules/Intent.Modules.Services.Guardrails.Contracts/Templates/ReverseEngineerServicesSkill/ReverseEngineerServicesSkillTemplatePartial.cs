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
description: Reverse-engineer an existing C# application layer (CQRS commands/queries, DTOs, services, controllers/endpoints, authorization) into the Intent Architect Services designer so the model mirrors the code and the Intent.Services.Guardrails.Contracts module adopts the existing files in place — including HTTP exposure, security and the primary Domain entity action per command/query, installing the WebApi/Security/DomainInteractions metadata modules when the code calls for them. Use when the user has installed the Services Guardrails module (or asks to "align / model / reverse-engineer the services") on a brownfield codebase.
short-description: Model existing commands, queries, DTOs and endpoints in the Services designer and adopt their files with the Services Guardrails module.
argument-hint: "[application name]"
---

# Reverse-engineer the Services

Goal: the Services designer describes the application layer **as it is** (requests, DTOs, endpoint surface, security, domain intent), and an SF run against the existing files changes **nothing but Intent file headers**. Handlers, validators, mapping profiles and controllers stay hand-written.

Run **reverse-engineer-domain** first when the code has a domain layer. Commands, DTOs and entity actions reference Domain types. Load **scripting-the-model**, **exploring-the-model** and **changing-the-model** before modelling, and **generating-and-applying-code** / **resolve-destructive-changes** before the first SF run.

## 0. Preconditions

1. `Intent.Services.Guardrails.Contracts` must be installed. If it isn't, stop and ask.
2. **Detect what the code exposes and install the matching modules.** Running this skill is the user's authorization to install the metadata/modeler modules that apply. Detect from the code (controllers, `Program.cs`, csproj `PackageReference`s, attributes):

   | Evidence in the code | Install (if not already installed) |
   |---|---|
   | MediatR-style requests (`IRequest<T>`, `ICommand` / `IQuery`, `*Command` / `*Query` classes) | `Intent.Modelers.Services.CQRS` |
   | HTTP endpoints: `[ApiController]` / `ControllerBase`, minimal APIs (`app.MapGet/MapPost…`), FastEndpoints, Azure Functions HTTP triggers | `Intent.Metadata.WebApi` |
   | Authorization: `[Authorize(...)]`, `[AllowAnonymous]`, `RequireAuthorization(...)`, policies or roles | `Intent.Metadata.Security` (WebApi normally brings it as a dependency; install explicitly if missing) |
   | Handlers that load, create, update or delete Domain entities via repositories or a DbContext, and a Domain designer exists | `Intent.Modelers.Services.DomainInteractions` |
   | Pagination types (`PagedResult<T>`, `PagedList`) | none; reference the `Intent.Modelers.Services.Pagination` package (§3) |
   | Integration events / messaging (MassTransit, Azure Service Bus, `IEventBus`) | `Intent.Modelers.Eventing` + `Intent.Modelers.Services.EventInteractions` |

   - Get the exact current version with `searchAvailableModules(id)`, then call `install_or_update_modules` and state in one line what you installed and why.
   - After installing, a designer tab may open a dialog that blocks tools. Ask the user to close it, then continue.
   - Ask only when the evidence is ambiguous (e.g. gRPC or a custom transport).

3. Decompile the installed template (see **reverse-engineer-domain** §0) to confirm:
   - **Path:** `GetFolderPath()` → path = Template Output + designer folders.
   - **Name:** the class name is `Model.Name` verbatim.
   - **Type kind:** the DTO kind. Older builds always emit `class`. Newer builds honour the **`Type Configuration`** stereotype (`Type`: `record` | `class`).

## 1. Extract the code (read-only, in parallel)

Dispatch `Explore` sub-agents in one message, one per feature folder group (e.g. Customers+Products, Orders+Payments+Wishlists). Each slice also reads its controllers. Ask for:

- **Each command / query:**
  - Path, namespace, name and kind (`class` or `record`).
  - Every property: exact type, nullability, collection-ness and default value.
  - Return type (`Guid`, a DTO, a list of DTOs, `PagedResult<…>`, or nothing).
  - `[Authorize(Roles/Policy)]` / `[AllowAnonymous]`.
  - Brief validator rules.
- **Each DTO:** path, namespace, `class` or `record`, and every property.
- **Each controller action:**
  - HTTP verb, **absolute route**, and success status code.
  - Which properties bind from the route, query or body.
  - Which request it dispatches.
- **Shared types:** `PagedResult<T>`, marker interfaces, permission constants.

## 2. Align the Codebase Structure

Move the Services Guardrails Template Outputs (CommandContract, QueryContract, DtoContract, EnumContract, ServiceContract) to the **Application project folder that contains the feature folders**, usually the project root. They often start in the Api project. Save the designer.

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
3. **DTOs first** (dependency order), then commands and queries:
   - **Fields:** one `DTO-Field` per property, with exact names and types. `isNullable` for `T?`, `isCollection` for lists (never `List<…>` as a type name), and `setValue("1")` for parameter defaults.
   - **Collections:** the template emits `List<T>`. Wherever the code uses anything else (`IEnumerable<T>`, `IList<T>`, `IQueryable<T>`, `T[]`, …), apply the **`Collection Type`** stereotype to that field, service operation parameter or service operation (for its return type) and set `Type` to match (`Custom` + `Custom Format` with `{0}` for anything else). Otherwise Merge mode rewrites the existing signature.
   - **Parameter modifiers:** where a service operation parameter is declared `ref`, `out`, `in`, `ref readonly` or `params`, apply the **`Parameter Modifier`** stereotype and set `Modifier` to match (for `params`, also set `Collection Type = Array`). Otherwise Merge mode drops the modifier from the existing signature. An extension method (`this` parameter) can't be reproduced; leave it unmodelled and report it.
   - **Return type:**
     - Plain type: `setType("guid")` or `setType("CustomerDetailDto")`.
     - Paged: `setType("PagedResult<CustomerSummaryDto>")`.
     - List: `setType({ name: "WishlistSummaryDto", isCollection: true })`.
     - Nothing: leave it unset (void).
   - **Domain enums:** use them directly as field types.
   - **`Type Configuration`** (newer module only). Apply it **only when required**: set `Type = record` on a DTO or request that is a `record` in code. Leave the default for classes. If the stereotype doesn't exist, the code has records, and the template emits `class`, **ask the user**: convert the records to classes, or wait for the module. Merging a `class` into a `record` file adds a duplicate type.
4. **Validate** (`getDesignerValidationErrors`), then save the designer (`saveOnSuccess: true`). A script-created package is invisible to the SF until saved.

## 4. Endpoint surface and security (HTTP solution; WebApi/Security metadata installed in §0)

For each command and query, set the following from its controller action:

- **`Http Settings`:**
  - `Verb` and the **absolute** `Route` (e.g. `api/customers/{customerId}/addresses`).
  - `Success Response Code` from the actual result: `Created(...)` → `201 (Created)`, `NoContent()` → `204 (No Content)`, `Ok(...)` → `200 (Ok)`.
- **`Parameter Settings.Source`:**
  - `From Route` for every field whose camelCase name is a `{routeParam}`.
  - `From Query` for the remaining fields of GET and DELETE actions.
  - Leave body fields at the default.
- **Security:**
  - `Secured` with `Roles` (or `Policy`) mirroring `[Authorize(...)]`; resolve constants, so `Permissions.RoleAdmin` becomes `Admin`.
  - `Unsecured` for `[AllowAnonymous]`.
  - Apply them per element. Use the package level only if the code secures everything uniformly.

If the code has no HTTP surface, skip this section.

## 5. Primary domain intent (DomainInteractions installed in §0)

Give each command and query **one** primary Entity Action to the Domain class it acts on. Follow the designer's own rules; read them in `getDesignerSchema()`:

| Request purpose | Entity Action |
|---|---|
| Create aggregate root | `Create Entity Action` → root |
| Add a compositional child | `Create Entity Action` → child (`Address`, `OrderItem`, …), not the root |
| Change root state (rename, deactivate, confirm, cancel, advance) | `Update Entity Action` → root |
| Update or set-default a child; update an owned 1:1 child | `Update Entity Action` → child (e.g. `Preferences`, `Payment`) |
| Delete or remove a root or child | `Delete Entity Action` → that entity |
| Get by id / reference | `Query Entity Action` → entity, target `"1"` |
| Search / list | `Query Entity Action` → entity, target `"0..*"` |

- **One action per request.** Don't add secondary actions for side effects the aggregate operation implies (e.g. cancelling an order that also fails its payment).
- **Projections.** If a handler projects one entity through another (e.g. listing payments via orders), still target the conceptually primary entity and mention the projection in the comment.
- **Mappings.** Create the associations without field mappings (the designer's default). Add mappings only if the user asks.

## 6. Diagram

Create a `Diagram` in the package if none exists:

- All commands and queries in **one vertical column on the left**, exactly **100px apart top-to-top**, grouped by feature folder.
- The related Domain entities **200px to the right**, grouped by feature.
- Route every Entity Action association. DTOs can't be placed on this diagram.

## 7. Generate, verify and adopt

Same as **reverse-engineer-domain** §6:

1. **Every change must be header-only.** Expect `using` + `IntentTemplate`, the merge directive, and the csproj package reference.
2. **Diagnose anything else:** `create` means a path or name mismatch, or a type that lives in another file without `File Config` (§3). Member inserts mean model drift. Deletes mean destructive drift (use **resolve-destructive-changes**). A duplicate type means the record/class kind is wrong.
3. **Merge directive:** every adopted file needs `[assembly: DefaultIntentManaged(Mode.Merge)]`, plain form. Add it if the template didn't, **before** the next model change. Otherwise the SF strips `[Authorize]`, `IRequest<T>, ICommand` base types, XML docs and constructors.
4. **Apply** the reviewed files only.
5. **Finish with:** `dotnet build`, tests, and a final SF run proposing **zero** changes.

## Gotchas

- **Concurrent edits.** Elements, associations or stereotypes you didn't create are the user's in-progress work. Leave them, don't apply their output, and report them.
- **Blocking dialogs.** A dialog in a designer tab blocks all tools. Ask the user to close it.
- **Disabling templates.** Disabling a Template Output after its files were applied makes the SF propose deleting them all. Don't use it as a workaround after adoption.
""");
        }

        [IntentManaged(Mode.Fully)]
        public override IMarkdownFile MarkdownFile { get; }

        [IntentManaged(Mode.Fully)]
        public override ITemplateFileConfig GetTemplateFileConfig() => MarkdownFile.GetConfig();

    }
}
