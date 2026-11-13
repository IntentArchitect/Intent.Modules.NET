# The design document's shape (MDX)

Everything that isn't a block stays plain Markdown prose. The table below says which content shape earns which block — reach for one **where the content calls for it**, not to hit a count. Write the sections in this order, so the design reads from the high level down to the detail, and lead every section that has a shape with its diagram.

| Section | Carried by |
| --- | --- |
| **Overview** | Open with a `` ```mermaid `` `flowchart` or `sequenceDiagram` of the feature's runtime flow end to end, then prose. Every element that ALREADY exists is an inline `<ModelRef>`, never bare text. Close with one `<Callout tone="info">` summarising the scope in counts (N entities, M commands, K queries). |
| **Decisions confirmed with the user** | Consecutive `<Callout tone="decision">` (and `tone="risk"` where the choice carries one), **one per decision**, each closing with a bold **Why:** line. These are the answers you gathered — a design that resolved five trade-offs and mentions none of them reads as if it had no choices to make. |
| **Module & architecture prerequisites** | One `<Callout tone="warning">` — it blocks the first wave, and a warning is what a blocker looks like. Name each module's id + concrete version and the requirement(s) it satisfies. Say so explicitly if none is needed. A **missing application** for a codebase area belongs in the same callout, or in a `tone="decision"` one when the user chose to leave the area outside Intent. |
| **Domain model — aggregate shape** | A `` ```mermaid `` `classDiagram` with multiplicities and stereotypes on the members, scoped to what this change touches plus its immediate neighbours, followed by prose reading the notation back for a reader who doesn't parse diagrams. |
| **Per-entity / per-DTO detail** | One `<DataModel>` per entity and per DTO. `entity` names the ROLE (aggregate root / compositional child / value object), and each field carries its type, its `change`, and the stereotype that realizes its bound. A prose list of an entity's fields is the thing this block exists to replace. |
| **Endpoints** | One `<ApiEndpoint>` per Command / Query. `caption` carries the security posture, the interaction type, the return type and the criterion ids it satisfies. |
| **Realization plan** | One table with a row per requirement and columns for `[model]` / `[code]` / Depends-on. This is what `sdd-tasks` projects its task list from, so it has to be complete and mechanical rather than narrative. |
| **Hand-written code** | Prose, plus a `<Diff>` or `<AnnotatedCode>` where you are specifying an actual code change precisely enough to be worth showing. |
| **Assumptions** | A `<Checklist>` — each assumption then gets worked through and its tick persists, which a bullet list can't do. |

## Conventions the whole document depends on

- **Every element that exists in the model is a `<ModelRef>` carrying `application`.** The chip shows its CURRENT name, its own type icon, and strikes through if it has since been deleted — which is what stops a design quietly describing a model that has moved on. Where a name is duplicated, address it by the package-rooted `Package/Folder/Name` path `searchElements()` returns. An element this design is _proposing_ does not exist yet: name it in the mermaid fence, the `<DataModel>` or a `<ModelChanges>` entry, never as a `<ModelRef>` that would correctly render broken.
- **Every source file you name is a `<FileRef>`.** It shows red when the path doesn't exist — a cheap, standing check that the `[code]` bucket names real files.
- **Don't restate model structure you can point at.** Describe the DELTA and `<ModelRef>` the element.
- **Link a related spec by its slug**, `[[other-spec]]` or `[[other-spec/design#heading]]`, never by a path. See "Linking documents" in authoring-rich-documents' `references/linked-blocks.md`.

In a plain `.md` design (only when the user asked for one), the same live chips are available as `[Order](intent://element/Order?designer=Domain&application=MyApp)` links — but none of the blocks above are.

## Document self-review

A design that is correct but reads as a wall of prose has thrown away most of what this viewer can do, and every item below is one a block-free document fails:

- [ ] **Every element that already exists** and is named in prose is an inline `<ModelRef>` carrying `application` — not bare text, and not a plain-`.md` link in an `.mdx` document. Every source file I name is a `<FileRef>`.
- [ ] **The document reads high level to detail, diagram first.** The Overview opens with the runtime flow as a `` ```mermaid `` fence, and every structural relationship — the aggregate shape — is a fence placed before the prose and tables about it, not a paragraph describing one.
- [ ] **A related spec is linked by its slug** — `[[other-spec]]`, `[[other-spec/design#heading]]` — never by a path into `intent/.specs`.
- [ ] **Every entity's and DTO's fields** are a `<DataModel>`, and **every Command / Query** is an `<ApiEndpoint>`. No entity's shape is given as a prose list or an ad-hoc table.
- [ ] The **realization plan is a table** with `[model]` / `[code]` / Depends-on columns, complete enough for `sdd-tasks` to project a task list from mechanically.
- [ ] Every **decision I settled with the user** is its own `<Callout tone="decision">` (or `tone="risk"`) with a bold **Why:**, and module prerequisites are a `<Callout tone="warning">`.
- [ ] **Nothing normative lives only inside a block.** A block presents; a claim the implementation must honour is also stated in prose, so a reader skimming the text never misses a requirement.
- [ ] Where a section has NO block, that is a judgement I would defend — the content genuinely is prose — rather than an omission.
