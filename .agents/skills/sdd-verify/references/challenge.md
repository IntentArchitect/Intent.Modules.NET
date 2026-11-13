# Challenge every gap before you record it

A false `gap` sends `/sdd-heal` to change working code against a requirement it already satisfies. So before recording, **try to disprove each candidate gap** — passes need no challenge.

Dispatch **one** `discovery` sub-agent for all the candidate gaps, not one each, briefed to argue the opposite case for every one. Split them across two sub-agents only when there are more than eight. Per candidate:

> Requirement `<id>` is claimed to be unsatisfied because `<candidate reason>`. Read the model and the code and try to show it IS satisfied — the behaviour exists somewhere the reviewer didn't look, the element is linked under a different id, the criterion is met by generated code rather than hand-written code, or the reviewer misread the criterion. Report `refuted` with the exact element id / `file:line` that satisfies it, or `confirmed` with precisely what is missing.

Then:

- **Refuted with evidence** — record a `pass` and cite that evidence in your report.
- **Confirmed** — record the `gap`, folding the validator's specifics into `actual` / `location`; a challenged gap gives the healer a much sharper brief.
- **Inconclusive** — record the `gap` (the safe verdict) and say in the report it was inconclusive.

**Skip the challenge** for disagreements (a test failed or the sceptic already argued it; the user settles them) and for traceability gaps — the `broken`/`uncovered` ones from `coverage` and the untraced changes from `get_change_review` are machine-checked, so record them directly (`change-review.md` says why). The one judgement left on an untraced change is **attribution** (this spec or other work), and that is yours to make from `design.md` / `tasks.md`.
