# Sample Charter — Wolverine Eventing Reference Solution

Status: **RETRO-CHARTER, AWAITING APPROVAL.** Two decisions below block approval.

This charter was written _after_ the sample was partly built, which is not how Phase 0 is meant to run. It exists because the sample was built during implementation while T0.1–T0.8 were ticked and Gate G0 never ran. It is derived from `requirements.mdx` (R15, R16, R17) and `design.mdx` (the `golden-samples` table), read for intent rather than as ground truth — where a document and the compiler disagreed during verification, the compiler is recorded here and the document is left for a later amendment pass.

- Sample commit (as found, warts and all): `849aba061a`
- Location: `Tests/Wolverine.Publish.RabbitMQ`, `Tests/Wolverine.Subscribe.RabbitMQ`, `Tests/Wolverine.Reference.Tests`
- Pinned runtime: WolverineFx 5.39.5, .NET 8
- This charter lives beside the future dossier and baseline rather than at the sample's own root, because the applications may be renamed (see Decision D1) and a charter inside one of them would not survive it.

---

## Topology

Two Intent Architect applications on the Clean Architecture template, scaffolded with no Wolverine module installed, plus one test project that boots them.

```
Wolverine.Publish.RabbitMQ (publisher)          Wolverine.Subscribe.RabbitMQ (subscriber)
  .Api        Program.cs  -- host registration    .Api        Program.cs -- host registration
  .Application  message contracts                 .Application  message contracts (own copy)
                                                                IIntegrationEventHandler impls
  .Infrastructure                                 .Infrastructure
      WolverineEventingConfiguration                  WolverineEventingConfiguration
      WolverineMessageBus                             <Message>Consumer per subscription
      ApplicationDbContext                            ApplicationDbContext
            |                                                       |
            +--------- RabbitMQ (Testcontainers) --------------------+
            +--------- SQL Server (Testcontainers) ------------------+

Wolverine.Reference.Tests -- boots both applications' real startup paths
```

How it is built: **Intent-scaffolded, then hand-written on top.** The scaffold comes from already released modules (AspNetCore, Entity Framework Core, Eventing.Contracts); every _Wolverine_ artefact is hand-written (R15.6). This keeps each golden sample in the exact path and shape the templates will later target.

---

## Capabilities the sample must demonstrate

Each capability is the reason a later requirement exists. C12 is retro-chartered — the build forced it into existence and nothing recorded it.

| Id  | Capability                                                                                      | Requirements     | Depth                               |
| --- | ----------------------------------------------------------------------------------------------- | ---------------- | ----------------------------------- |
| C1  | Publish an Integration Event, fan-out to its Topic Name                                         | R3.1, R3.2       | Runtime                             |
| C2  | Send an Integration Command point-to-point to its Destination Queue                             | R4.1, R4.2       | Runtime                             |
| C3  | A generated Consumer receives from the transport and delegates to `IIntegrationEventHandler<T>` | R5.1, R5.2       | Runtime                             |
| C4  | Error Handling Policy: retry with cooldown, then the Error Queue                                | R7.2, R7.3       | Runtime                             |
| C5  | Empty delay list degrades to None rather than retrying forever                                  | R7.5             | Runtime                             |
| C6  | Transactional Outbox **Durable** — dispatched iff the transaction commits                       | R6.3             | Runtime                             |
| C7  | Transactional Outbox **None** — straight to transport, no database required                     | R6.2             | Runtime                             |
| C8  | Exactly one Wolverine Host Configuration, on the ASP.NET host only                              | R8.2, R8.3, R8.5 | Runtime                             |
| C9  | Transport registration per Transport                                                            | R2.2, R16.6      | RabbitMQ runtime; rest compile-only |
| C10 | Broker Topology: auto-provision, and externally-owned                                           | R2.7             | Auto runtime; external compile-only |
| C11 | `appsettings.json` sections and defaults                                                        | R2.3, R7.2       | Runtime (read by the host)          |
| C12 | **Handler discovery reaches the Consumers from the host's own application assembly**            | R5.1, R5.5       | Runtime                             |

C12 is not decoration. Verification proved that Wolverine discovers a concrete `<Message>Consumer` by naming convention **only in assemblies it scans**, that `ApplicationAssembly` is the entry assembly (`*.Api` in the shipped app), and that the Consumers live in `*.Infrastructure`. Without an explicit `Discovery.IncludeAssembly` call the shipped applications find zero handlers. Any template written from the current sample would reproduce that hole faithfully.

---

## Scope narrowed — D3

