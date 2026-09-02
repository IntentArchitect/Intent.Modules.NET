# Intent.Eventing.Wolverine — Context

## What this module is

The Wolverine eventing provider: transport selection, publish/send rules, listeners, transactional outbox and error-handling policy for cross-application messaging over [WolverineFx](https://wolverine.netlify.app/). It implements `Intent.Eventing.Contracts`'s `IMessageBus` and integrates with the Composite Message Bus so an application can run more than one eventing provider side by side.

It depends on `Intent.Wolverine.Common` for the single shared `builder.Host.UseWolverine(opts => ...)` registration — see that module's own CONTEXT.md for why the host registration is arbitrated there rather than here.

## Code paths this module has to account for

This section is the axis inventory — read it first if you're comparing this module against a
similar broker (`Intent.Eventing.MassTransit`, `Intent.Eventing.NServiceBus`) or building a new
one. The prose sections below (D1 onward) explain the *why*; this table is the *what*.

| Axis | Values | Where the branch lives | Notes |
|---|---|---|---|
| Transport | `Local` / `Rabbitmq` / `AzureServiceBus` / `AmazonSqs` | `WolverineMessageBusSettings.TransportOptions`; `AddConfigureEventing` dispatches to `AddLocalBody`/`AddRabbitMqBody`/`AddAzureServiceBusBody`/`AddAmazonSqsBody` | **The historical method names `AddConfigureLocal`/`AddConfigureRabbitMq`/`AddConfigureAzureServiceBus`/`AddConfigureAmazonSqs` you'll see in this file's own bug-history sections below no longer exist in code** — they were retired by the "Foundations pass." Grepping current code for those names finds nothing; the live names are the `*Body` methods listed here |
| Transactional Outbox | `None` / `Durable` | `ctx.TransactionalOutbox.IsDurable()` | Throws `FriendlyException` if `Intent.EntityFrameworkCore` isn't installed |
| Database Provider (sub-axis of Durable) | `SqlServer` / `PostgreSQL` | `Settings/DatabaseProviderExtensions.cs` | Anything else throws `FriendlyException` |
| Error Handling Policy | `None` / `Retry` / `RetryWithCooldown` / `ScheduleRetry` | `WolverineEventingRegistrationExtension.AddApplyErrorHandlingPolicyMethod` | The two delay-based policies share `AddParseDelaysMethod`, added once |
| Multi-tenancy (Finbuckle) | installed / not installed | `WolverineTenantStrategyTemplate`/`WolverineTenantMiddlewareTemplate.CanRunTemplate()` probing for `MultiTenancyConfiguration` | Tenancy is envelope-native (`Envelope.TenantId`), not header-based — see the Foundations-pass entry below |
| Composite message bus | on / off | `WolverineCompositeConfigurationTemplate.CanRunTemplate() => RequiresCompositeMessageBus()` | Design-time detection is seeded by `WolverineMessageBusInteropExtension.OnAfterMetadataLoad` calling `MessageBusRegistry.Register(...)` — this registration step is easy to miss because only its *consumption* (the composite template, `RegisterWolverineMessageBus`'s skip) is otherwise documented in this file |
| Bus registration against `IMessageBus` | registered / skipped | `RegisterWolverineMessageBus` | Skipped when composite mode is required (the shared bus interface is owned by `CompositeMessageBus` there) and when the app has zero Wolverine-designated published messages/sent commands (subscribe-only apps) |
| Handler discovery | conventional (owned by `Wolverine.Common`) + explicit `IncludeType<T>()` per subscribed handler | `AddHandlerTypeRegistrations`, called from each transport's `ConfigureListeners` or directly from `AddLocalBody` | See D1/D1b below for why double-registration is safe |
| Publish/Send name resolution | convention (kebab-case type name) / explicit override | `ResolvePublishName`/`ResolveSendName` reading the `Wolverine Message` stereotype | `ValidateNameOverride` throws `ElementException` on a blank/whitespace name **or one over 250 characters** — the limit itself isn't obvious from the requirement IDs alone, worth remembering as a concrete number |
| Subscriber queue naming | convention (kebab-case app+message name) / explicit override | `GetSubscriberQueueName` → `{appName-kebab}-{message-kebab}`, or the `Wolverine Subscription` stereotype's `Subscriber Queue Name` (see below) | Was never overridable prior to `1.0.0-pre.1`'s AWS runtime testing — see the dedicated section below for why that changed |
| AmazonSqs fan-out shape | published+subscribed / publish-only / subscribe-only | `AddAmazonSqsBody`'s `ConfigurePublishing` | See D7 below |
| OpenTelemetry tracing | wired / not wired | `WolverineTelemetryConfiguratorExtension` | Gated on `Intent.OpenTelemetry.OpenTelemetryConfiguration` being installed **and** the `telemetry-tracing`-tagged statement being enabled; appends `AddSource("Wolverine")`. Not mentioned elsewhere in this file — the only place this axis is documented |
| Eventing/dispatch interop (Durable outbox only) | ServiceContract/Controller / Wolverine CQRS / MediatR | `WolverineMessageBusInteropExtension`'s three `InstallMessageBusFor*Dispatch` methods | Strips `eventbus-flush`-tagged statements from three different foreign templates when the outbox makes the generic post-handler flush redundant — this module actively *removes* code another module generated, not just abstains from generating its own (see D5) |
| Appsettings defaults | additive-only | `PublishAppSettings` | Per-transport connection defaults and per-error-policy delay defaults are registered via `AppSettingRegistrationRequest`, which cannot remove a previously-registered key — same additive-only constraint documented for this repo's other modules |

**Sanity check before trusting version numbers in this file:** `Intent.Eventing.Wolverine.imodspec`
on disk is still pinned at `1.0.0-pre.0` (and lists `Intent.Wolverine.Common` at `1.0.0-pre.0` too),
even though the bug-fix history below narrates fixes landing through `1.0.0-pre.4`. That's an
imodspec-not-yet-repackaged signal, not a code defect — confirm the actual packaged version before
assuming the fixes below are in what's currently published.

## Design decisions (durable — read before changing behaviour this module relies on)

### D1 / D1b — Conventional discovery stays ON, _and_ handler types are registered explicitly

`Intent.Wolverine.Common` leaves Wolverine's conventional handler discovery **on** (see its own CONTEXT.md, D1). This module _additionally_ emits one `opts.Discovery.IncludeType<THandler>()` per distinct `IIntegrationEventHandler<T>` implementation it subscribes, alongside that message's listener (R5.1/R5.5). `DisableConventionalDiscovery()` is deliberately **not** emitted.

**Double-registering a handler type — once via conventional discovery, once via an explicit `IncludeType<T>()` — is safe.** This was measured, not assumed, because the two together could plausibly make Wolverine see two handlers for one message and emit a duplicate local (`CS0128`) in the consumer's generated handler pipeline — that turns out not to happen.

**What was actually measured** (WolverineFx 5.39.5, a real host started and the handler pipeline generated and invoked, four configurations):

| Configuration                           | Chains for the message | HandlerCalls in that chain | Pipeline codegen + invoke |
| --------------------------------------- | ---------------------- | -------------------------- | ------------------------- |
| `IncludeAssembly` only                  | 1                      | 1                          | OK                        |
| `IncludeAssembly` + `IncludeType`       | 1                      | 1                          | OK                        |
| `IncludeType` only                      | 1                      | 1                          | OK                        |
| `IncludeAssembly` + `IncludeType` twice | 1                      | 1                          | OK                        |

Wolverine deduplicates the discovered handler set, so an explicit registration for a type that conventional discovery also finds collapses to the same single `HandlerCall`. The probe was sensitivity-checked: adding a genuinely _second_ handler class for the same message moved the HandlerCall count from 1 to 2, so the counter does detect duplication when duplication exists.

Two consequences worth keeping:

- `Discovery.IncludeType` does **not** narrow discovery. In the `IncludeType`-only configuration the entry assembly was still scanned and the second handler was still found. It is purely additive — which is why it is safe to add here, and also why it earns its keep only for a handler conventional discovery might _not_ reach (one outside the scanned assemblies).
- `HandlerDiscovery._explicitTypes` is a plain `IList<Type>`, not a set, so the deduplication happens downstream in `HandlerGraph.Group()` rather than at registration. This module does not rely on that: it deduplicates by handler type name on its own side before emitting, so its output is idempotent regardless of Wolverine's internals.

**What R5.2's callout actually forbids** — and this part of the original entry was right — is a generated _intermediate consumer class_ between the transport and the handler. That would be a genuinely distinct second handler for the message (the `1 → 2` row above), which is the real duplicate-handler shape. An explicit type registration for the handler that already exists is not.

### D2 — One module, not one-per-transport or a durability companion

All four Transports (Local, RabbitMQ, Azure Service Bus, Amazon SQS) and both Transactional Outbox modes (None, Durable) live in this single module — there is no `Intent.Eventing.Wolverine.EntityFrameworkCore` companion the way MassTransit splits its durable outbox out.

**Why:** the requirement this module realizes (R14.1) promises exactly one install for a developer to get everything. MassTransit's split into a companion module predates the way this module packages durability — here, the durability packages (`WolverineFx.EntityFrameworkCore`, `WolverineFx.SqlServer`, `WolverineFx.Postgresql`) are just conditional NuGet registrations, and one module can carry conditional registrations without needing a second package to hold them. Do not resurrect a companion module for a future durability feature without a concrete reason a conditional registration in this module can't handle.

### D4 — `Destination Queue Name` lives on the `Integration Command` element, not the send association

MassTransit's equivalent (`Command Distribution` → `Destination Queue Name`) is attached to the _send association_ (`Send Integration Command Target End`). This module attaches the same-named stereotype to the **Integration Command element itself**.

**Why:** the queue an Integration Command is delivered to is a property of the command's destination, not of any one application's decision to send it. If the property lived on the send association, two different applications sending the same Integration Command could each set a different destination queue, and the command would be delivered inconsistently depending on who sent it. Putting it on the element means there is exactly one place it can be declared, and every sender resolves the same value. Do not move this property onto an association-level stereotype even to match MassTransit's exact placement — that would reopen the inconsistency this design closes.

### D5 — This module supplies the bus and its flush method; it does not call it from a dispatch seam — with one carve-out

`WolverineMessageBus` (the buffered `IMessageBus` implementation) and its flush method are generated here. Nothing this module generates into the **dispatch layer** (commands/queries) calls that flush method.

**Why:** the flush seam belongs to whichever dispatch mechanism is buffering commands/queries around it, not to the eventing provider — that seam is per-dispatch-mechanism, and the precedent already exists on both sides of the codebase: `Intent.Application.Wolverine` supplies `MessageBusFlushMiddleware` for a Wolverine-CQRS application, and a MediatR application gets `MessageBusPublishBehaviour` from its own module. Each of those middleware/behaviour implementations already only emits when an eventing module's bus interface actually resolves, so the two halves find each other without this module needing to know which dispatch mechanism (if any) is installed. If a future change makes this module flush itself from that seam, check whether that breaks an application with no dispatch/CQRS module installed at all — R8's requirement that this module works standalone depends on it not needing one.

**Carve-out — the durable-outbox `DbContext` splice is not the dispatch seam D5 is about, and this module does call the flush there.** When `TransactionalOutbox.IsDurable()`, the `hasBusFlush`-tagged dispatch-layer statements are stripped (see the defect-fix note below) and `WolverineMessageBusInteropExtension.InstallMessageBusForDbContextForTransactionalOutboxPattern` splices `_messageBus.FlushAllAsync(...)` directly into `ApplicationDbContext.SaveChanges`/`SaveChangesAsync` instead — mirroring `Intent.Eventing.MassTransit`'s `MessageBusInteropExtension.InstallMessageBusForDbContextForTransactionalOutboxPattern`. This is **dispatcher-agnostic**: it fires on every `SaveChangesAsync` regardless of whether MediatR, Wolverine dispatch, or no dispatch module at all is installed, which is exactly why it cannot live in a per-dispatch-mechanism middleware/behaviour. D5's rationale (a per-dispatch-mechanism seam finds its own dispatch module without this module needing to know which one is installed) holds for the **non-durable** path, where the tagged statements are left in place and the dispatch module's own middleware/behaviour does the flush. The durable-outbox path is a different case D5 did not originally account for: there is no dispatch-layer flush left to strip-and-replace-with, because the strip already removed it, so the DbContext splice is the only remaining seam. Do not read this carve-out as license to move the non-durable flush into this module too — that path stays exactly as D5 originally described it.

**Fixed defect (this version line):** prior to this fix, the durable-outbox path stripped the dispatch-layer flush and added no replacement, so every published message was silently discarded once buffered onto `WolverineMessageBus._pendingActions` and never flushed. See the "Durable outbox regenerating with missing NuGet packages" entry below for the sibling defect (missing NuGet dependencies) found in the same area.

### D7 — Amazon SQS fan-out goes through SNS, in a second package

An Integration Event's publish rule on the Amazon SQS transport targets an **SNS topic** (`opts.PublishMessage<T>().ToSnsTopic(name)`), not an SQS queue. Integration Commands stay point-to-point on `ToSqsQueue`.

**Why:** `WolverineFx.AmazonSqs` exposes no fan-out publish expression at all — its only publish targets are `ToSqsQueue`, `ToSqsQueueOnNamedBroker` and the sharded-queue variants (confirmed by reflecting over `Wolverine.AmazonSqs.AmazonSqsTransportExtensions` at 5.39.5). SQS is a queue service; SNS-to-SQS is AWS's fan-out primitive, and Wolverine ships it as a **separate package**, `WolverineFx.AmazonSns`. So the Amazon SQS transport selection now pulls in _two_ packages, and the generated configuration registers _two_ transports (`UseAmazonSqsTransport` and `UseAmazonSnsTransport`) against the same region and credentials. Do not try to express Integration Event fan-out through `WolverineFx.AmazonSqs` alone — there is no API for it.

**The subscriber-side wrinkle:** the topic-to-queue subscription is only reachable through a publish expression — `ToSnsTopic(topic).SubscribeSqsQueue(queue)`. There is no standalone binding API on the SNS transport expression the way RabbitMQ has `transport.BindExchange(...).ToQueue(...)`. A subscribing application therefore declares its own subscription _through a publish expression for an event it subscribes to_; the routing rule that produces is inert in a subscriber that never publishes that event. An event both published and subscribed by the same application emits one `ToSnsTopic` rule with the subscription chained onto it, not two competing rules for the same topic.

## `Intent.Wolverine.Common` as an `.imodspec` dependency

This module's `.imodspec` `<dependencies>` block lists `Intent.Wolverine.Common` as a `<dependency>`, even though the `.csproj` only holds a `<ProjectReference>` to it (same-repository, not published as its own NuGet package). Regenerating through the Module Builder designer (`Module Settings → Version`) followed by `run_software_factory` emits the `<dependency id="Intent.Wolverine.Common" version="..." />` entry correctly. The same applies to `Intent.Application.Wolverine`'s imodspec.

This satisfies R14.1 at the install-time/package-manager level: installing `Intent.Eventing.Wolverine` (or `Intent.Application.Wolverine`) automatically brings in `Intent.Wolverine.Common` — a developer never installs it separately.

**Never hand-edit `.imodspec`.** Always change version/dependency-affecting state through the designer and regenerate — a hand-edit is silently reverted (or left wrong) on the next real regeneration.

## Bugs found fixing wave 9 (first real consumer-app generation) — all fixed in this same 1.0.0-pre.0 build

Waves 1-8 only ever regenerated the Golden Sample RabbitMQ apps, which already had a working copy of every file on disk from before the module existed — so several template defects that only manifest on a **first-ever generation for a given app** went undetected until wave 9 generated six brand-new test apps from scratch. Two of the five bugs below (`BindExchange`, DI lifetime) are not first-run-specific and were silently wrong in the Golden Sample too, just never re-verified once written.

- **`WolverineMessageBusTemplatePartial.cs` / `WolverineTenantMiddlewareTemplatePartial.cs` — `UseType`/`GetTypeName` called before any class exists on `CSharpFile`.** The SDK derives a template's own output filename from its first added class; calling `UseType` earlier throws `NullReferenceException` on an app that has never generated that file before. Fix: resolve foreign types **inside** the `.AddClass(...)` callback, never before it, in every template. **Correction (see the tenant-wiring entry below): this fix is necessary but not sufficient.** `WolverineTenantMiddlewareTemplate` already resolved `WolverineTenantHeaderStrategyTemplate.TemplateId` inside its own `.AddClass(...)` callback and still hit the identical `NullReferenceException` — the remaining hazard is cross-template registration ORDER (whether the foreign template has itself finished constructing yet), not merely this template's own callback timing. The two hazards look identical from the stack trace and need separate fixes.
- **`WolverineMessageBusTemplatePartial.cs` — `UseType(...)`'s return value used as a `using X = Y;` alias target.** `UseType` returns the _shortest form valid given the file's other `using` directives_ (e.g. `IMessageBus` once `using Wolverine;` is present) — but C# resolves a using-alias's right-hand side **without** considering sibling `using` imports, only enclosing namespaces and fully-qualified names. A shortened name that compiles fine as an ordinary reference can fail to resolve as an alias target (`CS0246`). Fix: for `Wolverine.IMessageBus` specifically, hardcode the literal fully-qualified string in the alias — do not route it through `UseType`.
- **`WolverineEventingConfigurationTemplatePartial.cs` — missing `using Wolverine.ErrorHandling;`.** Every Error Handling Policy except `None` calls `opts.OnException<Exception>()`, which needs this using regardless of transport; the template never added it for any transport.
- **`WolverineEventingConfigurationTemplatePartial.cs` — `opts.BindExchange(...)` instead of `transport.BindExchange(...)`.** `BindExchange` is a member of the `RabbitMqTransportExpression` returned by `opts.UseRabbitMq(...)` (stored as the local `transport`), not of `WolverineOptions` itself.
- **`WolverineEventingRegistrationExtension.cs` — the bus's `ContainerRegistrationRequest` never called `.WithPerServiceCallLifeTime()`.** `ContainerRegistrationRequest` defaults to `Transient`; every sibling eventing module (MassTransit, Kafka, etc.) registers its bus `Scoped`. Fixed by adding the explicit lifetime call.

**How this was caught:** generating for six brand-new applications and running a real `dotnet build` after every regeneration — not just checking the Software Factory's own "idempotent" report, which only proves the generated content matches its own prior output, never that the prior output actually compiled. See also `known-build-gotchas.instructions.md`'s "Consumer App Name Colliding With a Referenced Broker Library's Root Namespace" entry — naming a consumer app `Wolverine.*` is a separate, sixth issue hit in the same pass (a C# name-resolution collision with the `WolverineFx` package's own namespace), fixed by renaming the app, not the module.

## D6 — This module declares the `MessageBusImplementation` role and hand-writes no interface members

`WolverineMessageBus` declares `FulfillsRole(TemplateRoles.Application.Eventing.MessageBusImplementation)`, as every other eventing provider in this repository does — MassTransit, NServiceBus, Kafka, Solace, AzureServiceBus, AzureEventGrid, AzureQueueStorage, Dapr and Aws.Sqs.

**Why it matters:** when a broker module adds a member to the shared `IMessageBus` interface, it also walks every template holding that role and gives each one a default implementation, so all providers still compile. `Intent.Eventing.MassTransit`'s `MessageBusInterfaceExtension` is the live example: it adds the addressed `Send<TMessage>(TMessage, Uri)` overload to the interface, and supplies non-MassTransit providers with a `throw new NotSupportedException(...)` body — compiles everywhere, fails loudly at runtime where the concept does not apply. Never hand-write an interface member into this bus to satisfy a build; declare the role and let the contributing module supply it.

### Why `WolverineMessageBus` does not implement the addressed `Send` overload

`WolverineMessageBus` does not hand-implement `Send<TMessage>(TMessage, Uri)`. That overload is added to the shared `IMessageBus` interface by whichever broker module needs it (MassTransit's addressed send, for example), via the `MessageBusImplementation` role mechanism described above — it is never this module's to implement.

Hand-writing it here would mean emitting the method **unconditionally**, including in applications with no MassTransit installed, producing a public method that implements nothing. The interface's remark that providers "may ignore this overload" means they need not _support_ it — MassTransit's own extension interprets that as throwing `NotSupportedException`. Silently discarding the address and sending to the default destination would be misdelivery, not tolerance: a caller asking for a specific endpoint would get a different one, with nothing failing to say so.

**Lesson to carry:** a missing interface member on this bus is a symptom of a missing role declaration, not something to hand-write. Check the role before writing the member.

## Bugs found running the `WolverineEventing.MultiTenancy` golden sample end-to-end (fixed in 1.0.0-pre.3)

Both `WolverineTenantHeaderStrategyTemplate` and `WolverineTenantMiddlewareTemplate` compiled and passed Software Factory's own idempotency check under `1.0.0-pre.1` — neither issue below is visible from generated-content diffing alone; both required actually running the generated app and sending a real request.

- **`WolverineTenantHeaderStrategy` was never registered in DI.** `WolverineMessageBusTemplate` injects it into `WolverineMessageBus`'s constructor by **concrete type** (there is no interface for it), but `WolverineTenantHeaderStrategyTemplate.BeforeTemplateExecution()` only published an `AppSettingRegistrationRequest` for the header-name key — never a `ContainerRegistrationRequest`. Every request to a Finbuckle-enabled application threw `InvalidOperationException: Unable to resolve service
for type 'WolverineTenantHeaderStrategy'` the instant `WolverineMessageBus` was constructed, i.e. on every request, not just ones that publish. Fix: `BeforeTemplateExecution()` now also publishes `ContainerRegistrationRequest.ToRegister(this).ForConcern("Infrastructure").WithPerServiceCallLifeTime()` — a concrete-type self-registration, scoped to match `IMultiTenantContextAccessor`'s own lifetime and `WolverineMessageBus`'s own registration (avoiding a captive-dependency mismatch). This is the same self-registration shape several other modules already use for a template with no corresponding interface (e.g. `RedisOmUnitOfWorkTemplatePartial.cs`).
- **`WolverineTenantMiddlewareTemplate`'s constructor could crash with the exact same `NullReferenceException` as the wave-9 bug above, despite already resolving `WolverineTenantHeaderStrategyTemplate.TemplateId` inside its own `.AddClass(...)` callback.** Reproduced deterministically by rebuilding this module mid-session and regenerating `WolverineEventing.MultiTenancy` — not a flake: reverting to the previously-loaded module build made it disappear again, then reapplying the same rebuild reproduced it again. The remaining hazard is cross-template construction ORDER: `GetTypeName` on a foreign template whose own instance has not yet finished constructing in this Software Factory pass hits `CSharpTemplateBase.NormalizeNamespace` trying to read that foreign template's own (not-yet-populated) file metadata. Fix: call `GetTemplate<object>(WolverineTenantHeaderStrategyTemplate.TemplateId, new
TemplateDiscoveryOptions { ThrowIfNotFound = false, TrackDependency = false })` first — this forces the target template to fully construct — and only call `GetTypeName` once that returns non-null. Mirrors `WolverineMessageBusTemplatePartial.cs`'s own existing guard for `MultiTenancyConfiguration`, which never hit this because it guards a _different_ foreign template.
- **Lesson to carry:** any `GetTypeName`/`UseType` call on a foreign template inside a constructor is ordering-fragile regardless of where in the callback chain it sits. The robust shape is: check existence via `GetTemplate<T>(id, new TemplateDiscoveryOptions { ThrowIfNotFound = false, TrackDependency
= false })` first, and only resolve the type name once that succeeds. Apply this to any _new_ cross-template `GetTypeName` call added to this module, not just the two hit here.

## Bug found modelling the first Local-transport subscriber (fixed in 1.0.0-pre.4)

**`AddConfigureLocal` never called `AddHandlerTypeRegistrations`, so a Local-transport subscribed handler was silently never invoked.** Every other `Configure{Transport}` method (`AddConfigureRabbitMq`/`AddConfigureAzureServiceBus`/`AddConfigureAmazonSqs`) gets a handler-type registration for free as the last statement inside `AddListenerRules` (R5.1/R5.5, see the D1/D1b entry above). `AddConfigureLocal` has no listener rules to piggyback on - Local transport needs no queue/exchange-binding plumbing - so it never called `AddHandlerTypeRegistrations` at all. Wolverine's default `Discovery` only scans the entry (`.Api`) assembly; a subscribed handler lives in the `.Application` assembly, so without an explicit `opts.Discovery.IncludeType<T>()` it is never found. The app builds cleanly, starts cleanly, and Wolverine logs `Wolverine found no handlers` at startup and `No known handler for {Message}` on every publish - no compiler error, no Software Factory error, no destructive/missing-file signal. The only way to see it is to actually run the generated app and publish a message a Local-transport handler subscribes to.

**Why this was never caught before:** every application that previously had a Local-transport handler either had no subscriber at all (publish-only apps like `WolverineEventing.MultiTenancy`) or used a different transport (RabbitMQ/SQL Server outbox). The four `WolverineEventing.ErrorPolicy.*` golden samples were the first Local-transport apps ever modelled _with_ a subscribed handler - fixing their "no handler exists to demonstrate the policy" gap is what surfaced this.

**Fix:** `AddConfigureLocal` now calls `AddHandlerTypeRegistrations(method)` directly (not via `AddListenerRules`, which Local transport doesn't need at all).

**Separately, and unrelated to discovery:** `CreateOrderCommandHandler.Handle` across several golden samples (`ErrorPolicy.*`, `Outbox.SqlServer.Publish`, and later also `Coexist.Cqrs` and `MultiTenancy`) published `new OrderCreatedEvent { }` without mapping `OrderId` from the request - the field was silently dropped even once the handler was reachable. Not a module bug: each app's `CreateOrderCommand → OrderCreatedEvent` [Publish Integration Event] association simply had no `Publish Message Mapping`. Fixed per-app by mapping `OrderId → OrderId` on that association. **Lesson to carry:** when scaffolding a new Publish/Send Integration Event or Command association in any golden sample, map its fields immediately - an unmapped association compiles and runs without error, silently emitting a default-valued message.

## D8 — Host registration appends to a lambda `Intent.Wolverine.Common` seeds, and `Order` is load-bearing

`WolverineEventingRegistrationExtension` calls `ConfigureHostBuilderChainStatement("UseWolverine", ...)` directly, appending `{WolverineEventingConfiguration}.Configure{Transport}(opts, builder.Configuration)` to the lambda `Intent.Wolverine.Common` has already seeded. That DSL method is find-or-create, so this appends rather than emitting a competing registration.

**`Order` must stay above both `Intent.Wolverine.Common` and `Intent.Application.Wolverine`.** The values are Common `0`, `Intent.Application.Wolverine` `10`, this module `20` — so this module's transport configuration is layered on top of that module's core configuration. Statements land inside the lambda in ascending factory-extension `Order`; Intent's own `priority` argument cannot be used instead, because the ASP.NET implementation of `ConfigureHostBuilderChainStatement` accepts it and never reads it, and the `ConfigureServices` overload it delegates to has no priority parameter at all.

**Never call `lambdaBlock.Statements.Clear()`** — it discards whatever another contributor has already added.

This module declares `Intent.Wolverine.Common` as an `.imodspec` `<dependency>` but takes **no `ProjectReference`** on it and references none of its types. A `PrivateAssets="All"` reference would bundle a duplicate copy of that module's DLL alongside the one it ships itself, with the loader binding to whichever loads first — see `Intent.Wolverine.Common`'s CONTEXT.md for the full mechanism and why contributors are wired this way.

## Foundations pass (1.0.0-pre.1) — five defects closed

Following the D8 consolidation into `Intent.Wolverine.Common`'s `WolverineConfiguration` (this module's `ContributeEventingConfiguration` now emits a single `ConfigureEventing` method, replacing the retired per-transport `Configure{Transport}` names and the standalone `WolverineEventingConfiguration` class), five further defects were fixed in this same version line:

- **Indentation.** Two raw multi-line strings in what is now `WolverineEventingRegistrationExtension.AddApplyErrorHandlingPolicyMethod`/`AddParseDelaysMethod` emitted body text at column 0 (the C# builder indents per logical *statement*, not per physical line — an unterminated multi-line raw string is one statement, so only its first line gets indented). Fixed: the `RetryWithCooldown`/`ScheduleRetry` if/else now uses `AddIfStatement`/`AddElseStatement`; `ParseDelays`'s body is now one semicolon-terminated line instead of a wrapped raw string. The `UseRabbitMq` lambda was *already* correct (`CSharpLambdaBlock` + `CSharpInvocationStatement`) — that fix predates this pass.
- **Tenancy is now envelope-native.** `WolverineTenantHeaderStrategy` (custom header name, configurable via `Wolverine:TenantHeader`, manual `DeliveryOptions.Headers` read/write) is deleted. `WolverineTenantStrategy` replaces it: a stateless `IMultiTenantStrategy` that reads `Envelope.TenantId` directly, registered into the *foreign* `MultiTenancyConfiguration` chain via `WithStrategy<T>(ServiceLifetime.Scoped)` + `InsertAbove` (mirrors `Intent.Eventing.MassTransit`'s `FinbuckleConfiguratorExtension`, see `WolverineFinbuckleConfiguratorExtension.WireupWolverineTenancyStrategy`). `WolverineTenantMiddleware` is two lines now (`BeforeAsync`, no `FinallyAsync` — restoring the pre-message context had no caller). `WolverineMessageBus` builds `DeliveryOptions.TenantId` itself from `IMultiTenantContextAccessor`, no longer taking a strategy-class dependency.
- **CompositeMessageBus conformance.** New template `WolverineCompositeConfiguration`, gated `CanRunTemplate() => RequiresCompositeMessageBus()` — a non-composite app never generates it. Declares `FulfillsRole(TemplateRoles.Application.Eventing.MessageBusConfiguration)` so `CompositeMessageBusConfiguration`'s role-based discovery finds it, and emits `AddWolverineEventingConfiguration(IServiceCollection, IConfiguration, MessageBrokerRegistry)` — `services.AddScoped<WolverineMessageBus>()` plus one `registry.Register<TMessage, WolverineMessageBus>()` per Wolverine-designated published message/sent command. `WolverineEventingRegistrationExtension.RegisterWolverineMessageBus` (the non-composite path) now skips its `ContainerRegistrationRequest` when `RequiresCompositeMessageBus()` is true, so `WolverineMessageBus` is never registered against the shared bus interface twice — in composite mode only `CompositeMessageBus` itself owns that registration.

### Two real bugs found and fixed while implementing the above

- **`RabbitMqTransportExpression` is not `IRabbitMqTransportExpression`, and it is not in `Wolverine.RabbitMQ`.** `WolverineEventingRegistrationExtension.AddRabbitMqBody`/`ConfigureListeners` declared the transport type as `IRabbitMqTransportExpression` — no such interface exists in WolverineFx 5.39.5; the real, concrete, public type is `Wolverine.RabbitMQ.Internal.RabbitMqTransportExpression`. This had **never been build-verified**: every RabbitMQ-transport app regenerated before this pass had only ever been checked against Local transport, so the CS0246 this produces was latent since the original Point-3 refactor. Fixed by correcting the type name and adding `file.AddUsing("Wolverine.RabbitMQ.Internal")` (conditionally, only for the Rabbitmq transport branch — `Wolverine.RabbitMQ` alone does not bring the `.Internal` sub-namespace into scope).
- **`WolverineMessageBusTemplatePartial.cs` had an unconditional `.AddUsing("Wolverine")`.** Added for `DeliveryOptions` (needed only by `BuildDeliveryOptions()`, itself only emitted when Finbuckle is installed) but placed outside the `if (finbuckleInstalled)` guard — so every app got `using Wolverine;` in `WolverineMessageBus.cs` regardless. Harmless in that specific file (no bare `IMessageBus` reference there), but it is exactly the anti-pattern the module's own CS0104 fix (see the `SendOnWolverineInteractionStrategy` entry in `Intent.Application.Wolverine`'s CONTEXT.md) exists to avoid. Moved inside the guard.

## Integration event handlers already have a unit-of-work seam under Durable outbox — verified empirically, no middleware needed

A handover investigation (2026-09-01) hypothesized that Wolverine integration-event handlers have no
transaction/save seam at all — unlike MassTransit's generated `IntegrationEventConsumer`, this module
generates no intermediate consumer class (by design, `CONTEXT.md` R5.2), and `UnitOfWorkMiddleware` is
`ICommand`-gated so it never sees an integration event (see the D1/D1b entry's sibling reasoning and #3
in `Intent.Modules.EntityFrameworkCore`'s work). The concern: a subscribed handler that writes via a
repository might silently never commit.

**Settled by direct measurement, not inferred.** `AddApplyTransactionalOutboxMethod` already emits
`opts.Policies.AutoApplyTransactions();` whenever `Transactional Outbox = Durable` (this predates the
investigation). Modelled a real `Order` entity + repository in `WolverineEventing.Coexist.Cqrs` (the
only Wolverine-eventing + Wolverine-dispatch app), switched it to `Transactional Outbox = Durable` +
SQL Server, gave `OrderCreatedEventHandler` a repository write with **no explicit `SaveChangesAsync`
call**, and ran it end-to-end against a real SQL Server (LocalDB) instance:

- **Happy path:** the handler took only `IOrderRepository` (no `DbContext` parameter in its signature)

  and never called `SaveChangesAsync`. The row was persisted anyway — confirmed by direct query.

- **Failure path:** same handler, repository `Add()` followed by a forced exception. The row was

  **not** persisted — confirmed by direct query, and the message went to Wolverine's configured
  retry policy rather than half-committing.

- **Mechanism, visible in the host log:** Wolverine logs a "Utilizing service location" warning for

  the handler's resolved `ApplicationDbContext` dependency, reached transitively through
  `IOrderRepository`/`IUnitOfWork` — i.e. `AutoApplyTransactions()` walks the handler's full DI
  dependency graph, not just its own parameter list, to find the `DbContext` to wrap a transaction
  and a save around.

**Conclusion: no new middleware is needed.** `AutoApplyTransactions()` already supplies everything the
originally-proposed `WolverineEventUnitOfWorkMiddleware` would have supplied — transaction wrap,
save-on-success, rollback-on-failure — for any handler whose resolved dependencies eventually touch a
DbContext, integration event handlers included. This only holds under `Transactional Outbox = Durable`
(the gate `AutoApplyTransactions()` is already conditioned on) — a subscriber under `Outbox = None` has
no such safety net, which is consistent with what "None" means, not a gap this module needs to close.

**Do not build a "GOLDEN-SAMPLE" `WolverineEventUnitOfWorkMiddleware` template for this** without a new,
concrete measured gap — the premise it would have closed does not hold under the conditions it would
have applied in. If a future report claims integration event handlers "don't commit," reproduce it the
same way: a real repository write, no explicit save, under Durable outbox, watched end-to-end — a
generated-output diff alone cannot show this, since the whole point is runtime behaviour with no
line of generated code to diff.

**Handler convention this confirms:** `OrderCreatedEventHandler` (and any integration event handler
that writes via a repository under Durable outbox) should follow the same convention Command/Query
handlers already do — the middleware saves, the handler body does not call `SaveChangesAsync` itself.

## Durable outbox regenerating with missing NuGet packages (fresh-app-only defect)

`ContributeEventingConfiguration` emitted `using Wolverine.EntityFrameworkCore;` and `using
Wolverine.SqlServer;`/`Wolverine.Postgresql`, plus `opts.PersistMessagesWithSqlServer(...)` /
`UseEntityFrameworkCoreTransactions()`, whenever `ctx.TransactionalOutbox.IsDurable()` — but never
called `AddNugetDependency` for the `WolverineFx.EntityFrameworkCore` /
`WolverineFx.SqlServer`/`WolverineFx.Postgresql` packages those statements require, even though the
static factory methods for both already existed in `NugetPackages.cs` and D2 (above) explicitly
documents these as intended conditional NuGet registrations. Fixed by adding the two missing
`template.AddNugetDependency(...)` calls next to the existing `DatabaseProvider.IsSupported()` guard,
gated the same way the `using` statements already are.

**Why this was invisible in this repo's own golden samples.** Both `WolverineEventing.Outbox.SqlServer.Publish`
and `.Subscribe`'s `.csproj` files already had `WolverineFx.EntityFrameworkCore`/`WolverineFx.SqlServer`
hand-pinned from before this regression, so regenerating them kept building fine regardless of whether
the module declared the dependency itself — NuGet package references SF writes are additive-only and
never pruned, so a stale hand-pin (or one left over from before a bug was introduced) silently keeps
masking the bug it should have caught. A **brand-new** application selecting Transactional Outbox =
Durable + SQL Server/PostgreSQL for the first time hit `CS0234`/`CS1061` immediately, because it never
had those packages to begin with. **Lesson to carry:** a golden sample that predates a regression is not
proof the current module code is correct — verify a fix like this by stripping the suspect package
references from a golden sample's `.csproj` and re-running the Software Factory, not by trusting that
sample's `.csproj` already looks right.

## Amazon SQS/SNS subscriber never actually receives a message (two stacked defects, found only by testing against real AWS)

Neither defect below showed up in a generated-output diff or a local build — both needed a real AWS
account, because both are about what AWS actually does with a name/attribute at runtime, not about
what code got emitted. **Lesson to carry, alongside the NuGet-pin one above:** for this module, "the
generated code compiles and looks right" and "the message actually arrives" are proven by two
different kinds of check, and only the second one exercises AWS's own naming/format rules.

### 1. `GetSubscriberQueueName`'s dotted app name isn't a valid AWS SQS/SNS resource name

`GetSubscriberQueueName` (see above) returns `{ApplicationNameKebab}-{messageNameKebab}`, and
`ApplicationNameKebab` is `GetApplicationConfig().Name.ToKebabCase()` — a kebab-cased *application
display name*. `.ToKebabCase()` only inserts hyphens at word boundaries; it does not strip characters
that aren't valid in an identifier. Every app in this repo is named with dots
(`WolverineEventing.Transport.AmazonSqs`, `WolverineEventing.Outbox.SqlServer.Publish`, …), so this
name is *routinely* dotted — e.g. `wolverine-eventing.transport.amazon-sqs-order-created-event`.

RabbitMQ queue names and Azure Service Bus entity names both accept a literal period, so this was
invisible on those two transports. AWS SQS/SNS resource names do not — only alphanumerics, `-` and
`_`. Confirmed live: `ListenToSqsQueue` with the dotted name still created a queue (AWS/the SDK
silently tolerated the invalid character in that specific call), but Wolverine's SNS transport
resolves the queue to subscribe **by that same literal string**, and no queue by that exact name
exists — only the one actually created, whose name and this string are never equal. Every startup
failed permanently with `WolverineSnsTransportException` → `QueueDoesNotExistException`, retrying
20 times before giving up.

**Fixed** by `SanitizeAmazonQueueName` (`.Replace('.', '-')`), applied only inside `AddAmazonSqsBody`
— at both `SubscribeSqsQueue` call sites and the `ListenToSqsQueue` call for `ctx.SubscribedMessages`
— so RabbitMQ's and Azure Service Bus's dotted names, which are valid there, are untouched.

### 2. SNS wraps the payload in its own JSON envelope unless `RawMessageDelivery` is set

Fixing #1 got a message all the way to the subscriber's SQS listener, which then threw
`FormatException: The input is not a valid Base-64 string` inside Wolverine's own
`DefaultSqsEnvelopeMapper.ReadEnvelopeData`. AWS SNS-to-SQS subscriptions default to wrapping the
published payload in SNS's own notification envelope (`Type`/`MessageId`/`TopicArn`/`Message`/
`Timestamp`/…) as the SQS message body. Wolverine's SQS listener expects its own raw base64-encoded
envelope there instead — the two formats are incompatible, and the mismatch only appears once a
message is actually in flight, never in a generated-output diff.

**Fixed** by passing a configuration action to `SubscribeSqsQueue(name, config => config.RawMessageDelivery
= true)` (WolverineFx 5.39.5's `Wolverine.AmazonSns` — verified against wolverinefx.net's own SNS
docs, not decompiled), which tells AWS to deliver the original message body unwrapped so the SQS side
sees exactly what a direct `ListenToSqsQueue` subscriber would.

**Trap for next time:** `Subscribe` (the underlying SNS API `SubscribeSqsQueue`/`AutoProvision` call)
is idempotent on protocol+endpoint — re-running an app against a subscription that already exists
does **not** update that subscription's attributes. A subscription created before this fix keeps
`RawMessageDelivery = false` forever unless it is deleted (or its attributes are updated out of band)
so `AutoProvision` creates it fresh. Symptom if this bites again: the FormatException above persists
even after the code fix is deployed, because the *old* subscription is still the one in effect.

**Verified live, end to end, against a real AWS account (`us-east-1`):** `POST /api/orders` → 201 →
`OrderCreatedEvent` published to SNS topic `order-created-event` → delivered via the now-corrected
subscription to SQS queue `wolverine-eventing-transport-amazon-sqs-order-created-event` → Wolverine's
SQS listener correctly decodes it → routed to `OrderCreatedEventHandler.HandleAsync`. The only
failure at that point was the handler's own scaffolded `NotImplementedException` stub body — not a
module defect.

## `Wolverine Subscription` stereotype — the subscriber queue name became overridable

`GetSubscriberQueueName`'s convention (`{appName-kebab}-{message-kebab}`) was "never overridable" by
design from R5.7/R5.9 onward — the point being every subscriber of a fanned-out event automatically
gets its own uniquely-named queue, with no configuration required. Testing
`WolverineEventing.Outbox.SqlServer.Subscribe` against a real Azure Service Bus account broke that:
the app's own long, dot-segmented name pushed the convention name to 69 characters —
`wolverine-eventing.outbox.sql-server.subscribe-order-created-event` — against ASB's hard 50-character
subscription name limit (`ArgumentOutOfRangeException`, live, on startup).

**Why not just drop the app-name prefix, matching MassTransit's default?** Checked first, not assumed:
MassTransit's own `KebabCaseEndpointNameFormatter.Instance` (`Intent.Eventing.MassTransit`'s
`MessageBrokerBase.cs`) is the *unconfigured* singleton — no prefix — so its default endpoint name is
just the consumer class's own name, kebab-cased, with no app-identifying anything. That is not a safer
convention; it is an unguarded one. Two different applications with an identically-named consumer
class for the same event (a very plausible coincidence — `OrderCreatedEventHandler` is the natural
name for it in both) would silently share one queue under MassTransit's default. Wolverine's
app-name-prefixed convention is the more *correct* default on the uniqueness axis; the fix here adds
an escape hatch without giving that up as the default.

**The fix:** the `Wolverine Subscription` stereotype (`Subscriber Queue Name` property), attached to
the **Subscribe Integration Event association's target end** — the exact same attachment point
MassTransit's own `Azure Service Bus Consumer Settings`/`RabbitMQ Consumer Settings` stereotypes use
for their `Endpoint Name` (`SubscribeIntegrationEventTargetEndModel`, confirmed by reading
`Intent.Eventing.MassTransit`'s own stereotype definition and its consumer-enumeration code, not
guessed). `GetSubscriberQueueName` checks `ctx.SubscriptionsByMessageId` (a lookup built once in
`EventingContext.Build`, keyed by subscribed message id) for an override before falling back to the
convention. Consistent with `Topic Name`/`Destination Queue Name` on `Wolverine Message`: **an
override is taken verbatim — never kebab-cased or otherwise transformed** — only validated via the
existing `ValidateNameOverride` (blank/whitespace, >250 chars). The convention-generated default is
unaffected and still gets kebab-cased; only an explicit override skips that.

**Tool limitation hit building this — record it so it isn't rediscovered.** `run_designer_script`'s
association-end handles (`assoc.getTargetEnd()`) expose `hasStereotype`/`getStereotype(s)` but **not**
`addStereotype`/`ensureStereotype`. Verified exhaustively, because the obvious workarounds all look
plausible and all fail: `findElements({ specialization: "Subscribe Integration Event Target End" })`
returns 0 results; the end does not appear as a child element (`handler.getChildren()` yields the
association itself); and `lookupById` on either the association or the end returns the same
non-mutating handle — **the target end shares the association's id**. Only element and package handles
carry the mutating stereotype methods. So a stereotype targeting an association end (this one, and
MassTransit's consumer-settings stereotypes) **cannot be *applied* via script** — that step must go
through the Intent Architect desktop UI.

**But the split is finer than "not scriptable", and the useful half is scriptable:** once the
stereotype instance exists on the end, its *property values* ARE settable from script —
`assoc.getTargetEnd().getStereotype("Wolverine Subscription").getProperty("Subscriber Queue Name").setValue(...)`
works and reports a proper `changes[]` entry. So the only genuinely manual step is the initial apply;
everything after it can be automated. (The stereotype *definition* — targeting, properties — is fully
scriptable via `pkg.addStereotypeDefinition(...)`.)

**Icons:** both `Wolverine Message` and `Wolverine Subscription` carry a bespoke icon derived from the
module's own package icon (the claw-mark "W") rather than a generic FontAwesome glyph — `Wolverine
Message` uses it as-is (outbound motion, fits publishing), `Wolverine Subscription` uses a horizontal
mirror of the same artwork (fits receiving). Same source asset, so the two read as a matched pair
rather than unrelated icons. Setting a `UrlImagePath` icon via script requires passing the full `data:`
URI string directly to `setIcon(...)` — passing `{ type, source }` throws (`setIcon` only accepts a
string and parses the type from its shape: `fa-`-prefixed → FontAwesome, otherwise treated as a raw
`UrlImagePath` source). Also: a script that throws **anywhere** rolls back its **entire** set of
mutations, including ones that had already succeeded earlier in the same script — verified by
inspecting the on-disk model file after a script errored on an unrelated line **after** a working
`setIcon(...)` call, and finding the old icon still there. Keep scripts that set icons free of any
other statement that could throw afterward, or split them into separate calls.

## The DbContext flush splice must agree with the bus-registration gate (subscribe-only apps)

The D5 carve-out's DbContext splice (`InstallMessageBusForDbContextForTransactionalOutboxPattern`,
fix #1) was originally gated on `IsTransactionalOutboxPatternSelected(application)` alone — copied
from `Intent.Eventing.MassTransit`'s equivalent, which gates on exactly that. **That copy was wrong
for this module**, and it is the one defect in this whole line that was *introduced* by a fix rather
than found by one.

`RegisterWolverineMessageBus` deliberately registers **no bus at all** for a subscribe-only
application — no Wolverine-designated published Message and no sent Integration Command means no
`IMessageBus` registration (its own XML doc states this: *"a subscribe-only application gets no
registration"*). The splice, gated only on the outbox setting, injected `IMessageBus` into
`ApplicationDbContext`'s constructor anyway. The two disagreed, and a subscribe-only app with
`Transactional Outbox = Durable` died at startup on DI validation:

```
Unable to resolve service for type '...Application.Common.Eventing.IMessageBus'
while attempting to activate '...Infrastructure.Persistence.ApplicationDbContext'
```

**Why MassTransit's version doesn't need the guard:** MassTransit has no subscribe-only registration
skip, so its splice can't disagree with its own registration. This is therefore a **deliberate
divergence from the reference implementation**, not an oversight — noted in the method's XML doc so
the next person to "align it with MassTransit" doesn't reintroduce the bug.

**The guard is semantic, not defensive.** A subscribe-only application buffers no outgoing messages
on the bus, so there is nothing for a flush to dispatch — splicing one in was pointless as well as
fatal. `PublishesAnyWolverineMessages` mirrors `RegisterWolverineMessageBus`'s own test exactly
(published messages OR sent commands, both filtered for this broker). Composite-bus apps are
unaffected: they publish by definition, and the shared interface is registered by
`CompositeMessageBus`.

**Invariant to preserve:** anything that injects the bus interface into generated code must agree
with `RegisterWolverineMessageBus`'s gate. If that gate ever changes, every injection site has to
move with it.

**Found only by running the app** — the generated code compiled cleanly, and the bad constructor
parameter looks perfectly reasonable in a diff. Same lesson as the AWS section above.

## Round trip verified — Azure Service Bus + Durable outbox + SQL Server

The `WolverineEventing.Outbox.SqlServer.Publish` → `.Subscribe` pair, run end to end against a real
Azure Service Bus namespace (`dandre-test`) and a real SQL Server instance:

- `POST /api/orders` → **201**, no `ambient transaction has been detected` (proves fix #2's

  `TransactionMiddlewareMode.Lightweight`)

- Subscriber logged `HANDLED OrderCreatedEvent OrderId=<the exact posted GUID>` and

  `Successfully processed message ... from asb://topic/order-created-event/outbox-subscribe-order-created-event`
  (proves fix #4's topic→subscription binding, and the `Subscriber Queue Name` override being used
  verbatim — the listener bound to the 36-character override, not the 69-character convention name)

- `wolverine_outgoing_envelopes` drained to **0** in the publisher's database — proves fix #1's

  DbContext splice actually dispatched the message. Without it the message is silently discarded and
  this count would never have been non-zero in the first place.

- `wolverine_dead_letters` **0** on both sides; subscriber's `wolverine_incoming_envelopes` = 1
- **Zero** `MSDTC` / `distributed transaction` / `ambient transaction` entries in either log

This is the combination (ASB + Durable outbox + a real subscriber) that had never been generated or
run before this work, and it is what the four publish-side defects were hiding in.

## LOAD-BEARING INVARIANT — the flush seam must stay OUTSIDE the unit-of-work `TransactionScope`

`WolverineMessageBus.FlushAllAsync` does **not** wrap its dispatch in a suppressing
`TransactionScope`, and **must not be given a blanket one** (see the trap below). It is safe today
only because of *registration ordering*, which is implicit and easy to break. Write this down before
anyone "tidies" the middleware list.

**The hazard is real, not theoretical.** Azure Service Bus enlists in an ambient
`TransactionScope` as a *volatile participant*, and ASB explicitly does **not** support DTC /
2-phase-commit with another resource manager. So an ASB send issued inside a scope that a SQL
connection has also enlisted in throws
`InvalidOperationException: Local transactions are not supported with other resource managers/DTC`
([Transactions in Azure Service Bus](https://learn.microsoft.com/en-us/azure/service-bus-messaging/service-bus-transactions)).

**This module is the ONLY eventing provider here that does not suppress.** Establish this by
searching generated output, not module source — the suppression lives in several different template
families and a folder-scoped grep of two modules misses most of it:

| Module | Suppresses? | Where |
|---|---|---|
| `Intent.Eventing.NServiceBus` | yes | `NServiceBusMessageBus.FlushAllAsync`, wrapping BOTH the outbox (`ITransactionalSession`) and non-outbox paths — commented *"prevent ASB/RabbitMQ/SQL clients from attempting DTC enlistment"* |
| `Intent.Eventing.AzureServiceBus` | yes, unconditionally | `AzureServiceBusMessageBus.FlushAllAsync` — the closest structural analogue to `WolverineMessageBus` |
| `Intent.Eventing.MassTransit` | yes | `ServiceRequestClient` (the request/response client, around `IRequestClient.GetResponse`) — NOT in its message bus |
| **`Intent.Eventing.Wolverine`** | **no** | — |

**MEASURED — and the answer is different for the raw ASB SDK than for Wolverine's bus.** Two repros
against the real `dandre-test` namespace. Read both tables together; the second is the one that
governs this module.

*Raw `Azure.Messaging.ServiceBus` SDK (what NServiceBus's and `Intent.Eventing.AzureServiceBus`'s buses
call directly):*

| Scenario | Result |
|---|---|
| send, no ambient transaction | OK |
| send inside `TransactionScope(ReadCommitted)`, SQL enlisted | **throws** `InvalidOperationException: The only supported IsolationLevel is Serializable` |
| send inside `TransactionScope(ReadCommitted)`, **no SQL at all** | **throws — same error** |
| send inside `TransactionScope(Serializable)` | send succeeds, then commit throws `TransactionInDoubtException` |
| send wrapped in `TransactionScopeOption.Suppress` | OK |

Worth knowing on its own: it is **not** a DTC problem (`DistributedIdentifier` stayed all-zeros —
nothing was ever promoted; ASB rejects the scope purely on isolation level), a database is **not**
required to trigger it, and `Serializable` is not an escape.

*Wolverine's `IMessageBus.PublishAsync` — what this module's `FlushAllAsync` actually calls, with a
real Wolverine host on the ASB transport:*

| Scenario | Result |
|---|---|
| `PublishAsync` inside `TransactionScope(ReadCommitted)`, **no suppression** | **OK** |
| same, wrapped in `Suppress` | OK |

**So this module was never exposed, and the difference is architectural, not an oversight.**
Wolverine's `PublishAsync` hands the envelope to Wolverine's own sending agent; the ASB SDK call
happens on a background sender, outside any ambient transaction. NServiceBus and
`Intent.Eventing.AzureServiceBus` call the broker SDK *inline* inside their flush, which is precisely
why they need the suppression and this module does not. Confirmed the module never emits
`SendInline()` — every generated endpoint is buffered (default) or durable — so the inline case cannot
arise from generated configuration.

**This also corrects an earlier claim in this file:** the flush-outside-the-unit-of-work ordering was
described here as the only thing preventing every ASB publish from throwing. That is **false for
Wolverine** — measured above. The ordering remains sound design, but it is not load-bearing for this
hazard.

**Decision: a `Suppress` scope IS emitted, but only when `Transactional Outbox = None`** (see
`WolverineMessageBusTemplate.FlushAllAsync`). Given the measurement it is **inert by construction, not
a fix** — it is cheap insurance that only becomes load-bearing if an endpoint is ever configured for
inline sending, at which point the raw-SDK table above applies again. Verified it costs nothing:
regenerated, rebuilt, and re-ran the ASB app end to end with it in place — message still delivered to
the subscriber, no errors. **Do not cite it as evidence a hazard existed here.** If it is ever removed
as dead code, that is a defensible call — but re-add it the moment inline sending appears.

**The Durable path deliberately keeps NO suppression**, because the two suppressing siblings are not
precedents for it: NServiceBus's outbox uses `ITransactionalSession` with an explicit `Commit` so it
never leans on the ambient scope, and `Intent.Eventing.AzureServiceBus` has no outbox at all. Under
Durable, fix #1's DbContext splice calls `FlushAllAsync` *inside* `SaveChangesAsync` — inside the
ambient scope on purpose — and Wolverine enrols outgoing envelopes on the DbContext's own connection
so they commit atomically with the entity changes. No broker call happens there (the durability agent
dispatches later, out of band), so there is nothing to protect against; suppressing would only risk
decoupling the envelope write from the commit.

**Why this module is nonetheless safe:** in BOTH dispatch stacks the flush seam is the OUTER
wrapper around the unit of work, so the broker call always happens after `tx.Complete()` /
`tx.Dispose()` — there is no ambient transaction in scope by the time anything reaches the broker.

| Dispatch stack | Registration (order = outermost first) | Effect |
|---|---|---|
| Wolverine | `AddMiddleware<MessageBusFlushMiddleware>` **then** `AddMiddleware<UnitOfWorkMiddleware>` | flush wraps the UoW; send happens after the scope closes |
| MediatR | `AddOpenBehavior(MessageBusPublishBehaviour<,>)` **then** `AddOpenBehavior(UnitOfWorkBehaviour<,>)` | same shape |

**Break the ordering and you get a runtime failure no build or generated-output diff will show** —
and only on a transport that enlists (ASB), only when the handler also wrote to SQL in the same
scope. Empirically confirmed passing on `WolverineEventing.Transport.AzureServiceBus`
(`Outbox = None`, real ASB): POST → 201, message reached the topic, no DTC error. **Note that run
did not actually exercise the hazard** — that app writes nothing to its DbContext, so no SQL resource
manager ever enlisted, and the flush was outside the scope anyway. It is evidence the happy path
works, NOT evidence the guard is unnecessary.

> **Trap — do NOT "fix parity with NServiceBus" by adding a blanket `Suppress` to `FlushAllAsync`.**
> It would break the Durable outbox. Under `Transactional Outbox = Durable`, fix #1's DbContext
> splice calls `FlushAllAsync` *inside* `SaveChangesAsync`, i.e. deliberately **inside** the ambient
> scope — and there the flush writes Wolverine's outgoing envelopes to SQL on the DbContext's own
> connection, which **must** enlist in that transaction or the outbox stops being atomic, which is
> the entire reason the outbox exists. No ASB call happens at that moment (the durability agent
> dispatches later, out of band), so there is no DTC hazard on that path to suppress. NServiceBus can
> suppress unconditionally because its outbox path uses `ITransactionalSession` with an explicit
> `Commit`, so it does not depend on the ambient scope the way this one does.
>
> If suppression is ever genuinely needed, it must be conditional — applied only on the
> `Outbox = None` path, never on the Durable path.
