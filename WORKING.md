# Working Context — NServiceBus Module

> **Branch:** `feature/nservicebus`
>
> **How to use this file:** Read it before touching anything under
> `Modules/Intent.Modules.Eventing.NServiceBus/` or `Tests/NServiceBus.*`.
> Read `Modules/Intent.Modules.Eventing.NServiceBus/CONTEXT.md` first for durable module
> knowledge; this file only captures the current branch/task state.
> If your task contradicts what is documented here, stop and flag the conflict — do not proceed.
> If the work has moved on from what is documented here, offer to revise or extend this file.

---

## Current Goal

Stabilize the NServiceBus module around the intended single-endpoint architecture and restore
the correct explicit generic handler-registration behavior without reintroducing the rejected
multi-endpoint approach.

---

## Branch-Specific Direction

Use the durable architecture in `CONTEXT.md`, with these additional branch-specific emphases:

- Keep `ConfigureMainEndpoint(...)` as the single endpoint-construction method
- Inline transport/persistence/installers/serialization/recoverability in that method
- Delegate only conventions, handler-registration emission, and sent-command routing
- Restore explicit `RegisterHandler<NServiceBusMessageHandler<T>, T>(endpointConfiguration);`
  generation for subscribed events/messages and subscribed commands
- Keep `RouteToEndpoint(...)` generation only for commands this app sends
- Do not reintroduce multi-endpoint configuration methods

---

## Current Broken State

**The config template shape has drifted from the intended single-endpoint design and the
current generated output no longer reliably reflects the desired registration model.**

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
- [ ] Capture the full acceptance matrix in test notes: mixed-broker coexistence, Azure Service Bus, Learning Transport, RabbitMQ, SQS gap, and outbox
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
