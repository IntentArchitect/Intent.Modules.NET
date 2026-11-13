# Intent.Common.DriftEngine: phase 0 specification

Phase 0 of [the plan](../plan.md). It is done when it is signed off.

The specification gives the requirements, the interfaces between the five building blocks, and the scope model. Beside it is an acceptance suite that later phases must pass. The suite's cases are built from the 321 checks in [`check-classification.md`](../check-classification.md), the real codebases, and scope cases.

## Documents

| Document                                         | What it fixes                                                                                                             |
| ------------------------------------------------ | ------------------------------------------------------------------------------------------------------------------------- |
| [requirements.md](requirements.md)               | What the engine must do, numbered `R1`… and traced to the plan's principles and to suite cases                            |
| [claims-and-findings.md](claims-and-findings.md) | The claim, its reading and outcome, the finding, policy and severities, and the mapping onto Intent.Common.DriftReporting |
| [vocabulary.md](vocabulary.md)                   | Block 1: how any designer, element type or stereotype turns into claims                                                   |
| [code-reader.md](code-reader.md)                 | Block 2: the code graph every language reader produces, and framework readers                                             |
| [rules.md](rules.md)                             | Blocks 3 and 4: structure comparison, recipes, queries, plug-ins and opt-in "must be modelled" rules                      |
| [packs.md](packs.md)                             | The pack format: vocabulary, rules, language profiles and plug-ins in one bundle                                          |
| [scope.md](scope.md)                             | Block 5's scope, on the Codebase Structure designer                                                                       |
| [conversion.md](conversion.md)                   | How Common.Drift recipe files convert, construct by construct                                                             |
| [claim-catalog.md](claim-catalog.md)             | The 321 checks reduced to claim kinds, scope cases, opt-in rules and engine cases                                         |
| [acceptance-suite.md](acceptance-suite.md)       | The suite: case format, what runs now, what each phase must pass                                                          |
| [schemas/](schemas)                              | JSON Schemas for every data format above                                                                                  |

The suite is the test project `Modules/Intent.Modules.Common.DriftEngine.Tests`.

## Decisions taken in phase 0

User decisions are marked as such. The others follow from the plan's principles, and are open to change at sign-off.

| #  | Decision                                                                                                                                                                                                                                                                                | Source                                        |
| -- | --------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- | --------------------------------------------- |
| 1  | Scope lives on the **Codebase Structure** designer: a `Drift Scope` stereotype on the Root Folder, folders and projects, with rows of match, in or out, and reason.                                                                                                                     | User, 2026-11-12                              |
| 2  | The engine reads **Intent's model directly** (designers, elements, stereotypes, associations). There is no intermediate model format. The suite keeps its models as data: the test harness turns a fixture's model into Intent model objects, so only the tests carry that translation. | User, 2026-11-12                              |
| 3  | **Readers never build or run the code.** A framework reader reads committed files only, such as EF Core's migration `ModelSnapshot.cs` or a checked-in `openapi.json`.                                                                                                                  | User, 2026-11-12                              |
| 4  | **The model states a value only where the code states it explicitly.** A table name is modelled only when the code has a `ToTable`. Conventional values aren't modelled, so no rule re-derives a framework convention to make a modelled value pass.                                    | User, 2026-11-12                              |
| 5  | The suite is **JSON Schemas plus data fixtures**. Tests that check the fixtures pass now; tests that need the engine are skipped until phase 1 plugs it in.                                                                                                                             | User, 2026-11-12                              |
| 6  | Code-only explicit values the model doesn't state (a `ToTable` with no modelled table name) are not drift by default. They are an opt-in "must be modelled" rule (`unmodelled.value`).                                                                                                  | Principle 1, from decision 4                  |
| 7  | A modelled item missing from the code is a finding, never a pending Software Factory change.                                                                                                                                                                                            | Principles 2 and 4                            |
| 8  | Every claim with no rule is its own can't-verify finding, not one warning per kind.                                                                                                                                                                                                     | Principle 2                                   |
| 9  | Unnamed indexes are claims. Common.Drift left them out to avoid permanent can't-verify rows.                                                                                                                                                                                            | Principle 2                                   |
| 10 | Empty model values are not claims. Common.Drift stated them so code-only values were caught; that is now the opt-in rule in decision 6.                                                                                                                                                 | Principle 1                                   |
| 11 | Model-only rules (naming, a Layer needs a Path) are designer validation, not drift. The engine doesn't run them.                                                                                                                                                                        | Plan, "from module building: live validation" |
| 12 | Coverage inventories of code the model can't state are dropped.                                                                                                                                                                                                                         | Principle 1                                   |

## Open questions for sign-off

1. **Listing exclusions in the report needs a non-failing row.** Phase 1 must list excluded code in the drift report. Intent.Common.DriftReporting has only Error and Warning, so listing exclusions as warnings would mark every run "Passed with warnings". The proposal is an `Info` severity in the hub, rendered in the Area but never counted, and written to the report even when a run is otherwise clean. That changes another module (`D:\Dev\Intent.Modules\Modules\Intent.Modules.Common.DriftReporting`), so it needs your agreement.
2. **"Same results" in phase 6 means the same verdicts, apart from the listed semantic changes.** Decisions 4 and 6 to 10 change what Common.Drift reported. For example, a modelled table name with no `ToTable` passed in Common.Drift through `absent: {Entity}`, and now is contradicted. [conversion.md](conversion.md) lists each change, and phase 6's side-by-side run must explain every difference by one of them.
3. **Default severities.** The proposal is: every can't-verify outcome is an error, as contradicted is. A stale scope entry or exception is a warning. A missing reason on an Out entry is an error. See [claims-and-findings.md](claims-and-findings.md).
4. **Where a house pack lives.** The proposal is `drift/` in the folder that holds the application's `.application.config`, beside Common.Drift's `guardrails/`, committed with the code.

## Glossary

| Term        | Meaning                                                                                                                                   |
| ----------- | ----------------------------------------------------------------------------------------------------------------------------------------- |
| Claim       | One checkable statement the model makes about the code: a claim kind, the element that states it, the expected value and its placeholders |
| Claim kind  | A named kind of claim, such as `table.name` or `member.type`                                                                              |
| Vocabulary  | Pack data that turns model elements into claims (block 1)                                                                                 |
| Code graph  | What the code readers produce: files, types, members, statements, imports, calls, each with its location (block 2)                        |
| Rule        | How a claim's value is read from the code graph: a recipe, a query or a plug-in (block 4)                                                 |
| Reading     | A rule's result for one claim: a value and where it came from, or why there is none                                                       |
| Outcome     | Verified, contradicted, or can't verify with a reason                                                                                     |
| Finding     | A row in the drift report: a claim that isn't verified, or a scope, configuration or opt-in rule finding                                  |
| Scope       | Which code is read: In and Out entries on the Codebase Structure designer                                                                 |
| Pack        | A bundle of vocabulary, rules, language profiles, conventions and plug-ins for one technology, or for one codebase's house style          |
| Opt-in rule | A "must be modelled" rule a team turns on; it reports in-scope code of a kind the model doesn't describe                                  |
| Silence     | A reading taken from `default` or `absent`: the code says nothing about the claim                                                         |
