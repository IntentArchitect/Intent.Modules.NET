---
name: local-module-application-setup
description: "Set up a brand-new Intent Architect module application in this repo's Modules folder so it generates into the right folder, is named the way every sibling is, and is ready to install modules and run the Software Factory. USE ONLY WHEN creating a new module application (a new module, or a new module family such as a core plus satellites). DO NOT USE FOR changing an existing module (see module-building-strategies), setting its version (see module-versioning), or modelling its templates (see file-builder-expert). REQUIRES the module's id and which solution (.isln) under Modules it belongs to."
argument-hint: "[module id, e.g. Intent.Application.Mediator]"
keywords: [new module, create application, module builder, scaffold, relative location, rename package, codebase structure]
---

# Skill: local-module-application-setup

A freshly created Module Builder application is **not** ready to use. Three things are wrong out of the box: the code goes to the wrong folder, the application carries the wrong name, and the package has the wrong module id. The agent creates the application, fixes the output location and verifies the result. **Steps 3 and 4 (the application name and the package id) must be done by the user in the Intent Architect UI**, because the agent tools do them destructively (see "Why Steps 3 And 4 Are UI-Only").

## The Naming Convention

Four names exist for one module. Keep them distinct:

| What                                             | Value for module `Intent.Application.Mediator` | Where it is set                                |
| ------------------------------------------------ | ---------------------------------------------- | ---------------------------------------------- |
| Module id (the package)                          | `Intent.Application.Mediator`                  | Module Builder designer, package name (step 4) |
| Application **name** (the label in the solution) | `Application.Mediator`                         | Application settings (step 3)                  |
| Application folder and files                     | `Intent.Modules.Application.Mediator`          | Chosen at creation (steps 1-2); never renamed  |
| Codebase output folder                           | `Intent.Modules.Application.Mediator`          | Relative output location (step 5)              |

The folder is always `Intent.Modules.` + the module id minus its leading `Intent.`. The application name is the module id minus its leading `Intent.`.

## What Creation Does On Its Own

`create_application` does not stop at scaffolding: it **starts a Software Factory run immediately**, before any of the steps below. Because the location is wrong at that point, that run writes to the repo root. It is reproducible: after step 1, expect all of these:

- an empty `<repo root>/Intent.Modules.<...>` folder;
- untracked agent-harness files at the repo root: `.agents/`, `.codex/`, `.cursor/`, `.github/`, `opencode.json`, `.opencode/instructions/{guidance-precedence,model-is-source-of-truth,spec-sync}.md`, a handful of `.opencode/skills/<name>/` folders, and `.claude/rules/{guidance-precedence,model-is-source-of-truth,spec-sync}.md`;
- five added lines in `.gitignore` (`.codex/config.toml`, `.github/mcp.json`, `.cursor/mcp.json`, `opencode.json`) and a `lastUsedApplicationTemplate` line in the `.isln`.

Treat these as the expected by-product of the wrong location, list them for the user, and **do not delete them yourself** (unless the user asks you to clean up; see "Cleaning Up An Attempt"). Do not run the Software Factory again until step 5 is done.

## Steps

Confirm the module id and the target solution with the user first; never invent either.

### 1. Create the application under `Modules` (AGENT)

Use `create_application` (load `creating-applications` first) with:

- Architecture `Intent.ApplicationTemplate.ModuleBuilder`.
- `solutionPath` set to the `.isln` under `Modules` that the module belongs to (for example `Modules/Intent.Modules.NET.Dispatchers.isln`). Several solutions govern this repo, so it is required.
- Components to match the sibling modules: Module Builder - C#, and add Services, CQRS, Domain, Event Interactions and so on only when the module's templates or designer extensions need them. Compare with a sibling's `modules.config` rather than guessing.

### 2. Name it `Intent.Modules.<id without leading Intent.>` (AGENT)

For module id `Intent.Application.Mediator` that is `Intent.Modules.Application.Mediator`. This sets the folder and file names (`Modules/Intent.Modules.Application.Mediator/...`). Check `git status` against the by-product list above.

### 3. Rename the application name only (USER, in the UI)

