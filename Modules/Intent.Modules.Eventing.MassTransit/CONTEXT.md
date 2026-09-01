# MassTransit Module Context

> **Module:** `Intent.Modules.Eventing.MassTransit`
> **Companions:** `Intent.Modules.Eventing.MassTransit.EntityFrameworkCore` (durable/EF outbox), `Intent.Modules.Eventing.MassTransit.RequestResponse` (CQRS-over-the-bus), `Intent.Modules.Eventing.MassTransit.Scheduling` (delayed/scheduled publish)
>
> **Purpose:** durable architectural context for this module family, organized around the
> *code paths* its generation logic has to keep working across — read this before changing
> templates, settings, stereotypes, or factory extensions, and read it first if you are building
> a **new, similar broker module** and want to know what axes of variation to plan for.

---

## What this module is

The MassTransit eventing/messaging provider: transport selection, publish/consume rules,
outbox/durability, retry policy, multi-tenancy and (via companions) request-response and
scheduled publish, for cross-application messaging over
[MassTransit](https://masstransit.io/). It implements `Intent.Eventing.Contracts`'s
`IMessageBus`/`IEventBus` and participates in the Composite Message Bus mechanism so an
application can run more than one eventing provider side by side (see `Intent.Eventing.Wolverine`'s
own CONTEXT.md for the sibling implementation of the same composite-bus contract).

Of the broker modules in this repository, this is the most feature-complete — it is the best
reference for the *shape* a new broker module's decision surface tends to take, even where the
concrete mechanism (MassTransit's own API) doesn't transfer.

---

## Code paths this module has to account for

This is the axis table — read it as "if you're building something similar, these are the
dimensions your own module will probably need too."

| Axis | Values | Where the branch lives | Notes |
|---|---|---|---|
| Message broker/transport | `in-memory` (default) / `rabbitmq` / `azure-service-bus` / `amazon-sqs` | `MassTransitConfigurationTemplate.GetMessageBroker()` picks one of `InMemoryMessageBroker \| RabbitMqMessageBroker \| AzureServiceBusMessageBroker \| AmazonSqsMessageBroker` (`Templates/MassTransitConfiguration/MessageBrokers/*.cs`), all deriving from `MessageBrokerBase` | Every strategy must supply **all six** hook methods unconditionally (bus-factory-configurator type name, `UsingXxx(...)` registration statement, appsettings shape, NuGet dependency, and the per-consumer endpoint-customization pair) — a new transport that has "nothing broker-specific" for endpoint customization still has to supply a no-op wrapper method, because the orchestrator calls all six regardless |
| Composite message bus | on / off | `MessageBusExtensions.RequiresCompositeMessageBus()` → `MessageBusRegistry.HasMultipleMessageBusImplementations()`, a static registry populated by every eventing module's `OnAfterMetadataLoad` (`FactoryExtensions/MessageBusRegistrationExtension.cs`) | When ≥2 broker modules are installed, generation shape changes materially: `AddMassTransitConfiguration` takes an extra `registry` parameter and calls `registry.Register<TMessage, MassTransitMessageBus>(...)` instead of binding the shared interface directly; every message/command lookup is filtered through `FilterMessagesForThisMessageBroker(...)`, which **throws** for any message with no broker stereotype at all once composite mode is active — not just "skips it" |
| Bus interface naming | `IEventBus` (legacy) / `IMessageBus` | `Intent.Eventing.Contracts.Settings.UseLegacyInterfaceName()`, read via `MessageBusExtensions.GetBusInterfaceName()`/`GetBusVariableName()` | Owned by `Intent.Eventing.Contracts`, not this module, but every broker module (including this one) must read it rather than hardcoding either name |
| Outbox / durability | `none` / `in-memory` (default) / `entity-framework` | `MassTransitMessageBusSettings.OutboxPattern()`; branches in `MassTransitConfigurationTemplatePartial` (bus-level wiring), `ConsumerHelper.AddConsumerDefinitionClass` (per-consumer `UseInMemoryInboxOutbox`/`UseEntityFrameworkOutbox<TDbContext>`), and `ConsumerHelper.ApplyUnitOfWorkSaves` (flush placement) | The root module only *wires* the EF outbox call sites conditionally — the actual `WolverineFx.EntityFrameworkCore`-equivalent nuget/DbContext plumbing for `entity-framework` lives in the **companion** `.EntityFrameworkCore` module; without it installed, the root module just logs a warning |
| Outbox flush placement (sub-axis of the above) | flush after UoW statement / flush omitted (spliced into `SaveChanges` instead) | `ConsumerHelper.ApplyUnitOfWorkSaves`: `None`/`InMemory` add `messageBus.FlushAllAsync()` as an ordinary post-UoW statement; `EntityFramework` omits it here entirely (MassTransit's own EF outbox flushes as part of `DbContext.SaveChanges`, wired separately by `FactoryExtensions/MessageBusInteropExtension.cs`) | Getting this wrong either double-flushes or double-transacts; `allowTransactionScope` is also forced `false` only for the EF-outbox case because MassTransit's own outbox already owns the transaction |
| Retry policy | `retry-none` / `retry-immediate` / `retry-interval` (default) / `retry-incremental` / `retry-exponential` | `RetryPolicy()` setting; `GetMessageRetryStatement` maps each to a distinct `r.Immediate/Interval/Incremental/Exponential/None(...)` call with its own parameter set, each parameter overridable via `MassTransit:Retry<Name>:<Prop>` in appsettings.json | Default numeric values are explicitly a judgment call, not a standard — see the `PublishRetryPoliciesAppSettings` docstring |
| Multi-tenancy (Finbuckle) | installed / not installed | `FinbuckleConfiguratorExtension.OnAfterTemplateRegistrations` probes for `Intent.Modules.AspNetCore.MultiTenancy.MultiTenancyConfiguration`; each Finbuckle template's own `CanRunTemplate()` re-checks the same | Four extra classes generated (`FinbuckleConsumingFilter<T>`, `FinbucklePublishingFilter<T>`, `FinbuckleSendingFilter<T>`, `FinbuckleMessageHeaderStrategy`), wired via metadata-tag lookup into the broker-config lambda (see below); tenant propagates via a configurable message header, default `Tenant-Identifier` |
| Point-to-point vs pub/sub | `Send` (addressed or convention-mapped) / `Publish` | `MassTransitMessageBus`'s `DispatchType.Publish`/`DispatchType.Send` buffers; `FlushAllAsync` branches `PublishMessagesAsync` (via `ConsumeContext.PublishBatch` or `IPublishEndpoint`) separately from `SendMessagesAsync`, which itself branches on **both** whether a `ConsumeContext` exists (consumer-originated vs standalone) **and** whether an explicit `Uri` address was supplied — four total combinations | The addressed `Send(TMessage, Uri)` overload is not generated by this module directly — it's bolted onto the shared `IMessageBus` interface by `FactoryExtensions/MessageBusInterfaceExtension.cs` via the `MessageBusImplementation`/`MessageBusInterface` template roles, exactly the mechanism `Intent.Eventing.Wolverine`'s CONTEXT.md (D6) describes from the other side — non-MassTransit implementations get a generated `throw new NotSupportedException(...)` body |
| Commands vs Events routing | point-to-point named queue / per-app-instance competing-consumer queue | See "Commands vs Events" section below | Commands get an explicit outbound producer factory and endpoint-convention registration; Events don't need one because pub/sub requires no compile-time endpoint mapping |
| Azure Service Bus endpoint shape | `ReceiveEndpoint` / `SubscriptionEndpoint` | Per-consumer stereotype `EndpointTypeSelection()` (`IAzureServiceBusConsumerSettings`) | Unset/`null` resolves to `ReceiveEndpoint` for backward compatibility with models saved before this property existed — not a "default value," a compatibility carve-out |
| Non-default per-consumer endpoint config | present / absent | `ShouldConfigureNonDefaultEndpoint` — excludes the consumer from the default `.Endpoint(...)` convention and routes it through a broker-specific `ConfigureNonDefaultEndpoints` method | Only RabbitMQ and Azure Service Bus have a consumer-settings stereotype at all; AmazonSqs/InMemory consumers are always on the default convention |
| Message topology override | convention (type name) / explicit `Entity Name` | `MessageTopologySettings.EntityName()` stereotype (`Api/MessageModelStereotypeExtensions.cs`) | Also read by the Azure Service Bus subscription-endpoint branch to decide whether to pass an explicit topic name |
| Cross-EF-version compatibility | consumer's EF Core major version `<9` / `>=9` | `MassTransitConfigurationTemplate.AfterTemplateRegistration()` force-pins MassTransit/RabbitMQ/AzureServiceBus/AmazonSQS/EntityFrameworkCore NuGet packages down to the `8.4.1` family when the consumer app's own EF Core is pre-9 | MassTransit `>8.5.0` requires EF9; this is a real example of a broker package having a transitive-dependency floor tied to a *different* module's chosen version — a new broker module should anticipate the equivalent for its own package family |
| Request-response (companion, optional) | installed / not installed | `Intent.Modules.Eventing.MassTransit.RequestResponse`'s `MediatRConsumerFactory`/`CommandQueryProducerFactory`, contributed via the decorator contract (see coupling mechanisms below) | A **third** consumer shape alongside Commands/Events: CQRS `Command`/`Query` models with a `Message Triggered` stereotype get `IsSpecificMessageConsumer = true` with a `QueueName()`-or-kebab-case destination — structurally identical to the root module's command-consumer shape |
| Scheduled/delayed publish (companion, optional) | installed / not installed | `Intent.Modules.Eventing.MassTransit.Scheduling` adds a third `DispatchType.Schedule`, a `SchedulePublishAsync` method, and `AddDelayedMessageScheduler()`/`UseDelayedMessageScheduler()` wiring | Located purely via statement-metadata-tag lookup and structural `FindMethod`/`NestedEnums` search into the root module's generated file — there is no formal extension point for this, unlike request-response's decorator contract |
| OpenTelemetry tracing | wired / not wired | `TelemetryConfiguratorExtension.UpdateOpenTelemetryConfiguration` | Currently entirely commented out — "Until we can modify wrapped invocation statements we can't go with this solution." This is a known gap, not a deliberate omission — don't assume tracing is live just because the extension class exists |

---

## Commands vs Events/Messages — structurally different treatment

- **Events** (`MessageModel`, subscribed via `IntegrationEventSubscriptions()`): consumed through
  `ServiceIntegrationEventingConsumerFactory` — always `IsSpecificMessageConsumer = false`,
  `DestinationAddress = null`. Because `IsSpecificMessageConsumer` is false, these consumers get
  the **default** endpoint convention (`.Endpoint(config => config.InstanceId =
  "<sanitized-app-name>")`) — every subscribing app gets its own competing-consumer queue named
  after itself: standard pub/sub fan-out.
- **Commands** (`IntegrationCommandModel`, subscribed via `IntegrationCommandSubscriptions()`):
  consumed through `ServiceIntegrationCommandConsumerFactory` — always `IsSpecificMessageConsumer
  = true`, with `DestinationAddress` read from the `CommandConsumption` stereotype's `Queue Name`
  (falling back to the kebab-cased type name). Because `IsSpecificMessageConsumer` is true, these
  consumers are **excluded** from the default endpoint convention and instead grouped into
  explicit `ReceiveEndpoint(...)` blocks by destination address — point-to-point, not
  per-app-instance.
- **Only Commands get an explicit outbound producer path**: `ServiceIntegrationCommandSendProducerFactory`
  computes a `queue:<name>` URN (from the `CommandDistribution` stereotype's `Destination Queue
  Name`, else kebab-cased type name) and registers it via `EndpointConvention.Map<TCommand>(...)`.
  Events have no equivalent — publishing is just `IPublishEndpoint`/`ConsumeContext.Publish` at
  runtime with no compile-time endpoint registration, because MassTransit pub/sub needs none.
- Both consumer shapes ultimately wrap the same generic `IntegrationEventConsumer<THandler,
  TMessage>` template (`ConsumerHelper` is shared) — the difference is routing/endpoint topology,
  not handler-interface shape.

---

## Cross-module coupling mechanisms (how the companion modules extend this one)

Several distinct mechanisms coexist — worth knowing which one a given extension point actually
uses before assuming a different one applies:

1. **Template-decorator contract** (the one *formal* extension point): `MassTransitConfigurationDecoratorContract`
   (abstract, `Priority`, virtual `GetConsumerFactories()`/`GetProducerFactories()`) is
   implemented by companions — e.g. RequestResponse's `FactoriesForMassTransitConfiguration`. The
   root template's `GetConsumers()`/`GetProducers()` concatenate every decorator's factories onto
   its own built-ins. Registered via a generated `DecoratorRegistration<MassTransitConfigurationTemplate,
   MassTransitConfigurationDecoratorContract>` keyed by a `DecoratorId` string.
2. **Metadata-tagged-statement lookup** (the *dominant* mechanism, used by nearly every companion
   including this module's own Finbuckle wiring): key statements are tagged
   (`.AddMetadata("configure-masstransit", true)`, `"message-broker"` → broker key,
   `"in-memory-outbox"`, `"eventbus-flush"`, `"mediatr-config"`, `"telemetry-tracing"`, etc.).
   Downstream extensions find the tagged template via `FindTemplateInstance<T>`, drill into the
   `CSharpFile`'s statement tree via `FindMethod`/`FindStatement(p => p.HasMetadata(...))`, and
   mutate in an `OnBuild`/`AfterBuild` callback with an explicit numeric priority. **This is
   inherently fragile to root-module refactors** — renaming a method, a metadata key, or
   restructuring lambda nesting depth is a silent breaking change to every companion relying on
   it.
3. **Role-based template discovery** (`TemplateRoles.Application.Eventing.MessageBusInterface`/
   `MessageBusImplementation`, `TemplateRoles.Domain.UnitOfWork`, etc.): lets an extension reach
   *any* template fulfilling a role without knowing its concrete `TemplateId` — the mechanism that
   makes the addressed-`Send` interface addition and the composite-bus scenario work regardless of
   which concrete module produced the interface/implementation template.
4. **Static process-wide registry** (`MessageBusRegistry` in `Intent.Eventing.Contracts`):
   accumulates `(messageBusId → brokerStereotypeIds[])` from every eventing module's
   `OnAfterMetadataLoad`. This is what actually implements the composite-message-bus axis above —
   `RequiresCompositeMessageBus()` and `FilterMessagesForThisMessageBroker(...)` both read it.
5. **`.imodspec` `<interoperability><detect>` install-cascade**: this module's `.imodspec`
   declares that detecting certain *other* modules installed (`Intent.EntityFrameworkCore`,
   `Intent.Application.MediatR.CRUD`, `Intent.Eventing.MassTransit.RequestResponse`, etc.)
   auto-installs specific companion packages at specific minimum versions — a *push* mechanism
   distinct from the *pull* mechanisms above; it's what gets a companion installed in the first
   place before its own extensions can run.
6. **Factory-extension `Order` convention**: root-module extensions all declare `Order => 0`;
   companions that must run *after* the root module has built its statement tree use a higher
   value (the EF companion's `EFOutboxPatternConfiguratorExtension` uses `Order => 10`).
7. **Raw-string `TemplateId` lookup with an acknowledged TODO**: `MessageBusInteropExtension.InstallMessageBusForMediatRDispatch`
   looks up `"Intent.Application.DependencyInjection.DependencyInjection"` by hardcoded string
   with an inline comment flagging it should become role-based later — a known-incomplete corner
   of the coupling story, worth treating as "still fragile" rather than a settled pattern to copy.

---

## Module Settings

| Setting | Values | Gates |
|---|---|---|
| Messaging Service Provider | `in-memory` (default) / `rabbitmq` / `azure-service-bus` / `amazon-sqs` | Broker strategy selection |
| Outbox Pattern | `none` / `in-memory` (default) / `entity-framework` | Outbox wiring + flush placement |
| Retry Policy | `retry-immediate` / `retry-interval` (default) / `retry-incremental` / `retry-exponential` / `retry-none` | Which retry statement + appsettings block |
| Use Pre-Commercial Version | switch, default `true` | Intended to pin MassTransit below its v9 commercial-licensing change — **not observed to be read anywhere in the templates**; verify (possibly consumed only in `NugetPackages.cs`) before assuming it works |

(Owned by `Intent.Modules.Eventing.Contracts`, not this module: `UseLegacyInterfaceName` — `IEventBus` vs `IMessageBus` naming.)

## Stereotypes

| Stereotype | Applies to | Gates |
|---|---|---|
| `MassTransitMessage` | `MessageModel`, `IntegrationCommandModel` | Marker identifying a message as belonging to this broker in a composite-bus app |
| `MessageTopologySettings` | `MessageModel` | `Entity Name` override |
| `AzureServiceBusConsumerSettings` | subscribe target-end models | Endpoint Name/Type, prefetch, sessions, TTL, lock duration, dedup, dead-lettering, etc. |
| `RabbitMQConsumerSettings` | subscribe target-end models | Endpoint Name, prefetch, lazy/durable/purge/exclusive, concurrency |
| `CommandConsumption` | subscribe (command) target-end | `Queue Name` override |
| `CommandDistribution` | send (command) target-end | `Destination Queue Name` override |

---

## Known gotchas / footguns

- **EF9/MassTransit-8.5 version coupling** — see the axis table; a real precedent for
  transitive-dependency floors tied to another module's version choice.
- **Outbox flush placement is pattern-specific, not a simple on/off switch** — see the axis table;
  getting it wrong double-flushes or double-transacts. Release notes v7.2.2 document a real
  regression here: selecting the EF outbox in a **Wolverine**-dispatched app double-flushed events
  (once via Wolverine's own middleware, once via the `DbContext.SaveChanges` splice) until
  `MessageBusInteropExtension` learned to strip Wolverine's `eventbus-flush`-tagged statements too,
  matching what it already did for MediatR (`mediatr-config` removal) and controller dispatch.
  **Any new broker module supporting a transactional-outbox-style durability mode must replicate
  this de-duplication across every dispatch mechanism it might coexist with.**
- **`FilterMessagesForThisMessageBroker` 2-arg vs 3-arg overload trap**: passing `this` (a
  template) to the `ISoftwareFactoryExecutionContext` overload compiles but silently returns wrong
  filtering results — pass `ExecutionContext` explicitly (see the repo's own
  `known-build-gotchas.instructions.md`).
- **`UseMessageScope` must be inserted above the outbox statement, not below** — `FinbuckleConfiguratorExtension.WireupMassTransitFilters`
  does this deliberately; MassTransit's own pipeline semantics require message scope to wrap the
  outbox.
- **Azure Service Bus time-span stereotype properties are free text, validated only at generation
  time** (`AzureServiceBusMessageBroker.ValidateTimeSpanString`, `TimeSpan.TryParse`) and throw a
  plain `Exception`, not an `ElementException` — arguably miscategorized per this repo's own
  exception guidelines, since it's tied to a specific model property.
- **Finbuckle filters must tolerate an absent tenant** — MassTransit-generated fault/reply
  messages correlate via `RequestId` after the AsyncLocal tenant context has unwound, so a
  resolved tenant is legitimately optional on publish/send; the filters must never throw, only
  conditionally set the header.
- **Telemetry integration is a documented no-op**, not a hidden feature — see the axis table.
- **Naming a consumer/test app after the broker package's own root namespace** (`MassTransit.*`)
  risks the same C# enclosing-namespace resolution collision documented repo-wide for Wolverine/
  NServiceBus — this module's own test apps are careful to avoid it, but a new consumer app is
  not automatically protected.

---

## Golden-sample / test-app coverage

| Test app(s) | Axis exercised |
|---|---|
| `MassTransit.RabbitMQ`, `MassTransit.AzureServiceBus`, `MassTransitShared` | Broker selection |
| `MassTransit.RetryPolicy.{Exponential,Immediate,Incremental,Interval}` | Retry policy |
| `MassTransit.RequestResponse.Client` | Request-response companion |
| `Publish.AspNetCore.MassTransit.OutBoxEF/OutBoxNone.TestApplication`, `Publish.CleanArch.MassTransit.OutboxEF/OutboxNone.TestApplication` | Outbox pattern × architecture style (crossed) |
| `Subscribe.MassTransit.OutboxEF`, `Subscribe.MassTransit.OutboxMemory` | Outbox pattern, subscribe side |
| `Subscribe.MassTransit.DomainInteractionsRepro` | Domain-interactions edge case |
| `MassTransitFinbuckle.Test` | Multi-tenancy |

No golden sample currently exercises the Scheduling companion or a composite-message-bus scenario
for MassTransit specifically — worth a gap flag if either axis needs verified coverage.
