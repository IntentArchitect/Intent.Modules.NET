# NServiceBus Module Context

> **Module:** `Intent.Modules.Eventing.NServiceBus`
>
> **Purpose:** Durable architectural and implementation context for future work in this
> module. Read this before changing templates, settings, stereotypes, validations, or test
> applications related to NServiceBus.

---

## Core Architecture

### Single Endpoint Per Application

The intended architecture is **one NServiceBus endpoint per application**.

- The generated configuration should expose one endpoint-construction path:
  `ConfigureMainEndpoint(IConfiguration configuration)`
- The endpoint name is read from configuration via `NServiceBus:EndpointName`
- Do **not** generate multiple endpoint methods or multiple
  `services.AddNServiceBusEndpoint(...)` registrations per application

This is a deliberate simplification. Earlier work explored per-command/per-endpoint
architectures, but that added too much complexity for the value it provided.

### Generic Handler + Explicit Registration

The module uses a single open generic handler concept:

- `NServiceBusMessageHandler<TMessage>` implements `IHandleMessages<TMessage>`
- It delegates to `IIntegrationEventHandler<TMessage>`
- In this module, `IIntegrationEventHandler<TMessage>` is the application-facing abstraction
  for both Integration Events/messages **and** Integration Commands

Handlers must be registered **explicitly** in generated configuration using the NServiceBus
internal registries. Do **not** rely on DI registration or assembly scanning to make open
generic handlers discoverable.

Expected shape:

```csharp
RegisterHandler<NServiceBusMessageHandler<TMessage>, TMessage>(endpointConfiguration);
```

### Intended `NServiceBusConfiguration` Shape

`ConfigureMainEndpoint(...)` should be the readable “story” of endpoint construction.
Core infrastructure should be wired **inline** in that method:

- create `EndpointConfiguration`
- configure transport inline
- configure outbox/persistence inline when enabled
- enable installers inline
- configure serialization inline
- configure recoverability inline

Secondary concerns may be delegated to focused helpers only:

- `ConfigureMessageConventions(endpointConfiguration)`
- `RegisterHandlers(endpointConfiguration)`
- `ConfigureCommandRouting(routing, configuration)`

Do **not** hide endpoint construction behind vague helpers such as
`ConfigureCommonSettings(...)`. If helper extraction is necessary, the helper name must match
one real narrow responsibility.

---

## NServiceBus Technical Constraints

### Assembly Scanning Constraint

NServiceBus discovers handlers at endpoint build time via assembly scanning.

Critical constraint:

- Open generic types (`type.IsGenericTypeDefinition == true`) are skipped by the scanner
- Therefore `NServiceBusMessageHandler<TMessage>` will not be discovered automatically

### DI Is Not Discovery

ASP.NET Core DI registration such as:

```csharp
services.AddTransient<IHandleMessages<T>, NServiceBusMessageHandler<T>>();
```

does **not** make NServiceBus aware of the handler. DI is used for resolution only after the
NServiceBus handler registry has already been built.

### Internal Registries Are the Source of Truth

The handler registry and message metadata registry are the authoritative source of handler
discovery when using the generic handler approach:

```csharp
var settings = AdvancedExtensibilityExtensions.GetSettings(endpointConfiguration);
var messageHandlerRegistry = settings.GetOrCreate<MessageHandlerRegistry>();
var messageMetadataRegistry = settings.GetOrCreate<MessageMetadataRegistry>();
messageHandlerRegistry.AddMessageHandlerForMessage<THandler, TMessage>();
messageMetadataRegistry.RegisterMessageTypeWithHierarchy(typeof(TMessage), Array.Empty<Type>());
```

This mirrors what `AddHandler<T>()` compiles down to in the newer NServiceBus source-generated
approach.

### Intent Template Pipeline Constraint

- `GetTypeName(...)` / `UseType(...)` calls that depend on full type resolution must be done
  in `OnBuild` / `AfterBuild`, not in the template constructor
- `FindTemplateInstances<NServiceBusMessageHandlerTemplate>(...)` is safe from `CSharpFile.OnBuild`
  because template instances have already been registered by then

---

## Commands vs Events

### Routing and Handler Rules

Handler registration and command routing are separate concerns:

