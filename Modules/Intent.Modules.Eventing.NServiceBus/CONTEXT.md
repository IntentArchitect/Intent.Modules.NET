# NServiceBus Module Context

> **Module:** `Intent.Modules.Eventing.NServiceBus`
>
> **Purpose:** Durable architectural and implementation context for future work in this
> module. Read this before changing templates, settings, stereotypes, validations, or test
> applications related to NServiceBus.

---

## Code paths this module has to account for

Read this table first if you're building a **new, similar broker module** — it's the axis
inventory a comparable module tends to need, cross-referenced to where each branch actually lives.

| Axis | Values | Where the branch lives | Notes |
|---|---|---|---|
| Transport | Learning Transport / RabbitMQ / Azure Service Bus / Amazon SQS / SQL Server | `TransportOptionsEnum`, `AddTransportStatements` | Amazon SQS and SQL Server don't fit the "read a `ConnectionStrings:X` key" convention the others follow — see Transport-Specific Quirks below |
| Recoverability policy | none / immediate-only / delayed-only / immediate-and-delayed | `Recoverability Policy` setting | Each generated policy block itself reads further nested config keys (`ImmediateRetries`, `DelayedRetries`, `DelayIncreaseSeconds`, `ErrorQueue`) with their own hardcoded defaults — the setting controls *which blocks exist*, not the numbers inside them |
| Persistence / outbox | none / sql-persistence / nhibernate, crossed with Enable Outbox on/off | `Persistence`, `Enable Outbox` settings | Three genuinely distinct "flush" code paths exist, not one — see Outbox and Persistence below |
| Host wiring (.NET major version) | `<10` (NServiceBus 9, `IHostBuilder.UseNServiceBus`) / `>=10` (NServiceBus 10, `AddNServiceBusEndpoint` via DI) | `OutputTarget.GetMaxNetAppVersion().Major < 10` (`_isLegacyFramework`) | Two structurally different registration APIs, not just a version bump |
| Per-transport/persistence NuGet version | multiple framework-keyed tiers (roughly net2/6/8/10 bands) | `NugetPackages.cs` | A whole additional versioning axis, independent of the host-wiring split above — every transport **and** persistence backend **and** `NServiceBus.Extensions.Hosting` has its own framework-keyed version ladder with transitive pins |
| Audit queue | on/off | `Enable Audit Queue` setting | Reads `NServiceBus:AuditQueue` (required) and `NServiceBus:AuditTimeToBeReceived` (optional) |
| Instance identification | on/off | `Enable Instance Identification` setting | Reads `NServiceBus:InstanceId` (required) |
| Multi-broker coexistence | single broker / composite (≥2 broker modules installed) | `MessageBusExtensions.RequiresCompositeMessageBus()` / `FilterMessagesForThisMessageBroker(...)` (shared in `Eventing.Contracts`) | In composite mode this is a **hard failure mode, not a soft convention**: any Integration Command/Event with no broker stereotype at all throws once ≥2 broker modules are installed — see Coexistence below |
| Multi-tenancy | **not supported** | — | No `Finbuckle`/`MultiTenan*` reference anywhere in this module, confirmed by full read + grep. Not a hidden gap — genuinely out of scope today. If a similar module needs tenancy, look at `Intent.Eventing.MassTransit`'s or `Intent.Eventing.Wolverine`'s Finbuckle wiring instead |
| Dispatcher coverage (outbox flush stripping) | MediatR / ServiceContract controllers / Wolverine | `NServiceBusMessageBusInteropExtension.InstallNServiceBusFor{MediatR,ServiceContract,Wolverine}Dispatch` | Strips the generic post-handler flush wherever the outbox is active, since the flush is already spliced into `DbContext.SaveChanges`/`SaveChangesAsync` — see Dispatcher Coverage below |

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

## Module Settings — Current Set

| Setting ID | Title | Type | Notes |
|---|---|---|---|
| `537d4def-...` | Transport | select | learning-transport, rabbitmq, azure-service-bus, amazon-sqs, sql-server |
| `4060477a-...` | Recoverability Policy | select | none, immediate-only, delayed-only, immediate-and-delayed |
| `61e27361-...` | Persistence | select | none, sql-persistence, nhibernate |
| `a249c7a3-...` | Enable Outbox | checkbox | Default false. Requires Persistence = sql-persistence or nhibernate |
| `40a8127e-...` | Enable Audit Queue | checkbox | Default false. Reads `NServiceBus:AuditQueue` (required) and `NServiceBus:AuditTimeToBeReceived` (optional) |
| `6321cb9f-...` | Enable Instance Identification | checkbox | Default false. Reads `NServiceBus:InstanceId` (required) |

`License Path` is not a module setting — it is always read from `NServiceBus:LicensePath` in `appsettings.json` and applied when present.

**Known MCP limitation**: `update_application_settings` fails with a NullReferenceException for `checkbox`-type settings. These must be set manually in the Intent Architect UI. `select`-type and `switch`-type settings work fine via MCP.

