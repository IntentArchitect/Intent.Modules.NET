# Context: Intent.Application.Interop.CRUD

## Purpose

A dispatcher-neutral CRUD choice. The module has no templates, factory extensions, settings or designer metadata. Its only behaviour is the hand-written `<interoperability>` block in its `.imodspec`. That block installs the CRUD implementation module matching whichever dispatcher module the application has:

| Detected                                    | Installs (floor)                                                   |
| ------------------------------------------- | ------------------------------------------------------------------ |
| `Intent.Application.MediatR`                | `Intent.Application.MediatR.CRUD` 7.2.2                            |
| `Intent.Application.Wolverine`              | `Intent.Application.CQRS.CRUD` 1.0.0                               |
| `Intent.Application.ServiceImplementations` | `Intent.Application.ServiceImplementations.Conventions.CRUD` 5.3.5 |
| `Intent.Application.Mediator`               | nothing (see below)                                                |

The design source is `intent/.plans/2026-10-26-interop-crud-module.mdx`.

## Architectural Decisions

- **An interop-only module, not an architecture-template component that lists the CRUD modules** (2026-10-26). Before this module, each CRUD module sat as an optional module inside its dispatcher's component (MediatR Dispatch, Wolverine Dispatch, Application Services). Each dispatcher therefore had its own CRUD tick-box, and a new dispatcher needed its own CRUD wiring in every template. With one module, the template gets a single CRUD component and the dispatcher-to-CRUD mapping lives here, versioned with the modules. Rejected: a template-only approach using component dependencies. The template cannot express "install X only if dispatcher Y was also picked" without one CRUD component per dispatcher, which is what already existed.
- **No rule for Mediator** (2026-10-26). `Intent.Application.Mediator` is designed so that an AI writes every handler body, and `Intent.Application.CQRS.CRUD` is never used with it (see `Modules/Intent.Modules.Application.Mediator/CONTEXT.md`, Purpose). CQRS.CRUD finds handlers by role and takes over the `Handle` body, so it would overwrite AI-written code. The CRUD option is a deliberate no-op for Mediator's handlers; an app that also has Application Services (every `Tests/TestMediator.*` app does) still gets Conventions.CRUD through that rule, which is correct because it targets service operations, not handlers. Add a Mediator rule only once a Mediator-safe CRUD implementation exists.
- **A CQRS dispatcher (MediatR or Wolverine) plus Application Services installs both CRUD modules.** The dispatchers can coexist on Clean Architecture, and each rule fires on its own. This matches the template defaults this module replaces, where both CRUD modules came pre-ticked, so it is accepted rather than guarded against.
- **It uses the shared CRUD icon** (developer decision, 2026-10-26). The PNG is copied from `Intent.Application.CQRS.CRUD`, and `Intent.Application.MediatR.CRUD` and `Intent.Application.ServiceImplementations.Conventions.CRUD` carry the same one, so the module reads as the CRUD option. Rejected: the same motif with a three-way branch badge to tell it apart from the CRUD modules in a module list.

## Invariants & Constraints

- **The module must not declare a dependency on any dispatcher or CRUD module.** A `<dependency>` is installed unconditionally, which would defeat the point of a conditional rule. `<dependencies>` holds only the Module Builder floors (`Intent.Common`, `Intent.Common.Types`). Keep it that way: install nothing metadata-only into this module's own application, or the Software Factory adds it as a dependency.
- **Rule versions are floors, not pins.** Each is the CRUD module's version when the rule was written. Raise one when a CRUD module ships a change that the current dispatcher needs, so a fresh install does not settle on an older CRUD module.
- **`<interoperability>` and `<tags>` are hand-maintained.** The Software Factory never writes either, and a regeneration was checked to leave both intact. `<summary>`/`<description>` come from the application description, so change them there, not in the `.imodspec`.
- **Interoperability only installs.** Uninstalling this module leaves the CRUD module it brought in, and uninstalling only the CRUD module lets the rule re-install it on a later install or update. The README tells users to remove both.

## Install Order: Both Directions Fire (verified 2026-10-26)

Verified against `1.0.0-pre.0` in throwaway Clean Architecture apps:

- **Dispatcher first, then this module:** Wolverine installed `Intent.Application.CQRS.CRUD` 1.0.0, MediatR installed `Intent.Application.MediatR.CRUD` 7.2.2, and Application Services installed `Intent.Application.ServiceImplementations.Conventions.CRUD` 5.3.5. Each app then generated and built cleanly.
- **This module first, then the dispatcher:** with no dispatcher present, this module installed nothing. Installing `Intent.Application.Wolverine` afterwards brought in `Intent.Application.CQRS.CRUD`. So the rules are re-evaluated when a detected module arrives later, and the architecture template needs no install-order workaround.
- **Mediator only:** nothing else installed.

## Module Interactions

- **Intent.Application.MediatR.CRUD / Intent.Application.CQRS.CRUD / Intent.Application.ServiceImplementations.Conventions.CRUD**: installed by the rules above. No code or metadata reference in either direction.
- **Intent.Application.MediatR / Wolverine / ServiceImplementations / Mediator**: detected by id only.
- **Architecture templates (`Intent.ArchitectureTemplates`, out of scope here)**: expected to offer one _CRUD_ component holding this module, drop the three CRUD modules from the dispatcher components, and pin each CRUD module in `minimumDependencyVersions`.
