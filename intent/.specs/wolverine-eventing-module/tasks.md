# Implementation Plan — Wolverine Eventing Module

Derived from the design's Realization plan and Modelling order. `[model]` is Module Builder model work; `[code]` is the hand-written C# **inside** templates and factory extensions (a module's templates are C# that emits C#), plus probes, READMEs and module docs.

Per the design's *Code placement* section: each module's `release-notes.md`, `docs/README.md` and `CONTEXT.md` are updated in the same wave as the behaviour they describe, and version increments happen before any template work in that module.

## Tasks

- [ ] T0. Prerequisites — module creation, version increments, probes
  - [ ] T0.1 [module] (satisfies: R18) — Create the `Intent.Wolverine.Common` module package
    - New Intent Module package in the Module Builder designer; `Module Settings` stereotype: Version `1.0.0-pre.0`, API Namespace `Intent.Wolverine.Common.Api`, Include Release Notes true
    - No C# Template — this module emits no file of its own (factory extension + API surface only)
  - [ ] T0.2 [module] (satisfies: R14.1) — Create the `Intent.Eventing.Wolverine` module package
    - New Intent Module package; Version `1.0.0-pre.0`, API Namespace `Intent.Eventing.Wolverine.Api`, Include Release Notes true
    - Single module covering all four Transports and both Outbox modes per D2 — no `.EntityFrameworkCore` companion
    - Module dependency on `Intent.Eventing.Contracts` `6.1.3` (R14.1 — one install, no companion)
  - [ ] T0.3 [module] (satisfies: R18.3) — Increment `Intent.Application.Wolverine` to `1.1.0-pre.0`
    - Set via the Module Builder model Version property, never by hand-editing the `.imodspec`
    - Minor (not patch) because generated output changes; if any consumer runs it on an Azure Functions host, correct to `2.0.0-pre.0` per design assumption d-a4
    - Add a module dependency on `Intent.Wolverine.Common` `1.0.0-pre.0`
  - [ ] T0.4 [code] (satisfies: R7.1, R12.2, R2.6) — Extend the D6 compile-only probe with the four uncited call shapes
    - Extends `golden-sample/probes/DurableAndTransportProbe/Probe.cs`; code-only, belongs to no module
    - Prove: `Retry` and `Schedule retry` policy call shapes (R7.1); the Finbuckle-aware Wolverine middleware shape (R12.2); the configuration read that must fail fast for Azure Service Bus and Amazon SQS (R2.6)
    - **Blocks** T3.6 (R2.6), T6.7 / T6.8 (retry policies) and T7.5 / T7.6 (tenancy) — those templates may not be authored until the probe compiles
    - Fallback if a probe fails, per D6: drop R12 and defer the `Retry` / `Schedule retry` policy options