**D3 — How much does this sample have to prove? A SIMPLE NON-TRANSACTIONAL PATTERN, and no more.**

Developer decision, quoted: _"we're not busy testing every thing scenario here. we just want a simple non-transactional-outbox sample that will give us a rough idea what the pattern will look like when integrated into clean architecture"_, and on the test harness: _"what is this ReferenceSolutionFixture test? Looks completely unnecessary and a huge hindrance."_

Acted on. `Tests/Wolverine.Reference.Tests` is **deleted**, and with it the Testcontainers dependency, the `extern alias Sub` scheme, the `ObservingHandler`/`AlwaysThrowingHandler` doubles and the `.slnx`. The sample is now two Intent applications carrying hand-written Wolverine artefacts that build clean, in the module's default configuration: RabbitMQ, Transactional Outbox = None, auto-provision, retry with cooldown.

**Consequence, recorded rather than glossed:** with no automated test there is no automated _gate_ for G0 #2 and #3 — nothing fails a build if this regresses. The runtime evidence is the real-host verification recorded below, reproducible in minutes by the runbook at the end of this document. The test list that used to be here — T-1 to T-7, including the transaction-boundary test — is **descoped**, not deferred silently. If the outbox, error handling or the remaining three transports are ever brought into scope, those tests come back with them.

**Do not share libraries between Intent applications.** An earlier draft of this charter floated a shared contracts project to dodge the CS0433 collision below. That is wrong and is retracted: Intent deliberately gives each application its own copy of the message contracts, matched across the wire by type name, and coupling two applications' solutions to suit a test harness would have distorted the sample away from what every real consumer gets.

### Runtime evidence — real host, docker RabbitMQ, separate processes

Verified against WolverineFx 5.39.5 with `docker run rabbitmq:3.13-management`, no database anywhere. The subscriber ran as its **own real ASP.NET host** (`dotnet run` on `Wolverine.Subscribe.RabbitMQ.Api`, i.e. `Program.Main`), and the publisher published from a **separate process** using the sample's own `WolverineEventingConfiguration` and `WolverineMessageBus` verbatim. Separate processes is both the production topology and the reason the CS0433 collision below cannot arise.

What the subscriber's real host did on startup, from its own log:

- discovered handlers by scanning `Wolverine.Subscribe.RabbitMQ.Infrastructure` — the `Discovery.IncludeAssembly` fix working where it actually matters, in the shipped host
- auto-provisioned the topology: exchange `order-shipped-event` (fanout), queue `wolverine-subscribe-rabbitmq-order-shipped-event` with its binding, queue `process-order-command`, plus `wolverine-dead-letter-queue` and its exchange and binding
- started listening on both queues; RabbitMQ reported 1 consumer on each

What happened when the publisher published one Integration Event and sent one Integration Command:

| Behaviour                                                    | Evidence                                                                                                                                                    | Requirement |
| ------------------------------------------------------------ | ----------------------------------------------------------------------------------------------------------------------------------------------------------- | ----------- |
| Event published fan-out to its Topic Name                    | Arrived on the bound queue                                                                                                                                  | R3.1, R3.2  |
| Command sent point-to-point to its Destination Queue         | Arrived on `process-order-command`                                                                                                                          | R4.1, R4.2  |
| Generated Consumer received it and delegated to the handler  | Stack shows `Internal.Generated.WolverineHandlers.OrderShippedEventHandler861627025` → `OrderShippedEventConsumer` → `OrderShippedEventHandler.HandleAsync` | R5.1, R5.2  |
| Handler failure surfaced to the policy, not swallowed        | The scaffold stub's `NotImplementedException` propagated                                                                                                    | R5.6        |
| Retry with cooldown on the configured schedule               | Attempts at `11:27:49`, `11:27:50`, `11:27:55` — the `00:00:01, 00:00:05, 00:00:15` sequence                                                                | R7.2        |
| Exhausted message retained on the Error Queue, not discarded | `wolverine-dead-letter-queue` went to 2 messages; both working queues returned to 0                                                                         | R7.3        |

The handler bodies are still the generated `throw new NotImplementedException` stubs, and that is what makes this evidence unambiguous: reaching the stub proves the whole chain — transport, generated Consumer, DI resolution, hand-written handler — without a single test double in it.

Two faults were found and fixed getting here, both of which the templates would otherwise have reproduced:

Two faults were found and fixed getting there, both of which the templates would otherwise have reproduced:

