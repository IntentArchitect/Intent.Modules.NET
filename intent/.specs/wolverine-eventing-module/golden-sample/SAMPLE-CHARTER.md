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

## Test list

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

## Decisions blocking approval

**D1 — Rename the applications off the `Wolverine.*` root?** Recommendation: **yes, and now.** It is the correct fix for G0 #8, it follows the `N_ServiceBus.*` precedent this repository already set, and it churns every file in both applications — which gets strictly more expensive with every file added. The alternative is a recorded descope with sign-off, accepting that the templates are calibrated against unusual name resolution.

**D2 — Does this stay one gate-sized sample, or split?** The surface is four transports x two outbox modes x two topology modes, plus coexistence with `Intent.Application.Wolverine`, plus telemetry and tenancy variants. That is pushing at the skill's L-split tripwires (more than ~3 generated-file pattern families; more than ~12 hand-written files). Recommendation: keep **one** sample for the eventing pattern family, and split the shared host module (`Intent.Wolverine.Common`) coexistence proof into its own later loop — it has a different pattern family and a different consumer, and it does not need to be inside this gate.

Neither decision is mine to take. Both change what gets built next, so approval of this charter should carry an answer to each.

---

## What approval of this charter authorises

Fixing the sample, in this order. No module work, no requirements or design edits.

1. Protect the `Program.cs` host registration (confirmed stripped — see the dossier's G0 #5 entry)
2. Add the Consumer discovery registration (C12 / a2 / n1)
3. Rename, if D1 says rename — cheapest while the sample is small
4. Split durability out so Outbox=None and Transport=Local have their own variants (C7, T-6, T-7)
5. Re-point the test fixture at the real host (G0 #2)
6. Add T-4, make T-3 observe the Error Queue (G0 #3)
7. Close the ledger rows with artefacts (a1, a8, a9, a11, n1, n2, n3)
8. Produce the licence inventory (G0 #9)
9. Re-run G0, write the dossier, commit and tag, bring the scorecard back for approval
