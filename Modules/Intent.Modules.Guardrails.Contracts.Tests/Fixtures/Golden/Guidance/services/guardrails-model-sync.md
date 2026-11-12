---
description: Keep the Intent Architect model true to the hand-written code of this Guardrails application. Whenever you change code, update the model in the same piece of work; once the drift check is set up, every Software Factory run fails while the model and the code disagree.
alwaysApply: true
contentHash: 2B51E62E86DC8CD95E6F28644B83A0FF2A31AC4450263D0D8C1EF4D135A349A2
---
# Keep the model true to the code

In this application the code is hand-written and the Intent Architect model describes it. Whenever you change code here, update the model in the same piece of work.

| If you change… | Also update in the model |
| --- | --- |
| A command, query, DTO, service, enum, message, handler or API client: the type, a field, a method or its signature | That element in the Services designer |
| An endpoint's route, verb or parameter binding, or its authorization | Its `Http Settings` and `Parameter Settings`, and `Secured` or `Unsecured` |
| What a command, query, service operation or event handler does: the entities it creates, updates, deletes or reads, what it calls, the events it publishes and the commands it sends | Its entity actions, `Perform Invocation`, `Publish Integration Event` and `Send Integration Command` |

The check reports a modelled interaction the code no longer performs, but never a new one, such as something a handler starts doing (an entity it acts on, a call, an event it publishes or a command it sends): add it to the model as you write the code.

## The Software Factory checks it

Every Software Factory run compares the model with the code on disk. Each drift is one of the run's errors: it starts with its designer in square brackets, such as `[Domain]`, and one about a model element is on that element, so Intent offers to fix it with AI. Each designer with drift then ends with one line, `Drift in <designer>: N errors, M warnings. Report: <path>`, naming the application's drift report, `intent/.cache/drift/Acme/DriftReport.mdx`, in the `.cache` folder beside the solution's `.isln` file, which git ignores. Each designer is an Area of the report (Services). The report is only written when a run finds something, and the run then lists it in its changes as a new file; a run with no drift writes none. A run with errors applies none of its changes, so the report isn't on disk until it is applied: apply it with `apply_staged_file_changes` and its path, then read it. The check is on for a designer once its recipe file, `guardrails/<designer>.drift.json`, exists; until then every run warns that the check is off, and the **configure-drift-check** skill sets it up.

A designer's Area has up to two tables:

- **Model / Code**: one row per difference. Its Subject is the model element (the innermost one, such as an attribute rather than its class), Against is the code file (where the code states it, or where it would), and the Finding says what the drift is. The model must never be wrong, so every row is an error and fails the run. A row says that the model and the code state different values, that a fact can't be verified (the check couldn't read it from the code), that code of a kind the model must describe isn't modelled, or that a modelled type or member is missing from the code.
- **Code**: the recipe file itself. A recipe file that can't be read is an error. A kind of fact with no recipe, or an exception that matches nothing, is a warning: warnings never fail the run, and only the report shows them. **configure-drift-check** adds and fixes recipes, and a stale exception is removed.

A recipe can misread the code. Before you change the model or the code for a contradiction, open the code at the row's location and check that it says what the row says it does; if it doesn't, the recipe is wrong: fix it with **configure-drift-check**. **validate-drift-recipes** shows what every recipe reads, on demand.

A type or member the code lacks is never a row: the Software Factory writes it, and its changes show that, whether you added it to the model or the code removed it. Any other change you make in the model is left to the Software Factory until it is applied, as long as its code hasn't changed: the check remembers, on this machine, what the model and the code looked like when they last agreed. A difference the code made is reported, and so is one where both changed, because applying the model's change would overwrite the code's; that row ends by saying both have changed. A fact about a type or member you have just added (a column's configuration, a route), or about a method whose parameters you changed, is checked on the same run, against the code as it will be once the Software Factory's changes are applied: the member itself isn't a row, but anything the code still lacks then (no `IsRequired()`, no route) is, and fails the run. On a machine's first run, or on CI, there is nothing to remember, so any other change you modelled first is reported there until it is applied. A run with errors doesn't apply its changes by itself (Intent leaves them unticked): apply the ones you expect with `apply_staged_file_changes` and their paths, then run the Software Factory again.

## Before you finish

1. Check that the model shows what the code you changed now does, including anything new it does: nothing reports that.
2. Save every designer you changed (`saveOnSuccess: true` on the last script).
3. Run the Software Factory, apply the changes you expect, and run it again.
4. The last run must report no drift. Resolve every drift row with the **sync-model** skill.
5. If code you wrote states a fact in a way the recipes don't read yet (a new technology, or a new way of declaring it), the fact is reported as can't be verified. Load **configure-drift-check** and extend the recipes, then check them with **validate-drift-recipes**.

Never edit the drift report: it is rewritten on every run.
