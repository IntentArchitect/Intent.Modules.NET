---
description: Check that the Guardrails drift check's recipe files read the right code. Runs each Guardrails designer's ValidateDriftRecipes module task, which reads every fact the model states through its recipe and returns what it read, then checks it against the code; or narrows it to one kind of fact, element or code file and lists every reading there. Use after writing or changing a recipe file, when drift rows look wrong in the same way (contradictions or can't-be-verified facts the code doesn't bear out), before changing the model for a contradiction, or when the developer questions what the drift check reports.
short-description: Check what each drift recipe reads from the code.
contentHash: F4FD5565AB9D9B0F251C6908D7FDEF22F14E4A46029533BC6098BB1981C40133
---
# Validate the drift recipes

A recipe file tells the drift check where each fact the model states is in the code, and how to read it. A recipe that misreads the code fails loudly (a contradiction the code doesn't bear out) or silently (it agrees with the model by accident), and the drift report can't tell which. Each Guardrails designer has a module task that reads every fact through its recipe, on demand, and returns what it read as JSON. It changes nothing.

## 1. Run the task

Load **scripting-the-model**. Module tasks run only in `run_designer_script`, not in `run_designer_query`. Run this against any of the application's designers, with the same application id, once per designer:

```js
return JSON.parse(await executeModuleTask("<task>", JSON.stringify({ applicationId: "<the application's id>" })));
```

| Designer | Recipe file | Task |
| --- | --- | --- |
| Domain | `guardrails/domain.drift.json` | `Intent.Domain.Guardrails.Contracts.ValidateDriftRecipes` |

It reads only the recipe file and the code on disk as they are now (save the recipe file first), and the model as saved. Nothing from a Software Factory run goes in, so there is no need to run one first.

## 2. Read what it returns

- `errorMessage`: the task couldn't run, such as for an unknown application id.
- `recipeFileExists`: `false` when the designer has no recipe file yet: load **configure-drift-check**.
- `problems`: what is wrong with the recipe file itself: invalid JSON, an unknown key, a bad find path or regular expression.
- `notChecked`: the kinds of fact the model states that have no recipe, with their counts.
- `facts`: per kind of fact, how many facts the model states, and how many were `readFromCode`, `fromDefault`, `fromAbsent`, `unverifiable`, and `contradicted` (the code's value differs from the model's, wherever it came from). Each has up to three `samples`: the `element`, the `model` and `code` values, and the `location` the value was read at, or the place `near` which the code would state it, or the `problem`. `readCasesMatchingNothing` lists the read cases no fact matched.
- `conventions`: per `own` entry, how much code it `found`, and the first few `samples`, named as the drift report would name them, with their locations.
- `probeWarnings`: read cases whose reading didn't change when the code they matched was changed in memory: the case isn't reading the code it seems to.
- `wrongElementWarnings`: read cases that read some facts from code that names another element of the same kind and not their own (another attribute's configuration, another action's attributes): the case takes whichever such code comes first.

## 3. Check the recipes against the code

Open the code at the samples' locations, and check that each states what its sample says. A recipe is wrong when:

- the probe warns about one of its read cases: make the case's `match` cover the code that states the fact;
- a wrong-element warning names one of its read cases: tie its `find` or `match` to the element, usually with the element's placeholder (`{Attribute}`, `{Operation}`), so that each fact reads its own code;
- a sample's value comes from another line, or from `default` or `absent` where the code does state the fact;
- most of a kind's facts come from `default` or `absent`, or many can't be verified;
- its `contradicted` facts share one code value (every one `(none)`, say): it misreads the code. A contradiction the code bears out is drift, for **sync-model**, not a recipe fault;
- a read case matches nothing, when the code does use that form;
- a convention finds code that isn't of its element type (helpers, base classes), or finds far more or fewer than the model has of that element type.

Fix the recipe file with **configure-drift-check**, then run the task again until none of these is left. The probe proves that a case reads some code, and the wrong-element warnings catch a case that reads a sibling's, but neither proves it reads the right code, and a value from `default` or `absent` has no code to change: the samples, and the readings in §4, are what show that a recipe reads the right code. Never change the code, or make a recipe return a fixed value, to make a finding go away.

## 4. Look at one recipe, element or file

To see everything one recipe reads, or how one element's facts or one file were read, add any of these to the argument:

```js
return JSON.parse(await executeModuleTask("<task>", JSON.stringify({ applicationId: "<the application's id>", fact: "column.maxLength", element: "Order" })));
```

- `fact`: a kind of fact, as the recipe file names it. One the designer doesn't state returns an `errorMessage` listing those it does.
- `element`: an element as the drift report names it. `Order` also selects `Order.Notes`.
- `file`: a code file's path relative to the codebase, or its end (`AppDbContext.cs`). It selects the facts read there, or that would be stated there.

It returns the same, for those facts only and without `conventions`, plus `readings`: every fact selected, not just samples. Each has its `kind`, `element`, `model` and `code` values, `from` (`code`, `default`, `absent` or `unverifiable`), whether it is `contradicted`, the `findPath` and `readCase` that produced it (counted from 1, as the recipe file lists them), for a value read from the code its `level` (0 for the code its `find` found, 1 or more for code inherited from further out) and the text it `matched`, and its `location`, `near` or `problem`. Use it:

- after adding or changing a recipe, with `fact`, to check that every element it covers reads its own code;
- before changing the model for a contradiction, with the row's `element` and `fact`, to see the code the value was read from;
- after a wrong-element warning, with its `fact`, to see which elements read whose code.

A kind of fact with many elements returns many readings, so narrow it with `element` or `file` where you can.