Ask the user to rename the application to `Application.Mediator` **in the Intent Architect UI, declining any offer to rename the underlying files**.

### 4. Rename the package in the Module Builder designer (USER, in the UI)

Ask the user to rename the package to `Intent.Application.Mediator` in the Module Builder designer, **accept the rename prompt, and save**. The package name becomes the module's `<id>` in the `.imodspec`. Check the package's name before the user starts: renaming the application may already have renamed it.

### 5. Fix the output location, in two places (AGENT), stereotype first

Both must be set; either alone leaves the output in the wrong folder. Do them in this order.

1. **Codebase Structure designer: add `Root Folder Options` to the `root` folder first.** The stereotype is not applied by default, so it has to be added before anything can be set on it. The `root` folder is a `Root Folder` element, reachable by id (not through `getPackages()`, whose package handle has no stereotype members). Using `run_designer_script` against the Codebase Structure designer:

   ```js
   const rootId = findElements({})[0].getPath()[0].id;   // path[0] is the Root Folder
   const root = lookupById(rootId);
   if (!root || root.getSpecialization() !== "Root Folder") throw new Error("root folder not found");
   const st = root.ensureStereotype("Root Folder Options");   // only property: "Relative Location"
   st.setProperty("Relative Location", "..");
   return st.getProperty("Relative Location").getValue();
   ```

   Use `saveOnSuccess: true`. It saves in place; confirm `root.pkg.config` under the application's own `Intent.Metadata/Codebase Structure/root/` now contains `Relative Location" value=".."` and that no `home/` folder appeared at the repo root.
2. **Application settings, relative output location** = `..\Intent.Modules.Application.Mediator`. Use `update_application_settings` with `relativeLocation` and nothing else. It changes the application's output `location` only (verified: `../../<folder>` became `..\<folder>`); `metadataLocation` and every file and folder name are untouched.

Why: left at the defaults, `create_application` produced `location="../../<folder>"`, which resolves to the repo root instead of `Modules/`. Existing sibling modules use `..\<folder>`.

### 6. Verify (AGENT), then it is ready

Verify by reading back, never by trusting the UI:

- **Names and folders.** `Modules/Intent.Modules.Application.Mediator/` and its `.application.config` still exist under their original names, and the `.isln` entry's `relativePath` still points at `Intent.Modules.Application.Mediator/...`.
- **Package.** `run_designer_query` on the Module Builder designer, `getPackages()[0].getName()` is `Intent.Application.Mediator`; the package files sit inside the application's own `Intent.Metadata` folder; nothing new appeared at the repo root (for example a `home/` folder).
- **Output location.** `update_application_settings` with **only** `applicationId` makes no change and returns `metadataLocation` and `outputLocation`. Both must be `<repo>/Modules/Intent.Modules.Application.Mediator` (before step 5 `outputLocation` is `<repo>/Intent.Modules.Application.Mediator`).

Only then install the other modules and run the Software Factory. After the first run confirm `Modules/Intent.Modules.Application.Mediator` holds the `.csproj` and `.imodspec` and that the `.imodspec` `<id>` is `Intent.Application.Mediator`.

## Why Steps 3 And 4 Are UI-Only

Tested 2026-10-13 on `Intent.Modules.Application.Mediator`:

- **Step 3 via `update_application_settings` with `name`** did not rename only the label. It renamed the application folder to `Modules/Application.Mediator`, the `.application.config` file, and the `.isln` `relativePath`, and it renamed the Module Builder package to `Application.Mediator`.
- **Step 4 via `run_designer_script` with `pkg.setName(...)`** suppresses the "Rename underlying file?" prompt and applies the recommended choice. It then wrote the package files to a doubled path under the repo root (`<repo>/home/dandrejvv/.worktrees/.../Modules/Application.Mediator/Intent.Metadata/...`), leaving the application's real `Intent.Metadata` folder with only the two `.designer.config` files. The metadata ended up split across two trees.

Neither is recoverable by the agent: the files live inside `Intent.Metadata`, which agents must not modify.

The two tools in step 5 do not have this problem: setting a stereotype property saves in place, and `relativeLocation` changes only the output `location`. Neither renames anything.

