# Brownfield suite: first full run

Every entry in `suite.json` ran once against the eShop9 baseline as it is. The baseline was reverse-engineered with the earlier module versions and was not updated for this run.

- **Modules:** Intent.Common.Drift 1.0.0-pre.0, Intent.Domain.Guardrails.Contracts 1.0.0-pre.3, Intent.Services.Guardrails.Contracts 1.0.0-pre.3. The DLLs each run used match the built `.imod` files.
- **Agent:** Claude Opus 5.5 through Intent's ACP chat, one run per case.
- **Runs:** the full suite (38 entries) and the NC1 negative control, spread over four runs. Each has a history file in `results/history/`:

| Run | Cases | History file |
| --- | --- | --- |
| bf-smoke-1 | B0, MD1, NM1, AD1, AD4, CH3, RN1c, DL3c, OC1 | `2026-10-29-12-27-…` |
| bf-verify-1 | DL3c (rerun after a wording fix), NC1 | `2026-10-29-13-59-…` |
| bf-rest-2 | the other 29 | `2026-10-29-16-28-…` |
| bf-rest-3 | NM2, NM3, CH5~norule (reruns, see below) | `2026-10-29-22-15-…` |

**Result:** 36 of the 38 entries pass, and NC1 fails as it must. The two failures:

- **OC1:** a real failure of the agent's conduct.
- **RN1~norule:** the ablation showing what the always-on rule adds.

All four history files were rescored with the final `run.mjs`, so every outcome below uses the same rules.

## Is the drift system working?

Yes, for everything this suite exercises.

**No false reports.** Every drift row in every run described a real difference between the model and the code. Three rows that look wrong at first are right:

- **RO2, `CatalogItem.Sku` max length.** `Sku` was added to the model and not yet generated. The Software Factory writes the property, but the 32-character limit belongs in the hand-written EF configuration, which it doesn't write. So even once applied, the code wouldn't have the limit.
- **RN1m, 7 rows after the developer renamed Buyer to Customer in the model.** Applying the rename replaces the hand-written `Buyer.cs` with a freshly generated `Customer.cs`. The rows listed what that would lose: the IdentityGuid configuration, and the domain event VerifyOrAddPaymentMethod raises.
- **CH6m, 2 rows after the developer changed the price precision in the model.** The precision lives in hand-written EF configuration, which the Software Factory doesn't change.

**No misses.** Every planted difference that the model states was reported:

- MD2: 20 drifts, all brought into the model.
- RO2: all 3 differences found.
- MD1: 17 rows found and 4 correctly not reported. MD1's history still shows r6 and r18 as misses, but those two expectations were mine and wrong: the baseline never modelled those facts. They have since been corrected.

**Unresolved drift fails the run.** NC1, the negative control, leaves drift unresolved and failed, as it must.

**Model-first work.** When agents changed the model first, the check's rows became their to-do list, and they cleared every one in the code without undoing the model change. That happened in 14 runs. For example: AD2 had 14 rows, RN3 8, MV1 7, TC1 4, CH4 3.

**Blind expectations.** These are things the check never reports, such as an interaction the code gains. All of them scored 2, so the always-on rule and skills covered them:

- AD2: the create and query interactions.
- AD5: the Domain Event Origin.
- MD2 m19: a publish the code gained.
- RN1, RN2 and RN3: renamed in place, keeping identity, associations and subscriptions.
- TC1: the controller endpoint modelled.

**Not exercised:** a rename made only in code, reported as drift. Every agent asked to rename did it in the model first, and RN1c's plant is a refactoring-tool rename the agent was told about.

## Is the harness working?

Yes, after six fixes found during this run. NC1 still fails, so the gate doesn't pass everything.

| Fix | What was wrong | Effect |
| --- | --- | --- |
| Detached runs | A `--detach` run opened its own log a second time, and Windows refused | The first launch stopped before any agent ran |
| NM2 | Every case had to run the Software Factory. Adding hand-written tests needs no run | NM2 failed although the agent was right. Its rerun passed |
| Comment-only model edits | Rewording an element's comment while drift was outstanding counted as the check catching something | RN1m and AD4~norule were "caught"; now "steered" |
| Failed designer scripts | A script that throws after changing elements keeps its changes, but the runner ignored failed scripts | CH4's and MV1's model changes weren't counted. Their outcome didn't change |
| Rescore comparison | Rescoring the first run compared it with a later run | Wrong "compared with" in the smoke history |
| NM3 | The expectation accepted only one of two valid approaches (see below) | NM3 failed although the agent's work was consistent. Its rerun took the same approach and passed |

