---
description: The structured MDX block reference for Intent Architect's document viewer — Callout, Diagram, FileTree, DataModel, ApiEndpoint, Diff, AnnotatedCode, Code, Checklist, QuestionForm, TabsBlock, the visual surfaces (ModelChanges, Wireframe, Canvas, Mockup), plus the live model- and file-linked blocks (ModelRef, FileRef, ApprovalGate). Load before authoring ANY `.mdx` document — which is the default form of a plan, a recap, and a spec's `requirements` / `design` artifact.
short-description: The MDX block reference for Intent's document viewer. Load before authoring any .mdx document.
intent-updates: automatic
requiredTools:
  - write_plan
  - write_spec
  - write_file
  - read_file
group: Documents, preview and tasks
---

# Authoring rich documents

Intent Architect renders `.md` and `.mdx` through the same document viewer. `.mdx` adds structured blocks — diagrams, split diffs, tables, checklists, question forms — and **live** blocks that resolve against the designer model and workspace every time the document is opened. This skill is the single reference for them; the plan agent, `sdd-requirements` and `sdd-design` point here.

## `.mdx` is the default — but only once THIS skill is loaded

Plans, recaps, and a spec's `requirements` / `design` artifacts are `.mdx` unless the user or team chose plain markdown. `write_plan` / `write_spec` follow the team setting (`.agents/team-settings.json`), then the user's default format, then `mdx`; pass `format` only when the user asked for one.

On the FIRST `write_plan` / `write_spec` call for a new document, the tool checks whether this skill was loaded (`use_skill`, or the bridged native `Skill` tool) earlier in your own conversation; a sub-agent's own load counts. If not, the document is written as `.md`, and later writes without `format` keep that extension. **So `use_skill` this skill BEFORE your first write, every time you intend a rich document.** If a document did come out as `.md` when it should be rich, write it again in full with `format: "mdx"`: that converts it and removes the `.md`.

Most of an MDX document is still Markdown prose; a document with no blocks should have been `.md`. But when the content IS structural — a shape change, a before/after, options, an open decision — use the block that shows it: a `<DataModel>` or `<Diff>` reads in one glance, and a wrong prose description is easier to skim past than a rendered one is to misread.

**Lean towards a picture wherever it reads better** (a strong preference, not a rule). A flow, sequence, hierarchy, state machine, before/after or screen layout lands faster drawn; keep prose for the _why_. A short document that genuinely is an argument stays prose. Two encouraged habits:

- **Colour mermaid diagrams** where colour carries meaning — new vs existing vs removed, layer or actor, happy vs error path. Colour by _category_, never by node; an uncoloured diagram is fine.
- **Show UI changes visually** — `<Wireframe>` for one state, `<Canvas>` for several, `<Mockup>` when the app has a mockup snapshot and the real look matters. A described screen is what readers most often reconstruct wrong.

## MDX is stricter than Markdown

The whole file is parsed as JSX, so one stray character fails the ENTIRE document:

- **A bare `<` is a syntax error.** `if x < 5`, `List<Order>`, `<T>`, `<your-name-here>` in prose go in backticks, a fenced block, or as `&lt;`.
- **A bare `{` is a syntax error** — it opens a JSX expression. Backtick it or write `&#123;`.
- **`{name}` placeholders fail too** — `{min}` is a valid JS expression, so it compiles, then throws `ReferenceError` when the viewer renders. Quote UI messages like `Enter between {min} and 24` inside backticks, or write `&#123;min&#125;`.

Backticked spans and fences are never parsed as JSX; prose is the danger. When in doubt, backtick it. **Never rename an existing `.md` to `.mdx`** — it will usually fail on the first `<`. Author `.mdx` from the start, or leave it `.md`.

**Multi-line JSX attributes lose two spaces per continuation line.** A `before={…}` template literal pasted verbatim renders its first line two spaces deeper than the rest — pre-pad continuation lines by two spaces, or write the excerpt flush-left.

## Every `.mdx` write is validated — a reported failure is yours to fix, now

