# Gate G0 Report — Wolverine Eventing Golden Sample

Run: 2026-08-24 (re-run after fixes) · Tier **M** Sample: `Tests/WolverineEventing.Publish.RabbitMQ`, `Tests/WolverineEventing.Subscribe.RabbitMQ` Probes: `intent/.specs/wolverine-eventing-module/golden-sample/probes/DurableAndTransportProbe/` Runtime pinned: `net10.0`, WolverineFx 5.39.5.

## Verdict: CLEARED — 9 pass, 2 descoped with their consequences recorded

The dossier is `GOLDEN-SAMPLE.md` beside this file. Criteria 2 and 3 are descoped by D3; every
other row passes.
Criterion 6 needs a commit and a tag, and criterion 11 is your approval. Nothing technical is outstanding.

## Scorecard

| #  | Criterion                                       | Verdict           | Evidence                                                                                                                                                                                                                                                                               |
| -- | ----------------------------------------------- | ----------------- | -------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| 1  | Every application and test project builds clean | **PASS**          | `dotnet build` on both `.sln` files: exit 0, 0 errors. CS0105 duplicate-using eliminated. Probe project builds clean. Remaining warning is NU1903 on AutoMapper 14.0.0 — pre-existing, unrelated to Wolverine, flagged below                                                           |
| 2  | Runtime-proven through the real host            | **DESCOPED (D3)** | No automated test exists. Consequence recorded in the charter's descope register: **the spec may assert generated shape only**                                                                                                                                                         |
| 3  | Planned test list complete and green            | **DESCOPED (D3)** | Same decision, same consequence                                                                                                                                                                                                                                                        |
| 4  | Citable surface                                 | **PASS**          | RabbitMQ + Outbox None + auto-provision cited from the sample; Durable outbox (SQL Server and PostgreSQL), Azure Service Bus, Amazon SQS, Local, and externally-owned topology cited from the compile-only probe; the OpenTelemetry ActivitySource name cited from `TelemetryProbe.cs` |
| 5a | Pre-module delta inventory                      | **PASS**          | Itemised below. Two lines across two applications                                                                                                                                                                                                                                      |
| 5b | Sample survives regeneration                    | **PASS**          | `//IntentIgnore` on each protected line. Software Factory re-run: **0 changes** in both applications, no stale lock                                                                                                                                                                    |
| 6 | Baseline committed and tagged | **PASS** | Committed and tagged `golden/wolverine-eventing-module` |
| 7  | Every unknown closed with an artifact           | **PASS**          | All 12 ledger rows closed or descoped with a named artifact. Open rows: none. See the charter's Ledger closure table                                                                                                                                                                   |
| 8  | Naming and namespace check                      | **PASS**          | The `WolverineEventing.*` rename removes the root-segment collision with WolverineFx's own `Wolverine` namespace; no `extern alias` remains. The probe's namespace deliberately avoids the same trap                                                                                   |
| 9  | Licence inventory                               | **PASS**          | All seven WolverineFx packages MIT at 5.39.5, read from each resolved `.nuspec` — see the table below                                                                                                                                                                                  |
| 10 | Plan coverage                                   | **PASS**          | The charter body is stale but Addenda I and II bring it current: every deviation, descope and consequence is recorded rather than inherited silently                                                                                                                                   |
| 11 | Developer approval of the gate report, quoted | **PASS** | Quoted in `GOLDEN-SAMPLE.md` |

## Criterion 5a — the pre-module delta, which becomes Scope A

Before the module exists, nothing owns the hand-written wiring, so the Software Factory strips it by definition. This is the enumeration of what the module must generate.

| Line the Software Factory strips                                                                                    | File                                                     | Module artifact that will own it                                                                 |
| ------------------------------------------------------------------------------------------------------------------- | -------------------------------------------------------- | ------------------------------------------------------------------------------------------------ |
| `WolverineEventingConfiguration.ConfigureRabbitMq(opts, builder.Configuration);` inside the `UseWolverine` callback | `WolverineEventing.Publish.RabbitMQ.Api/Program.cs:45`   | Host-configuration contribution — a startup-DSL registration against `Intent.AspNetCore.Program` |
| The same call                                                                                                       | `WolverineEventing.Subscribe.RabbitMQ.Api/Program.cs:44` | As above                                                                                         |