## What Building A Whole Family Showed (2026-10-13)

Learned building the five Mediator module applications. None of it changes the steps above; it changes what to expect afterwards.

- **Choose the creation components carefully.** `modules.config` is written at creation, and in the environment this was tested in, `install_or_update_modules`, `uninstall_modules` and `listInstalledModules` all failed for every application (`Application with id [...] could not be found`, or `Cannot read properties of undefined (reading 'split')`), including for apps that existed before the family. A module that was not part of the creation components therefore could not be added afterwards. In particular a satellite that models element types from a designer extension (for example Domain Events) needs that modeler selected at creation. This is an Intent Architect fault, not a repo one; report it to the Intent support team rather than working around it.
- **Group the family in one solution folder by editing the `.isln`.** The applications land at the solution root. Setting `solutionFolder="Modules/<Family>"` on each `<application>` entry by hand is the only way found, and it is the one hand edit of an `.isln` this repo allows. The entry is otherwise identical to its siblings.
- **Every Software Factory run stages the same by-products.** After step 5 the run still stages `../.codex/instructions/csharp-guidelines.md`, `../.cursor/rules/...`, `../.github/instructions/...` and a `../<Solution>.sln`. Never pass them to `apply_staged_file_changes`; apply an explicit list of the module's own files.
- **A hand-maintained `.csproj` is flagged destructive on every later run.** The Software Factory rewrites it from the model (for example it drops `<Nullable>enable</Nullable>` and the SDK pins a hand edit set). Either scope nullable with `#nullable enable` in the files that need it and apply the generated project, or leave the hand-authored project unapplied and say so; do not fight it silently.
- **The `.imodspec` dependency list is re-derived from `modules.config`.** Dependencies removed by hand come back on the next run, and dependencies the code needs but `modules.config` lacks are never added. Treat the list as something the dependency audit re-checks after every run.
- **Template ids come from the element name and folder in the designer.** Create elements at the package root when another module will find them by id.

## Cleaning Up An Attempt

When the user asks you to undo a failed or abandoned attempt and **only then**:

1. `git checkout -- <each tracked file the attempt modified>`: `.gitignore`, the `.isln`, and anything else `git status` lists as modified that is the attempt's doing.
2. `git clean -fd -- <explicit paths>` on the untracked paths `git status` listed (**never `git clean -x`**, **never a bare `git clean`**): `-x` also deletes gitignored files that existed before the attempt, such as the generated `.opencode/skills/*` copies.
3. `rmdir` each empty folder left at the repo root.
4. Remove `opencode.json` at the repo root; it is ignored while the attempt's `.gitignore` line is in place, so it only shows up after step 1. Check its timestamp is from the attempt first.
5. Re-run `git status`; it must be clean.

Then tell the user Intent Architect may still hold the removed application in memory: after cleanup it recreated an empty `Modules/<old app name>/.intent/` folder (lock and cache files only). That folder is Intent's own and off-limits; leave it and tell the user.

## Then

- A new module starts at `1.0.0-pre.0` (see `module-version-increment`; set it with `module-versioning`).
- Read `module-building-workflow.instructions.md` for the phases that follow.

## Must

- Hand steps 3 and 4 to the user; do step 5 yourself, stereotype first.
- Verify every step by reading the result back from disk or the designer; a successful tool call is not proof.
- Complete step 5 before any Software Factory run.
- Leave the shell in the project root: a persistent `cd` elsewhere makes the repo's agent-gate hook fail with "Couldn't find a project to run". Always `cd` back to the project root.

## Must Not

- Do not use `update_application_settings` with `name`, or `pkg.setName` in a script, to do steps 3 or 4.
- Do not call `update_application_settings` with any field other than the one you mean to change; with only `applicationId` it is a safe read-back.
- Do not run the Software Factory "to see what happens" before step 5.
- Do not edit `.application.config` files or anything under `Intent.Metadata` by hand.
- Do not delete the files and folders creation leaves at the repo root unless the user asks; show them first.
- Do not create several applications before the sequence has been proven on the first one.
