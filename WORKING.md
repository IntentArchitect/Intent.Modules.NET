# Working Context — NServiceBus Module

> **Branch:** `feature/nservicebus`
>
> **How to use this file:** Read it before touching anything under
> `Modules/Intent.Modules.Eventing.NServiceBus/` or `Tests/NServiceBus.*`.
> If your task contradicts what is documented here, stop and flag the conflict — do not proceed.
> If the work has moved on from what is documented here, offer to revise or extend this file.

---

## Conceptual Models

### How NServiceBus Handler Discovery Works

NServiceBus discovers message handlers at **endpoint build time** via **assembly scanning** —
not via DI. The scanner finds all types implementing `IHandleMessages<T>` in the loaded assemblies.

**Critical constraint:** The scanner explicitly skips any type where
`type.IsGenericTypeDefinition == true`. Open generic types are invisible to it.

**DI is for resolution, not discovery.** Registering `IHandleMessages<T>` in ASP.NET Core DI
(e.g. `services.AddTransient<IHandleMessages<TestMessageEvent>, NServiceBusMessageHandler<TestMessageEvent>>()`)
has **no effect** on whether NServiceBus knows about the handler. DI is only consulted after
the handler registry has already been built from scanning.

**The handler registry** (`NServiceBus.Unicast.MessageHandlerRegistry`) and
**message metadata registry** (`NServiceBus.Unicast.Messages.MessageMetadataRegistry`) are the
source of truth. They can be populated directly via NSB's advanced extensibility API —
bypassing the scanner entirely:

```csharp
var settings = AdvancedExtensibilityExtensions.GetSettings(endpointConfiguration);
var messageHandlerRegistry = settings.GetOrCreate<MessageHandlerRegistry>();
var messageMetadataRegistry = settings.GetOrCreate<MessageMetadataRegistry>();
messageHandlerRegistry.AddMessageHandlerForMessage<THandler, TMessage>();
messageMetadataRegistry.RegisterMessageTypeWithHierarchy(typeof(TMessage), Array.Empty<Type>());
```

This is what `AddHandler<T>()` (NSB 10.2 source generator) compiles down to internally.

### What the Module Generates

Three templates, all in `Infrastructure/Eventing/` or `Infrastructure/Configuration/`:

| Template | Output file | Purpose |
|---|---|---|
| `NServiceBusConfiguration` | `Configuration/NServiceBusConfiguration.cs` | Static class — endpoint setup, transport config, handler registration, routing |
| `NServiceBusMessageHandler` | `Eventing/NServiceBusMessageHandler.cs` | Single open generic `IHandleMessages<TMessage>` — delegates to `IIntegrationEventHandler<TMessage>` |
| `NServiceBusMessageBus` | `Eventing/NServiceBusMessageBus.cs` | `IMessageBus` implementation wrapping NSB's `IMessageSession` / `IMessageHandlerContext` |

### How Handler Registration Fits Together

`NServiceBusConfiguration.ConfigureMainEndpoint` (generated) calls a private
`RegisterHandler<THandler, TMessage>(endpointConfiguration)` helper for each subscribed
message and command type. This populates the handler registry directly.

`NServiceBusConfigurationTemplate` reads the subscribed types from
`NServiceBusMessageHandlerTemplate.SubscribedMessageModels` and
`NServiceBusMessageHandlerTemplate.SubscribedCommandModels` at build time.

### Intent SF Pipeline Constraints

- `GetTypeName` / `UseType` calls must be **deferred to `AfterBuild`** (or `OnBuild`) — they
  require type resolution infrastructure that is not ready in the template constructor.
- `FindTemplateInstances<NServiceBusMessageHandlerTemplate>` is safe inside `CSharpFile.OnBuild`
  callbacks because all template instances are registered before `OnBuild` fires.

### Endpoint Architecture (Current)

One NSB endpoint per application. All events and commands flow through it. The endpoint name
is read from `appsettings.json` (`NServiceBus:EndpointName`) at runtime — it is **not**
hardcoded in generated code.

The `NServiceBus` stereotype on Integration Commands/Events carries the `EndpointName` used
for **routing** (i.e. where to send commands). Commands this app *handles* do not need a
routing entry. Commands this app *sends* must have EndpointName set on the stereotype.

---

## Decision Log

### ✅ Use `RegisterHandler` + NSB internal registry APIs (not assembly scanner)
- **Decided:** commit `22ac223725`
- **Why:** Scanner skips open generics. Internal registry APIs are the documented bypass.
  Keeps `NServiceBusMessageHandler<TMessage>` as a single open generic — explicit, readable,
  no boilerplate subclasses.
- **Status:** Decision correct. Implementation was **accidentally lost** in commit `e593812272`
  (per-command endpoint refactor rewrote the config template). Needs to be restored.

### ✅ Single endpoint architecture
- **Decided:** commit `71cfdbe614` (after reverting per-command-endpoint approach)
- **Why:** Per-command-endpoint added significant complexity for marginal benefit. Single
  endpoint with explicit conventions is simpler and sufficient for the current scope.

### ✅ `EndpointName` stereotype is mandatory on Integration Commands/Events
- **Decided:** commit `71cfdbe614`
- **Why:** Without it, routing is ambiguous and commands silently misroute. A missing endpoint
  name is a build-time error, not a runtime surprise.