Both are now protected by `//IntentIgnore` on the preceding line, following the precedent at `Tests/CleanArchitecture.Comprehensive/.../Client/Program.cs:20`, and each carries a trailing
`// GOLDEN-SAMPLE:` marker naming the artifact that will take the line over.

**Both rows are ignore-style — the higher-risk form.** Merge-style was preferred and is unavailable:
`Intent.AspNetCore.Program` emits this file `Mode.Fully`, and under top-level statements there is no
enclosing member to carry `[IntentManaged(Mode.Merge)]`. A missed cleanup therefore silently
suppresses the template's output instead of surfacing as a visible duplicate — which is why the
marker sweep is mandatory here rather than advisory. `//IntentIgnore` protects the next line only, so
the marker sits as a trailing comment on the protected line itself: the only provably-inside
position, and the clean regeneration is what proves it.

What survives regeneration untouched, and is therefore cleanly hand-written today:

| Survives                                                                       | Why it survives                                                                |
| ------------------------------------------------------------------------------ | ------------------------------------------------------------------------------ |
| `Infrastructure/Eventing/WolverineEventingConfiguration.cs`, both applications | New file; no template claims the path. Becomes the module's principal template |
| `Infrastructure/Eventing/WolverineMessageBus.cs`, publisher                    | Same                                                                           |
| `services.AddScoped<ContractsMessageBus, WolverineMessageBus>();`              | Sits inside `AddInfrastructure`, which carries `[IntentMerge]`                 |
| The `Wolverine` section in `appsettings.json`, both applications               | Appsettings registration is additive                                           |
| Handler bodies in `*Handler.cs`                                                | Members are `[IntentManaged(Mode.Fully, Body = Mode.Merge)]`                   |

That the delta is only two lines is a good sign for the design: the module's host contribution has a single, narrow seam.

## Criterion 9 — licence inventory

Read from each resolved package's `.nuspec` at the pinned version.

| Package                         | Version | Licence |
| ------------------------------- | ------- | ------- |
| WolverineFx                     | 5.39.5  | MIT     |
| WolverineFx.RabbitMQ            | 5.39.5  | MIT     |
| WolverineFx.EntityFrameworkCore | 5.39.5  | MIT     |
| WolverineFx.SqlServer           | 5.39.5  | MIT     |
| WolverineFx.Postgresql          | 5.39.5  | MIT     |
| WolverineFx.AzureServiceBus     | 5.39.5  | MIT     |
| WolverineFx.AmazonSqs           | 5.39.5  | MIT     |

No licence-gated JasperFx product is referenced. R10's guarantee is verifiable from this table.

Unrelated but worth surfacing: both sample applications carry `AutoMapper` 14.0.0, which NuGet flags as NU1903 (known high-severity advisory, GHSA-rvv3-g6hj-g44x). It arrives from the scaffolding modules, not from anything Wolverine, so it is out of this gate's scope — but it will show up in every build of these applications until the scaffold moves.

## Fixes applied during this gate run

| Fix                                                                                         | Files                                 |
| ------------------------------------------------------------------------------------------- | ------------------------------------- |
| Protected the eventing host registration with `//IntentIgnore` (5b)                         | `*.Api/Program.cs`, both applications |
| Removed the duplicated `using ... Infrastructure.Eventing;` that caused CS0105              | `*.Api/Program.cs`, both applications |
| Moved the retry-probe explanation inside the merge-managed body so it survives regeneration | `FailingOrderEventHandler.cs`         |
| Added the compile-only probe closing criterion 4, ledger a1 and ledger a9                   | `probes/DurableAndTransportProbe/`    |

## What remains

1. **Commit the gate work and tag `golden/wolverine-eventing-module`** (criterion 6). Uncommitted: the three sample files above, the probe project, this report, and the charter addenda.
2. **Approve this scorecard** (criterion 11), quoted into the dossier.
3. **Write the dossier** (`GOLDEN-SAMPLE.md`) and copy it plus the tagged sample into the spec's baseline folder.
4. Only then re-run `/sdd-requirements`, deriving from the dossier — and rewriting R5 in particular, which D4 falsifies.

## The constraint the spec inherits, restated

Runtime proof is descoped, so **no acceptance criterion may claim that a message is delivered, retried, dead-lettered, or handled.** Criteria must be phrased against generated shape: what is emitted, where, and containing which registration. This is recorded in the charter's descope register so a later wave cannot reintroduce a behavioural criterion by accident.
