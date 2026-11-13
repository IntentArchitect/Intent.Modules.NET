---
description: How to run the Software Factory and get its generated code onto disk — run_software_factory applies its own output unless a change is destructive; get_file_diffs and apply_staged_file_changes for the destructive-change branch; and restoring missing output targets ('no output target for role' errors, unassigned Template Outputs). Load after model changes are verified and you need to generate and apply code.
short-description: Run the Software Factory (auto-applies when safe); destructive changes and missing output targets.
intent-updates: automatic
requiredTools:
  - run_software_factory
  - get_file_diffs
  - apply_staged_file_changes
  - read_file
  - run_designer_query
  - run_designer_script
  - run_solution_query
  - install_or_update_modules
group: Code and generation
---

# Generating and Applying Code

After model changes are verified and the requirement is met, run the Software Factory (SF) to generate the boilerplate and architectural code. `run_software_factory` **applies its own output to disk in the same call** unless a change is destructive.

The flow: **`run_software_factory` → build.** Only when it reports destructive changes (nothing applied): `get_file_diffs` (for those without a diff inlined under their line) → resolve → `apply_staged_file_changes`.

## The Software Factory implements NO business logic — assume nothing, implement it yourself

**Never assume the Software Factory implements ANY business logic.** It typically generates **contracts and infrastructure only** — interfaces, DTOs, entity/property scaffolding, WebApi/controller wiring, DI registrations, EF configuration, and method **signatures** with empty or `NotImplementedException`/`TODO` bodies. It does **not** automatically write operation bodies, command/query handlers, domain method bodies, event handlers, mapping logic, validation logic, UI views, or any behaviour. Treat every generated business logic method body as a stub until you have read it and proven otherwise.

Implement business logic, mappings, UI components, tests, etc. yourself with the file tools, loading the application's codebase skills that `run_software_factory` lists.

## 1. `run_software_factory`

- Run it for an application **after** making model changes. It returns when the SF reaches staging (changes calculated and visible in the UI) or when it completes/fails before staging.
- **Do NOT call it repeatedly in a loop** to retry the same unresolved error — investigate and fix the underlying (usually modelling) issue first; never patch generated code to make it compile, that masks the error and is overwritten on regeneration. This does **not** mean never re-run: any model/code change made after a run, including one made while implementing the logic, invalidates it — run it again.
- The response includes an `outputBasePath` and `changes` grouped by change type, one line per file (no contents): `<relative path> +<insertions> -<deletions>`, then `destructive: yes|unknown` when it applies. Changes outside the output root are named in the feedback. Get a change's absolute path by combining `outputBasePath` with its relative path.
- **Applied or staged.** The response's first sentence says whether the changes are on disk. "auto-applied": they are on disk; do NOT call `apply_staged_file_changes`, just build/test. Otherwise they are still staged: a change was destructive (nothing applied), the run had errors, you passed `autoApply: false`, or the apply failed part-way (the feedback says how many were written; the rest are staged). Fix the cause, resolve any destructive change, then `apply_staged_file_changes` for what is still staged. Pass `autoApply: false` only to keep a run staged on purpose.

### Destructive changes

A change can be flagged `destructive: yes` = a file was DELETED or hand-written/customised code was OVERWRITTEN, or `destructive: unknown` = could NOT be verified as safe (no previous template output to compare); no flag = safe or not checked. **Do NOT assume `"yes"`/`"unknown"` are safe.** Whenever any are present, load the **`resolve-destructive-changes`** skill and follow its playbook — inspect each as a diff (inlined under its change line when small, else `get_file_diffs`), decide whether the loss is intended, and prefer reproducing unintended losses by MODELLING them (code-management directives only for genuinely bespoke code). A run with any destructive change applies NOTHING: resolve them, then apply.

### Missing output targets

**Output-target errors** ("Unable to find output target for template…", or "targets the anchor/role … but the Codebase Structure has no output target") are a Codebase Structure problem, not a code problem: `read_file` [references/missing-output-targets.md](references/missing-output-targets.md) and follow it.

## 2. `get_file_diffs`

- Shows what the SF did to one or more files as a Git-style unified diff: the staged content (not yet on disk) vs the file on disk. A file the run already applied shows as `unchanged`, because nothing is staged for it any more: inspect applied files with `git diff -- <files>`, or pass `autoApply: false` to `run_software_factory` to keep them staged for this tool.
- This is the tool to reach for when a change came back `destructive: "yes"`/`"unknown"` — read the diff to see exactly what was deleted/overwritten, then follow the `resolve-destructive-changes` skill.
- Provide ABSOLUTE paths inside an application's output directory. A file with no change yields an empty diff (`status: unchanged`). The aggregated result is a readable multi-file unified diff; per-file metadata (`status`: added / modified / deleted / unchanged) is also returned for programmatic checks.

## 3. `apply_staged_file_changes`

- Writes the given SF staged changes to disk. Only needed when a run left them staged (see **Applied or staged** above), or a `read_file` result notes staged content you need on disk. Its result is a count plus any files that failed or were skipped.
- **Pass the exact relative paths you reviewed** (from `run_software_factory`'s `changes` list or `get_file_diffs`) via `relativePaths` — required and non-empty. It applies ONLY those files; anything else still pending is left untouched. Never guess or assume it applies "everything pending".

## Stop conditions

The task is complete only when **all** hold:

- The requested capability is represented in the appropriate designer(s), with no validation errors (see the **changing-the-model** skill).
- The Software Factory has been run and its changes are on disk.
- The codebase compiles and existing tests pass.
- Required bespoke logic is in place — no `NotImplementedException`, `TODO`, or stubs remain in new files.
- **A fresh `run_software_factory` re-run, taken right before declaring done, proposes zero changes to the bespoke code** (an empty `changes` list; once applied, `git diff` shows what it changed) — an earlier run in the conversation doesn't count, especially after model fixes made while implementing the logic. If it proposes changes, protection is missing/wrong — fix it (model it, or add a code-management ignore directive) and repeat until clean. Ignore any diff that is purely the SF reordering or removing C# `using` declarations (e.g. one becomes implicitly unnecessary) — that is normal housekeeping, not a destructive change or a sign that protection is missing.
