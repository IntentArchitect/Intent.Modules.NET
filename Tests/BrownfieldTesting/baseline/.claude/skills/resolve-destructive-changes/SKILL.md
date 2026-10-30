---
description: Resolve destructive Software Factory changes — inspect each deletion/overwrite as a diff, decide if it is intended, and (if not) reproduce the lost code by modelling it in the designer or protecting it with a code-management directive. Triggered when run_software_factory reports a change as destructive "yes" or "unknown".
short-description: Resolve destructive Software Factory changes — inspect, decide, and model or protect the lost code.
intent-updates: automatic
requiredTools:
  - run_software_factory
  - get_file_diffs
  - write_file
group: Code and generation
---

# Resolve Destructive Software Factory Changes

You are here because you ran the Software Factory (SF) and one or more of its changes came back `destructive: "yes"` (a file was DELETED, or hand-written / customised code was OVERWRITTEN) or `destructive: "unknown"` (the change could NOT be verified as safe — there was no previous template output to compare against). Do **not** assume these are safe. Work through the playbook below for **every** affected file before you consider the run complete.

The guiding principle: the **model is the source of truth**. If regeneration destroyed something that matters, the fix is almost always to make the model (or a code-management directive) reproduce it — not to hand-restore code that the next SF run would just destroy again.

## 1. Inspect — see exactly what changed

A small destructive change already carries its diff under its line in the `run_software_factory` result — that IS its diff; don't fetch it again. For each other destructive / unknown change, call **`get_file_diffs`** with the file's absolute path (combine the SF result's `outputBasePath` with the change's relative path). It returns a Git-style unified diff of the SF's output against its pre-run baseline. This works in **both** modes:

- **Staging mode** — the change is not yet on disk; the diff is the staged content vs the file on disk.
- **Write-through mode** — the change is already on disk; the diff is the file on disk vs the shadow-git baseline (the pre-run content).

Read what is being deleted or overwritten. Distinguish generated boilerplate from hand-written / AI-authored logic (operation bodies, custom methods, event handlers, UI behaviour, bespoke helpers).

## 2. Decide — correct deletion, or unintended loss?

- **Correct** — the code is genuinely stale or superseded (e.g. you renamed/removed the modelled element it belonged to, so its generated output _should_ disappear). Let it go; no fix needed.
- **Unintended** — the diff removes hand-written / AI logic that must survive regeneration. This is the case you resolve in the steps below.

The SF also reorders or drops C# `using` declarations that become unnecessary (e.g. a namespace is no longer implicitly required after other output in the file changed). This is normal housekeeping, not a sign of lost logic — treat a diff that is *only* `using` reordering/removal as **correct**, never as destructive or an "outstanding" (unresolved) change.

When unsure, treat it as **unintended** and protect it — losing hand-written logic is far worse than an unnecessary directive.

## 3. If write-through: restore the pre-run content first

In write-through mode the destructive change is **already on disk** — the hand-written code is gone. Get it back **before** applying any fix, so your fix is built on the original, not the destroyed version:

- Prefer the write-through **Changes panel → Undo** for that file (surface this to the user).
- Or restore it yourself: the pre-run content is the **`a/` (original) side** of the diff you just fetched from `get_file_diffs` — `write_file` that content back to the file.

(Staging mode needs no revert — the destructive content is not on disk yet.)

## 4. Fix — model first, directives only as a fallback

**Prefer modelling the change in the relevant designer** so the SF _reproduces_ it on every run:

- Missing entity / property / value object → add it in the **Domain** designer.
- Missing operation, command/query, DTO field, or service member → add it in the **Services** designer.
- Missing page / component / UI element → add it in the **User Interface** designer.

Model it, re-run the SF, and (for operation bodies etc.) re-fill the generated body. This is the durable fix — the code survives because generation produces it.

**Only when the code is genuinely bespoke / non-modellable** (a private helper, local implementation detail, custom logic the designer cannot represent) protect it in place with a code-management directive, matching how they are already used elsewhere in the codebase:

- `[IntentIgnore]` — the generator ignores the whole member/file and never overwrites it.
- `[IntentManaged(Mode.Merge)]` — merge generated + hand-written changes.
- `[IntentManaged(Mode.Fully, Body = Mode.Ignore)]` / `[IntentIgnoreBody]` — keep the generated signature but protect the body you wrote.

Restore the original code (step 3) **before** adding the directive, so you are protecting the real hand-written version — not the overwritten one.

## 5. Verify and surface

- **Re-run the SF** and inspect `get_file_diffs` again for the affected files. If the change is now modelled or protected, regeneration should be **idempotent** — no destructive diff to the hand-written code. Repeat until clean and the build passes.
- **Surface it to the user**: briefly report which changes were destructive, which you judged correct (let go) vs unintended (fixed), and how you fixed each (modelled where, or which directive). In write-through mode note that the affected files were already on disk and whether you reverted them.
