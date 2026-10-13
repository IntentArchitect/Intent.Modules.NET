---
name: local-module-application-setup
description: "Set up a brand-new Intent Architect module application in this repo's Modules folder so it generates into the right folder, is named the way every sibling is, and is ready to install modules and run the Software Factory. USE ONLY WHEN creating a new module application (a new module, or a new module family such as a core plus satellites). DO NOT USE FOR changing an existing module (see module-building-strategies), setting its version (see module-versioning), or modelling its templates (see file-builder-expert). REQUIRES the module's id and which solution (.isln) under Modules it belongs to."
argument-hint: "[module id, e.g. Intent.Application.Mediator]"
keywords: [new module, create application, module builder, scaffold, relative location, rename package, codebase structure]
---

# Skill: local-module-application-setup

A freshly created Module Builder application is **not** ready to use. Three things are wrong out of the box: the code goes to the wrong folder, the application carries the wrong name, and the package has the wrong module id. Two of the fixes (steps 3 and 4) **must be done by the user in the Intent Architect UI**, because the agent tools do them destructively (see "Why steps 3 and 4 are UI-only").

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

`create_application` does not stop at scaffolding: it **starts a Software Factory run immediately**, before any of the steps below. Because the location is wrong at that point, that run writes to the repo root. Expect, after step 1:

- an empty `<repo root>/Intent.Modules.<...>` folder, and
- untracked agent-harness files at the repo root (`.agents/`, `.codex/`, `.cursor/`, `.github/`, `.opencode/...`, `.claude/rules/guidance-precedence.md`, `.claude/rules/model-is-source-of-truth.md`, `.claude/rules/spec-sync.md`) plus five added lines in `.gitignore`.

Treat these as the expected by-product of the wrong location, list them for the user, and **do not delete them yourself**. Do not run the Software Factory again until step 5 is done.

## Steps

Confirm the module id and the target solution with the user first; never invent either.

### 1. Create the application under `Modules`

Use `create_application` (load `creating-applications` first) with:

- Architecture `Intent.ApplicationTemplate.ModuleBuilder`.
- `solutionPath` set to the `.isln` under `Modules` that the module belongs to (for example `Modules/Intent.Modules.NET.Dispatchers.isln`). Several solutions govern this repo, so it is required.
- Components to match the sibling modules: Module Builder - C#, and add Services, CQRS, Domain, Event Interactions and so on only when the module's templates or designer extensions need them. Compare with a sibling's `modules.config` rather than guessing.

### 2. Name it `Intent.Modules.<id without leading Intent.>`

For module id `Intent.Application.Mediator` that is `Intent.Modules.Application.Mediator`. This sets the folder and file names (`Modules/Intent.Modules.Application.Mediator/...`).

### 3. Rename the application name only (USER, in the UI)

Ask the user to rename the application to `Application.Mediator` **in the Intent Architect UI, declining any offer to rename the underlying files**. Then verify (do not trust the UI):

- `Modules/Intent.Modules.Application.Mediator/` and its `.application.config` file still exist under their original names.
- The `.isln` entry's `relativePath` still points at `Intent.Modules.Application.Mediator/...`.

Do **not** use `update_application_settings` with `name`.

### 4. Rename the package in the Module Builder designer (USER, in the UI)

Ask the user to rename the package to `Intent.Application.Mediator` in the Module Builder designer, **accept the rename prompt, and save**. Check the starting name first: renaming the application may already have renamed the package (it did, to `Application.Mediator`, when step 3 was done through the tool).

The package name becomes the module's `<id>` in the `.imodspec`. Afterwards read the name back with `run_designer_query` (`getPackages()`) and confirm that the package files sit inside the application's own `Intent.Metadata` folder and that nothing new appeared at the repo root (for example a `home/` folder).

Do **not** rename the package with `run_designer_script`.

### 5. Fix the output location, in two places

Both must be set; either alone leaves the output in the wrong folder.

1. **Application settings, relative output location** = `..\Intent.Modules.Application.Mediator`.
2. **Codebase Structure designer:** locate the `Root` element, open its `Root Folder Options` stereotype, and set `Relative Location` to `..`.

`update_application_settings` returns `outputLocation`; after the change it must end in `/Modules/Intent.Modules.Application.Mediator`. (Not yet exercised through the tool; treat it as untested and read `outputLocation` back to prove it.)

Why: left at the defaults, `create_application` produced `location="../../<folder>"`, which resolves to the repo root instead of `Modules/`. Existing sibling modules use `..\<folder>`.

### 6. Ready

Check `git status` against the by-product list above, then install the other modules and run the Software Factory. After the first run confirm `Modules/Intent.Modules.Application.Mediator` holds the `.csproj` and `.imodspec` and that the `.imodspec` `<id>` is `Intent.Application.Mediator`.

## Why Steps 3 And 4 Are UI-Only

Tested 2026-10-13 on `Intent.Modules.Application.Mediator`:

- `update_application_settings` with `name` did not rename only the label. It renamed the application folder to `Modules/Application.Mediator`, the `.application.config` file, and the `.isln` `relativePath`, and it renamed the Module Builder package to `Application.Mediator`.
- `run_designer_script` with `pkg.setName(...)` suppresses the "Rename underlying file?" prompt and applies the recommended choice. It then wrote the package files to a doubled path under the repo root (`<repo>/home/dandrejvv/.worktrees/.../Modules/Application.Mediator/Intent.Metadata/...`), leaving the application's real `Intent.Metadata` folder with only the two `.designer.config` files. The metadata was effectively split across two trees.

Neither is recoverable by the agent: the files involved live inside `Intent.Metadata`, which agents must not modify. If it happens, stop, report exactly what moved, and let the user decide (the user previously removed such an app and started again).

## Then

- A new module starts at `1.0.0-pre.0` (see `module-version-increment`; set it with `module-versioning`).
- Read `module-building-workflow.instructions.md` for the phases that follow.

## Must

- Do steps 3 and 4 only through the UI, by the user.
- Verify every step by reading the result back from disk or the designer; a successful tool call is not proof.
- Complete step 5 before any Software Factory run.
- Leave the shell in the project root: a persistent `cd` elsewhere makes the repo's agent-gate hook fail with "Couldn't find a project to run". Always `cd` back to the project root.

## Must Not

- Do not use `update_application_settings` `name`, or `pkg.setName` in a script, to do steps 3 or 4.
- Do not run the Software Factory "to see what happens" before step 5.
- Do not edit `.application.config` files or anything under `Intent.Metadata` by hand.
- Do not delete the files and folders creation leaves at the repo root; show the user.
- Do not create several applications before the sequence has been proven on the first one.