1. **Missing handler discovery.** `WolverineOptions.ApplicationAssembly` is the entry assembly (`*.Api`); the Consumers live in `*.Infrastructure`. Without `options.Discovery.IncludeAssembly(...)` Wolverine logs _"found no handlers"_ and no Consumer is ever invoked — in the shipped application, not merely under test.
2. **Outbox=None is the absence of a message store, not a mode.** `DurabilityMode.MediatorOnly` is emphatically _not_ how to express it: it disables external messaging and makes `PublishAsync` throw.

### The co-hosting constraint (carry into the design)

Two Intent applications that share message contract namespaces **cannot be hosted in one process** under Wolverine:

```
CS0433: The type 'OrderShippedEvent' exists in both
  'Wolverine.Publish.RabbitMQ.Application' and 'Wolverine.Subscribe.RabbitMQ.Application'
```

Wolverine compiles its handler wrappers at runtime, emitting source that names the message type. Each application generates its own copy of the contract under the publisher's namespace — correct, and what makes names match on the wire — so in a single process that name is ambiguous and codegen fails. An `extern alias` cannot rescue it: the ambiguous source is Wolverine's, not ours. This is the reason the deleted fixture could never have worked, and it constrains any future in-process integration test.

## Descoped test list (retained for reference)

Every test boots the sample's **real** startup path — `Program`/`WebApplication.CreateBuilder` and the application's own `AddApplication`/`AddInfrastructure`/`UseWolverine` chain. A test that constructs its own host with `Host.CreateDefaultBuilder()` and re-registers DI by hand does not count for this gate, and that is what the current four tests do.

| Test                                                                          | Capability  | Requirement | Currently                                       |
| ----------------------------------------------------------------------------- | ----------- | ----------- | ----------------------------------------------- |
| T-1 Published event reaches the subscriber's handler                          | C1, C3, C12 | R17.1       | Exists, **fails**                               |
| T-2 Sent command arrives on its queue, handled exactly once                   | C2, C3, C12 | R17.2       | Exists, **fails**                               |
| T-3 Throwing handler retried per policy, then **observed on the Error Queue** | C4          | R17.3       | Exists, **fails**, and never observes the queue |
| T-4 Rolled-back transaction dispatches none; committed dispatches all         | C6          | R17.4       | **Absent**                                      |
| T-5 Empty delay list degrades to None                                         | C5          | R7.5        | Exists, **fails**                               |
| T-6 Outbox=None publish and send reach the subscriber with no database        | C7          | R6.2        | **Absent**                                      |
| T-7 Default configuration (Local + None) starts and round-trips in-process    | C7, C8, C9  | R14.3       | **Absent**                                      |

T-6 and T-7 are new, and they are not padding. Transport=Local and Outbox=None are the module's _defaults_ (R2.1, R6.1), and G0 requires every default to have its own variant — not only the interesting non-default. The current sample hardwires RabbitMQ and SQL Server durability unconditionally, so the shape a developer gets on a fresh install is the one shape never exercised.

T-3 must assert the message is _on_ the Error Queue. The existing test infers it from re-delivery attempts ceasing, which is also what a silently-discarded message looks like.

---

## Variants

