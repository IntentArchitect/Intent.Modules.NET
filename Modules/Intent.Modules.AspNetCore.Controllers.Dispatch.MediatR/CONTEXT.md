# CONTEXT — Intent.Modules.AspNetCore.Controllers.Dispatch.MediatR

## `IControllerModel.Folder` means something different for CQRS than for traditional services

This matters whenever a `Registration Filter` (Codebase Structure → `Template Output Settings`) is used to
route controllers to a particular output project — e.g. a multi-host application with a `App.Api` and a
`Mobile.Api` (see `Modules/Intent.Modules.AspNetCore/CONTEXT.md`).

There are **two** registrations producing `IControllerModel`, and they populate `Folder` differently:

| Registration | Model | `Folder` is… |
|---|---|---|
| `Controllers/Controller/ControllerTemplateRegistration` | `ServiceControllerModel` (one per `Service` element) | the **Service's own** containing folder (`_model.Folder`) |
| `ImplicitControllers/ImplicitControllerTemplateRegistration` | `CqrsControllerModel` (one per folder of endpoints) | `folderElement.ParentElement` — the **PARENT** of the grouping folder |

`ImplicitControllerTemplateRegistration.GetModels` does `GroupBy(x => x.ParentElement)` over HTTP-exposed
Commands and Queries, so `folderElement` is the folder *directly containing* them, and `Folder` is one level
above that. The net effect is an **off-by-one** relative to traditional services:

| Modelling style | Element location | `Folder.Name` |
|---|---|---|
| Traditional `Service` | `App/CatalogService` | `App` |
| Traditional `Service` | `App/Orders/CatalogService` | `Orders` |
| CQRS Command/Query | `App/CreateFooCommand` | **null** (`App`'s parent is the package, not a Folder) |
| CQRS Command/Query | `App/Orders/CreateFooCommand` | `App` |

### Consequence — silent drop

A filter such as `np(Folder.Name) == "App"` matches a traditional service placed directly in `App`, and a
Command placed in `App/Orders`, but **not** a Command placed directly in `App` — `Folder` is null there, so no
Template Output claims the model and **no controller is generated at all**. The command, handler and validator
are still generated into the Application layer, so the only symptom is a missing HTTP endpoint: no error, no
warning. Verified 2026-07-30 against the `AppSplitTest` / `TestApp` application.

This is usually masked because the Services designer convention is to put Commands and Queries in a feature
folder (`Orders`, `Buyers`, …), which happens to supply the extra level the filter needs.

### Guidance

- When routing by folder, keep one modelling paradigm per application and place it consistently: traditional
  services directly under the host folder, CQRS commands/queries in a feature folder beneath it.
- If both shapes (or arbitrary nesting) must be supported by one filter, OR in a clause that reads the endpoint
  element's own parent rather than the model's `Folder`, e.g.
  `np(Folder.Name) == "App" || np(Operations.FirstOrDefault().InternalElement.ParentElement.Name) == "App"`.
  Dynamic LINQ cannot recurse, so each supported depth must be written out explicitly. Filters must stay
  mutually exclusive — disjoint host subtrees (`App` vs `Mobile`) guarantee that.

### Also note

`CqrsControllerModel.Name` is the **concatenated parent path**, not just the folder name
(`App/Orders` → `AppOrders` → `AppOrdersController`). Introducing a host folder therefore renames CQRS
controllers relative to a single-host layout.
