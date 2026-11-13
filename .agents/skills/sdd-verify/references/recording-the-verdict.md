# What record_spec_verdict checks, and where the verdict lives

Detail for **Record the verdict, then report** in `SKILL.md`.

## It re-runs the change review

`record_spec_verdict` **re-runs the change review itself** and warns in its feedback when untraced changes remain after your verdict. It does not store that review: the untraced lists are solution-wide, so they describe the branch rather than this spec, and your chat report is where they are accounted for. Treat the warning as a prompt to correct the verdict, not as noise.

## Over the range you reviewed

It re-runs over **the range you reviewed**, not a fresh default: the range of your last `get_change_review` call in this conversation, so you do not have to repeat it. Passing `refA`/`refB`/`baseBranch` to `record_spec_verdict` overrides it.

If that range was narrowed, the feedback says so too. An empty untraced list over a working-tree-only baseline is a much weaker result than an empty one over the branch's merge-base; if that is what you got, widen the range with `refA` and record again rather than reporting it as clean.

## The files

`verdict.json` and `test-judgements.tsv` are committed with the spec. The tool hashes each criterion's wording and test body itself into `test-judgements.tsv`. That file merges line by line (`merge=union`), so two branches' bindings combine. When `verdict.json` conflicts on merge, re-run verify rather than merging it by hand.
