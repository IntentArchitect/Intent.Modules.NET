# Brownfield regression testing for the Guardrails modules

This proves that, in a configured brownfield app, the Intent Architect model stays aligned with the code under both kinds of development:

- **AI development.** An agent given an ordinary request keeps the model aligned. Either the steering (the always-on rule and the skills) makes it model the change, or the drift check reports what it missed and it fixes that.
- **Manual development.** A developer changes the code by hand. The drift check reports it, and an agent asked to sync the model then aligns it.

The app is eShop, adopted with the Guardrails modules (`baseline/`). The eval tool in `D:\Dev\Intent\Tools\evals` drives Intent Architect and judges each run, and is used unchanged.

The loop: change the modules, refresh the baseline with them, rerun the suite, and read what regressed.

## Two safety nets, measured separately

Some changes the drift check can never report, by design: an interaction the code gains (a new publish, entity action or raised domain event), and code that no ownership convention covers. For those, steering is the only net. So every expected model change in a case is tagged **visible** (the check would report it) or **blind** (only steering can catch it), and every AI run gets a net outcome:

| Outcome | Meaning                                                                                                                                                                                                  |
| ------- | -------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| steered | Every expected change was modelled, and the agent didn't change the model while drift was outstanding: any drift it saw (such as a modelled route not yet in the code) it resolved in the code            |
| caught  | Every expected change was modelled, and the agent changed the model while drift was outstanding: the check caught something the agent hadn't modelled                                                    |
| missed  | Something is misaligned at the end: a **visible miss** (the check could see it, so the check failed or the agent ignored or silenced a row) and/or a **blind miss** (only steering could have caught it) |

The runner reads the outcome from two places. The chat history gives each `run_software_factory` result's drift count per designer, and when designer changes were made. The end state gives `regenerate` (clean means nothing staged), plus the judge's score for each expected change.

A designer change here is a `run_designer_script` call whose result lists changed elements, even if the script then threw: Intent keeps what a script changed before its error. A script that only calls `setComment` doesn't count, since the check never reports a comment.

## What's here

| Path                                              | What it holds                                                                                                                                                                                                        | In git          |
| ------------------------------------------------- | -------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- | --------------- |
| `baseline/`                                       | The configured eShop, from `D:\Dev\Temp\eShop9`: code, `intent/`, `.agents`, and the drift memory in `intent/.cache/guardrails`. No `bin`, `obj`, `node_modules`, `artifacts`, `.git`, `.intent` or `.cache/modules` | yes (see below) |
| `plants/<name>.patch`                             | Changes applied on top of the baseline to make a case's starting point                                                                                                                                               | yes             |
| `plants/_code/<name>.json`                        | A code plant's edits, for re-making it after a baseline refresh                                                                                                                                                      | yes             |
| `plants/_setup/<name>/case.json`                  | The eval case that makes a model plant: one designer change, saved, nothing else                                                                                                                                     | yes             |
| `cases/<id>/case.json`                            | A case: prompt or turns, plants, and its expected model changes tagged visible or blind                                                                                                                              | yes             |
| `suite.json`                                      | Shared settings, lanes and suites                                                                                                                                                                                    | yes             |
| `run.mjs`                                         | The runner                                                                                                                                                                                                           | yes             |
| `results/history/<date>-<modules>.json` and `.md` | Each suite run's per-case outcome: the regression record                                                                                                                                                             | yes             |
| `results/runs/<runId>/`                           | Raw output: log, status, each run's `agent.patch` and drift reports                                                                                                                                                  | no              |
| `reports/`                                        | Findings write-ups                                                                                                                                                                                                   | yes             |

The baseline holds files that eShop's own `.gitignore` files ignore and that it needs: the drift memory under `intent/.cache`, `.mcp.json`, `build/` and `src/Identity.API/wwwroot/lib`. Commit it with `git add -f Tests/BrownfieldTesting/baseline`.

## Before the first run

