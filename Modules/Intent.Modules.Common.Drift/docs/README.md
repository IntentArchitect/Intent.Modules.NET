# Intent.Common.Drift

The recipe-based drift check that compares a designer's model with hand-written code, shared by every module that contributes a designer to it. Intent.Domain.Guardrails.Contracts, Intent.Services.Guardrails.Contracts and Intent.UI.Guardrails.Contracts contribute the Domain, Services and User Interface designers, and install this module with them.

Intent.Common.DriftEngine replaces this check. Updating this module to 2.0 moves an application onto it (below). The rest of this page describes the recipe-based check, which still runs wherever a Guardrails Contracts module is installed.

## Moving to the drift engine

Update Intent.Common.Drift to 2.0 in the Module Manager. The update installs Intent.Common.DriftEngine, and then its migration:

- **Releases the contract files.** It removes the Intent attributes (`[assembly: DefaultIntentManaged]`, `[assembly: IntentTemplate]`, `[IntentManaged]`, `[IntentIgnore]` and the rest, and their `using`) from every file a Guardrails contract template generated. It also removes those files from the application's output records (`.intent/output-log.xml`, the committed `<application>.application.output.log` and `<application>.application.output.config.xml`), so the Software Factory no longer owns them and doesn't delete them. Nothing else in the files changes.
- **Converts the recipe files** in `guardrails/` into rows on the first Codebase Structure package's Root Folder. It skips this when that application already has a Drift Scope or Drift Check.
  - **Drift Scope:** one In row for `**`, since v1 checked code wherever its recipes found it, and an Out row for each file-pattern exception, with its reason.
  - **Drift Check Opt-in Rules:** an `optin/unmodelled.type` row for each `own` entry, over the entry's `files` (else its recipe file's), with the entry's `kind` as its Description. There is also one `optin/unmodelled.member` row over `**/*.cs`, because v1 always reported unmodelled public members of modelled types.
  - **Drift Check Exceptions:** a row for each type exception (`optin/unmodelled.type`) and each `Type.Member` exception (`optin/unmodelled.member`), one row per reason.
  - An exception naming a convention's finding (`CatalogApi: MapGet [catalogtypes]`) means that convention gets no row. Notes and comments aren't kept; they stay in git history.
- **Removes the Guardrails Contracts modules.** That takes their module entries, designer packages, settings and Template Outputs, their stereotypes on every element, and the Integration Event and Integration Command Subscription links between a subscriber's copy of a message and the published one. The engine matches a subscriber's copy to the message by name, so the links aren't needed.
- **Deletes** the recipe files and the old per-machine baselines in `.cache/guardrails/<application>/`.

Every file it rewrites and every row it writes is listed in the install's output. The migration changes code outside Change Review, so review it with `git diff` before committing.

### The first Software Factory run

- No contract file is proposed for deletion.
- The old AI files are proposed for deletion: the `guardrails-model-sync` rule, the sync-model, configure-drift-check, validate-drift-recipes and reverse-engineer skills, and the recipe guides. Apply those deletions.
- Findings appear under the engine's Areas of the drift report (`Domain (DriftEngine)`, `Services (DriftEngine)`, …). The engine reads differently from v1, so expect some new findings. In particular, an opt-in row takes a glob, not the `own` entry's find, so `unmodelled.type` now reports every unmodelled type in that glob: a helper class beside the DTOs, say. Model it, or add an Exceptions row with a reason.
- If the run finds drift, its errors untick every staged change, the old AI file deletions included. Resolve the findings, or delete those files by hand.

If the application had a Guardrails Contracts module but no recipe files, v1's check was off, so no rows are written. Set up a Drift Scope as the engine's README describes.

### Teammates

Commit the move. A teammate's own `.intent/output-log.xml` still lists the contract files after they pull it, but their Software Factory runs don't propose deleting them. Intent.Common.Drift counts each of those files as generated, without writing it, and removes it from that log during the run. A run whose changes are applied through an AI agent writes the entries back, and lists the contract files in `<application>.application.managed-files.xml` until the next run removes them again. CI and fresh clones aren't affected.

### If the move fails

A failed move logs an error naming its cause, such as a contract file that is read-only, and leaves the v1 check in place: the next Software Factory run puts back any Intent attributes it had removed and deletes nothing. Fix the cause, then run the module task **Migrate to Drift Engine** (`Intent.Common.Drift.MigrateToDriftEngine`) with `{ "applicationId": "<the application's id>" }`. It returns everything it changed. Then close and reopen the solution, so Intent loads the application without the Guardrails Contracts modules. Running the task again changes nothing.

### Afterwards

