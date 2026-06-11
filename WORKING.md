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

## Current State

The config template has been stabilized. The broken state described below is now resolved.

**What was fixed:**

- `ConfigureCommonSettings` (vague helper) removed
- `ConfigureMainEndpoint` now inlines transport/persistence/installers/serialization/recoverability
- `RegisterHandlers` + `RegisterHandler<THandler,TMessage>` generation added; uses NSB internal
  registry APIs (no DI registration, no assembly scanning)
- `ConfigureMessageConventions` extracted as a narrow-responsibility helper
- `RouteToEndpoint` kept inline in `ConfigureMainEndpoint` for sent commands only

**Runtime verification (2026-06-11):**

- LearnerTransport: PUT `/api/external-message-publish/publish-external-message` → `[HANDLER HIT] TestMessageHandler received: Runtime verification - handler hit test` ✓
- RabbitMQ: POST `/api/animals/publish-test-event` (bespoke endpoint) → `[HANDLER HIT] RabbitMQ.TestMessageHandler received TestMessageEvent` ✓
- OutboxPattern Publish→Subscribe: PUT `/api/test-event-send` on Publish → Subscribe `TestEventHandler` re-published → `[HANDLER HIT] Subscribe.AnotherTestMessageHandler received: OutboxPattern runtime verification - handler hit test` ✓
- AzureServiceBus: PUT `/api/external-message-publish/publish-external-message` → `[HANDLER HIT] AzureServiceBus.TestMessageHandler received TestMessageEvent` ✓ (user secrets provided real credentials)

**SF applied to:**
- NServiceBus.LearnerTransport ✓ (builds + runtime verified)
- NServiceBus.RabbitMQ ✓ (builds + runtime verified)
- NServiceBus.AzureServiceBus ✓ (builds + runtime verified)
- NServiceBus.OutboxPattern.Publish ✓ (builds + runtime verified)
- NServiceBus.OutboxPattern.Subscribe ✓ (builds + runtime verified)

---

## Still To Do

- [x] Restore `RegisterHandler` in `NServiceBusConfigurationTemplatePartial.cs`
- [x] Reshape `NServiceBusConfigurationTemplatePartial.cs` around one `ConfigureMainEndpoint(...)`
- [x] Inline transport/persistence/installers/serialization/recoverability into `ConfigureMainEndpoint(...)`
- [ ] Ensure commands require `EndpointName`; events/messages do not (validation — separate task)
- [x] Build module DLL, run SF on all 5 test apps (skip SQS), verify 0 errors
- [x] Build all test app Infrastructure projects — 0 compile errors
- [x] Runtime verify: start at least one app, dispatch a message, confirm handler executes
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