**Known `install_or_update_modules` footgun**: calling with the default `installApplicationSettings: true` when reinstalling to refresh the settings schema resets all checkbox values to their defaults. Always use `installApplicationSettings: false` when the only goal is to pick up a new module DLL without changing application settings.

**Unverified: possible duplicate app-settings registration.** `NServiceBusRegistrationExtension.OnBeforeTemplateExecution`
publishes its own `AppSettingRegistrationRequest`s for `NServiceBus:EndpointName` (hardcoded
default `"MyApplication"`), `NServiceBus:Recoverability:*`, `NServiceBus:ErrorQueue`, and
`ConnectionStrings:RabbitMQ`/`ConnectionStrings:AzureServiceBus` — overlapping in purpose (and
diverging in default value/shape) with `NServiceBusConfigurationTemplate.PublishAppSettings`, which
does the same job with different, more correct-looking defaults (e.g. `OutputTarget.ApplicationName()`
instead of the literal `"MyApplication"`). `NServiceBusRegistrationExtension.cs` carries a comment
stating its intended scope is "app-settings + Program.cs host-builder wiring only," which suggests
this may be vestigial from before `PublishAppSettings` existed rather than intentional
double-registration. Verify and prune before extending either path further — don't assume the
overlap is deliberate.

## Outbox and Persistence

The outbox supports two persistence backends:

### SQL Persistence
- `NServiceBus.Persistence.Sql` + `NServiceBus.TransactionalSession`
- Shares the EF Core `DbConnection`/`DbTransaction` for exactly-once dispatch
- Requires `Intent.EntityFrameworkCore` (enforced with SF-time `FriendlyException`)
- `NServiceBusMessageBusInteropExtension` injects `IMessageBus.FlushAllAsync()` into `DbContext.SaveChanges/SaveChangesAsync` when `Enable Outbox = true && Persistence = sql-persistence`

### NHibernate
- `NServiceBus.NHibernate` + `NServiceBus.NHibernate.TransactionalSession`
- NHibernate manages its own session/connection — no EF Core dependency
- The `Intent.EntityFrameworkCore` guard is **not** applied for NHibernate
- `NServiceBusMessageBusInteropExtension` also fires for NHibernate when `Enable Outbox = true && Persistence = nhibernate`
- Two NHibernate test apps exist as of v1.0.0-pre.3: `Tests/N_ServiceBus.Persistence.NHibernate.Publish` and `Tests/N_ServiceBus.Persistence.NHibernate.Subscribe` (Learning Transport + RabbitMQ, NServiceBus 10.x / net10.0)

Both outbox paths are important supported scenarios.

### Three Distinct Outbox-Related Code Paths — Not One

It's easy to read the above as "the outbox is one splice point into `DbContext.SaveChanges`." In
practice there are **three separate mechanisms**, each solving a different half of the problem:

1. **Outbound splice (API/controller-originated dispatch, no active NSB handler context).**
   `NServiceBusMessageBusInteropExtension.InstallNServiceBusForXDispatch` wires
   `IMessageBus.FlushAllAsync()` into `DbContext.SaveChanges`/`SaveChangesAsync` — this is the
   mechanism described above and in Dispatcher Coverage.
2. **Inbound handler bridging (SQL persistence only).** `NServiceBusMessageHandlerTemplatePartial.cs`
   generates code inside `Handle()` that pulls `context.SynchronizedStorageSession.SqlPersistenceSession()`,
   calls `_dbContext.Database.SetDbConnection(...)` + `UseTransactionAsync(...)`, then
   `SaveChangesAsync`, then `FlushAllAsync` — bridging the *inbound* NSB transaction into the app's
   own `DbContext` so a handler's DB writes and its outbound messages commit atomically.
   **NHibernate's handler body has no equivalent** — NHibernate's session is opaque to the app, so
   this bridging step simply doesn't exist on that path. Don't assume SQL-persistence and
   NHibernate handler bodies are structurally parallel; they aren't.
3. **`NServiceBusMessageBus.FlushAllAsync`'s own `ActiveContext` branch.** Independent of the two
   above: `FlushAllAsync` checks whether it's running inside a message handler
   (`ActiveContext is IMessageHandlerContext` → dispatch via `handlerContext.Publish/Send`
   directly, riding the ambient outbox transaction NSB already opened for the inbound message) or
   not (any other context, e.g. a controller call with no active handler → open its own
   `ITransactionalSession`/`IMessageSession`, wrapped in a **`TransactionScope(...,
   TransactionScopeOption.Suppress, ...)`** block). The suppression exists to stop ASB/RabbitMQ/SQL
   clients from attempting DTC enlistment — removing it would risk reintroducing that failure mode.

A change to "how the outbox flushes" needs to be evaluated against all three, not just the one
that happens to be top of mind.

### Transport-Specific Quirks

- **Amazon SQS takes no connection string at all.** `new SqsTransport()` relies purely on the AWS
  SDK's ambient credential/region resolution; no `ConnectionStrings:*` appsetting is registered for
  it. Every other transport follows the "read a `ConnectionStrings:X` key" convention — SQS is the
  one exception.