Intent.Common.Drift stays installed: it keeps the output records right on teammates' machines, and carries the recovery task. Once every machine has run the Software Factory after the move, uninstall it. Going back to 1.x doesn't restore the v1 check: reinstall the Guardrails Contracts modules and restore the recipe files from git.

## The recipe-based check

The module knows no designer. A contributing module supplies, for its designer, the facts the model states and how to read them from the model, the templates whose generated types are compared with the code, and the designer's text in the files below. This module supplies everything else:

- **The recipe engine.** Each designer's recipe file, `guardrails/<designer>.drift.json` beside the application's `.application.config`, says where each fact is in the code and how to read its value. It is read against a C# index of the code (without comments, layout or disabled code) and, for other files such as `.razor`, `.ts` or `.html`, their text. The files of one component in one folder (`Checkout.razor` and `Checkout.razor.cs`, or `customer-list.component.ts` and `.html`) are read as one place, and the `kebab` and `replace` placeholder filters name such files and the routes in them.
- **The structure comparison.** Each type a contributing template generates is compared with its file on disk, on what its element can state.
- **The drift check.** After every Software Factory run's templates have run, each designer's check adds what it finds to its Area of the application's drift report, which Intent.Common.DriftReporting logs and writes to `.cache/drift/<application>/DriftReport.mdx`, in the folder that holds the solution's `.isln`, when there is something to act on. Every drift is an error that fails the run: on its innermost model element when the designer knows it, against the code's file and line. Problems with the recipe file are the Area's Code findings. The check remembers, per machine, what the model and the code looked like when they last agreed (`<Designer>.model.json`), so a change made in the model first is left to the Software Factory.
- **Recipe validation, on demand.** Each contributing module's ValidateDriftRecipes module task returns, as JSON, what every recipe reads from the code.

Each contributing module's README describes its designer's facts, what is compared, and its technology guides.

## Files for AI agents

The module writes four files, each covering every designer contributed in the application:

- `guardrails-model-sync.md` into the application's rules folder (the `AI.Context.Instructions` output): an always-on rule to update the model in the same change as the code, run the Software Factory and resolve the drift it reports.
- `skills/sync-model/SKILL.md` (the `AI.Context.Skills` output): the `/sync-model` skill, which resolves each drift row by updating the model, the code, a recipe or the exceptions. Beside it, `skills/sync-model/references/intent-drift.md` says how to resolve a row about a modelled interaction.
- `skills/configure-drift-check/SKILL.md`: the `/configure-drift-check` skill, which documents the recipe file format and every fact each designer states, and writes and extends the recipe files for whatever technology the code uses. Each contributing module writes its technology guides beside it.
- `skills/validate-drift-recipes/SKILL.md`: the `/validate-drift-recipes` skill, which runs each designer's ValidateDriftRecipes module task and checks what each recipe reads against the code.

None of them is written until a module contributes a designer.

## Contributing a designer

A module contributes one designer with a `DriftDesigner`: the designer's name (which names its recipe file and its Area of the drift report), its id, its order in the files above, its kinds of fact, a function reading the facts and one reading the elements by id, the id of its ValidateDriftRecipes task, and its text in the files above (`DriftGuidance`). Each `FactKind` names its recipe key, its report label, its placeholders with what each names, which of them name the fact's own element, and the model's vocabulary for its value.

- The module's drift reporter, a factory extension deriving from `DesignerDriftReporter`, returns its `DriftDesigner` and takes its `Order`. The base class reports the designer's drift with `DriftCheckRunner` and records the designer at the start of the run, which is how the files above find it. Mark the reporter `[IntentManaged(Mode.Merge)]`: the Module Builder's default would put back `FactoryExtensionBase` as its base class.
- The module's ValidateDriftRecipes module task delegates `Execute` to `RecipeValidationTask.Execute`, passing its `DriftDesigner`.
- Each template whose generated C# file is compared with the code implements `IDriftStructureSource`, naming the designer, and records what its element can state on each type it builds, with `Modellable` under `Modellable.MetadataKey`.
- `DriftReports.RelativePath` (Intent.Common.DriftReporting) gives the drift report's path as guidance names it.

The contributing module compiles against this module without shipping it: a project reference that isn't copied to output, and a `<dependency>` on Intent.Common.Drift in its `.imodspec`. It references Intent.Common.DriftReporting the same way, as the `Intent.Modules.Common.DriftReporting` NuGet package and a `<dependency>` on the module, whose installed copy is the only one loaded. Two copies of this module's assembly would leave the contribution implementing interfaces the loaded copy doesn't know.

One module contributes each designer. The code index reads C#; other files are read as text. A designer with no generated types to compare, such as the User Interface designer, contributes only its facts.
