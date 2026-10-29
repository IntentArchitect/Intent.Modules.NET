---
description: Fix drift between the Intent Architect model and the hand-written code of a Guardrails application. Resolves every row of the drift reports the Software Factory writes, by updating the model, the code, a recipe or the exceptions, until the run reports no drift. Use when the user asks to fix, resolve or sync drift, when a Software Factory run fails with drift, or to sync the model with the code.
short-description: Resolve every row of the drift reports.
argument-hint: "[designer]"
contentHash: 4FCD791F78A0DBF533ECEB777DE90CCF63EDD2902A05285346B8A30B0CA6F5B3
---
# Sync the model with the code

In a Guardrails application the code is hand-written and the model describes it. Every Software Factory run compares the two and writes `intent/.cache/guardrails/Acme/DomainDriftReport.mdx`, in the `.cache` folder beside the solution's `.isln` file, only when there is something in them; the run lists each one in its changes as a new file. A report has a **Model and code drift** table (the model element, the code file and what the drift is; every row fails the run) and a **Warnings** table. A run with errors applies nothing, and its one error only says how many drifts there are and names the report: apply the report with `apply_staged_file_changes` and its path, then read it. You are done when a run finds no drift: it logs that, and writes no report. A report that says the check is off has no recipe file yet: load **configure-drift-check** to set it up.

Load **scripting-the-model** and **changing-the-model** before changing the model, and **generating-and-applying-code** before running the Software Factory.

## How the check works