`write_spec`, `write_plan` and `write_file` compile every `.mdx` exactly as the viewer will. **The write is NOT rolled back**: the tool returns success with the broken document on disk. When a result says `WILL NOT RENDER`, fix it **in that same turn, before anything else**. It names the line and reason for:

- **A syntax error** — a bare `<` or `{`, a stray `\` in a JSX attribute, an unclosed block.
- **A block that doesn't exist** (see the closed set). Most likely and worst: it compiles, then throws at render and fails the **entire document**.
- **A `` ```mermaid `` fence mermaid cannot parse** — checked with the drawing engine, reported at the diagram line with mermaid's reason.
- **A block missing a prop it can't render without** — a warning; the document still renders.

Fix it with `searchBlock` / `replaceBlock` on the same `write_spec` / `write_plan` call (the `patch_file` contract), not by re-sending the document. Both sides are **literal document text, never a diff** — a leading `-` is a bullet and `---` a front-matter fence. `.md` is never compiled, except that its mermaid fences are checked (they render in `.md` too).

## Never author a spec's `tasks` artifact as MDX

`tasks.md` is always plain markdown. Its GitHub checkboxes **are** the wave-progress engine: the Specs panel ticks them, the implementation cockpit derives progress from them, and a Tasks reset clears them. A `<Checklist>` there shows a spec with zero tasks. `write_spec` refuses `format: "mdx"` for `tasks`; don't route around it.

## Rules for every document

- **Every block needs a stable, unique `id`.**
- **Every existing model element named in prose is a `<ModelRef>`**, not bare text — that is how a document knows whether `OrderLine` still exists. An element the document only proposes goes in a mermaid fence, `<DataModel>` or `<ModelChanges>` instead.
- **Every edge case considered and tradeoff accepted gets its own `tone="warning"` `<Callout>`**, beside the part it constrains: the case, what happens, why it's acceptable. Don't manufacture them.
- **Model shape:** a `` ```mermaid `` `classDiagram` / `erDiagram` fence scoped to the touched elements plus neighbours; then `<DataModel>` for one entity's fields, `<ModelChanges>` for a proposed delta, `<ModelRef>` inline. Never paste a designer screenshot — it goes stale.
- **Mermaid fences** render in `.mdx` and `.md` (contents are exempt from MDX escaping). At most ~10–12 nodes — split otherwise, one idea each. `flowchart LR` for wide shallow flows, `TD` for deep. Labels of 4 words or fewer; group with `subgraph`. **Never put a `"` inside a `["…"]` label, escaped or not.** When colouring, **always set `color:` alongside `fill:`** (light and dark themes) — recipe in `references/structural-blocks.md`.

## Block reference — a CLOSED set

`Callout` · `Diagram` · `FileTree` · `TabsBlock` · `DataModel` · `ApiEndpoint` · `Diff` · `AnnotatedCode` · `Code` · `Checklist` · `QuestionForm` · `ModelRef` · `FileRef` · `ModelChanges` · `Wireframe` · `Canvas` · `Mockup` · `ApprovalGate`

**Tables are always markdown pipe tables** (see `references/structural-blocks.md`). `<Table>` is deprecated: it still renders in older documents, and every one in a write is reported as a warning. When editing such a document, convert the `<Table>`s in the part you change and leave the rest.

Any other capitalized JSX tag fails the **whole document** — no plain-text fallback. If none fits, use prose (see **Known gaps**). Retired tags still render as a "no longer supported" note but are reported as unknown in a NEW document:

- `<ModelDiagram>` — use a mermaid `classDiagram` / `erDiagram` fence (`<ModelChanges>` for field detail).
- `<ModelView>` — use a mermaid `classDiagram` fence, `<DataModel>` / `<ModelChanges>` for field detail, `<ModelRef>` for live naming. (It showed neither associations nor stereotypes, and its drift check fired on any new member.)

The three every document uses:

```mdx
<ModelRef application="MyApp.Sales" name="OrderLine" designer="Domain" type="Class" />
<FileRef path="src/handlers/CreateOrder.cs" line={42} />
<Callout id="concurrent-edit" tone="warning" title="Concurrent edits">

Last write wins. Accepted because the designer reloads on external change.

</Callout>
```

