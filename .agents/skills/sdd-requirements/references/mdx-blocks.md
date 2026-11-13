# Write it as MDX — which block carries which section

The skeleton and heading conventions are **fixed** — the catalog parser and the live coverage overlays key off them, so never restructure a heading to suit a block. What MDX adds is the form each section's content takes _within_ those headings. Reach for a block **where the content shape calls for it**, not to hit a count: a genuinely narrative section that stays prose is a judgement, not an omission. Load the **authoring-rich-documents** skill for each block's syntax.

| Section | Carried by |
| --- | --- |
| **Introduction**, **Glossary** | Prose and bullets as the skeleton has them — but **every entity that already exists in the model** is an inline `<ModelRef>` (carrying `application`) rather than bare text, so the reader sees at a glance which nouns are new and which already exist (and whether they still do). A Glossary term the model doesn't have yet stays plain text. A related or overlapping spec is a spec link by its slug, `[[other-spec]]`, never a path (see "Linking documents" in authoring-rich-documents' `references/linked-blocks.md`). |
| **Key User Journeys** | One `` ```mermaid `` `flowchart TD` **per journey**, placed after that journey's bullets, encoding the same beats — entry state, each path beat, the climax, the resolution, **and the edge case as a branch**. The bullets stay: the diagram is the shape, the bullets are the detail, and a reader needs both. |
| **A validation gauntlet or a lifecycle** | `<Diagram>` where the layout genuinely needs more than mermaid gives — a multi-stage validation funnel, a state machine with parallel guards. Otherwise a `` ```mermaid `` `stateDiagram-v2` is less to maintain. |
| **A requirement's bounds** | A markdown pipe table (never `<Table>`) immediately **before** the criteria it quantifies — field, type, required, format, length/range, default. This is the step-4 "inputs" probe rendered rather than dissolved into a dozen SHALL clauses. |
| **A requirement's unhappy paths** | A failure-matrix pipe table — condition, exact response, state left behind, what is NOT done — wherever the failures are enumerable. Each failure still gets its own criterion; the table is what makes a gap visible. |
| **A risk or a decision that qualifies a requirement** | `<Callout tone="risk">` / `<Callout tone="decision">` placed **after** the criteria it qualifies, never before them — a callout above the criteria reads as the requirement itself. |
| **Assumptions** | A `<Checklist>`, so each assumption can be worked through and its tick persists across opens. |
| **Open Questions** (rare — normally empty) | `<QuestionForm>`. Its answers post straight back into this conversation, so anything that surfaces _after_ the write still gets resolved rather than sitting there. It does not license finishing with open questions. |

## Why the conventions in SKILL.md matter

- **Back-referencing the criterion id** on every diagram node, table row, caption and callout lets `/sdd-design` map a block to the requirement it realizes and `/sdd-verify` tell whether a block was covered. A block that floats free of the ids is decoration.
- **`Table N.N` captions** (the requirement's number, then its own running index — `Table 13.1 — what each permission mode gates`) are linked by the viewer when cited in prose. A caption written as a criterion id (`R13.1 — …`) collides with the real criterion `R13.1`, so a cite of it can't tell the two apart.
- **Citing every table from a criterion** keeps the criteria the contract. A bounds table is cited by reference ("THE Customer_Service SHALL reject a field outside the limits in Table 2.1"), not restated bound by bound. A table that no criterion cites is one `/sdd-verify` has nothing to check against.

## Self-review for an `.mdx` artifact

Part of SKILL.md's gate — every item below is one a block-free document fails:

- [ ] **Every entity that already exists in the model** and is named in prose is an inline `<ModelRef>` carrying `application` — not bare text, and not a plain-`.md` link in an `.mdx` document.
- [ ] **Every `UJ-N` has a diagram** — a `` ```mermaid `` `flowchart TD` (or a `<Diagram>` where the layout needs it) encoding its beats including its edge case, alongside its bullets.
- [ ] Wherever a requirement's inputs or failures are enumerable, they are a **table** — bounds before the criteria, failure matrix beside them — rather than dissolved into prose.
- [ ] Every diagram node, table row, caption and callout **back-references its criterion id**.
- [ ] Every table is a markdown pipe table, captioned `Table N.N` (not a criterion id) in an italic line under it, and cited in prose by that label.
- [ ] **Nothing normative lives only inside a block** — every table is cited by a criterion (by reference to `Table N.N`).
- [ ] Where a section has NO block, that is a judgement I would defend — the content genuinely is narrative — rather than an omission.