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

### .NET Version / NServiceBus Version Split

NServiceBus 9 (.NET 8/9) and NServiceBus 10 (.NET 10+) use different host-wiring APIs.
The template branches on `OutputTarget.GetMaxNetAppVersion().Major < 10` (`_isLegacyFramework`).

**.NET 10+ (NServiceBus 10):** `NServiceBus.Extensions.Hosting` v4 — endpoint registered via DI:

```csharp
services.AddNServiceBusEndpoint(ConfigureMainEndpoint(configuration));
```

**.NET 8/9 (NServiceBus 9):** `NServiceBus.Extensions.Hosting` v3 — endpoint registered on
the host builder. The template generates a public extension method and injects a call into
`Program.cs`:

```csharp
// Generated in NServiceBusConfiguration.cs:
public static IHostBuilder UseNServiceBusHost(this IHostBuilder hostBuilder)
    => hostBuilder.UseNServiceBus(ctx => ConfigureMainEndpoint(ctx.Configuration));

// Injected into Program.cs (via IProgramTemplate.AddHostBuilderConfigurationStatement):
builder.Host.UseNServiceBusHost();
```

`ConfigureMainEndpoint` stays `private static` in both paths. The `UseNServiceBusHost`
extension method is the public surface; it keeps the endpoint-config logic encapsulated.

The injection into `Program.cs` is registered from the **template constructor** using:

```csharp
programTemplate.CSharpFile.OnBuild(file =>
{
    file.AddUsing(this.Namespace);
    programTemplate.ProgramFile.AddHostBuilderConfigurationStatement(
        new CSharpStatement(“builder.Host.UseNServiceBusHost();”), priority: 500);
}, order: 30);
```

This must be done in the constructor because `AddHostBuilderConfigurationStatement` internally
calls `CSharpFile.OnBuild`, which requires `_isBuilt == false`. Doing it from
`BeforeTemplateExecution` is too late.

`NServiceBus.Extensions.Hosting` is added as an **explicit** NuGet dependency for all targets
(it arrives transitively in practice, but explicit avoids fragile transitive resolution).

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

`EndpointName` is mandatory for **all NServiceBus commands** — both sent and subscribed.
Events/messages do not carry endpoint names.

- `EndpointName` is a property of the command **definition**, not of any particular sender or
  subscriber. It declares which endpoint owns (handles) that command type.
- Both the app that sends a command and the app that subscribes to it must carry the same
  `EndpointName` value on the stereotype. Without it the definition is incomplete.
- Events/messages are publish/subscribe and do not require an endpoint name.

**Validation is enforced at SF time** in `NServiceBusConfigurationTemplatePartial.cs`. If any
sent command is missing a `NServiceBus` stereotype `EndpointName`, SF throws an
`ElementException` pointing at the specific command element. Intent Architect displays this
in the UI with the element highlighted:

```
Integration Command `OrderAnimal` is sent by this application but has no NServiceBus
endpoint name configured. Apply the NServiceBus stereotype and set Endpoint Name to the
destination endpoint.
```

There is no separate designer-level (real-time) validator — the SF-time `ElementException`
is the authoritative gate. This is consistent with how other validation is done in this repo.

See `.agents/instructions/exception-guidelines.md` for the general rule on when to use
`ElementException` vs `FriendlyException` vs `InvalidOperationException`.

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
  module integrations, Kafka, and similar technologies.
- The `NServiceBus` stereotype is the disambiguation mechanism — it marks which Integration
  Commands/Events belong to NServiceBus in a multi-broker application.
- `CompositeMessageBus` mode (multiple brokers in one app) is a verified scenario. In this
  mode `AddNServiceBusConfiguration` accepts a `MessageBrokerRegistry` parameter and registers
  message types against the NServiceBus bus rather than publishing a `ServiceConfigurationRequest`.

### Transport Coverage

