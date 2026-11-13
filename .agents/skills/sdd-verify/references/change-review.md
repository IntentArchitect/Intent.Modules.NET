# Reading the change review and coverage

Detail for step 1's `get_change_review(section: "traceability", specSlug: "<this spec>")` and step 2's **Coverage** and **Untraced changes** checks in `SKILL.md`.

## The report

It returns a compact text report: the traced map (spec → requirement → the changes realizing it, `[stale]`-marked) followed by the `untraced elements` and `untraced custom files` lists. It is the deterministic review the Changes Review tab renders, over the merge-base → working tree range.

- **`specSlug`** names the one spec whose requirements get expanded. Every other spec in range is reported as a name plus its requirement count, and omitting `specSlug` expands none of them. The untraced lists stay solution-wide either way, because they are the gate. It also decides what counts as traced there: this spec's links count even when recorded before the range, another spec's only when recorded inside it.
- **Rolled-up rows.** Rows are rolled up into their parent: a row ending `(+N child change(s))` is **N+1 changes**, not one — the parent plus N of its attributes / fields / operations that changed too. Read it as a group when you resolve it, and don't mistake it for a single finding.
- **Overflow.** Every list is capped (`maxRows`, default 50) and any overflow is stated in the output with the `offset` of the next page. If you see rows omitted, page through that section before judging, rather than treating the visible rows as the whole set.

## The range line

Check the range line in the feedback before you conclude anything.

- **HEAD fallback.** When no base branch could be resolved (a solution sitting on its own trunk, a single-branch clone) the review falls back to a HEAD baseline and says so. That range covers **only the working tree**, so committed waves are invisible in it. Re-run with an explicit `refA` (the commit the spec's work started from) or `baseBranch` before you conclude that anything is untraced or missing.
- **Reversed or diverged.** A range whose refs aren't a walk from an older baseline to a newer comparand — reversed, or two diverged branches — is also called out on that line. The diff is correct, but added work reads as deleted in a reversed one, so fix the range rather than judging from it.
- **"(last verify)".** When the spec already has a verdict, a defaulted range starts at the commit that verdict recorded, and the range line says "(last verify)". That verify resolved every untraced change before it. Pass an explicit `refA` only when you need the whole branch.

Whichever range you settle on is the one `record_spec_verdict` re-checks (see `recording-the-verdict.md`).

## What is never an untraced finding

Three things are deliberately **not** untraced findings and never appear in the lists:

- a deleted element (nothing is left to link);
- a `FullyManaged` file (generated output — its traceability belongs on the model element that produced it);
- a test file (its title cites the criteria it asserts).

## What a broken or uncovered requirement usually means

`complete_spec_task` refuses to tick a `[model]`/`[code]` task while its designer is unsaved **or while any of that task's recorded links is broken**, and `record_spec_traceability` rejects a target that doesn't resolve rather than storing it. So a wave that reached "all ticked" implies its links resolved at the time.

- A **`broken`** link at verify time means it broke **after** the tick — a later rename or delete, or a hand-edited `traceability.json`.
- An **`uncovered`** requirement usually means a rejected link was never re-recorded.

Both are still gaps; the distinction is what the healer should go looking for, so put it in the gap's `actual`.

## Why traceability gaps skip the challenge

Both the `broken`/`uncovered` requirements `read_spec(slug, artifact: "coverage")` computed and the untraced changes `get_change_review` computed are already machine-checked: a link either resolves or it doesn't, and a changed node either carries a requirement ref or it doesn't. There is nothing for a sceptic to refute, so dispatching one only burns a turn.
