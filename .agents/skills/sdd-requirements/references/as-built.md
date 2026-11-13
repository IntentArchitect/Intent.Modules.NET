# As-built mode (extraction)

When `/sdd-extract` loads this skill, the user has decided to **document a feature that already exists** — that is a user decision, so these deltas override the interview rules in SKILL.md:

- **A Capability, not a Change.** `/sdd-extract` runs its own guard and makes the first write with `kind: "capability"` and the Capability's `parent` Area, so skip step 1.
- **No interview.** Every open question becomes an item in the `## Assumptions` `<Checklist>`, stated as what the code does today. `## Open Questions` stays empty, and you do not call `ask_user_question` here — `/sdd-extract` owns the questions.
- **Describe, never propose.** Every criterion states what the code does now, quirks included. A behaviour that looks wrong is still written down as it behaves; `/sdd-extract` raises suspected bugs separately.
- **Evidence or nothing.** Every criterion must rest on a behaviour from the inventory you were handed, which carries `file:line` evidence. Do not add a criterion the code can't show.
- **Cite by id.** A criterion that names another cites its full id, `R4.2`, never "criterion 2", so the viewer links it. One in another spec is a spec link anchored at its full id, `[[other-slug#R4.2]]`, never `other-slug R4.2`.
- **Keep the prose non-technical.** No file names, class names or paths in the document — the evidence lives in design and traceability.
- The Introduction says in one sentence that this spec was **extracted from the existing implementation**.
- Journeys, Glossary and Non-Goals still apply: Non-Goals are the neighbouring features the scope pass put out of bounds.
- The self-review items about interviewing the user are satisfied by the inventory instead.