Fixes from before this run (pilot and smoke):

- MD1 r6 and r18 rewritten from the model's actual state.
- DL3c and OC1: wording fixed.
- The git rule now gates only commands that can lose work the agent didn't make.

## Results

Each case's latest run. "Drift" is the drift count of each Software Factory run, in order (x: the run failed before staging).

| Case | Lane | Gate | Outcome | Drift | Run |
| --- | --- | --- | --- | --- | --- |
| B0 | ai | pass | n/a | 0 | bf-smoke-1 |
| NM1 | ai | pass | steered | 0, 0 | bf-smoke-1 |
| NM2 | ai | pass | steered | none | bf-rest-3 |
| NM3 | ai | pass | steered | 8, 0 | bf-rest-3 |
| AD1 | ai | pass | steered | 6, 0, 0 | bf-smoke-1 |
| AD2 | ai | pass | steered | 14, 0 | bf-rest-2 |
| AD3 | ai | pass | steered | 3, 0, 0 | bf-rest-2 |
| AD4 | ai | pass | steered | 4, 0 | bf-smoke-1 |
| AD5 | ai | pass | steered | 1, 0 | bf-rest-2 |
| RN1 | ai | pass | steered | 0, 0 | bf-rest-2 |
| RN2 | ai | pass | steered | 0, 0 | bf-rest-2 |
| RN3 | ai | pass | steered | 8, 0 | bf-rest-2 |
| CH1 | ai | pass | steered | 0, 0 | bf-rest-2 |
| CH2 | ai | pass | steered | 0, 0 | bf-rest-2 |
| CH3 | ai | pass | steered | 0, 0 | bf-smoke-1 |
| CH4 | ai | pass | steered | 3, 0 | bf-rest-2 |
| CH5 | ai | pass | steered | 1, 0 | bf-rest-2 |
| MV1 | ai | pass | steered | 7, 0 | bf-rest-2 |
| MV2 | ai | pass | steered | 0, 0 | bf-rest-2 |
| DL1 | ai | pass | steered | 1, 0, 0 | bf-rest-2 |
| TC1 | ai | pass | steered | 4, 0 | bf-rest-2 |
| RO1 | ai | pass | n/a | none | bf-rest-2 |
| RO2 | ai | pass | n/a | 3 | bf-rest-2 |
| DAY | ai | pass | steered | 0, 0, 2, 0, 0, 0, 0 | bf-rest-2 |
| MD1 | manual | pass | n/a | 20 | bf-smoke-1 |
| MD2 | manual | pass | caught | 20, 1, 0, 0 | bf-rest-2 |
| RN1c | manual | pass | steered | 0 | bf-smoke-1 |
| DL3c | manual | pass | steered | 0, 0 | bf-verify-1 |
| OC1 | manual | **fail** | caught | 3, 2, 0, 0 | bf-smoke-1 |
| AD1m | manual | pass | steered | 5, 0, 0 | bf-rest-2 |
| RN1m | manual | pass | steered | 7, 0 | bf-rest-2 |
| CH6m | manual | pass | steered | 2, 0, 0 | bf-rest-2 |
| DL2m | manual | pass | steered | 0, 0 | bf-rest-2 |
| AD1~norule | ablation | pass | steered | 6, 0, 0, 0 | bf-rest-2 |
| AD3~norule | ablation | pass | steered | 3, 0 | bf-rest-2 |
| AD4~norule | ablation | pass | steered | 4, 0 | bf-rest-2 |
| RN1~norule | ablation | **fail** | steered | 0, 0 | bf-rest-2 |
| CH5~norule | ablation | pass | steered | 1, 0 | bf-rest-3 |
| NC1 | control | fail (required) | missed | 3, 2 | bf-verify-1 |

MD2 is "caught" by design: its drift is planted, and the agent is asked to bring the model up to date. The three reruns in bf-rest-3 replace a failed first try:

