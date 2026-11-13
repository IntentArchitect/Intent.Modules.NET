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

A small destructive change already carries its diff under its line in the `run_software_factory` result — that IS its diff; don't fetch it again. For each other destructive / unknown change, call **`get_file_diffs`** with the file's absolute path (combine the SF result's `outputBasePath` with the change's relative path). It returns a Git-style unified diff of the staged content (a destructive change is never auto-applied, so it is not on disk yet) vs the file on disk.

Read what is being deleted or overwritten. Distinguish generated boilerplate from hand-written / AI-authored logic (operation bodies, custom methods, event handlers, UI behaviour, bespoke helpers).

## 2. Decide — correct deletion, or unintended loss?

- **Correct** — the code is genuinely stale or superseded (e.g. you renamed/removed the modelled element it belonged to, so its generated output _should_ disappear). Let it go; no fix needed.
- **Unintended** — the diff removes hand-written / AI logic that must survive regeneration. This is the case you resolve in the steps below.

The SF also reorders or drops C# `using` declarations that become unnecessary (e.g. a namespace is no longer implicitly required after other output in the file changed). This is normal housekeeping, not a sign of lost logic — treat a diff that is *only* `using` reordering/removal as **correct**, never as destructive or an "outstanding" (unresolved) change.

When unsure, treat it as **unintended** and protect it — losing hand-written logic is far worse than an unnecessary directive.

## 3. Fix — model first, directives only as a fallback

**Prefer modelling the change in the relevant designer** so the SF _reproduces_ it on every run:

- Missing entity / property / value object → add it in the **Domain** designer.
- Missing operation, command/query, DTO field, or service member → add it in the **Services** designer.
- Missing page / component / UI element → add it in the **User Interface** designer.

Model it, re-run the SF, and (for operation bodies etc.) re-fill the generated body. This is the durable fix — the code survives because generation produces it.

**Only when the code is genuinely bespoke / non-modellable** (a private helper, local implementation detail, custom logic the designer cannot represent) protect it in place with a code-management directive, matching how they are already used elsewhere in the codebase:

- `[IntentIgnore]` — the generator ignores the whole member/file and never overwrites it.
- `[IntentManaged(Mode.Merge)]` — merge generated + hand-written changes.
- `[IntentManaged(Mode.Fully, Body = Mode.Ignore)]` / `[IntentIgnoreBody]` — keep the generated signature but protect the body you wrote.

Add the directive to the original, hand-written version — not the overwritten one. While the destructive change is staged, `read_file` and `patch_file` see the SF's staged content, so:

1. Read the original with `read_file` and `readOriginal: true` (or take it from the `a/` side of the diff).
2. Add the directive to that text and save the whole file with `write_file`. The write replaces the SF's staged content for that path (the result says "Staged"); it does not reach disk yet.
3. `apply_staged_file_changes` for that path, so the protected original is on disk before the SF reads it again.

## 4. Verify and surface

- **Re-run the SF** and check that the affected files are no longer flagged `destructive`. If the change is now modelled or protected, regeneration should be **idempotent** — no destructive diff to the hand-written code. Repeat until clean and the build passes.
- **Surface it to the user**: briefly report which changes were destructive, which you judged correct (let go) vs unintended (fixed), and how you fixed each (modelled where, or which directive).