- **RabbitMQ hardcodes `RoutingTopology.Conventional(QueueType.Quorum)`**, and **Azure Service Bus
  hardcodes `TopicTopology.Default`.** Both are fixed choices baked into the template, not
  currently exposed as settings — treat as a deliberate simplification to revisit only with a
  concrete reason, not as an oversight to "fix" silently.
- **`IMessageHandlerContext.Publish`/`Send` have no `CancellationToken` overload** — pass
  `PublishOptions`/`SendOptions` instead. Easy to reach for the wrong overload by habit from other
  broker APIs in this repo.

### Dispatcher Coverage

When the outbox is selected, `NServiceBusMessageBusInteropExtension.InstallNServiceBusForXDispatch`
strips the generic post-handler `IMessageBus.FlushAllAsync()` call out of whichever dispatcher is
installed — MediatR (`InstallNServiceBusForMediatRDispatch`), ServiceContract controllers
(`InstallNServiceBusForServiceContractDispatch`), and Wolverine
(`InstallNServiceBusForWolverineDispatch`, added alongside `Intent.Application.Wolverine`'s
`MessageBusFlushMiddleware` — see that module's `CONTEXT.md`) — since the flush is already spliced
into `DbContext.SaveChanges`/`SaveChangesAsync` and must not run twice. The Wolverine path finds its
target by the `"eventbus-flush"` metadata tag on the `ApplicationHandlerPolicy` template's
`AddMiddleware`/`AddTransient` statements, the same tag convention the other two dispatchers already
use on their own flush statements.

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

### NServiceBus Source-Generated Handler Registration (`AddHandler<T>()`)

NServiceBus provides a source-generated `AddHandler<T>()` API that registers handlers generically
without manual registry manipulation. This was rejected because:

- The source generator emits code that requires an experimental C# language feature to compile
- Enabling experimental features in user-generated code is unacceptable
- The internal registration logic it generates was safe to copy and reproduce directly

Instead, Intent generates the handler registrations explicitly — mirroring what the source generator
would have emitted, but without the experimental dependency. This is the correct long-term approach.

Do not revisit `AddHandler<T>()` unless the experimental requirement is removed upstream.

---

## Acceptance Matrix

Changes in this module must be evaluated against the broader matrix, not just one happy path.

### Coexistence

- NServiceBus must coexist with other broker modules such as MassTransit, Azure Service Bus
  module integrations, Kafka, and similar technologies.
- The `NServiceBus` stereotype is the disambiguation mechanism — it marks which Integration
  Commands/Events belong to NServiceBus in a multi-broker application.
- **Once ≥2 broker modules are installed in the same app, tagging is not optional — it's
  enforced.** `FilterMessagesForThisMessageBroker` (shared in `Eventing.Contracts`) throws if
  `RequiresCompositeMessageBus()` is true and a message/command has no broker stereotype at all.
  This is a hard generation-time failure, not a soft convention: every Integration Command/Event in
  a composite-bus app must carry a broker stereotype or the Software Factory run fails outright.
- `CompositeMessageBus` mode (multiple brokers in one app) is a verified scenario. In this
  mode `AddNServiceBusConfiguration` accepts a `MessageBrokerRegistry` parameter and registers
  message types against the NServiceBus bus rather than publishing a `ServiceConfigurationRequest`.

### Transport Coverage

Each transport scenario has a dedicated test application with its own README describing
infrastructure requirements, how to run it, and expected output. The test apps are the
authoritative source of the acceptance matrix — this section summarises their status.

| Transport | Test App | README | Runtime Verified |
|---|---|---|---|
| Learning Transport | `Tests/NServiceBus.LearnerTransport` | [README](../../../../Tests/NServiceBus.LearnerTransport/README.md) | ✓ 2026-06-11 |
| RabbitMQ | `Tests/NServiceBus.RabbitMQ` | [README](../../../../Tests/NServiceBus.RabbitMQ/README.md) | ✓ 2026-06-11 |
| Azure Service Bus | `Tests/NServiceBus.AzureServiceBus` | [README](../../../../Tests/NServiceBus.AzureServiceBus/README.md) | ✓ 2026-06-11 |
| Amazon SQS | `Tests/NServiceBus.SQS` | [README](../../../../Tests/NServiceBus.SQS/README.md) | Requires live AWS credentials |
| SQL Server | *(none)* | — | **Gap.** SQL Server transport shipped in v1.0.0 and is fully wired in `AddTransportStatements`/`PublishAppSettings`, but has no dedicated test-app/verification row — add one before relying on this transport being exercised end-to-end |
| RabbitMQ + SQL Outbox | `Tests/NServiceBus.OutboxPattern.Publish` + `.Subscribe` | [Publish README](../../../../Tests/NServiceBus.OutboxPattern.Publish/README.md) · [Subscribe README](../../../../Tests/NServiceBus.OutboxPattern.Subscribe/README.md) | ✓ 2026-06-11 |

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
