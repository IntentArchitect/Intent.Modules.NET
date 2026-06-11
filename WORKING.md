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

### Intended `NServiceBusConfiguration` Shape

`NServiceBusConfiguration` should expose **one** generated endpoint setup path:
`ConfigureMainEndpoint(IConfiguration configuration)`.

That method is the authoritative, readable "story" of endpoint construction and should wire
core NSB infrastructure **directly in the method body**, not hide it behind a vague
`ConfigureCommonSettings(...)` / `ConfigureTransportSettings(...)` helper.

Specifically, `ConfigureMainEndpoint(...)` should:

- create `EndpointConfiguration` from `NServiceBus:EndpointName`
- configure the selected transport inline
- configure outbox/persistence inline when enabled
- enable installers inline
- configure serialization inline
- configure recoverability inline
- call focused helper methods for secondary concerns only:
  - `ConfigureMessageConventions(endpointConfiguration)`
  - `RegisterHandlers(endpointConfiguration)`
  - `ConfigureCommandRouting(routing, configuration)`

This keeps the main method explicit while still allowing the repetitive generated
`RegisterHandler<...>` and `RouteToEndpoint(...)` statements to be delegated to narrowly
named helpers.

**Do not** reintroduce a shared helper whose job is "do all the common endpoint setup"
under a fuzzy name like `ConfigureCommonSettings(...)`. If helper extraction is needed,
the helper name must describe a real narrow responsibility.

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

### Handler Registration and Routing Rules

The generated configuration must keep **handler registration** and **command routing** as
two separate concerns:

- **Handler registration** is explicit and always targets the single main endpoint via
  `RegisterHandler<NServiceBusMessageHandler<TMessage>, TMessage>(endpointConfiguration);`
- **Command routing** is explicit and only applies to commands this app **sends**
  via `routing.RouteToEndpoint(typeof(TCommand), ...)`
- **Events/messages do not have endpoint names**
- **Subscribed events/messages** get handler registrations, not routing entries
- **Subscribed commands** get handler registrations, not routing entries
- **Sent commands** get routing entries and therefore require endpoint-name metadata

The generic `NServiceBusMessageHandler<TMessage>` remains the single handler concept.
Do not generate concrete subclasses per message type.

---

## Decision Log

### ✅ Use `RegisterHandler` + NSB internal registry APIs (not assembly scanner)
- **Decided:** commit `22ac223725`
- **Why:** Scanner skips open generics. Internal registry APIs are the documented bypass.
  Keeps `NServiceBusMessageHandler<TMessage>` as a single open generic — explicit, readable,
  no boilerplate subclasses.
- **Status:** Decision correct. The branch later drifted because pieces of the registration
  logic and endpoint-configuration refactors became entangled. Restore the explicit
  registration pattern **without** restoring the multi-endpoint architecture.

### ✅ Single endpoint architecture
- **Decided:** commit `71cfdbe614` (after reverting per-command-endpoint approach)
- **Why:** Per-command-endpoint added significant complexity for marginal benefit. Single
  endpoint with explicit conventions is simpler and sufficient for the current scope.

### ✅ `ConfigureMainEndpoint(...)` owns core endpoint construction inline
- **Decided:** current working direction on this branch
- **Why:** The main endpoint method should be the most readable summary of how the endpoint
  is assembled. Transport, persistence, installers, serialization, and recoverability are
  part of endpoint construction itself, not secondary concerns to hide behind a generic helper.
- **Implementation shape:** keep small focused helpers for `ConfigureMessageConventions(...)`,
  `RegisterHandlers(...)`, and `ConfigureCommandRouting(...)`; keep the transport and
  infrastructure wiring directly in `ConfigureMainEndpoint(...)`.

### ✅ `EndpointName` stereotype is mandatory on Integration Commands/Events
- **Decided:** commit `71cfdbe614`
- **Why:** Without it, routing is ambiguous and commands silently misroute. A missing endpoint
  name is a build-time error, not a runtime surprise.

### ✅ `EndpointName` is mandatory for commands, not events/messages
- **Decided:** current working direction on this branch
- **Why:** Commands need an explicit destination endpoint for routing. Events/messages are
  publish/subscribe and do not carry endpoint names.
- **Implication:** `WORKING.md`, validators, stereotype defaults, and generated code must all
  reflect that the mandatory endpoint-name rule is for commands specifically.

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

### ❌ Multiple generated endpoint methods / registrations per application
- **Tried:** per-command endpoint approach, especially around commit `e593812272`
- **Why rejected:** It solved one problem by introducing a much larger one: multiple endpoint
  registrations, extra configuration surface area, more DI/runtime complexity, and drift away
  from the intended single-endpoint mental model.
- **Keep from that work:** the explicit registration idea and any compact builder patterns
  worth reusing.
- **Do not keep:** per-command-group endpoint methods, multiple
  `services.AddNServiceBusEndpoint(...)` calls, or endpoint-name-derived method proliferation.

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

**The config template shape has drifted from the intended single-endpoint design.**

The branch needs `NServiceBusConfigurationTemplatePartial.cs` to generate:

- one `ConfigureMainEndpoint(...)`
- inline endpoint construction in that method
- explicit generic handler registration
- explicit command routing
- no per-command endpoint methods

When this drifts, the failure modes are:

- `No handlers could be found for message type: ...` because handler registration disappeared
- commands not being delivered correctly because routing metadata/rules are inconsistent
- the configuration class becoming harder to reason about because endpoint construction is
  split across too many indirections or multiple generated endpoint methods

**Fix direction:**

1. Keep `ConfigureMainEndpoint(...)` as the single endpoint-construction method
2. Inline transport/persistence/installers/serialization/recoverability in that method
3. Delegate only conventions, handler-registration emission, and sent-command routing
4. Restore explicit `RegisterHandler<NServiceBusMessageHandler<T>, T>(endpointConfiguration);`
   generation for subscribed events/messages and subscribed commands
5. Keep `RouteToEndpoint(...)` generation only for commands this app sends
6. Do not reintroduce multi-endpoint configuration methods

Useful commit references:

- `22ac223725` — explicit generic handler registration pattern
- `903459f819` — compacted configuration structure worth learning from
- `e593812272` — contains ideas worth studying, but **must not** be copied wholesale because
  it includes the rejected multi-endpoint direction

---

## Still To Do

- [ ] Restore `RegisterHandler` in `NServiceBusConfigurationTemplatePartial.cs`
- [ ] Reshape `NServiceBusConfigurationTemplatePartial.cs` around one `ConfigureMainEndpoint(...)`
- [ ] Inline transport/persistence/installers/serialization/recoverability into `ConfigureMainEndpoint(...)`
- [ ] Ensure commands require `EndpointName`; events/messages do not
- [ ] Build module DLL, run SF on all 5 test apps (skip SQS), verify 0 errors
- [ ] Build all test app Infrastructure projects — 0 compile errors
- [ ] Runtime verify: start at least one app, dispatch a message, confirm handler executes
- [ ] Revisit test coverage/doc notes for mixed-broker coexistence scenarios
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
| `903459f819` | Compacts configuration via helper extraction; useful shape reference, but not the final endpoint-construction layout |
| `e593812272` | Contains useful registration/routing ideas, but also the rejected per-command endpoint architecture |
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