- [ ] T1. Shared Wolverine host module (R18)
  - [ ] T1.1 [model] (satisfies: R18.2) — Add the `WolverineFx` NuGet Package to `Intent.Wolverine.Common`
    - `NuGet Package` element with a `Package Version` child pinned to `5.39.5`, Minimum Target Framework `.NETCoreApp,Version=v8.0`
  - [ ] T1.2 [model] (satisfies: R18.2) — Add the `WolverineHostRegistrationExtension` Factory Extension element
    - Factory Extension in `Intent.Wolverine.Common`; owns the single `ConfigureHostBuilderChainStatement("UseWolverine", ...)` call
  - [ ] T1.3 [code] (satisfies: R18.2) — Write the broadcast request type in the module's API namespace
    - Follows the established `ContainerRegistrationRequest` / `AppSettingRegistrationRequest` pattern — a direct module reference is deliberately not used
    - Carries (a) statements to add inside the `UseWolverine` lambda, (b) an ordering priority, (c) zero or more assemblies needing handler discovery
  - [ ] T1.4 [code] (satisfies: R18.2, R8.2) — Implement `WolverineHostRegistrationExtension`
    - Handle every broadcast request; sort contributions by priority for deterministic output; de-duplicate the assembly set; emit **one** `UseWolverine` lambda with one `opts.Discovery.IncludeAssembly(...)` per distinct assembly
    - Conventional discovery stays ON per D1 — never emit `DisableConventionalDiscovery()`, and never emit `IncludeType` for a type an assembly registration already covers (D1b: double registration causes Wolverine's `CS0128` codegen failure)
  - [ ] T1.5 [code] (satisfies: R18.3, R18.6) — Rework `WolverineRegistrationFactoryExtension` in `Intent.Application.Wolverine`
    - Remove the `lambdaBlock.Statements.Clear()` line — this is what destroys other modules' contributions depending on factory-extension execution order
    - Stop calling `ConfigureHostBuilderChainStatement` directly; broadcast a host-configuration request instead, declaring its `Configure(opts)` statement and the Application-layer assembly it needs discovered
    - Remove the Azure Functions host loop outright (R8.7); name the removal in the module's release notes
  - [ ] T1.6 [code] (satisfies: R18.6) — Marker sweep on the Golden Sample
    - Remove the `//IntentIgnore` directive and its `GOLDEN-SAMPLE:` marker from both `*.Api/Program.cs` files, regenerate, and confirm the template emits that exact line unaided
    - Ignore-style directives suppress template output, so leaving one in place makes parity pass while proving nothing: `grep -rn "GOLDEN-SAMPLE:"` returning nothing is part of this wave's report
  - [ ] T1.7 [code] (satisfies: R18.3) — Docs for both host-side modules
    - `Intent.Wolverine.Common`: `release-notes.md`, `docs/README.md`, `CONTEXT.md` (why a module rather than shared code — design assumption d-a3)
    - `Intent.Application.Wolverine`: release note for the `1.1.0-pre.0` change, calling out the Azure Functions registration removal; update its `CONTEXT.md` to record that the host registration now lives in the common module

- [ ] T2. Provider foundation and designation (R1, R9)
  - [ ] T2.1 [model] (satisfies: R9.1) — Add the `Wolverine Message` stereotype definition
    - Under a `Stereotypes` folder in `Intent.Eventing.Wolverine`; applicable to `Message` and `Integration Command`; no properties — a marker
  - [ ] T2.2 [model] (satisfies: R1.1, R9.1, R9.2, R9.3) — Add the `Wolverine` Message Bus Provider element
    - Under a `Message Bus Providers` folder; `Message Bus Provider Settings.Applicable Stereotypes` references the `Wolverine Message` stereotype from T2.1 — this is how the provider becomes selectable on the Message Bus stereotype
  - [ ] T2.3 [code] (satisfies: R1.1, R1.2, R1.6) — Model-reading seam shared by the module's templates
    - Read the Services designer's existing message and subscription elements; add nothing to that designer
    - Gives the eventing configuration template one place to enumerate messages, subscriptions and their designation from, which R2–R7 template work then builds on
  - [ ] T2.4 [model] (satisfies: R9.6) — Add the `WolverineMessageBusInteropExtension` Factory Extension element
  - [ ] T2.5 [code] (satisfies: R9.4, R9.5, R9.6) — Designation filtering and Composite Message Bus participation
    - Filter every emission by provider designation: a message designated to another provider produces no Wolverine publish rule, send rule or listener
    - Participate in the Composite Message Bus so publishes route to the designated provider
    - Per design assumption d-a2, designation rests on routing not registration — with conventional discovery on, a handler for a foreign-designated message is still registered but receives nothing because no listener is bound

- [ ] T3. Transport selection and broker topology (R2, R10)
  - [ ] T3.1 [model] (satisfies: R14.2, R2.1, R2.7) — Add the `Wolverine Message Bus Settings` Module Settings Configuration with its Transport and Broker Topology fields
    - Settings Type = Application Settings
    - `Transport` — Select, Is Required true, Default `Local`, options `Local` / `RabbitMQ` / `Azure Service Bus` / `Amazon SQS` as `Module Settings Field Option` children; Hint carries R2.5's obligation that Local is in-process only
    - `Broker Topology` — Select, Is Required true, Default `Auto-provision`, options `Auto-provision` / `Externally owned`
    - No setting for the Subscriber Queue naming rule, serialization format or durable storage technology — R14.2 forbids it; each is derived
  - [ ] T3.2 [model] (satisfies: R10.1, R10.3, R10.5, R2.2) — Add the seven `NuGet Package` elements
    - `WolverineFx`, `WolverineFx.RabbitMQ`, `WolverineFx.AzureServiceBus`, `WolverineFx.AmazonSqs`, `WolverineFx.EntityFrameworkCore`, `WolverineFx.SqlServer`, `WolverineFx.Postgresql`
    - Each with a `Package Version` child pinned to `5.39.5` and Minimum Target Framework `.NETCoreApp,Version=v8.0`
    - This model is R10's realization — the licence guarantee is verified by reading these seven registrations against the requirements' inventory, and R10's re-check obligation attaches to changing a `Package Version`
  - [ ] T3.3 [model] (satisfies: R2.1, R2.3) — Add the `WolverineEventingConfiguration` template and `WolverineEventingRegistrationExtension` elements
    - `WolverineEventingConfiguration`: C# Template, C# File Builder, Single File, Role `Infrastructure.DependencyInjection.Wolverine.Eventing`, Default Location `Eventing`
    - `WolverineEventingRegistrationExtension`: Factory Extension
  - [ ] T3.4 [code] (satisfies: R2.1, R2.2, R2.4, R2.5) — Transport branch in `WolverineEventingConfiguration`
    - One branch per Transport option emitting that transport's configuration; add the matching NuGet package conditionally so an application never carries a transport package it does not use
    - `Local` emits in-process configuration only
  - [ ] T3.5 [code] (satisfies: R2.3) — Broadcast the host-configuration request and raise the appsettings registrations
    - In `WolverineEventingRegistrationExtension`: broadcast the T1.3 request to `Intent.Wolverine.Common` carrying the eventing configuration statement and its priority; raise the appsettings registration requests for the selected transport's connection settings
    - Appsettings registration is additive only — document that; do not promise removal
  - [ ] T3.6 [code] (satisfies: R2.6) — Fail-fast configuration read for Azure Service Bus and Amazon SQS
    - Use the call shape proven by T0.4; emit an `InvalidOperationException` startup guard in the generated code (it runs in the consumer's app, not at Software Factory time)
  - [ ] T3.7 [code] (satisfies: R2.7) — Broker Topology emission
    - `Auto-provision` emits provisioning; `Externally owned` emits none and never declares exchanges, queues or bindings
  - [ ] T3.8 [code] (satisfies: R14.2) — Document the settings surface and the transport matrix in `docs/README.md` and release notes

- [ ] T4. Publishing and sending (R3, R4)
  - [ ] T4.1 [model] (satisfies: R3.3, R3.4, R3.5, R3.6) — Add the `Message Topology Settings` stereotype definition
    - Applicable to `Message`; property `Topic Name` — text box, optional; the name deliberately matches MassTransit's so a migrator meets the same vocabulary
  - [ ] T4.2 [model] (satisfies: R4.3, R4.4, R4.5, R4.6) — Add the `Command Distribution` stereotype definition
    - Applicable to `Integration Command`; property `Destination Queue Name` — text box, optional
    - On the element, not on the send association, per D4 — so two senders of the same Command cannot resolve different queues
  - [ ] T4.3 [model] (satisfies: R3.8, R8.8) — Add the `WolverineMessageBus` template element
    - C# Template, C# File Builder, Single File; Role `Infrastructure.Eventing.WolverineEventBus`, Default Location `Eventing`
  - [ ] T4.4 [code] (satisfies: R3.1, R3.2, R3.10) — Publish-rule emission with the kebab-cased convention name
  - [ ] T4.5 [code] (satisfies: R3.3, R3.4, R3.5, R3.6) — `Topic Name` override resolution and its `Validate Function`
    - The override replaces the convention name; validate for emptiness and a 250-character ceiling, reported as an `ElementException` against the offending element
  - [ ] T4.6 [code] (satisfies: R4.1, R4.2, R4.7) — Send-rule emission and destination-queue resolution
    - Resolve `Destination Queue Name` from the single site it is declared (the Integration Command), falling back to the convention name
  - [ ] T4.7 [code] (satisfies: R3.8, R4.1, R8.8) — Implement `WolverineMessageBus`
    - Buffered implementation of the Eventing.Contracts Message Bus interface, with its flush method
    - Generates the bus and the flush method and nothing that calls it — per D5 the flush caller belongs to the dispatch module
  - [ ] T4.8 [code] (satisfies: R3.9) — Conditional DI registration for the bus in `WolverineEventingRegistrationExtension`

- [ ] T5. Consuming messages (R5)
  - [ ] T5.1 [code] (satisfies: R5.1, R5.2) — Listener emission per subscribed message
    - No consumer template exists and none is added — the `IIntegrationEventHandler<T>` implementation from `Intent.Eventing.Contracts` is itself the Wolverine handler; R5.2 forbids reintroducing a generated consumer
    - R5.1 is satisfied by the handler being reachable through the host's discovery configuration (T1.4), not by a per-message registration — design assumption d-a1
  - [ ] T5.2 [code] (satisfies: R5.3) — Subscriber Queue naming derivation
    - Derived, never a module setting (R14.2)
  - [ ] T5.3 [code] (satisfies: R5.7, R5.9) — Exchange-to-queue binding emission
    - Emitted only under `Auto-provision`; suppressed under `Externally owned`
  - [ ] T5.4 [code] (satisfies: R5.8) — Host-scope placement of the error handling policy hook
    - Reserve the seam that T6's policy emission writes into; policies are host-scope, not per-listener
  - [ ] T5.5 [code] (satisfies: R5.4) — Confirm no competition with the `Intent.Eventing.Contracts` handler template
    - That template ships in a body-preserving mode; verify the Wolverine module registers nothing for its Role and overwrites no handler body

- [ ] T6. Transactional Outbox and Error Handling Policy (R6, R7)
  - [ ] T6.1 [model] (satisfies: R6.1) — Add the `Transactional Outbox` settings field
    - Select, Is Required true, Default `None`, options `None` / `Durable`; names no persistence technology — durable storage is derived from the modelled Database Provider
  - [ ] T6.2 [model] (satisfies: R7.1) — Add the `Error Handling Policy` settings field
    - Select, Is Required true, Default `Retry with cooldown`, options `None` / `Retry` / `Retry with cooldown` / `Schedule retry`
  - [ ] T6.3 [code] (satisfies: R6.1, R6.3) — Durability registration emission
    - Conditional on `Durable`; adds `WolverineFx.EntityFrameworkCore` plus the matching `WolverineFx.SqlServer` / `WolverineFx.Postgresql` package
  - [ ] T6.4 [code] (satisfies: R6.5, R6.7) — Derive the durable storage technology from the modelled Database Provider
  - [ ] T6.5 [code] (satisfies: R6.4, R6.8) — Stopping conditions as `FriendlyException`
    - R6.4 and R6.8's unsupported combinations stop the Software Factory with a plain-prose `FriendlyException` (no Markdown — it is not rendered) stating what is wrong, then the fix
  - [ ] T6.6 [code] (satisfies: R6.3) — `eventbus-flush` tag handling in `WolverineMessageBusInteropExtension`
    - Applies only when a durable outbox is selected
  - [ ] T6.7 [code] (satisfies: R7.1, R7.3) — Emit the `None`, `Retry` and `Retry with cooldown` policies
    - Uses the call shapes proven by T0.4; every policy terminates in move-to-error-queue
  - [ ] T6.8 [code] (satisfies: R7.1, R7.4, R7.5) — Emit the `Schedule retry` policy and the empty-delay branch
    - Uses the call shape proven by T0.4; also terminates in move-to-error-queue
  - [ ] T6.9 [code] (satisfies: R7.2) — Appsettings keys for the retry configuration
    - Raised as registration requests from `WolverineEventingRegistrationExtension`
  - [ ] T6.10 [code] (satisfies: R6.1, R7.1) — Document outbox and error handling in `docs/README.md` and release notes

- [ ] T7. Coexistence, telemetry and multi-tenancy (R8, R11, R12)
  - [ ] T7.1 [code] (satisfies: R8.1, R8.3, R8.4, R8.5) — Type resolution across module boundaries
    - Resolve both Message Bus interfaces via resolved type references, never literal name strings
    - The eventing module must work with no CQRS module installed, and the CQRS module with no eventing module installed
  - [ ] T7.2 [model] (satisfies: R11.1) — Add the `WolverineTelemetryConfiguratorExtension` Factory Extension element
  - [ ] T7.3 [code] (satisfies: R11.1) — Conditional `AddSource("Wolverine")` registration
    - Emits only when `Intent.OpenTelemetry` is installed; emits nothing otherwise
  - [ ] T7.4 [model] (satisfies: R12.2) — Add the `WolverineTenantHeaderStrategy`, `WolverineTenantMiddleware` and `WolverineFinbuckleConfiguratorExtension` elements
    - Two C# Templates plus one Factory Extension; emitted only when the Finbuckle multi-tenancy module is installed
  - [ ] T7.5 [code] (satisfies: R12.2) — Implement the tenant header strategy
    - Reads and writes the Tenant Identifier header on inbound and outbound messages
  - [ ] T7.6 [code] (satisfies: R12.2) — Implement the tenancy middleware and its conditional wiring
    - Establishes and restores Finbuckle context around handler invocation, registered once at host scope, using the shape proven by T0.4
    - `WolverineFinbuckleConfiguratorExtension` wires the templates when the module is installed and emits nothing when it is not

- [ ] T8. Settings close-out, migration guidance and module docs (R13, R14)
  - [ ] T8.1 [code] (satisfies: R14.5) — Uninstall behaviour and the additive-appsettings caveat
    - Document that appsettings registration is additive only — the module registers its section but cannot remove it on uninstall; promise only the registration half
  - [ ] T8.2 [code] (satisfies: R13.1, R13.3, R13.5) — Write the MassTransit-to-Wolverine migration guidance
    - Migration section, a setting-equivalence table, and the artefact list of what changes in a migrated application
    - In `docs/README.md` rather than modelled Documentation Topics, per design assumption d-a5
  - [ ] T8.3 [code] (satisfies: R14.1, R14.2) — Final module docs and `CONTEXT.md` for `Intent.Eventing.Wolverine`
    - Record D1 / D1b (discovery on, never double-register), D2 (single module), D4 (queue name on the element) and D5 (bus here, flush elsewhere) as durable rationale

- [ ] T9. Test Applications — transports and topology (R19)
  - [ ] T9.1 [model] (satisfies: R19.1) — `Wolverine.Transport.Local` — Local, Outbox None
  - [ ] T9.2 [model] (satisfies: R19.1) — `Wolverine.Transport.RabbitMQ.Publish` and `.Subscribe`
    - The Golden Sample's own path, as a publish/subscribe pair — a subscriber-side discovery surface only appears across a process boundary
  - [ ] T9.3 [model] (satisfies: R19.1) — `Wolverine.Transport.AzureServiceBus`
  - [ ] T9.4 [model] (satisfies: R19.1) — `Wolverine.Transport.AmazonSqs`
  - [ ] T9.5 [model] (satisfies: R19.1) — `Wolverine.Topology.ExternallyOwned` — Externally owned, with name overrides on every message
  - [ ] T9.6 [code] (satisfies: R19.1) — READMEs, committed generated output and clean re-run confirmation for T9.1 to T9.5
    - Each README describes infrastructure requirements and how to run the app
    - Per design assumption d-a7, confirm for each: right module version installed, provider designation present, file not ignored, code-management mode permits writing — a clean run against a file no template would write proves nothing

- [ ] T10. Test Applications — durability and error policies (R19)
  - [ ] T10.1 [model] (satisfies: R19.1) — `Wolverine.Outbox.SqlServer.Publish` and `.Subscribe`
  - [ ] T10.2 [model] (satisfies: R19.1) — `Wolverine.Outbox.Postgresql.Publish` and `.Subscribe`
  - [ ] T10.3 [model] (satisfies: R19.1) — `Wolverine.ErrorPolicy.None`, `.Retry`, `.RetryWithCooldown` and `.ScheduleRetry`
    - One per Error Handling Policy, matching the four MassTransit retry-policy apps
  - [ ] T10.4 [code] (satisfies: R19.1) — READMEs, committed generated output and clean re-run confirmation for T10.1 to T10.3
    - Same four-point re-run check as T9.6

- [ ] T11. Test Applications — coexistence, multi-provider and multi-tenancy (R19)
  - [ ] T11.1 [model] (satisfies: R19.1, R18.6) — `Wolverine.Coexist.Cqrs` — eventing plus `Intent.Application.Wolverine` on one shared host
    - The end-to-end evidence that one `UseWolverine` lambda carries both modules' contributions with no `Statements.Clear()` collision
  - [ ] T11.2 [model] (satisfies: R19.1, R9.4, R9.5) — `Wolverine.MultiProvider` — Wolverine alongside MassTransit, messages designated to each
    - Sole evidence for R9, and the app that has to demonstrate design assumption d-a2 holds
  - [ ] T11.3 [model] (satisfies: R19.1, R12.2) — `Wolverine.MultiTenancy` — eventing plus the Finbuckle multi-tenancy module
    - Sole evidence for R12
  - [ ] T11.4 [code] (satisfies: R19.1) — READMEs, committed generated output and clean re-run confirmation for T11.1 to T11.3
    - Same four-point re-run check as T9.6

## Task Dependency Graph

```json
{
  "waves": [
    { "id": 1, "tasks": ["T0.1", "T0.2", "T0.3", "T0.4", "T1.1", "T1.2", "T1.3", "T1.4", "T1.5", "T1.6", "T1.7"], "label": "Shared host module and D6 probes" },
    { "id": 2, "tasks": ["T2.1", "T2.2", "T2.3", "T2.4", "T2.5"], "label": "Provider foundation and designation" },
    { "id": 3, "tasks": ["T3.1", "T3.2", "T3.3", "T3.4", "T3.5", "T3.6", "T3.7", "T3.8"], "label": "Transport and broker topology" },
    { "id": 4, "tasks": ["T4.1", "T4.2", "T4.3", "T4.4", "T4.5", "T4.6", "T4.7", "T4.8"], "label": "Publishing and sending" },
    { "id": 5, "tasks": ["T5.1", "T5.2", "T5.3", "T5.4", "T5.5"], "label": "Consuming messages" },
    { "id": 6, "tasks": ["T6.1", "T6.2", "T6.3", "T6.4", "T6.5", "T6.6", "T6.7", "T6.8", "T6.9", "T6.10"], "label": "Outbox and error handling" },
    { "id": 7, "tasks": ["T7.1", "T7.2", "T7.3", "T7.4", "T7.5", "T7.6"], "label": "Coexistence, telemetry and tenancy" },
    { "id": 8, "tasks": ["T8.1", "T8.2", "T8.3"], "label": "Settings close-out and migration docs" },
    { "id": 9, "tasks": ["T9.1", "T9.2", "T9.3", "T9.4", "T9.5", "T9.6"], "label": "Test Apps — transports and topology" },
    { "id": 10, "tasks": ["T10.1", "T10.2", "T10.3", "T10.4"], "label": "Test Apps — durability and error policies" },
    { "id": 11, "tasks": ["T11.1", "T11.2", "T11.3", "T11.4"], "label": "Test Apps — coexistence, multi-provider and tenancy" }
  ]
}
```
