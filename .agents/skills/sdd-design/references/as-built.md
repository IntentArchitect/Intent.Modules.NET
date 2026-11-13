# As-built mode (extraction)

When `/sdd-extract` loads this skill, or `/sdd-run` hands it a Capability, the user has decided to **document the design as it exists** — that is a user decision, so these deltas override the design rules in SKILL.md:

- **Describe, never propose.** No module search, no trade-offs to put to the user, and no `ask_user_question`. Application prerequisites, module prerequisites and model changes are all **"none"**.
- Explain the design with **diagrams**: a `` ```mermaid `` flowchart or sequence for the runtime flow, and a `<Wireframe>` for any UI.
- Use a `<FileRef>` for every file you name and a `<ModelRef>` for every modelled element.
- If a designer models any part of the feature, add a **Services contract** table (operation, inputs, result, the requirement it serves).
- The **realization table** maps each requirement to its `[model]` elements, its key files and the tests that cover it. This is what traceability reads, so leave no requirement out.
- Decisions become `<Callout tone="info">` notes on why the code is the way it is, where the code or its history shows it. Don't invent rationale.
