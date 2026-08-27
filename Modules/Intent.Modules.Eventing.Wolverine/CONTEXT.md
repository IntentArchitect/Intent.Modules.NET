# Intent.Eventing.Wolverine — Context

## What this module is

The Wolverine eventing provider: transport selection, publish/send rules, listeners, transactional outbox and error-handling policy for cross-application messaging over [WolverineFx](https://wolverine.netlify.app/). It implements `Intent.Eventing.Contracts`'s `IMessageBus` and integrates with the Composite Message Bus so an application can run more than one eventing provider side by side.

It depends on `Intent.Wolverine.Common` for the single shared `builder.Host.UseWolverine(opts => ...)` registration — see that module's own CONTEXT.md for why the host registration is arbitrated there rather than here.

## Design decisions (durable — read before changing behaviour this module relies on)

### D1 / D1b — Conventional discovery stays ON, _and_ handler types are registered explicitly

**This entry supersedes an earlier version of D1/D1b that recorded the opposite conclusion.** That version stated this module carries "no per-message Handler Type Registration", because registering a handler type explicitly while conventional discovery also found it would make Wolverine see two handlers for one message and emit a duplicate local (`CS0128`) in the consumer's generated handler pipeline. **That hazard does not exist.** It was assumed, not measured, and it is wrong.

`Intent.Wolverine.Common` leaves Wolverine's conventional handler discovery **on**. This module _additionally_ emits one `opts.Discovery.IncludeType<THandler>()` per distinct `IIntegrationEventHandler<T>` implementation it subscribes, alongside that message's listener (R5.1/R5.5). `DisableConventionalDiscovery()` is deliberately **not** emitted.

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

### D5 — This module supplies the bus and its flush method; it does not call it

`WolverineMessageBus` (the buffered `IMessageBus` implementation) and its flush method are generated here. Nothing generated by this module calls that flush method.

**Why:** the flush seam belongs to whichever dispatch mechanism is buffering commands/queries around it, not to the eventing provider — that seam is per-dispatch-mechanism, and the precedent already exists on both sides of the codebase: `Intent.Application.Wolverine` supplies `MessageBusFlushMiddleware` for a Wolverine-CQRS application, and a MediatR application gets `MessageBusPublishBehaviour` from its own module. Each of those middleware/behaviour implementations already only emits when an eventing module's bus interface actually resolves, so the two halves find each other without this module needing to know which dispatch mechanism (if any) is installed. If a future change makes this module flush itself, check whether that breaks an application with no dispatch/CQRS module installed at all — R8's requirement that this module works standalone depends on it not needing one.

### D7 — Amazon SQS fan-out goes through SNS, in a second package

An Integration Event's publish rule on the Amazon SQS transport targets an **SNS topic** (`opts.PublishMessage<T>().ToSnsTopic(name)`), not an SQS queue. Integration Commands stay point-to-point on `ToSqsQueue`.

**Why:** `WolverineFx.AmazonSqs` exposes no fan-out publish expression at all — its only publish targets are `ToSqsQueue`, `ToSqsQueueOnNamedBroker` and the sharded-queue variants (confirmed by reflecting over `Wolverine.AmazonSqs.AmazonSqsTransportExtensions` at 5.39.5). SQS is a queue service; SNS-to-SQS is AWS's fan-out primitive, and Wolverine ships it as a **separate package**, `WolverineFx.AmazonSns`. So the Amazon SQS transport selection now pulls in _two_ packages, and the generated configuration registers _two_ transports (`UseAmazonSqsTransport` and `UseAmazonSnsTransport`) against the same region and credentials. Do not try to express Integration Event fan-out through `WolverineFx.AmazonSqs` alone — there is no API for it.

**The subscriber-side wrinkle:** the topic-to-queue subscription is only reachable through a publish expression — `ToSnsTopic(topic).SubscribeSqsQueue(queue)`. There is no standalone binding API on the SNS transport expression the way RabbitMQ has `transport.BindExchange(...).ToQueue(...)`. A subscribing application therefore declares its own subscription _through a publish expression for an event it subscribes to_; the routing rule that produces is inert in a subscriber that never publishes that event. An event both published and subscribed by the same application emits one `ToSnsTopic` rule with the subscription chained onto it, not two competing rules for the same topic.

## Superseded: `Intent.Wolverine.Common` was believed unable to appear as an imodspec `<dependency>`

An earlier version of this entry recorded that `Intent.Eventing.Wolverine.imodspec`'s `<dependencies>` block could not list `Intent.Wolverine.Common`, on the theory that the list is populated only from `PackageReference`s to _published_ Intent modules, and this module's `.csproj` only holds a `<ProjectReference>` to that sibling (same-repository, not yet published as its own NuGet package). **That theory was wrong, or the tooling has since changed.** Regenerating through the Module Builder designer (`Module Settings → Version`) followed by `run_software_factory` now emits `<dependency id="Intent.Wolverine.Common" version="..." />` correctly, with no manual edit involved — confirmed by bumping this module's version and inspecting the resulting `.imodspec` diff, which touched only the version line and left the dependency entry (already present and correctly formed) untouched. The same is true for `Intent.Application.Wolverine`'s imodspec.

R14.1's promise that "the Shared Host Module of R18 SHALL be brought in automatically as a dependency, not presented as a second decision" is therefore met at the install-time/package-manager level: installing `Intent.Eventing.Wolverine` (or `Intent.Application.Wolverine`) now brings in `Intent.Wolverine.Common` automatically, without a developer installing it separately.

**Caution for whoever next touches these two imodspecs by hand:** a working-tree copy of `Intent.Application.Wolverine.imodspec` was found with this dependency entry present but tab-indented and out of alphabetical order, and with the (still-needed) `Intent.OutputManager.RoslynWeaver` dependency missing entirely — the signature of a hand-edit rather than genuine Software Factory output. Never hand-edit `.imodspec`; it is silently reverted (or, worse, left wrong) on the next real regeneration. Always change the version/dependency-affecting state through the designer and regenerate.

## Bugs found fixing wave 9 (first real consumer-app generation) — all fixed in this same 1.0.0-pre.0 build

Waves 1-8 only ever regenerated the Golden Sample RabbitMQ apps, which already had a working copy of every file on disk from before the module existed — so several template defects that only manifest on a **first-ever generation for a given app** went undetected until wave 9 generated six brand-new test apps from scratch. Two of the five bugs below (`BindExchange`, DI lifetime) are not first-run-specific and were silently wrong in the Golden Sample too, just never re-verified once written.

- **`WolverineMessageBusTemplatePartial.cs` / `WolverineTenantMiddlewareTemplatePartial.cs` — `UseType`/`GetTypeName` called before any class exists on `CSharpFile`.** The SDK derives a template's own output filename from its first added class; calling `UseType` earlier throws `NullReferenceException` on an app that has never generated that file before. Fix: resolve foreign types **inside** the `.AddClass(...)` callback, never before it, in every template.
- **`WolverineMessageBusTemplatePartial.cs` — `UseType(...)`'s return value used as a `using X = Y;` alias target.** `UseType` returns the _shortest form valid given the file's other `using` directives_ (e.g. `IMessageBus` once `using Wolverine;` is present) — but C# resolves a using-alias's right-hand side **without** considering sibling `using` imports, only enclosing namespaces and fully-qualified names. A shortened name that compiles fine as an ordinary reference can fail to resolve as an alias target (`CS0246`). Fix: for `Wolverine.IMessageBus` specifically, hardcode the literal fully-qualified string in the alias — do not route it through `UseType`.
- **`WolverineEventingConfigurationTemplatePartial.cs` — missing `using Wolverine.ErrorHandling;`.** Every Error Handling Policy except `None` calls `opts.OnException<Exception>()`, which needs this using regardless of transport; the template never added it for any transport.
- **`WolverineEventingConfigurationTemplatePartial.cs` — `opts.BindExchange(...)` instead of `transport.BindExchange(...)`.** `BindExchange` is a member of the `RabbitMqTransportExpression` returned by `opts.UseRabbitMq(...)` (stored as the local `transport`), not of `WolverineOptions` itself.
- **`WolverineEventingRegistrationExtension.cs` — the bus's `ContainerRegistrationRequest` never called `.WithPerServiceCallLifeTime()`.** `ContainerRegistrationRequest` defaults to `Transient`; every sibling eventing module (MassTransit, Kafka, etc.) registers its bus `Scoped`. Fixed by adding the explicit lifetime call.

**How this was caught:** generating for six brand-new applications and running a real `dotnet build` after every regeneration — not just checking the Software Factory's own "idempotent" report, which only proves the generated content matches its own prior output, never that the prior output actually compiled. See also `known-build-gotchas.instructions.md`'s "Consumer App Name Colliding With a Referenced Broker Library's Root Namespace" entry — naming a consumer app `Wolverine.*` is a separate, sixth issue hit in the same pass (a C# name-resolution collision with the `WolverineFx` package's own namespace), fixed by renaming the app, not the module.

## D6 — This module declares the `MessageBusImplementation` role and hand-writes no interface members

`WolverineMessageBus` declares `FulfillsRole(TemplateRoles.Application.Eventing.MessageBusImplementation)`, as every other eventing provider in this repository does — MassTransit, NServiceBus, Kafka, Solace, AzureServiceBus, AzureEventGrid, AzureQueueStorage, Dapr and Aws.Sqs.

**Why it matters:** when a broker module adds a member to the shared `IMessageBus` interface, it also walks every template holding that role and gives each one a default implementation, so all providers still compile. `Intent.Eventing.MassTransit`'s `MessageBusInterfaceExtension` is the live example: it adds the addressed `Send<TMessage>(TMessage, Uri)` overload to the interface, and supplies non-MassTransit providers with a `throw new NotSupportedException(...)` body — compiles everywhere, fails loudly at runtime where the concept does not apply. Never hand-write an interface member into this bus to satisfy a build; declare the role and let the contributing module supply it.

### Corrected record — the addressed `Send` overload (supersedes the earlier note in this file)

An earlier entry here recorded that `WolverineMessageBus` hand-implemented `Send<TMessage>(TMessage, Uri)` by **ignoring the address**, and justified it as "the correct behaviour per the interface's own documented contract". That was wrong on both counts, and the code has been removed:

- The overload was only missing because this module lacked the role above. It was never Wolverine's to implement.
- Hand-writing it emitted the method **unconditionally** — including in applications whose `IMessageBus` never declared it (no MassTransit installed), producing a public method that implemented nothing.
- The interface's remark that providers "may ignore this overload" means they need not _support_ it. MassTransit's own extension interprets that as throwing `NotSupportedException`. Silently discarding the address and sending to the default destination is misdelivery, not tolerance — a caller asking for a specific endpoint got a different one, with nothing failing.

**Lesson to carry:** a missing interface member on this bus is a symptom. Check the role before writing the member.