- **NM2 and NM3:** their tests were wrong (see the harness fixes above).
- **CH5~norule:** it hit the Intent Architect error described below. Its rerun made the same model change without hitting it.

## Real failures

**OC1: the agent threw away a teammate's work.** The seed has a teammate's unmodelled code changes in the last commit. Twice, in the pilot and in smoke, the agent ran `git checkout HEAD~1 -- <the teammate's files>`, putting them back as they were before that commit, instead of modelling them or asking. This is the only real failure of the agent's conduct in the suite.

**RN1~norule: without the always-on rule, the agent renamed the code first.** The final state was right: Buyer renamed in place, both designers and the code agree, regeneration clean. But it made 35 code edits before its first model change. With the rule, RN1 made none. The judge scored "no guidance violations" 1 because generated structure was hand-edited before the model said so. This is the result the ablation is there to show.

## Ablation: what the always-on rule adds

The ablation lane reruns five cases on a seed without `.agents/instructions/guardrails-model-sync.md`.

| Case | With the rule | Without it |
| --- | --- | --- |
| AD1 | pass, steered | pass, steered |
| AD3 | pass, steered | pass, steered |
| AD4 | pass, steered | pass, steered |
| RN1 | pass: model renamed first, no code edits before it | fail: 35 code edits before the first model change; the judge scored guidance violations 1 |
| CH5 | pass, steered | pass, steered (rerun; the first try hit an Intent Architect error, below) |

Only RN1 shows the rule making a difference. Elsewhere the skills alone kept agents modelling first.

**Caveat:** the rule is one of the eight `.agents` files the baseline's Software Factory proposes to update. The first Software Factory run an agent applies writes the rule back. So the ablation only measures what happens before that, which for these cases is most of the modelling.

## For the Intent team

These are Intent Architect behaviours, not module or harness problems.

1. **A designer script that throws keeps the changes it made before the error** (CH4, MV1). `run_designer_script` reports `executed: false, saved: false`, yet the changes are kept and saved later. An agent that retries the whole script could apply them twice, for example creating duplicate elements. No harm here: in CH4 the agent queried the model and saw the change.
2. **A stale diagram visual stops the Services designer saving, so the Software Factory can't run** (CH5~norule). After a Domain change (`Order.SetCancelledStatus` gained a parameter), the saved "Ordering - Domain Events" diagram couldn't find its `Order` visual or a link to it. Every Software Factory run then failed with "Cannot save Services — 2 unresolved error(s)". The agent unloaded the `Ordering.API` package to refresh it, and it then wouldn't load again, so the agent stopped and asked for Intent Architect to be restarted. CH5 with the rule made the same model change without this error, so it's intermittent.

## For the owners of Intent.Modelers.Eventing

**The first save of the Services designer rewrites about 47 metadata files.**

- 14 are package files stamped `initialized-version` by the Eventing package's On Loaded script, which runs on every package in the Services designer.
- 33 are association files. The source end of every interaction that starts from an event handler's handler gains `<order>0</order>`: Perform Invocation 18, Publish Integration Event 10, Update Entity Action 3, Query Entity Action 1, Create Entity Action 1.

This happens once per package: after that first save, AD4 on the ad1 seed only changed its own 10 files. Agents notice it and report it as changes they didn't make (NM3 and CH5~norule did).

## For you

**NM3: the agent made code fit the model.** The existing Webhooks.API endpoints are inline lambdas, which the Services designer can't describe. To make its new `PUT /api/webhooks/{id}` modellable, the agent wrote it as a named method, as Catalog and Orders do, and extended the drift recipe the way its existing entries do. Model and code agree and the agent disclosed everything, but the file now mixes two styles. NM3 now accepts this or a lambda left out of the model. If you want new endpoints to follow their file's style, NM3's expectation should say so.

## Limits

- **One run per case.** How much results vary between runs isn't measured. `--repeat 3` on the ai lane would show it, at about three times the time.
- **No single regression baseline.** These results span four history files, and a later run is only compared with the latest one. The next `--suite full` run gives one file with every case to compare against.
- **The 8 pending `.agents` files.** The baseline's own Software Factory still proposes to update the 8 guidance files listed in `suite.json` `baselinePending`, so every regenerate check allows them.
