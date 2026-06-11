# Working Context — NServiceBus Module

> **Status:** In progress — read this before touching anything under
> `Modules/Intent.Modules.Eventing.NServiceBus/` or `Tests/NServiceBus.*`
>
> **Branch:** `feature/nservicebus`
>
> **Delete this file** when all items under "Still To Do" are complete and verified.
> Extract stable decisions into a proper skill at that point.

---

## What We Are Building

An Intent Architect module (`Intent.Eventing.NServiceBus`) that generates the NServiceBus
infrastructure for a .NET application: endpoint configuration, message handler wiring,
transport setup, and optional SQL Persistence outbox.

The module generates into a Clean Architecture test app structure. There are 5 test apps
(skip SQS — no AWS resources):

| App | Transport | Outbox |
|---|---|---|
| `NServiceBus.AzureServiceBus` | Azure Service Bus | None |
| `NServiceBus.LearnerTransport` | Learning Transport | None |
| `NServiceBus.RabbitMQ` | RabbitMQ | None |
| `NServiceBus.OutboxPattern.Publish` | RabbitMQ | SqlPersistence (publish side) |
| `NServiceBus.OutboxPattern.Subscribe` | RabbitMQ | SqlPersistence (subscribe side) |

---

## Active Design Decisions

### Handler Registration: `RegisterHandler<THandler, TMessage>` via NSB internal registry

**Do NOT change this without discussing first.**

NServiceBus handler discovery works via assembly scanning. The scanner **explicitly skips open
generic types**. `NServiceBusMessageHandler<TMessage>` is an open generic — it is invisible to
the scanner.

The correct fix is **not** concrete sealed subclasses (tried, reverted — see Rejected Approaches).

The correct fix is a private `RegisterHandler<THandler, TMessage>` helper in
`NServiceBusConfiguration` that directly calls NSB's internal registry APIs:

```csharp
private static void RegisterHandler<THandler, TMessage>(EndpointConfiguration endpointConfiguration)
    where THandler : class, IHandleMessages<TMessage>
    where TMessage : class
{
    var settings = NServiceBus.Configuration.AdvancedExtensibility.AdvancedExtensibilityExtensions.GetSettings(endpointConfiguration);
    var messageHandlerRegistry = settings.GetOrCreate<NServiceBus.Unicast.MessageHandlerRegistry>();
    var messageMetadataRegistry = settings.GetOrCreate<NServiceBus.Unicast.Messages.MessageMetadataRegistry>();
    messageHandlerRegistry.AddMessageHandlerForMessage<THandler, TMessage>();
    messageMetadataRegistry.RegisterMessageTypeWithHierarchy(typeof(TMessage), Array.Empty<Type>());
}
```