1. Install the eval tool's dependencies: `cd D:\Dev\Intent\Tools\evals` then `npm ci`.
2. Sign in once for the eval tool's isolated Claude Code config, `~/.claude-intent-evals`. `node run.mjs` prints the exact command when it isn't signed in (it uses Intent's bundled Claude Code when no `claude.exe` is on `PATH`).
3. Know what a run does to your machine. The eval tool starts the dev Intent Architect instance of `D:\Dev\Intent` (built already; runs pass `--no-build`), sets your preferred chat model and the chat's permission mode in your profile, and adds each run's solution to your Recent list. Each case's solution is copied to `~/IntentEvals/<run>`. The runner deletes its build output afterwards and keeps the source.

## Running

```powershell
node run.mjs --suite smoke --detach    # B0 MD1 NM1 AD1 AD4 CH3 RN1c DL3c OC1, about 2 to 3 hours
node run.mjs status                    # the latest run: running, done, failed or dead, and each finished case
node run.mjs --suite full --detach     # every case, both lanes, plus the ablation runs; overnight
node run.mjs --lane manual             # one lane
node run.mjs --case AD1 --case AD4~norule --repeat 3
node run.mjs --suite pilot --dry-run   # what would run, and on which seeds
node run.mjs list                      # every case, its seed and title
```

Options: `--evals <dir>` (default `D:\Dev\Intent\Tools\evals`), `--modules <dir>` (default `D:\Dev\.intent-modules`; the solution resolves modules from it first), `--no-judge`, `--keep` (keep the eval tool's case and seed copies, and each run's build output).

For each case the runner:

1. Builds the case's seed, once per suite run. It copies `baseline/`, commits it to a fresh git repository, then applies and commits each plant in turn. A plant that no longer applies stops the run and names the plant.
2. Writes the eval tool's `case.json`, which is the case plus `suite.json`'s shared settings, and runs `node run.js --case brownfield-<id> --target current --no-build --modules <modules>`.
3. Reads the eval tool's `metrics.json` and chat history, and the git diff of the agent's work against the seed. It also copies the drift reports, and records the md5 of the Guardrails and drift module DLLs the run used next to the ones in the built `.imod` files.
4. Works out the gate, the net outcome, the visible and blind misses, and, for detection cases, the detection rate.
5. Writes `results/history/<date>-<modules>.json` and `.md` after every case, compares the finished run with the previous history file, and lists regressions and improvements.

### The gate

A run passes when:

- the build and the Ordering, Basket and Application unit tests pass;
- `regenerate` proposes nothing beyond `suite.json`'s `baselinePending`, except in cases that leave planted drift in place on purpose (`expectClean: false`). `baselinePending` lists what the untouched baseline already has pending: its `.agents` guidance was generated before the module build it has installed was rebuilt, so the Software Factory proposes to update it. The baseline is tested as it is; B0 flags the list when it goes stale;
- the history checks pass (`run_software_factory` was called, unless the case sets `expectSoftwareFactory: false` because its work has nothing to check, such as hand-written tests; a read-only case called no tool that writes);
- no git command that can lose work the agent didn't make was run: a `reset`, `checkout` or `restore` to a
  revision other than HEAD (such as `HEAD~1`), which undoes the committed plant. `stash`, `clean`, and a reset,
  checkout or restore to HEAD can only touch the agent's own uncommitted work, so they are listed in the report for
  review and don't fail the gate;
- the judge scored "done", every expected change, "nothing extra" and "no violations" at 2, and "conduct" at 1 or more.

A drift recipe or the always-on rule changed by the agent is flagged in the report for a person to look at; the judge's "no violations" criterion decides whether it was a weakening.

### Reading a result

The history `.md` has a table of every case, followed by the regressions and improvements against the previous run, and then each case's detail. A **regression** is any of: a lower gate pass rate, a worse net outcome (steered to caught, or caught to missed), a new visible or blind miss, a lower detection rate, or a criterion that scored lower. The judge varies a little from run to run, so treat a single criterion moving by one on one run as noise until a `--repeat 3` confirms it.

`node run.mjs compare [<history.json> [<previous.json>]]` redoes the comparison. `node run.mjs rescore
<history.json>` re-analyses a run from the eval tool's saved results without running any agent, for example after changing how outcomes are worked out. `node run.mjs drift <chat .history.json>` prints the drift each Software Factory run in a chat reported.

## Refreshing the baseline after a module change

1. Build the changed modules with `dotnet build --no-incremental`.
2. Open `D:\Dev\Temp\eShop9\intent\eShop9.isln` in Intent Architect, update the Guardrails modules, run the Software Factory and apply it.
3. Copy it over `baseline/` the same way it was first made: `robocopy D:\Dev\Temp\eShop9 baseline /MIR /XD bin obj node_modules artifacts .git .vs .intent TestResults D:\Dev\Temp\eShop9\intent\.cache\modules`
4. Run `node run.mjs --case B0`. It must pass its gate, with DLL md5s that match the new build. Set `baselinePending` to what its `regenerate` still proposes (empty once the baseline's Software Factory output is applied).
5. Commit the baseline with the module change.
6. Run `node run.mjs --suite smoke --detach`, and read the regressions in the new history file. Run `--suite full` overnight before a release.
7. Re-make any plant that no longer applies (below).

## Plants

**Code plants** are a developer's hand edits: `md`, `rn1c`, `dl3c`, `oc1`, `ro2c` and `norule`. To re-make one:

```powershell
node run.mjs prepare baseline D:\Dev\Temp\bf-prep\md
node plants/_code/edit.mjs D:\Dev\Temp\bf-prep\md plants/_code/md.json   # fix any edit it reports, in the .json
node run.mjs plant D:\Dev\Temp\bf-prep\md md                             # before building: a build rewrites files
node run.mjs prepare md D:\Dev\Temp\bf-prep\md-check                     # then build and test that copy
```

Every code plant must build and pass the unit tests. A new code plant is made the same way: `prepare`, edit the code by hand (and record the edits in `plants/_code/<name>.json`), then `plant`.

**Model plants** are designer changes: `ad1m`, `rn1m`, `ch6m`, `dl2m` and `ro2m`. To make or re-make one: `node run.mjs capture <name>`. That runs `plants/_setup/<name>/case.json` through the eval tool, checks that only `Intent.Metadata` files changed and that no code was written or generated, and writes the patch.

**`ad1`** is AD1 done right: the discontinue feature in the model and the code. Make it from an AD1 run that passed its gate: copy that run's `results/runs/<runId>/AD1/1/agent.patch` to `plants/ad1.patch`. AD4 and AD4~norule need it.

A seed is the baseline plus plants in order. Ablation runs add plants with `~`: `AD4~norule` runs AD4 on `ad1+norule`.

## The cases

| Lane     | Cases                                                                                                                                                                                                                                                  |
| -------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------ |
| manual   | MD1 (detection: 17 planted rows that must be reported, and 3 hand edits that must not be: an unmodelled query removed, a new publish, a static helper), MD2 (sync all of MD1's edits), RN1c (rename in the code), DL3c (delete in the code), OC1 (a teammate's unmodelled changes), AD1m, RN1m, CH6m, DL2m (model first) |
| ai       | B0 (baseline self-test), NM1 to NM3 (no model change), AD1 to AD5 (additions), RN1 to RN3 (renames), CH1 to CH5 (changes), MV1, MV2 (moves), DL1 (delete), TC1 (a technology without recipes), RO1, RO2 (read only), DAY (three turns in one chat)     |
| ablation | AD1, AD3, AD4, RN1 and CH5 on the `norule` seed: the baseline without `.agents/instructions/guardrails-model-sync.md`                                                                                                                                  |

A case's `case.json`:

```json
{
  "id": "AD1",
  "title": "Add: discontinue catalog items",
  "lane": "ai",
  "plants": [],
  "prompt": "…",
  "done": "what the code must do",
  "expect": [
    { "id": "flag", "net": "visible", "text": "The Domain designer's CatalogItem has IsDiscontinued (bool, default false)." },
    { "id": "intent", "net": "blind", "text": "The discontinue operation has an update interaction on CatalogItem." }
  ]
}
```

Other fields: `turns` instead of `prompt` for several turns; `readOnly` (the agent must change nothing); `expectClean: false` (planted drift stays, so `regenerate` isn't gated); `expectSoftwareFactory: false` (no Software Factory run expected). An expectation with `"kind": "row"` is a drift row the agent must report, and `"kind": "noRow"` one it must not; those make up a case's detection rate. The runner adds the shared criteria ("done", "nothing extra", "no violations", "conduct") and the prompt suffix from `suite.json`.

Expectation ids are what the history compares across runs, so keep an id when you reword its text.

## Known limits

- **The `norule` ablation holds only until the agent's first Software Factory run.** The rule is a file the Guardrails modules generate, so that run writes it back. The ablation measures an agent that starts without the rule.
- **`ad1` and the model plants need runs of their own** before the cases that use them can run: AD4, the model-first cases and RO2.
- **The first save of the Services designer rewrites about 47 metadata files** across every Services package: the
  Eventing package's On Loaded script stamps `initialized-version` on 14 package files, and 33 interactions that start
  from an event handler gain `<order>0</order>` on their source end. It happens once per package, so on the baseline
  every run that saves the Services designer includes them. A run's diff stat is inflated by them; read the agent's real
  changes in `agent.patch`.
- **A designer rename, or a domain event, deleted and recreated rather than renamed in place** is only seen by the judge reading the model files, not by any deterministic check.