- Subscribed events/messages get handler registrations
- Subscribed commands get handler registrations
- Sent commands get routing entries
- Events/messages do **not** have endpoint names

Expected generated shapes:

```csharp
RegisterHandler<NServiceBusMessageHandler<TMessage>, TMessage>(endpointConfiguration);
```

```csharp
routing.RouteToEndpoint(typeof(TCommand), destinationEndpoint);
```

### `NServiceBus` Stereotype

The `NServiceBus` stereotype matters especially when multiple broker modules are installed.
It distinguishes which Integration Commands/Events belong to NServiceBus versus other broker
technologies.

### `EndpointName` Requirement

`EndpointName` is mandatory for **commands**, not for events/messages.

- Commands need an explicit destination endpoint for routing
- Events/messages are publish/subscribe and do not carry endpoint names

Validators, defaults, stereotype behavior, and generated code should all reinforce this rule.

---

## Outbox and Persistence

The outbox path is expected to use SQL Persistence:

- `NServiceBus.Persistence.Sql`
- shared EF Core `DbConnection` / `DbTransaction`
- asynchronous dispatch through NServiceBus after durable persistence

This is an important supported scenario, not an optional edge case.

---

## Rejected Approaches

### Concrete Sealed Handler Subclasses

Rejected because:

- the user explicitly does not want them
- they clutter generated output
- they obscure the real subscription model
- they are unnecessary when explicit generic registration is available

Do not revisit without explicit instruction.

### Multiple Generated Endpoints Per Application

Rejected because:

- it introduces too much runtime and configuration complexity
- it breaks the intended single-endpoint mental model
- it creates extra endpoint methods and extra `AddNServiceBusEndpoint(...)` calls

Useful ideas can still be learned from historical commits, but the multiple-endpoint topology
must not be restored.

### DI-Only Handler Registration

Rejected because it does not populate the NServiceBus handler registry and results in runtime
failures such as:

`No handlers could be found for message type: ...`

### Reliance on Assembly Scanning for Open Generics

Rejected because NServiceBus explicitly skips open generic type definitions during scanning.

---

## Acceptance Matrix

Changes in this module must be evaluated against the broader matrix, not just one happy path.

### Coexistence

- NServiceBus must coexist with other broker modules such as MassTransit, Azure Service Bus
  module integrations, Kafka, and similar technologies
- The `NServiceBus` stereotype is part of how that coexistence remains unambiguous

### Transport Coverage

Dedicated scenarios exist for:

- Azure Service Bus
- Learning Transport
- RabbitMQ
- Amazon SQS

### Runtime Validation Expectation

Expected runnable validation currently focuses on:

- Azure Service Bus
- Learning Transport
- RabbitMQ

Amazon SQS may exist in the test matrix but is currently a known gap for routine runtime
verification.

### Outbox Coverage

RabbitMQ + SQL Persistence outbox scenarios must continue to work.

### Message Flows

Coverage should include:

- service-level publish flows
- command sending flows
- Integration Event/message publish flows
- Integration Command sending flows
- subscription/handling for both commands and events

Success is not just “one app starts.” It is preserving the intended model across
mixed-broker, multi-transport, and outbox scenarios.

---

## Useful Historical Commits

| Commit | Why it matters |
|---|---|
| `22ac223725` | Reference for explicit generic handler registration using NSB internal registries |
| `903459f819` | Useful configuration-compaction ideas, but not the final desired endpoint-construction layout |
| `e593812272` | Contains some useful registration/routing ideas, but also the rejected multi-endpoint approach |
| `71cfdbe614` | Important single-endpoint direction and endpoint-name validation history |
| `620810cd12` | Historical attempt at concrete subclasses |
| `51c9198d38` | Revert of concrete subclass approach |

---

## Key Files

| File | Role |
|---|---|
| `Templates/NServiceBusConfiguration/NServiceBusConfigurationTemplatePartial.cs` | Primary configuration generation logic |
| `Templates/NServiceBusMessageHandler/NServiceBusMessageHandlerTemplatePartial.cs` | Generates the generic NSB handler and exposes subscribed models |
| `Templates/NServiceBusMessageBus/NServiceBusMessageBusTemplatePartial.cs` | Generates the message bus wrapper |
| `Settings/ModuleSettingsExtensions.cs` | Typed settings access |