- **Facts** (Domain: database tables, keys, columns and relationships, and the domain events constructors and operations raise) are read from the model's stereotypes and associations, and compared with the values that the recipes in `guardrails/<designer>.drift.json`, beside the application's `.application.config`, read from the code. A recipe says where a kind of fact is in the code and how to read its value, for whatever technology the code uses. The **configure-drift-check** skill describes them.
- **Structure:** each type the Guardrails templates generate is compared with its file on disk, on what its element can state: kind, modifiers, generic parameters, parameters on the type (a positional record, a Class's Primary Constructor), the base types the model names, and the members its element can model, with their types and signatures. What the designer can't express is never compared: a Service's, handler's or repository's primary constructor holding its injected dependencies, a DTO's or command's methods, a Service's properties, a handler's members, any field, event or indexer. A member the model doesn't describe is reported only when it's public: whether a protected, internal or private one belongs in the model is a judgement call, and once modelled it's compared like any other. These rows come from what the templates generate, not from the recipe file: no recipe adds or removes them, and an exception only covers an "isn't modelled" row. Spellings that mean the same type agree (`Nullable<int>` and `int?`, `System.String` and `string`, a qualified name and its last part).
- **Ownership:** the recipes' `own` lists say which code the model must describe, such as every controller action, every `Entity<T>` configuration, and every type that follows the convention of a modelled element type (every `*DTO` class in the DTO project, every class deriving from the business base class). Types the templates generate count as modelled. Members of a generated type count too, at the access levels the model uses for that kind of member.
- **Exceptions:** the recipe file's `exceptions` list code that may stay unmodelled. A type or member exception covers that declaration, and a type its members, but not code a convention finds inside a member. They never cover a contradicted or unverifiable fact.

## 1. Read the drift

Run the Software Factory, or read the reports of the last run. A failing run's error doesn't list the rows: apply the report with `apply_staged_file_changes` and its path, then read it. Each drift row names the model element, the code's file and line, and what the drift is. Read only the code the rows name.

## 2. Resolve each drift row

| The Drift column says | What to do |
| --- | --- |
| Any row ending (the model and the code have both changed since they last agreed) | Each side changed it since they last agreed, so the Software Factory's change would overwrite the code's. Don't apply that change, or touch either side, until you know which is right: a change made in this piece of work, or one the developer asked for, is right; otherwise ask the developer in one `ask_user_question` covering every such row, showing both versions. Then resolve it as its row below says |
| `<element>` *raises*: model `…`, code (none) (…), or can't be verified | The model says the element does this, and the recipe didn't find it in the element's code. That can mean the code stopped doing it, does it somewhere else, or does it in a form the recipe doesn't read, and each has a different fix: follow [references/intent-drift.md](references/intent-drift.md) before changing either side |
| `<element>` *fact*: model `…`, code `…` | First check the recipe read the code: open the code at the row's location, or look at the row's element and fact with **validate-drift-recipes** (its §4), which shows the text the value was read from. If it doesn't say what the row says (the value comes from another line, or from the recipe's default where the code does state the fact), the recipe is wrong: look at that fact for every element the same way, fix it with **configure-drift-check**, and touch neither side. Otherwise decide which side is right. A code value followed by a bracket comes from the code saying nothing: the bracket says what's missing and what the technology then does, and the row points at where the code would say it (the entity's configuration, the controller). A change made in this piece of work, or one the developer asked for, is right: update the other side. Otherwise ask the developer in one `ask_user_question` covering every such line, recommending the code where its change looks deliberate. Then update the model, or change the code to match it |
| `<element>` *fact* can't be verified: … | The recipe couldn't read the fact. Load **configure-drift-check** and fix the recipe until it reads the value. Never change the code so that a recipe can read it, and never make a recipe return a fixed value to get past it. If no recipe can read it (a value built in a helper method, or a line that says the code states it in a form the recipe can't read, such as a route from a constant), tell the developer which facts and why, and leave the model as it is |
| … isn't modelled | Decide what it is. A concept the model describes (an endpoint, a service operation, a DTO, a business class, an entity, a property of a request, DTO or entity, an entity configuration, an index): model it. Clear helpers and framework plumbing: add an exception. Anything else: ask the developer in one `ask_user_question` listing each item with what you think it is, and offer per item: model it, or add an exception |
| The recipe file is wrong: … | Load **configure-drift-check** and fix the recipe file |

Never change a recipe to make a contradiction pass: a recipe that reads the code correctly and disagrees with the model has found drift, and either the model or the code is wrong.

The **Warnings** table only warns, but resolve it too. **No recipe**: a kind of fact the recipe file doesn't cover, such as one a module upgrade added; load **configure-drift-check** and add its recipe. **Exception matches nothing**: a stale exception; remove it from the recipe file.

When several rows look wrong in the same way, or you changed a recipe, load **validate-drift-recipes**: it shows what each recipe reads and warns about read cases that don't read the code, or that read another element's.

## Exceptions

Add an exception to `exceptions` in the designer's recipe file: a file pattern (`src/Api/Health/*.cs`), a type (`HealthController`), a member (`OrdersController.Ping`) or the name the report gives, optionally with a reason. A type or member exception exempts that declaration, and a type its members, but not code a convention finds inside a member, such as each endpoint a mapping method registers or each entity a configuration method configures: exempt one of those by the name the report gives it, or by a file pattern.

```json
"exceptions": [
  { "match": "HealthController", "reason": "load-balancer probe" }
]
```

The report warns about any exception that matches nothing: remove it.

## 3. Confirm

Save the designers, run the Software Factory, apply its output, and run it again. The run must report no drift errors, and write no drift report.

## Gotchas

- The check reads the saved model and the code on disk: save the designers, and apply the Software Factory's output, before judging a report.
- A type or member the code lacks is never reported: the Software Factory adds it. Any other change in the model isn't reported while the Software Factory's change to it is waiting to be applied, on a machine where the check has run before (it remembers the model and the code in `.cache`), unless its code changed too. On a first run, or on CI, it is reported until applied. A run with errors applies nothing by itself: apply the changes you expect with `apply_staged_file_changes`.
- A fact about a type or member the Software Factory is adding, or about a method whose parameters it is changing, is read from the code as it will be once those changes are applied. Its row is a gap to fix now, alongside applying the changes, not something to wait for. While the code it read isn't on disk yet, its row points at the file without a line.
- When code has removed a modelled member, the drift report doesn't list it: the Software Factory proposes adding it back as an empty stub. Don't apply that unless the member should come back; update the model instead (rename the element rather than deleting it and creating another).
- Every row is about something the model can state. If one asks for something the designer has no way to express, don't work around it with an exception, a recipe or a code change: tell the developer, because the check shouldn't report it.
- The drift reports are in the solution's `.cache` folder, which git ignores, and a run lists each one it writes as a new file. They aren't code: they need no review, and they reach the disk when the run's changes are applied. A run with no drift deletes the last one and writes none. Never edit them.