`application` (display name or id) is **required** on `<ModelRef>`. `FileRef` paths are repo-root relative. Callout tones: `info` | `decision` | `risk` | `warning` | `success`.

Every other block's props live in `references/`. `use_skill` reports this skill's base directory; `read_file` the block's reference **before** authoring it — never author a block from memory.

| Block            | Use it for                                                                          | Reference                                                                        |
| ---------------- | ----------------------------------------------------------------------------------- | -------------------------------------------------------------------------------- |
| `Callout`        | A decision, risk, warning or accepted tradeoff                                      | `references/structural-blocks.md`                                                |
| `Diagram`        | Bespoke HTML+CSS diagram mermaid can't lay out                                      | `references/structural-blocks.md`                                                |
| `FileTree`       | Files touched, with change badges                                                   | `references/structural-blocks.md`                                                |
| `Checklist`      | Persistent verification checkboxes (never spec tasks)                               | `references/structural-blocks.md`                                                |
| `QuestionForm`   | Open questions — one block at the very bottom                                       | `references/structural-blocks.md`                                                |
| `DataModel`      | An entity's fields or a schema change                                               | `references/code-and-data-blocks.md`                                             |
| `ApiEndpoint`    | An API/route change                                                                 | `references/code-and-data-blocks.md`                                             |
| `ModelChanges`   | Proposed designer changes (static)                                                  | `references/code-and-data-blocks.md`                                             |
| `Diff`           | Split before/after code                                                             | `references/code-and-data-blocks.md`                                             |
| `AnnotatedCode`  | New code with line notes, no "before"                                               | `references/code-and-data-blocks.md`                                             |
| `Code`           | A plain snippet                                                                     | `references/code-and-data-blocks.md`                                             |
| `TabsBlock`      | Several file diffs under one tab strip                                              | `references/code-and-data-blocks.md`                                             |
| `ModelRef`       | Inline live chip for an existing element                                            | `references/linked-blocks.md`                                                    |
| `FileRef`        | Inline live chip for a codebase file                                                | `references/linked-blocks.md`                                                    |
| `[[wiki-links]]` | Linking another document or spec                                                    | `references/linked-blocks.md`                                                    |
| `ApprovalGate`   | Approving a spec phase inside a spec artifact                                       | `references/linked-blocks.md`                                                    |
| `Wireframe`      | One screen state                                                                    | `references/wireframe.md` — **always read first**                                |
| `Canvas`         | Two or more screen states on a board                                                | `references/canvas.md` **and** `references/wireframe.md` — **always read first** |
| `Mockup`         | A design-fidelity HTML mockup file of a real screen, before/after                   | `references/mockup.md` — **always read first**; build the file with `mockup-kit` |

Never author a `<Wireframe>` or `<Canvas>` from memory: a fragment written without those references silently loses content to the sanitizer or renders illegibly in one theme.

## Writing plain `.md` instead

`.md` renders through the same viewer, progressively enhanced: fenced code becomes the themed `Code` block, mermaid fences render, `intent://element/...` and relative source-file links become live `<ModelRef>` / `<FileRef>` chips, and a `requirements` / `tasks` artifact under `intent/.specs/<slug>/` gains coverage chips and wave progress. It cannot have the structured blocks — no `<Diagram>`, `<DataModel>`, `<ApiEndpoint>`, `<Diff>`, `<QuestionForm>`.

Write `.md` when the user asked for it, the document is all prose, or the content is dense with characters MDX would choke on (inline generics, template syntax, embedded markup). `tasks` is always `.md`.

## Known gaps — do not use these yet

- **Interactive prototypes.** No click-through/hotspot tier: no `Prototype` tag, no `data-goto`. Show a flow as a `<Canvas>` of the states with connectors.
- **Design-fidelity mockups inline.** `<Wireframe>` is a structural sketch — no brand colours, imagery or pixel-accurate styling; don't force it through inline `style`. For the real look, build a mockup file with the `mockup-kit` skill and embed it with `<Mockup>`.

`WireframeBlock`, `Screen`, `Prototype` and `MockupBlock` do not exist and will fail to render. The block is `<Wireframe>`.