| Transport | Test App | Runtime Verified |
|---|---|---|
| Learning Transport | `Tests/NServiceBus.LearnerTransport` | ✓ 2026-06-11 |
| RabbitMQ | `Tests/NServiceBus.RabbitMQ` | ✓ 2026-06-11 |
| Azure Service Bus | `Tests/NServiceBus.AzureServiceBus` | ✓ 2026-06-11 |
| Amazon SQS | `Tests/NServiceBus.SQS` | Handlers implemented; live SQS infra required to run |
| RabbitMQ + Outbox | `Tests/NServiceBus.OutboxPattern.Publish` + `Subscribe` | ✓ 2026-06-11 |

**SQS note:** The test app is fully generated and compiles. AWS credentials (IAM access key
or environment profile) are required to start it. `EnableInstallers()` will auto-create the
SQS queues and SNS topics on first run. Not suitable for routine CI verification without
real AWS credentials.

### Runtime Verification Protocol

For each transport, the minimum bar is:

1. App starts without errors
2. A message is published/sent via the HTTP API
3. `[HANDLER HIT]` log line appears in the console

Verified flows per test app (as of 2026-06-11):

| App | Flow | Trigger | Expected log |
|---|---|---|---|
| LearnerTransport | Event publish | `PUT /api/external-message-publish/publish-external-message` | `[HANDLER HIT] TestMessageHandler received: ...` |
| RabbitMQ | Event publish | `POST /api/animals/publish-test-event` | `[HANDLER HIT] RabbitMQ.TestMessageHandler received TestMessageEvent` |
| AzureServiceBus | Event publish | `PUT /api/external-message-publish/publish-external-message` | `[HANDLER HIT] AzureServiceBus.TestMessageHandler received TestMessageEvent` |
| OutboxPattern | Pub→Sub | `PUT /api/test-event-send` on Publish app | `[HANDLER HIT] Subscribe.AnotherTestMessageHandler received: ...` on Subscribe app |
| SQS | Event publish | `PUT /api/external-message-publish/publish-external-message` | `[HANDLER HIT] SQS.TestMessageHandler received: ...` |
| SQS | Command send | `POST /api/animals` | `[HANDLER HIT] SQS.CatchAllHandler received OrderAnimal: ...` |

### Outbox Coverage

RabbitMQ + SQL Persistence outbox scenario is verified. The outbox path:

- Requires `Intent.EntityFrameworkCore` module (enforced with an SF-time guard)
- Uses `NServiceBus.Persistence.Sql` + `NServiceBus.TransactionalSession`
- Shares the EF Core `DbConnection`/`DbTransaction`
- Separate Publish and Subscribe apps are needed to observe end-to-end behaviour

### Message Flows

Coverage should include:

- Integration Event publish flows (event → SNS/topic → subscriber handler)
- Integration Command send flows (command → queue → handler, with `RouteToEndpoint`)
- Service-level publish via `IMessageBus`
- Subscription/handling for both events and commands
- Outbox-buffered publish (transactional session → deferred dispatch after commit)

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
| `53b6812ae2` | Enriched test apps — `CreatePerson` command, `TestMessageEvent.Message` property, `AnimalsService` wiring |
| `ed5a8a913c` | v9/v10 conditional host registration — `UseNServiceBusHost` for .NET 8/9, `AddNServiceBusEndpoint` for .NET 10+ |
| `b74adcf4b0` | SQS handler bodies implemented for pub/sub verification |

---

## Key Files

| File | Role |
|---|---|
| `Templates/NServiceBusConfiguration/NServiceBusConfigurationTemplatePartial.cs` | Primary configuration generation logic |
| `Templates/NServiceBusMessageHandler/NServiceBusMessageHandlerTemplatePartial.cs` | Generates the generic NSB handler and exposes subscribed models |
| `Templates/NServiceBusMessageBus/NServiceBusMessageBusTemplatePartial.cs` | Generates the message bus wrapper |
| `Settings/ModuleSettingsExtensions.cs` | Typed settings access |
