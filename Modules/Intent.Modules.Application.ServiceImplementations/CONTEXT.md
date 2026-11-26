# Context: Intent.Application.ServiceImplementations

## Purpose

Generates the implementation class for each traditional (non-CQRS) Service modelled in the Services
designer, one method per Operation, plus the `application-service-implementation` AI skill. Other
modules fill or replace the operation bodies: `Intent.Application.ServiceImplementations.Conventions.CRUD`
implements them from mappings and conventions, and `Intent.Application.ServiceCallHandlers` turns them
into dispatchers to per-operation handler classes.

## Architectural Decisions

- **Operation body mode is keyed to whether a services CRUD module is installed.** `ServiceImplementationTemplate`
  takes each operation's attribute from `Templates/OperationBodyModeExtensions.GetOperationMethodIntentManagedAttribute`:
  - **`Intent.Application.ServiceImplementations.Conventions.CRUD` installed** → `Fully` + `Body = Mode.Merge`,
    unchanged from published versions. The CRUD module overrides it to `Body = Mode.Fully` on the operations
    it implements.
  - **Not installed** → `Fully` + `Body = Mode.Ignore`. Nothing can ever generate the body, so it belongs
    to the developer or AI from the first run.

  The stub is emitted in both cases. This mirrors `Intent.Application.MediatR`'s `HandlerBodyModeExtensions`.
  That module's `CONTEXT.md` records why Merge stays on unimplemented methods when CRUD is installed
  (upgrade-safe, and a mapping added later still gets implemented), the rejected alternatives, and the
  RoslynWeaver behaviour this relies on: `Merge`→`Ignore` keeps a hand-written body, and `Ignore` is sticky.
- **The check is a soft `InstalledModules.Any(...)` lookup.** Conventions.CRUD depends on this module,
  not the other way round, so there is no `.imodspec` dependency on it. MediatR.CRUD and CQRS.CRUD never
  touch service templates, so they do not count.
- **The skill keeps the published rule**, worded since 4.7.7 as "Mark each operation you write with
  `[IntentManaged(Mode.Fully, Body = Mode.Ignore)]`", which holds whether or not CRUD is installed. The
  AI only sees Fully/Merge ("not Ignore yet") or Ignore (yours).
- **Trimmed to the evaluated text, no pointer rule (4.7.7).** Shared with the handler skills; the
  reasoning is in `Intent.Application.MediatR`'s `CONTEXT.md`, "Handler skills: trimmed text, no
  pointer rule". The description names `*Service.cs` under `Implementation`, the folder
  `ServiceImplementationTemplate` writes to, so it carries the trigger. EF Core
  Repositories recognises this skill by `ServiceImplementation` in its template id to word the save
  rule for an operation instead of a handler, so keep that substring.
- **With Service Call Handlers, the skill adds a line to leave the dispatcher alone.**
  `GetServiceCallHandlerDispatcherSkillRule()` appends it only when `Intent.Application.ServiceCallHandlers`
  is installed. The dispatcher must stay `Body = Mode.Fully`, which the "switch to Ignore" rule would
  otherwise contradict, and there is no separate Service Call Handler skill.
- **`OperationBodyModeExtensions` is `public`.** `Intent.Application.ServiceCallHandlers` uses it for its
  `Handle` methods so both templates answer from the same module list.

## Module Interactions

- **Intent.Application.ServiceImplementations.Conventions.CRUD** — implements operation bodies (or SCH
  `Handle` bodies). Adding another module that implements service operation bodies? Add its id to
  `OperationBodyModeExtensions.CrudImplementationModuleIds`. Otherwise its operations will be generated
  `Body = Mode.Ignore` in apps without Conventions.CRUD, and Ignore is sticky.
- **Intent.Application.ServiceCallHandlers** — calls `GetOperationMethodIntentManagedAttribute` (added in
  4.7.6), so its dependency floor on this module is 4.7.6. Its `ServiceImplementationExtension` forces
  `WithBodyFully()` on the dispatcher method, overriding whatever this module set.