This was first introduced in commit `22ac223725` ("refactor: Replace concrete NSB handler
subclasses with generic handler + registry registration") and was then **accidentally lost**
during the per-command endpoint refactor in commit `e593812272`.

The generated `ConfigureMainEndpoint` (or each command endpoint) calls this helper once per
subscribed message/command type:

```csharp
RegisterHandler<NServiceBusMessageHandler<TalkToPersonCommand>, TalkToPersonCommand>(endpointConfiguration);
RegisterHandler<NServiceBusMessageHandler<TestMessageEvent>, TestMessageEvent>(endpointConfiguration);
```

This keeps `NServiceBusMessageHandler<TMessage>` as a single open generic class. No concrete
subclasses. The registration is explicit and readable — you can see exactly what is subscribed.

### Single endpoint architecture (current)

One NSB endpoint per application, configured via `ConfigureMainEndpoint`. Commands and events
all flow through it. The endpoint name comes from `NServiceBus:EndpointName` in appsettings
(mandatory — enforced via `EndpointName` stereotype on Integration Commands/Messages).

A per-command-endpoint architecture was explored (commit `e593812272`) but the current state
uses single-endpoint. Do not split into per-command endpoints without explicit instruction.

### `NServiceBusMessageHandler<TMessage>` — one class, open generic

Lives in `Eventing/NServiceBusMessageHandler.cs`. Implements `IHandleMessages<TMessage>`.
Injects `IIntegrationEventHandler<TMessage>`, `NServiceBusMessageBus`, and optionally
`ApplicationDbContext` (outbox only). No subclasses.

### Outbox pattern — SqlPersistence only

When `OutboxPattern = SqlPersistence`, the handler also injects `ApplicationDbContext` and
uses `SqlPersistenceSession` to share the EF Core transaction with NServiceBus.

---

## Rejected Approaches — Do Not Revisit Without Explicit Instruction

### ❌ Concrete sealed subclasses per message type

Tried in commit `620810cd12`, reverted in `51c9198d38`.

Generated one `internal sealed class NServiceBusXxxHandler : NServiceBusMessageHandler<Xxx>`
per subscribed message type. Solves the scanner problem but:
- The user explicitly does not want this
- Clutters the generated file with boilerplate subclasses
- Makes it harder to see what is subscribed at a glance
- The `RegisterHandler` approach achieves the same thing more cleanly

### ❌ `services.AddTransient<IHandleMessages<T>, NServiceBusMessageHandler<T>>()`

Registers the handler in ASP.NET Core DI but **does not feed NSB's handler registry**.
NSB's `LoadHandlersConnector` uses its own internal registry (built from assembly scanning),
not the DI container. These registrations are silently ignored for message dispatch.
Error produced: `No handlers could be found for message type: ...`

### ❌ Relying on NSB assembly scanner for open generics

NSB's scanner explicitly skips `IsGenericTypeDefinition == true`. Any solution that depends
on the scanner discovering `NServiceBusMessageHandler<TMessage>` will fail silently at startup
and produce "No handlers could be found" at dispatch time.

---

## Current Broken State

**The `RegisterHandler` helper is missing from `NServiceBusConfigurationTemplatePartial.cs`.**

It was lost when commit `e593812272` rewrote the configuration template. The template currently
generates `ConfigureMainEndpoint` without any handler registration calls, so all test apps
produce "No handlers could be found" when a message arrives.

The fix needs to go into `NServiceBusConfigurationTemplatePartial.cs`:
- Add a `CSharpFile.OnBuild` (or `AfterBuild`) callback that finds the `ConfigureMainEndpoint`
  method and inserts `RegisterHandler<...>(endpointConfiguration)` calls per subscribed type
- Add the private `RegisterHandler<THandler, TMessage>` static method to the class
- Cover both `SubscribedMessageModels` (events) and `SubscribedCommandModels` (commands)
  from `NServiceBusMessageHandlerTemplate`

Reference implementation: commit `22ac223725` —
`Modules/Intent.Modules.Eventing.NServiceBus/Templates/NServiceBusConfiguration/NServiceBusConfigurationTemplatePartial.cs`

---

## Still To Do

- [ ] Restore `RegisterHandler` in `NServiceBusConfigurationTemplatePartial.cs`
- [ ] Build module, run SF on all 5 test apps (skip SQS), verify builds clean
- [ ] Runtime verify: start at least one app, confirm endpoint initialises, dispatch a message
- [ ] Commit

---

## Key Files

| File | Purpose |
|---|---|
| `Modules/Intent.Modules.Eventing.NServiceBus/Templates/NServiceBusConfiguration/NServiceBusConfigurationTemplatePartial.cs` | Generates `NServiceBusConfiguration.cs` — endpoint setup, transport, handler registration |
| `Modules/Intent.Modules.Eventing.NServiceBus/Templates/NServiceBusMessageHandler/NServiceBusMessageHandlerTemplatePartial.cs` | Generates `NServiceBusMessageHandler.cs` — the open generic handler class |
| `Tests/NServiceBus.AzureServiceBus/NServiceBus.AzureServiceBus.Infrastructure/Configuration/NServiceBusConfiguration.cs` | Primary test target for the fix |

---

## Session History (brief)

- Built the module from scratch over several sessions
- Single-endpoint architecture adopted, `EndpointName` stereotype made mandatory
- Handler registration pattern was correctly solved with `RegisterHandler` + NSB internal APIs
  (commit `22ac223725`) then accidentally lost in the per-command-endpoint refactor
- Two wasted cycles trying concrete subclasses / AddTransient before tracing the issue back
  to the missing `RegisterHandler` helper
- This WORKING.md created to prevent further circles