### ✅ Outbox uses SqlPersistence (NServiceBus.Persistence.Sql)
- **Decided:** early in the build
- **Why:** SqlPersistence lets NServiceBus share the EF Core `DbConnection`/`DbTransaction`,
  making the outbox truly atomic with the application's writes.

---

## Rejected Approaches

### ❌ Concrete sealed subclasses per message type
- **Tried:** commit `620810cd12`, **reverted:** commit `51c9198d38`
- **Why rejected:** The user explicitly does not want this. It clutters the generated file,
  obscures what is actually subscribed, and is unnecessary given the `RegisterHandler` approach.
- **Do not revisit** without explicit instruction from the user.

### ❌ `services.AddTransient<IHandleMessages<T>, NServiceBusMessageHandler<T>>()`
- **Why rejected:** Registers with ASP.NET Core DI but does not feed NSB's handler registry.
  NSB's `LoadHandlersConnector` uses the internal registry, not DI.
  Error produced: `No handlers could be found for message type: ...`
- **Currently present** in test app generated files (restored by the revert) but will be
  overwritten by the next SF run once the template is fixed.

### ❌ Relying on NSB assembly scanner for open generics
- **Why rejected:** Scanner calls `type.IsGenericTypeDefinition` and skips the type. Silent
  at startup; fails at first message dispatch with "No handlers could be found."

---

## Current Broken State

**`RegisterHandler` is missing from `NServiceBusConfigurationTemplatePartial.cs`.**

Lost in commit `e593812272`. The config template currently generates `ConfigureMainEndpoint`
with no handler registration calls. All test apps produce
`No handlers could be found for message type: ...` when a message arrives.

**Fix:** Restore the `RegisterHandler` pattern in `NServiceBusConfigurationTemplatePartial.cs`:

1. Add a `CSharpFile.OnBuild` callback that:
   - Finds `NServiceBusMessageHandlerTemplate` via `FindTemplateInstances`
   - Reads `SubscribedMessageModels` and `SubscribedCommandModels`
   - Inserts `RegisterHandler<NServiceBusMessageHandler<T>, T>(endpointConfiguration);`
     calls into `ConfigureMainEndpoint` before the `return` statement
2. Add the private `static void RegisterHandler<THandler, TMessage>` helper method
   to the `NServiceBusConfiguration` class

Reference implementation: `git show 22ac223725` —
`Modules/Intent.Modules.Eventing.NServiceBus/Templates/NServiceBusConfiguration/NServiceBusConfigurationTemplatePartial.cs`

---

## Still To Do

- [ ] Restore `RegisterHandler` in `NServiceBusConfigurationTemplatePartial.cs`
- [ ] Build module DLL, run SF on all 5 test apps (skip SQS), verify 0 errors
- [ ] Build all test app Infrastructure projects — 0 compile errors
- [ ] Runtime verify: start at least one app, dispatch a message, confirm handler executes
- [ ] Commit

---

## Index

### Key source files
| File | Role |
|---|---|
| `Modules/Intent.Modules.Eventing.NServiceBus/Templates/NServiceBusConfiguration/NServiceBusConfigurationTemplatePartial.cs` | **Primary fix target** — generates endpoint config + handler registration |
| `Modules/Intent.Modules.Eventing.NServiceBus/Templates/NServiceBusMessageHandler/NServiceBusMessageHandlerTemplatePartial.cs` | Generates the open generic handler class; exposes `SubscribedMessageModels` / `SubscribedCommandModels` |
| `Modules/Intent.Modules.Eventing.NServiceBus/Templates/NServiceBusMessageBus/NServiceBusMessageBusTemplatePartial.cs` | Generates the message bus wrapper |
| `Modules/Intent.Modules.Eventing.NServiceBus/Settings/ModuleSettingsExtensions.cs` | Typed accessors for module settings (Transport, OutboxPattern, etc.) |

### Key commits
| Commit | Summary |
|---|---|
| `22ac223725` | **Reference** — introduced `RegisterHandler` + NSB internal registry pattern |
| `e593812272` | Per-command endpoint refactor — accidentally dropped `RegisterHandler` |
| `71cfdbe614` | Single-endpoint architecture + mandatory EndpointName stereotype |
| `620810cd12` | Concrete subclasses (tried) |
| `51c9198d38` | Revert of concrete subclasses |

### Test apps
| App | ID (IA) | Transport setting key |
|---|---|---|
| `NServiceBus.AzureServiceBus` | `c9e96aa7-003d-479b-9463-c3eba62a5617` | `azure-service-bus` |
| `NServiceBus.LearnerTransport` | `d69bfc1f-2e5b-4609-b1da-c715bf152c84` | `learning-transport` |
| `NServiceBus.RabbitMQ` | `d5971438-3fe8-4d7b-abb3-01978c0f447b` | `rabbit-mq` |
| `NServiceBus.OutboxPattern.Publish` | `3ea966d0-b6c8-4478-82b7-27652ec1cb89` | `rabbit-mq` |
| `NServiceBus.OutboxPattern.Subscribe` | `e82d01d3-006c-431b-8f3d-8f6d823c14ec` | `rabbit-mq` |

### Related skills
| Skill | Relevant for |
|---|---|
| `.claude/skills/intent-architect-mcp.md` | SF workflow, apply staged changes, build validation |
| `.claude/skills/file-builder-expert.md` | `CSharpFile` builder API, `OnBuild`/`AfterBuild` callbacks |
| `.claude/skills/intent-module-orchestrator.md` | `FindTemplateInstances`, cross-template reads, priority bands |