| Variant                                    | Depth                    | Reason                                                                                      |
| ------------------------------------------ | ------------------------ | ------------------------------------------------------------------------------------------- |
| RabbitMQ + Outbox Durable + Auto-provision | Full runtime             | The one fully-exercised transport (R16.6)                                                   |
| RabbitMQ + Outbox None                     | Full runtime             | Outbox None is a module default (G0 #4)                                                     |
| Local + Outbox None                        | Full runtime, in-process | The module's out-of-the-box default (R14.3)                                                 |
| Azure Service Bus                          | Compile-only             | No emulator in scope; hand runtime verification (a10)                                       |
| Amazon SQS                                 | Compile-only             | Same as above (a10)                                                                         |
| Broker Topology Externally-owned           | Compile-only             | Needs a pre-provisioned estate; auto-provision is the default and carries the runtime proof |

Downgrades to compile-only are inherited from R16.6 and assumption a10, which state them outright rather than promising four-transport runtime proof. R16.10 is the route back: if a compile-only transport turns out to need a different error-handling or dead-letter shape, it is promoted to full depth _before_ its template branch is written.

---

## Assumption ledger

Opened here for the first time. `T0.8` claimed to settle a1, a2, a8, a9 and a11; none of them has a committed artefact, and a2 turned out to be half wrong.

| Id  | Assumption                                                                                                   | State                                                                                                                                                | Probe that closes it                                                                                                                            |
| --- | ------------------------------------------------------------------------------------------------------------ | ---------------------------------------------------------------------------------------------------------------------------------------------------- | ----------------------------------------------------------------------------------------------------------------------------------------------- |
| a1  | The seven `WolverineFx.*` packages exist at 5.39.5 and release in lockstep                                   | **Partial** — core, `.RabbitMQ`, `.EntityFrameworkCore`, `.SqlServer` resolved at 5.39.5; `.AzureServiceBus`, `.AmazonSqs`, `.Postgresql` unverified | Restore each satellite in the compile-only variants                                                                                             |
| a2  | Wolverine discovers a concrete `<Message>Consumer` by naming convention                                      | **Closed, and it split.** Convention works; discovery of the assembly does not happen by default                                                     | Probe run against 5.39.5: default → 0 chains; `Discovery.IncludeAssembly` → 2 chains, both Consumers. Needs a committed call site in the sample |
| a8  | The Intent-managed `.sln` tolerates a foreign test project                                                   | **Open** — sidestepped with a `.slnx`, never answered                                                                                                | Add the test project to a managed `.sln`, run the Software Factory, see whether it survives                                                     |
| a9  | Wolverine's `ActivitySource` is named `"Wolverine"`                                                          | **Open** — no artefact. All of R11 is this one string                                                                                                | Read the pinned 5.39.5 assembly                                                                                                                 |
| a11 | Outbox Durable with Transport Local is coherent, and R2.5's "messages are lost on process stop" may be wrong | **Open** — no artefact                                                                                                                               | Exercise Durable + Local; if it contradicts R2.5, amend R2.5 rather than forbidding the combination                                             |
| n1  | The real ASP.NET host (`ApplicationAssembly` = `*.Api`) reaches Consumers in `*.Infrastructure`              | **Open** — this is C12                                                                                                                               | T-1 booting the real host                                                                                                                       |
| n2  | An exhausted message actually lands on the Error Queue                                                       | **Open** — nothing observes it                                                                                                                       | T-3 asserting queue contents                                                                                                                    |
| n3  | The `Wolverine.*` root namespace causes no resolution problem a normally-named consumer would not have       | **Open** — drives Decision D1                                                                                                                        | Rename, or record the descope with sign-off                                                                                                     |

---

## Naming and namespace pre-check (G0 #8)

**This check fails as the sample stands.**

The application root namespaces are `Wolverine.Publish.RabbitMQ` and `Wolverine.Subscribe.RabbitMQ`. Both share their root segment with the referenced package's own root namespace, `Wolverine` — which is exactly what this criterion forbids. Observable consequences already sitting in the golden files:

- `WolverineEventingConfiguration.cs:33,36` must fully qualify `Wolverine.Publish.RabbitMQ.Eventing.Messages.OrderShippedEvent` inside a file that also carries `using Wolverine;`
- the test project needs `extern alias Sub` and `Sub::`-prefixed usings throughout

This repository has already settled the same question once, in the opposite direction: the NServiceBus test applications are named `N_ServiceBus.*`, with the recorded reason _"Avoid clashes with actual NServiceBus namespace by renaming these test apps."_

The risk is specific, and it is not cosmetic: templates written against this sample are calibrated against name-resolution behaviour that no sensibly-named consumer application exhibits. Either the templates over-qualify everywhere, or they under-qualify and only fail in applications that happen to collide.

---

## Retro-chartered artefacts

Things the build forced into existence that no charter planned. Recorded so they do not become unexplained requirements later.

| Artefact                                                                          | Why it exists                                                                                                         | Keep?                                                                                                                     |
| --------------------------------------------------------------------------------- | --------------------------------------------------------------------------------------------------------------------- | ------------------------------------------------------------------------------------------------------------------------- |
| `Wolverine.Reference.Tests.slnx` (a `.slnx`, not a `.sln`)                        | Assumption a8 was never answered, so a separate solution was used to sidestep it                                      | Decide with a8                                                                                                            |
| `extern alias Sub` + `Sub::` usings, `Aliases="Sub"` on three `ProjectReference`s | The subscriber compiles its own copy of the publisher's message contract under the same namespace and type name       | Keep — Wolverine matches by full type name across the wire, so this is correct, but it must be documented, not incidental |
| `ObservingHandler<T>` / `AlwaysThrowingHandler<T>` test doubles                   | The shipped handler body is a generated `throw new NotImplementedException` stub, so there is no real logic to invoke | Keep, but they must sit behind the real Consumer rather than in place of it                                               |
| `.Api` projects absent from the test `.slnx`                                      | Never needed, because the fixture bypasses the real host                                                              | Remove the bypass; the `.Api` projects must be in scope                                                                   |

---

## Decisions — settled

**D1 — Rename the applications off the `Wolverine.*` root? NO.** Developer decision, quoted: _"no do not rename Wolverine as test app. keep it."_ The recommendation had been to rename, following the `N_ServiceBus.*` precedent; that recommendation is not carried, and the application names stay as they are.

Consequence, stated plainly rather than smoothed over: **G0 #8 does not pass — it is descoped with the sign-off above**, and the dossier's scorecard records it as a descope rather than a pass. Weakening the criterion to make it green would defeat the point of having it.

The residual risk is specific. The module's templates will be calibrated against a sample whose root namespace collides with `WolverineFx`'s own, so a template copying the sample's name-resolution form verbatim may over-qualify in every application, or under-qualify and fail only in applications that happen to collide. Ledger row **n3** is the mitigation, and it is cheap: take this sample's `WolverineEventingConfiguration` shape verbatim into a probe project whose root namespace does _not_ begin with `Wolverine`, and record which form compiles in both. That tells the templates what to emit without renaming anything, and closes the row with an artefact.

**D2 — One gate-sized sample, or split? ONE.** Developer decision, quoted: _"keep one sample."_ This matches the recommendation. One sample covers the eventing pattern family.

The surface still pushes at the skill's L-split tripwires, so the variant table above is what holds it to a gate-sized shape: three variants at full runtime depth, three explicitly compile-only. If the compile-only work uncovers a diverging error-handling or dead-letter shape, R16.10 promotes that transport rather than quietly widening this sample.

---

## Runbook — reproduce the runtime verification

No test project, no Testcontainers. Three terminals and about two minutes.

**1. Broker.** The default `appsettings.json` already points at `localhost:5672` with `guest`/`guest`, so nothing needs configuring.

```
docker run -d --name wolv-rabbit -p 5672:5672 -p 15672:15672 rabbitmq:3.13-management
```

**2. Subscriber.** Its real host, which is the point — this is `Program.Main`, not a test-constructed host.

```
dotnet run --project Tests/Wolverine.Subscribe.RabbitMQ/Wolverine.Subscribe.RabbitMQ.Api
```

Expect in its log: `Searching assembly Wolverine.Subscribe.RabbitMQ.Infrastructure ... for Wolverine message handlers`, the `Declared Rabbit MQ queue`/`binding` lines, then `Started message listening at rabbitmq://queue/...` for both queues. If the discovery line names only the `.Api` assembly, `Discovery.IncludeAssembly` has been lost and nothing will ever be consumed.

**3. Publish.** Trigger `ShipOrderCommand` on the publisher (it is modelled with a Publish Integration Event association, so `ShipOrderCommandHandler` publishes `OrderShippedEvent` and `MessageBusPublishBehaviour` flushes it). The publisher currently exposes no HTTP endpoint, so either add one or drive the bus from a short console host that calls `WolverineEventingConfiguration.ConfigureRabbitMq` and resolves the contracts `IMessageBus`. **Run it as its own process** — see the CS0433 constraint above; co-hosting it with the subscriber cannot work.

**4. What success looks like.** The subscriber logs `Failed to process message ... System.NotImplementedException: Implement your handler logic here...` with a stack running through `Internal.Generated.WolverineHandlers.*` → `<Message>Consumer` → the hand-written handler. That exception **is** the pass: the handler bodies are still generated stubs, so reaching one proves transport, Consumer, DI resolution and handler dispatch with no test double anywhere. Then watch the retry cadence match the configured delays, and:

```
docker exec wolv-rabbit rabbitmqctl -q list_queues name messages consumers
```

Once retries are exhausted, `wolverine-dead-letter-queue` holds the messages and the working queues return to 0.

**5. Tidy up.** `docker rm -f wolv-rabbit`

## What approval of this charter authorises

Fixing the sample, in this order. No module work, no requirements or design edits.

1. Protect the `Program.cs` host registration (confirmed stripped — see the dossier's G0 #5 entry)
2. Add the Consumer discovery registration (C12 / a2 / n1)
3. ~~Rename~~ — descoped by D1. Replaced by the n3 probe, which is what now carries G0 #8's risk
4. Split durability out so Outbox=None and Transport=Local have their own variants (C7, T-6, T-7)
5. Re-point the test fixture at the real host (G0 #2)
6. Add T-4, make T-3 observe the Error Queue (G0 #3)
7. Close the ledger rows with artefacts (a1, a8, a9, a11, n1, n2, n3)
8. Produce the licence inventory (G0 #9)
9. Re-run G0, write the dossier, commit and tag, bring the scorecard back for approval

---

# Addendum — 2026-08-24: charter staleness and three settled decisions

Recorded during an aborted `/sdd-requirements` run. The developer chose to gate the sample before
any requirements work, so this addendum carries forward what that run established rather than
letting it die with the conversation.

## Why the body above can no longer be read as current

Everything from `# Sample Charter` down to `## Decisions — settled` describes a sample that no
longer exists on disk. Verified against the working tree at `0bc5da7eb0`:

| Charter says                                                     | On disk today                                                                       |
| ---------------------------------------------------------------- | ----------------------------------------------------------------------------------- |
| `Tests/Wolverine.Publish.RabbitMQ`, `Tests/Wolverine.Subscribe.RabbitMQ` | `Tests/WolverineEventing.Publish.RabbitMQ`, `Tests/WolverineEventing.Subscribe.RabbitMQ` |
| `Tests/Wolverine.Reference.Tests` deleted by D3                  | Still absent — confirmed                                                            |
| Pinned runtime .NET 8                                            | `net10.0`; WolverineFx and WolverineFx.RabbitMQ both 5.39.5                         |
| D1: do NOT rename off the `Wolverine.*` root                     | Reversed — apps renamed to `WolverineEventing.*` at `1c5b23728c` / `02aa98e80b`     |
| G0 #8 namespace pre-check fails, descoped                        | The rename resolves it; no `extern alias` remains anywhere                          |
| Sample scaffolded on MediatR-style dispatch                      | `Intent.Application.Wolverine` 1.0.3-pre.0 — Wolverine CQRS dispatch throughout      |
| A generated `<Message>Consumer` sits between transport and handler | No Consumer class exists; see D4                                                     |

Commits that moved it: `02aa98e80b`, `ddabc452d8`, `5cc223a3b4`, `98b4b4df2a`, `4c474f0478`.

Ledger row `n3` and Decision D1 are therefore **closed by the rename**, not by the probe D1
proposed. G0 #8 should now be re-checked as a pass rather than carried as a descope.

## D4 — Is there a generated Consumer class? NO.

Developer decision: the `IIntegrationEventHandler<T>` implementation **is** the Wolverine handler.

This is not a preference; the sample proves the alternative is broken. Each handler is named
`<Message>Handler` and exposes `HandleAsync(TMessage, CancellationToken)`, which is exactly the
signature Wolverine invokes. Interposing a generated Consumer makes Wolverine find two handlers for
one message, and its runtime codegen emits the same local variable twice — CS0128, codegen fails,
no handler runs, messages are dropped in silence. Cited at
`WolverineEventing.Subscribe.RabbitMQ.Infrastructure/Eventing/WolverineEventingConfiguration.cs:38-45`.

Consequence: `requirements.mdx` R5.1, R5.2 and R5.5 are **falsified** as written and must be
re-derived once this gate clears. R5.3, R5.4, R5.6 and R5.7 survive.

## D5 — Who owns handler discovery? A COMMON MODULE DISABLES IT; EACH MODULE REGISTERS ITS OWN TYPES.

Developer decision, quoted: _"common module disables conventional discovery and the relevant
wolverine module introduces their relevant types"_.

This is a **change the sample must absorb before it can be gated.** As it stands the eventing
configuration does both jobs — it calls `DisableConventionalDiscovery()` and then registers the CQRS
module's handlers as well as its own:

- `WolverineEventing.Publish.RabbitMQ.Infrastructure/Eventing/WolverineEventingConfiguration.cs:49-54`
  registers `ShipOrderCommandHandler`, `RequestOrderProcessingCommandHandler`, `FailOrderCommandHandler`
  — all three are CQRS handlers owned by `Intent.Application.Wolverine`.
- `WolverineEventing.Subscribe.RabbitMQ.Infrastructure/Eventing/WolverineEventingConfiguration.cs:52-57`
  registers the three eventing handlers, which is correctly its own business.
- `Intent.Application.Wolverine` meanwhile still emits
  `opts.Discovery.IncludeAssembly(typeof(ICommand).Assembly)` in
  `Infrastructure/Configuration/WolverineConfiguration.cs`, which the eventing config's
  `DisableConventionalDiscovery()` then defeats — the two are in direct conflict inside one
  `UseWolverine` callback in `Program.cs`.

Target shape to build and re-verify:

1. A common Wolverine module owns the single host configuration and is the only caller of
   `DisableConventionalDiscovery()`.
2. `Intent.Application.Wolverine` registers its own CQRS handler types explicitly, replacing
   `IncludeAssembly`.
3. The eventing module registers only the eventing handler types.

## D6 — How much surface must the sample carry? THE FULL SURFACE.

Developer decision: keep all four Transports and both Transactional Outbox modes in scope rather
than narrowing to what is proven today.

The gating consequence is concrete and unavoidable: `/sdd-design` operates under a verbatim-API
rule, so every framework call a template will emit must already appear in a committed sample file.
The sample currently contains **no** Durable outbox call site, and no Azure Service Bus, Amazon SQS
or Local transport registration. Requirements R6.3, R6.5, R6.6, R6.8 and the non-RabbitMQ half of
R2.2 have nothing to cite. Those variants must be added — compile-only depth is sufficient for the
three extra transports per assumption a10, but a Durable-outbox call site has to exist.

## Open items this run did not settle

Left for the gate, not dropped:

- What flushes the Message Bus in an application that does **not** use Wolverine CQRS dispatch. The
  sample relies on `Intent.Application.Wolverine`'s `MessageBusFlushMiddleware`
  (`Infrastructure/Dispatch/Middleware/MessageBusFlushMiddleware.cs`) to call
  `FlushAllAsync`. A MediatR application installing only the eventing module has nothing that does.
- The `Program.cs` host registration is hand-written inside a file carrying
  `[assembly: DefaultIntentManaged(Mode.Fully)]` — a duplicated `using ... Infrastructure.Eventing;`
  at `WolverineEventing.Subscribe.RabbitMQ.Api/Program.cs:12-13` is the hand-edit fingerprint. The
  Software Factory will strip it. This is the charter's own G0 #5 item, still open.
- `WolverineMessageBus` is registered in the publisher's `AddInfrastructure` but not the
  subscriber's. Intended asymmetry or an omission — undecided.
- The subscriber-queue naming convention the sample uses for a fanned-out Integration Event,
  `{application-name-kebab}-{message-name-kebab}`, appears in no requirement. R3 covers the exchange
  name only.


---

# Addendum II — 2026-08-24: D3/D6 reconciled, and the descope register

## D3 and D6 do not conflict — they scope different things

Recorded because the first reading of this pair treated them as contradictory, and they are not.

- D6 scopes the **module's shipped surface**: four Transports, both Transactional Outbox modes.
- D3 scopes the **sample's proof surface**: one path, RabbitMQ with Outbox None, proven by hand.

A module may ship more than its sample proves. What it may not do is let the *spec* assert what
nothing supports. Gate criterion 4 is therefore not "every shipped default has a runtime variant"
but **citable surface**: every capability the design quotes verbatim must exist somewhere
committed — a full variant, a compile-only variant, or a probe.

## Descope register — each descope with the consequence it imposes

The rule this table exists to enforce: a descope whose consequence is not written down comes back
later as an approved requirement nobody can satisfy.

| Descoped | Decision | Consequence the spec inherits |
| --- | --- | --- |
| Automated tests, entire T-1..T-7 list (gate criteria 2 and 3) | D3 | **The spec may assert generated SHAPE only.** No acceptance criterion in `requirements.md` may claim that a message is delivered, retried, dead-lettered, or handled. Criteria must be phrased against generated code and configuration — what is emitted, where, containing which registration — never against runtime behaviour. This binds every later wave |
| Runtime proof for Transactional Outbox Durable | D3 + D6 | Design may cite the SQL Server and PostgreSQL durability APIs from the committed probe. No criterion may claim a message is dispatched if and only if the transaction commits |
| Runtime proof for Azure Service Bus, Amazon SQS, Local | a10, D3 | Design may cite their registration APIs from the committed probe. No criterion may claim delivery on those transports |
| Runtime proof for Broker Topology = Externally owned | D3 | Design may cite the non-declaring registration form from the probe. No criterion may claim it joins a pre-provisioned estate successfully |
| A third and fourth sample application for outbox variants | D3 | Replaced by the compile-only probe, which is cheaper and which criterion 7 accepts as closing evidence |

Manual real-host verification against docker RabbitMQ was performed and is recorded in the body of
this charter. It is retained as **context**, not as an oracle: nothing re-runs it, so no acceptance
criterion may cite it.

## Known deviation — the sample registers another module's handlers

`WolverineEventingConfiguration.ConfigureHandlerDiscovery` in the **publisher** registers
`ShipOrderCommandHandler`, `RequestOrderProcessingCommandHandler` and `FailOrderCommandHandler`
(`WolverineEventing.Publish.RabbitMQ.Infrastructure/Eventing/WolverineEventingConfiguration.cs:49-54`).

All three are CQRS handlers owned by `Intent.Application.Wolverine`, not by the eventing module.
They are registered there only because `DisableConventionalDiscovery()` is called in the same method
and would otherwise strand them.

**This is a deviation, not intended shape.** `/sdd-design` must not transcribe it into the eventing
module's templates. The intended shape is D5: a common module owns `DisableConventionalDiscovery()`,
and each Wolverine module registers its own handler types. The sample demonstrates the *effect* D5
requires while attributing it to the wrong owner, because no common module exists yet to attribute
it to.

## Gate fixes applied on 2026-08-24

| Fix | Files | Result |
| --- | --- | --- |
| Protected the eventing host registration with `//IntentIgnore` (criterion 5b) | `*.Api/Program.cs`, both applications | Software Factory re-run: **0 changes** in both applications |
| Removed the duplicated `using ... Infrastructure.Eventing;` | `*.Api/Program.cs`, both applications | CS0105 gone; both solutions build with 0 errors |
| Moved the retry-probe explanation inside the merge-managed body | `FailingOrderEventHandler.cs` | Survives regeneration |
| Added a compile-only probe for Durable outbox and the three uncovered transports | `golden-sample/probes/DurableAndTransportProbe/` | Builds clean; closes ledger row a1 and gate criterion 4 |

## Ledger closure — 2026-08-24

Gate criterion 7 requires every row closed with an artifact or descoped with sign-off.
"Investigated" is not closure; a file path is.

| Row | Was | Now | Artifact or reason |
| --- | --- | --- | --- |
| a1 | Partial — only 4 of 7 WolverineFx packages verified at 5.39.5 | **CLOSED** | `probes/DurableAndTransportProbe/` references and compiles against all seven at 5.39.5 |
| a2 | Closed but split | **CLOSED, superseded by D4** | No Consumer exists. Explicit `Discovery.IncludeType<T>()` in both `WolverineEventingConfiguration.cs` |
| a8 | Open — does an Intent-managed `.sln` tolerate a foreign test project | **DESCOPED** | No test project exists (D3). The probe is a standalone project outside every Intent-managed solution, so the question no longer arises |
| a9 | Open — is the ActivitySource named "Wolverine"; all of R11 rests on it | **CLOSED** | `probes/DurableAndTransportProbe/TelemetryProbe.cs`. `Wolverine.Runtime.WolverineTracing.ActivitySource.Name` is exactly `Wolverine` at 5.39.5, confirmed by reflection. A template emits `AddSource("Wolverine")` |
| a11 | Open — is Durable + Local coherent; is R2.5's loss claim wrong | **DESCOPED** | A behavioural claim, and runtime proof is descoped. R2.5 must be re-phrased as generated shape or dropped; it may not assert message loss |
| n1 | Open — does the real host reach Consumers in `*.Infrastructure` | **CLOSED, obsolete** | No Consumers exist. Handlers live in `*.Application` and are registered explicitly by type, so assembly scanning is not relied on |
| n2 | Open — does an exhausted message land on the Error Queue | **DESCOPED** | Behavioural. No criterion may assert it |
| n3 | Open — does the `Wolverine.*` root namespace cause resolution problems | **CLOSED** | The rename to `WolverineEventing.*` removes the collision. Gate criterion 8 now passes |
| Addendum I — host registration unprotected | Open | **CLOSED** | `//IntentIgnore` in both `*.Api/Program.cs`; Software Factory re-run reports 0 changes in both applications |
| Addendum I — flush path outside Wolverine CQRS | Open | **CLOSED by precedent** | The flush seam is per-dispatch-mechanism, not per-eventing-provider. A MediatR application already gets `Application/Common/Behaviours/MessageBusPublishBehaviour.cs` calling `FlushAllAsync` (shipped precedent in `Tests/Publish.CleanArch.MassTransit.OutboxNone.TestApplication`); a Wolverine-CQRS application gets `MessageBusFlushMiddleware`. The eventing module supplies the bus; the dispatch module supplies the flush |
| Addendum I — `WolverineMessageBus` registered in publisher only | Open | **CLOSED, intended** | The subscriber publishes nothing, so it needs no `IMessageBus`. The module must register the implementation exactly when the application has something to publish or send, not unconditionally |
| Addendum I — subscriber queue naming convention unstated | Open | **Reclassified** | Not an unknown but a Scope A input. The sample's convention is: Integration Event exchange = message name kebab-cased; subscriber queue = `{application-name-kebab}-{message-name-kebab}`; Integration Command queue = message name kebab-cased, shared by every sender. Cited at `WolverineEventing.Subscribe.RabbitMQ.Infrastructure/Eventing/WolverineEventingConfiguration.cs:20-25` |

Open rows remaining: **none.**
