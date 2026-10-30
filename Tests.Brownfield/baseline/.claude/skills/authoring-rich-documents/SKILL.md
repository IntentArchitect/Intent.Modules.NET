---
description: The structured MDX block reference for Intent Architect's document viewer — Callout, Diagram, FileTree, Table, DataModel, ApiEndpoint, Diff, AnnotatedCode, Code, Checklist, QuestionForm, TabsBlock, the visual surfaces (ModelChanges, Wireframe, Canvas), plus the live model- and file-linked blocks (ModelRef, FileRef, ApprovalGate). Load before authoring ANY `.mdx` document — which is the default form of a plan, a recap, and a spec's `requirements` / `design` artifact.
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

Intent Architect renders `.md` and `.mdx` through the same document viewer. `.mdx` adds the structured blocks below — diagrams, split diffs, tables, checklists, question forms — and the **live** blocks that resolve against the designer model and the workspace every time the document is opened.

This skill is the single reference for those blocks. The plan agent, `sdd-requirements` and `sdd-design` all point here rather than carrying their own copy.

## `.mdx` is the out-of-the-box default — but it rides on THIS skill being loaded, not on remembering to ask for it

Plans, recaps, and a spec's `requirements` / `design` artifacts are written as `.mdx` unless the user, or the team, chose plain markdown. They are structural documents — what changes, where, before vs after, which model elements — and that is exactly what the blocks below show and prose does not. `write_plan` and `write_spec` follow the team setting (`.agents/team-settings.json`), then the user's default format, and fall back to `mdx`; pass `format` only when the user asked for a specific one.

That default is enforced mechanically, not left to remembering a prompt instruction: on the FIRST `write_plan`/`write_spec` call for a new document, the tool checks whether **this skill** has already been loaded (via `use_skill`, or the bridged native `Skill` tool) somewhere earlier in the conversation. If it has, the document is written as `.mdx`. If it hasn't, the document is written as plain `.md` instead — silently, no error — because handing out `.mdx` to a caller that never read the escaping rules below is how a document ends up broken. **So `use_skill` this skill BEFORE your first write, every time you intend a rich document** — don't rely on the tool's stated default alone. The gate only applies to that first write: the extension it picks is permanent for that document (a later call reusing the same file cannot switch it), so loading the skill one call too late means living with `.md` for that document — the fix is loading it before the next one.

Being MDX does not mean every paragraph becomes a block. Most of the document is still plain Markdown prose; reach for a block where a diagram, a comparison, a file list, or a live model reference genuinely beats a paragraph. A document with no blocks at all should have been `.md`. That said, when the content IS structural — a shape change, a before/after, a set of options, an open decision — reach for the block that shows it rather than describing it in prose: a reader scanning a `<DataModel>` or a `<Diff>` gets the shape in one glance that a paragraph would take three sentences to establish, and a wrong plain-prose description of a diff is far easier to skim past than a rendered one is to misread.

### Lean towards a picture wherever a picture reads better

Prose is the fallback for anything with a shape. A flow, a sequence, a hierarchy, a state machine, a before/after, a screen layout — each of those lands faster as a drawing than as a paragraph describing the drawing. So when you catch yourself writing "A calls B, which fans out to C and D", draw it and keep the prose for the _why_ a picture can't carry. A strong preference, not a rule: a short document that genuinely is an argument stays prose.

Two habits follow from it, both encouraged rather than required:

- **Use colour in mermaid diagrams** where colour carries meaning the topology can't — new vs existing vs removed, which layer or actor a node belongs to, the happy path vs the error path. See the `classDef` recipe under `<Diagram>` below. Colour by _category_, never by node: colour that encodes nothing is noise, and a plain uncoloured diagram is perfectly fine.
- **Show UI changes visually.** Where a change alters what the user sees, show it — `<Wireframe>` for one state, `<Canvas>` for several. A paragraph describing a screen is the thing a reader has to reconstruct in their head, and the thing they most often reconstruct wrong.

### MDX is stricter than Markdown — this is the one real cost

The whole file is parsed as JSX, so a single stray character fails the ENTIRE document with a compile error instead of rendering. Two rules cover it:

- **A bare `<` is a syntax error.** `if x < 5`, `List<Order>`, `<T>`, `<your-name-here>` in prose must go inside backticks (`` `List<Order>` ``), inside a fenced code block, or be written as `&lt;`.
- **A bare `{` is a syntax error** — it opens a JSX expression. Put it in backticks or write `&#123;`.

Backticked spans and fenced code blocks are safe: nothing inside them is parsed as JSX. Prose is where the danger is. When in doubt, backtick it.

Because of this, **never rename an existing `.md` document to `.mdx`** to gain the blocks — it will usually fail to compile on the first `<` it meets. Author it as `.mdx` from the start, or leave it `.md`.

### Every `.mdx` write is validated — and a reported failure is yours to fix, now

`write_spec`, `write_plan` and `write_file` compile every `.mdx` they write, exactly as the viewer will, and report what would break. **The write is NOT rolled back**: the tool still returns success and the broken document is genuinely on disk, so the report in the tool result is the only thing standing between you and a committed document that shows the reader an error instead of your work.

So when a result says `WILL NOT RENDER`, fix it **in that same turn, before anything else**. It names the line and the reason. Four things get caught:

- **A syntax error** — a bare `<` or `{`, a stray `\` in a JSX attribute, an unclosed block.
- **A block that doesn't exist** (see the closed set below). This is the most likely one, and the worst: it compiles cleanly and then throws at render, so it fails the **entire document**, not just that block.
- **A `` ```mermaid `` fence that mermaid itself cannot parse** — checked with the same engine that draws it, so the verdict is what the reader would have seen: an error box where the diagram should be. Reported at the offending line of the diagram, with mermaid's own reason.
- **A block missing a prop it can't render without** — reported as a warning, since the document does still render.

Fix it with a targeted patch — `searchBlock`/`replaceBlock` on the same `write_spec` / `write_plan` call (the `patch_file` contract, same tolerances) — not by re-sending the whole document. A design artifact runs to thousands of tokens; re-emitting it to correct one line is how a correction gets skipped.

Both sides are **literal document text**, never a diff. There is no unified-diff form: in prose a leading `-` is a bullet and `---` is a front-matter fence, so the diff marker column collides with the content itself. Send the text as it currently reads, and the text you want in its place.

Compilation only applies to `.mdx`. `.md` is never compiled, by the viewer or the validator — with one exception: a `` ```mermaid `` fence renders as a diagram in `.md` too, so its grammar is checked in either format.

### Never author a spec's `tasks` artifact as MDX

`tasks.md` is always plain markdown — it is the one artifact the `.mdx` default does NOT apply to. Its GitHub-style checkboxes are not decoration — they **are** the wave-progress engine: the Specs panel ticks them in place, the implementation cockpit derives wave progress from them, and a Tasks reset clears them. An MDX `<Checklist>` there would silently show a spec with zero tasks and no way to advance. `write_spec` refuses `format: "mdx"` for `tasks`; don't try to route around it.

## Block reference

**This is a CLOSED set — the complete vocabulary, not a sample:**

`Callout` · `Diagram` · `FileTree` · `TabsBlock` · `Table` · `DataModel` · `ApiEndpoint` · `Diff` · `AnnotatedCode` · `Code` · `Checklist` · `QuestionForm` · `ModelRef` · `FileRef` · `ModelChanges` · `Wireframe` · `Canvas` · `ApprovalGate`

Any other capitalized JSX tag fails the **whole document**, not just that block — MDX throws on the first component it cannot resolve, so one invented name means the reader sees an error page instead of your work. There is no "renders as plain text" fallback. If none of these fits, use Markdown prose; see **Known gaps** at the end for what to reach for when a block you expected to exist doesn't.

Two tags have been **retired**. They still render — as a "no longer supported" note — so documents written before their retirement stay readable, but emitting one in a NEW document is reported as an unknown block:

- `<ModelDiagram>` — draw model shape as a `` ```mermaid `` `classDiagram` / `erDiagram` fence instead (`<ModelChanges>` still carries the field-level detail).
- `<ModelView>` — draw the shape as a `` ```mermaid `` `classDiagram` fence, give the field-level detail with `<DataModel>` for an entity or `<ModelChanges>` for a proposed delta, and name the element inline with `<ModelRef>` where you want live resolution. It was the only block that read an element's members at render time, but it showed neither associations nor stereotypes and its drift check fired on any unrelated new member — so a hand-listed shape reintroduced exactly the snapshot rot it existed to prevent.

Every block needs a stable, unique `id`.

### `<Callout>`

A tinted, bordered aside for a decision, risk, or warning called out from the surrounding prose.

```mdx
<Callout id="compat" tone="warning" title="Optional heading">

Body text — regular Markdown, rendered inline.

</Callout>
```

- `tone`: `info` | `decision` | `risk` | `warning` | `success` (default `info`).
- `title`: optional short heading.
- **Every edge case you considered and every tradeoff you accepted goes in a `tone="warning"` Callout.** A case the change does not handle, a limit it inherits, a simpler option taken knowing what it costs — each gets its own warning box, not a parenthetical mid-paragraph and not silence because you judged it acceptable. This is the content a reviewer most needs to be able to disagree with, and the content prose hides best: inside a paragraph it reads as detail and gets skimmed; in a warning box it reads as a decision and gets challenged. Omitting it entirely is the worst option — the reader then assumes you never thought of it.
- State each one as the case plus what happens in it plus why that is acceptable — "Two agents editing the same package concurrently: last write wins. Accepted because the designer already reloads on external change." Put it beside the part of the design it constrains rather than sweeping every caveat into a section at the end, and keep one Callout per tradeoff so each can be argued with separately. Don't manufacture them: a change with no real edge cases needs no warning box.
- Tones are not interchangeable. `warning` is for a case or cost you have already accepted; `risk` for something that might go wrong and is still open; `decision` for the choice itself, with the alternatives; `info` for context; `success` for what is now proven.

### `<Diagram>`

An architecture/flow/before-after diagram authored as plain HTML + CSS (not an image). Two equivalent forms — the `data={{ html, css }}` prop, or fenced `` ```html ``/`` ```css `` code blocks as children:

```mdx
<Diagram
  id="flow"
  caption="Optional caption under the diagram"
  data={{
    html: `<div class="wrap">
  <div class="diagram-panel">
    <div class="diagram-node">Step one</div>
    <div class="diagram-arrow">&#8595;</div>
    <div class="diagram-node">Step two</div>
  </div>
</div>`,
    css: `
.wrap { display: flex; gap: 16px; }
.diagram-panel { display: flex; flex-direction: column; align-items: center; gap: 6px; padding: 12px; }
`,
  }}
/>
```

- Prefer the `data` prop for anything non-trivial: the HTML is one string, so there is no ambiguity about what the renderer picks up, and a raw `<` inside it is safe. Its one cost is that a literal backtick or `${` inside the markup must be escaped, since it is a template literal.
- Use the `.diagram-panel`/`.diagram-node`/`.diagram-card`/`.diagram-box`/`.diagram-pill`/`.diagram-badge`/ `.diagram-accent`/`.diagram-mono`/`.diagram-muted` class-name conventions for structural elements — the renderer forces consistent color/border/typography onto these regardless of your CSS, so the diagram stays legible without you having to theme it by hand. Layout (flex/grid direction, gaps, sizing) is entirely up to your CSS.
- Use two-dimensional layouts (side-by-side panels, layers, quadrants) for anything that isn't genuinely a straight-line sequence.
- The CSS you provide is automatically scoped to this one diagram — it never leaks to the rest of the page or other diagrams, so ordinary class names (`.node`, `.panel`) are safe to reuse across diagrams.
- For standard architecture shapes — flowchart, sequence, ER/class, state — a plain `` ```mermaid `` fence is usually faster to author and renders as a diagram in **both** `.mdx` and `.md` (fence contents are exempt from MDX escaping). Reach for `<Diagram>` when you need bespoke layout the mermaid diagram types don't give you.
- **Colour a mermaid diagram wherever colour adds meaning.** `classDef` plus `:::name` is the whole recipe, and a one-line key in the caption saves the reader guessing what the colours mean:

  ```mermaid
  flowchart LR
    classDef added fill:#1f7a4d,stroke:#34d399,color:#ffffff
    classDef changed fill:#8a6d1f,stroke:#fbbf24,color:#ffffff
    classDef existing fill:#3f4b5b,stroke:#94a3b8,color:#ffffff
    A[Existing handler]:::existing --> B[New validator]:::added --> C[Order aggregate]:::changed
  ```

  The fence renders on mermaid's `default` theme in light mode and its `dark` theme in dark mode, so **always set `color:` alongside `fill:`** — a fill with no explicit text colour inherits the theme's and goes unreadable in one of the two. Fills dark enough for white text work in both. Subgraphs take `style <name> fill:…,stroke:…` the same way. Keep it to a handful of categories; a rainbow of one-off node colours reads as decoration and gets ignored.
- **Keep diagrams readable.** At most about 10–12 nodes per diagram — split otherwise, one structural idea each. `flowchart LR` for wide shallow flows, `TD` for deep ones. Labels of 4 words or fewer, with detail in the prose. Group with `subgraph` rather than long edge lists.
- **Never put a `"` inside a mermaid `["…"]` label, escaped or not.**

### `<FileTree>`

A compact list of files touched, with change badges.

```mdx
<FileTree
  id="files"
  title="Optional heading"
  entries={[
    { path: "src/foo/Bar.ts", change: "modified", note: "One-line reason this file changed" },
    { path: "src/foo/Baz.ts", change: "added" },
  ]}
/>
```

- `change`: `added` | `modified` | `removed` (or `deleted`) | `renamed`.
- `note`: optional short reason, shown beside the path.
- `snippet`: optional short code excerpt shown under the row — only include when it tells the reader something the path/note doesn't.
- Every row is clickable: a row with a `change` badge opens that file's diff, a row without one opens the file. So paths must be real and correctly spelled — a made-up path renders as a dead row.
- Write each `path` **relative to the repository root**, exactly as for `<FileRef>` below. A badged row keeps its badge colour rather than turning red, so a wrong path here is invisible until someone clicks it — which is why the write-time check reports one that resolves nowhere. Only the rows whose file is MEANT to be missing — `added`, `removed`, `deleted` — are exempt; a `modified` or `renamed` row is checked.

### Tables: a markdown pipe table by default, `<Table>` only when you need what it adds

**Write a plain GFM pipe table by default** — in `.mdx` as in `.md`. It renders identically (same inline markdown in cells, same zebra rows), reads as a table in the raw source and on GitHub, and an edit to one row diffs as that row. Put a caption, if you want one, as an italic line under it.

```markdown
| Option   | Tradeoff                          |
| -------- | --------------------------------- |
| Option A | Simple, but doesn't scale past X  |
| Option B | More moving parts, handles X      |
```

Reach for `<Table>` only when one of these actually applies:

- **Another document links to it** — it needs the `id` anchor, or a real `<figcaption>`.
- **Cells are full of `|`** — union types, `[[target|label]]` links, shell pipes — and escaping every one as `\|` would hurt.
- **It is long and wide enough that `density="compact"` matters.**

```mdx
<Table
  id="options"
  caption="Optional caption"
  density="normal"
  columns={["Option", "Tradeoff"]}
  rows={[
    ["Option A", "Simple, but doesn't scale past X"],
    ["Option B", "More moving parts, handles X"],
  ]}
/>
```

- `density`: `normal` (default) | `compact`.
- Headers and cells render **inline** markdown — backticks, `**bold**`, `*italic*` and `[links](…)` all work. Block syntax (headings, lists, fences) does not; keep a cell to one line. JSX in a cell also works: `<FileRef path="…" label="…" />` renders as a live chip.

### `<DataModel>` (structured field/type/change rows for an entity or schema)

Use this instead of a table for describing an entity's fields or a schema/migration change.

```mdx
<DataModel
  id="order-schema"
  entity="Order"
  caption="Optional caption"
  fields={[
    { name: "id", type: "Guid", change: "unchanged" },
    { name: "total", type: "decimal", change: "modified", note: "Was `int`, now supports cents" },
    { name: "discountCode", type: "string?", change: "added" },
  ]}
/>
```

- `entity`: optional heading shown above the field rows.
- `change` per field: `added` | `modified` | `removed` | `unchanged` (default when omitted). Only `added`/`modified`/`removed` render a badge — leave it off for fields shown purely for context.
- `type` and `note` render inline markdown, same as a `<Table>` cell.

### `<ApiEndpoint>` (structured method/path/params rows for a route)

Use this instead of a table for describing an API/route change.

```mdx
<ApiEndpoint
  id="create-order"
  method="POST"
  path="/api/orders"
  change="added"
  caption="Optional caption"
  params={[
    { name: "customerId", type: "Guid", required: true },
    { name: "discountCode", type: "string?", required: false, note: "New in this change" },
  ]}
/>
```

- `method`: any HTTP verb, rendered as a colour-coded chip next to `path`.
- `change`: optional badge on the endpoint itself — `added` | `modified` | `removed`.
- `params`: each row is `name`/`type`/`required`/optional `note` (inline markdown, same as `<Table>`).

### `<ModelChanges>` (the designer changes a document PROPOSES)

The Changes Review tab's visual idiom — element rows with change pills, field-level before/after rows — one tense earlier: for model changes that have not happened yet.

```mdx
<ModelChanges id="quote-changes" caption="Proposed model changes" changes={[
  { element: "Quote", designer: "Domain", change: "added",
    fields: [{ label: "Id", after: "guid" }, { label: "Total", after: "decimal" }] },
  { element: "Order", designer: "Domain", change: "modified", note: "Gains the converted-from link",
    fields: [{ label: "QuoteId", before: "—", after: "guid" }] },
]} />
```

- `change` per element: `added` | `modified` | `removed`. An unrecognised value still renders, as a neutral "Proposed" pill.
- `fields`: `label` plus `before` and/or `after`. With both, the before is struck through and an arrow joins them; with only `after`, the row reads as a new member. `note` is optional and renders inline markdown, as do `before`/`after`.
- **Static by design.** Unlike `<ModelRef>` it resolves nothing against the live model — the elements mostly do not exist yet. `element` and `designer` are still worth filling in accurately: they name what the implementation agent will create.
- Pair it with a `` ```mermaid `` `classDiagram` / `erDiagram` fence for anything structural. The diagram shows the shape; this shows the detail.

### `<Diff>` (a split before/after code comparison)

```mdx
<Diff
  id="diff-handler"
  filename="src/handlers/CreateOrder.ts"
  before={`old code here`}
  after={`new code here`}
  annotations={[
    { side: "after", lines: "3-5", label: "Short label", note: "Why this hunk matters" },
  ]}
/>
```

- `before`/`after`: full text of the relevant excerpt on each side (not a unified diff string — the renderer computes the split alignment itself from these two plain strings).
- `annotations`: optional; `side` is `"before"` or `"after"`, `lines` is a line number or `"start-end"` range on that side, keep to a few high-signal notes rather than one per line.

### `<AnnotatedCode>` (a single code block with line-numbered annotations, no diff)

Use this instead of `<Diff>` for a brand-new file or added block with no meaningful "before" — a one-sided split diff would just show an empty left panel.

```mdx
<AnnotatedCode
  id="new-handler"
  filename="src/handlers/CreateOrder.ts"
  language="typescript"
  code={`export async function handle(cmd) {\n  // ...\n}`}
  annotations={[
    { lines: "2", label: "Short label", note: "Why this line matters" },
  ]}
/>
```

- `annotations`: `lines` is a line number or `"start-end"` range; no `side` (there's only one column).

### `<Code>` (a plain snippet — no diff, no annotations)

```mdx
<Code id="snippet" filename="src/config.ts" language="typescript" caption="Optional caption" maxLines={20}
  code={`export const config = { ... };`}
/>
```

- `maxLines`: optional; truncates with a "N more lines" footer instead of dumping a very long file.

### `<Checklist>` (interactive checkboxes, strike-through when checked)

```mdx
<Checklist
  id="verify"
  items={[
    { id: "a", label: "Build passes", checked: true },
    { id: "b", label: "Manually verified the happy path", note: "Optional detail line" },
  ]}
/>
```

- `label` and `note` render inline markdown, same as a `<Table>` cell.
- Check state PERSISTS across closing and re-opening the document (stored in a sidecar beside it, never written back into the document itself) — so a verification list can genuinely be worked through over time rather than resetting on every open.
- This is NOT the spec task list. A spec's tasks live in `tasks.md` as real markdown checkboxes; see the rule above.

### `<QuestionForm>` (open questions the user answers, sent straight back to you)

Put this as the single block at the very bottom of the document for anything genuinely unresolved — do not scatter open questions through the body. A follow-up round after the user has answered should use a new `id` (e.g. `open-questions-2`).

```mdx
<QuestionForm
  id="open-questions"
  questions={[
    {
      id: "q1", title: "Which approach?", mode: "single",
      options: [
        { id: "a", label: "Option A", recommended: true, detail: "Why this is the default" },
        { id: "b", label: "Option B" },
      ],
    },
    { id: "q2", title: "Anything else?", mode: "freeform" },
  ]}
/>
```

- `mode`: `single` (radio) | `multi` (checkboxes) | `freeform` (textarea).
- Mark the recommended default with `recommended: true` on that option rather than leaving the reader to guess.
- The reader answers in the viewer and hits "Send answers to the agent", which posts them back into this conversation as a normal turn — so you DO receive them, and must not ask the user to copy-paste anything. (A copy-to-clipboard button remains for a document opened outside a conversation.) The answered state persists, so a re-opened document shows what was already sent.

### `<TabsBlock>` (group several diffs under one tab strip)

Use this when a step involves more than one file-level diff worth showing — one tab per file:

```mdx
<TabsBlock
  id="key-changes"
  tabs={[
    {
      id: "tab-a", label: "CreateOrder.ts",
      blocks: [{ id: "diff-a", type: "diff", summary: "One-line summary of this hunk", data: { filename: "...", before: "...", after: "...", annotations: [] } }],
    },
  ]}
/>
```

- The only supported nested `type` today is `"diff"` (its `data` shape is exactly the `<Diff>` props above). Anything else renders as "Unsupported block type" — don't nest other block kinds here.

### `<Wireframe>` (a screen mockup, one state)

**Read `references/wireframe.md` from this skill's directory before authoring ANY `<Wireframe>` — do not author one from memory.** The reference carries the constraints that make a mockup readable, comparable and safe; a fragment written without them silently loses content to the sanitizer or renders illegibly in one of the two themes.

```mdx
<Wireframe id="settings-after" surface="desktop" caption="Settings — after"
  data={{ html: `<div style="display:flex; flex-direction:column; gap:12px;">
  <h2>Notification settings</h2>
  <div class="wf-card">…</div>
</div>` }}
/>
```

- `surface`: `browser` | `desktop` | `mobile` | `popover` | `panel` — fixes the frame's footprint and chrome. **The fragment never sets its own width, height or position.**
- The body is a semantic HTML fragment plus helper classes (`.wf-card`, `.wf-row`, `.wf-pill`, `.wf-btn`, `.wf-muted`, `.wf-skeleton`, `data-icon="…"` — any icon name from lucide.dev/icons). No `<script>`, no `<style>`, no `on*` handlers — the renderer strips them. No hard-coded colours: the renderer themes the fragment for light and dark.
- **Surface fit:** a modal dialog or form is `panel`, not `popover`; `desktop` only when persistent chrome is the change. One idea per frame, about 6–8 rows, truncate real data — see the footprint table and density budget in `references/wireframe.md`.
- **Whenever a change alters what the user SEES, show it rather than describe it** — a screen, a dialog, a panel, a new control, a changed layout or empty/error state all warrant a mockup. One state is a `<Wireframe>`; two or more are a `<Canvas>`. Keep the prose for behaviour the picture can't show. Other kinds of change have their own visual: model shape gets `<ModelChanges>` plus a `` ```mermaid `` fence, architecture and flow get a `` ```mermaid `` fence or `<Diagram>`.

### `<Canvas>` (a board of screen states)

**Read `references/canvas.md` AND `references/wireframe.md` from this skill's directory before authoring ANY `<Canvas>`.**

```mdx
<Canvas id="settings-states" caption="Settings redesign — user-visible states"
  artboards={[
    { id: "default", title: "Default", surface: "desktop", html: `…wireframe fragment…` },
    { id: "invalid", title: "Validation error", surface: "desktop", html: `…` },
    { id: "confirm", title: "Discard changes", surface: "popover", html: `…` },
  ]}
  annotations={[{ target: "default", placement: "right", text: "Autosave replaces the Save button" }]}
  connectors={[{ from: "default", to: "invalid", label: "invalid input" }]}
/>
```

- A pan/zoom board, one artboard per user-visible state. It renders full-bleed — the only block that escapes the document's prose column — so it is worth the space only at two or more states. A single state is a `<Wireframe>`.
- **Props:** artboard height follows content within each lane (the wide row and the small row), so there is nothing to set. `height` is the optional override for the strip the board is seen through.
- **Legibility cap:** at most 4 artboards per board (3 per row-width) and at most one left/right annotation, because the board fits its full width on open and each extra frame lowers the zoom; validation warns (`canvas-opens-small`) when a board would open under 70% in a 1000px pane. Split by journey beyond that (`references/canvas.md`).
- Artboards auto-flow into two lanes (broad `browser`/`desktop` on top, compact `mobile`/`popover`/`panel` below) in authored order; `x`/`y` per artboard overrides that.
- `annotations` pin a one-line note to an artboard (`placement`: `right` default, `left`, `top`, `bottom`). `connectors` are for genuinely sequential states only.
- An annotation or connector naming an artboard that isn't on the board renders a visible warning strip; an empty artboard body renders a visible placeholder frame. Neither is acceptable in a document you hand over.

### Multi-line JSX attributes lose two spaces per continuation line

The MDX flow parser strips two spaces of indentation from every line of an attribute value after the first. So a `before={…}` template literal pasted verbatim out of a file renders with its first line two spaces deeper than the rest. Either pre-pad the continuation lines by two spaces, or write the excerpt already flush-left.

## Model- and file-linked blocks

These are what make a document here fundamentally different from the same document in any markdown-based tool: they are resolved against the LIVE designer model and workspace every time the document is opened, so a document cannot silently go stale. **Every existing model element you name in prose should be a `<ModelRef>`, not bare text** — that is the difference between a document that mentions `OrderLine` and one that knows whether `OrderLine` still exists.

### `<ModelRef>` (an inline chip for a designer element)

```mdx
The order total is calculated on <ModelRef application="MyApp.Sales" name="OrderLine" designer="Domain" type="Class" />.
```

- **`application` is REQUIRED** — the application's display NAME (what you and the user call it) or its id. Resolution never scans across applications: a solution with four applications has four designers called `Domain`, and guessing which one you meant sends the reader to the wrong element while looking confident. With no application in scope the chip renders as a warning rather than resolving against a guess.
- Address the element by `name` or by `elementId` (durable, but unreadable in prose). `name` takes **either** the bare element name **or** the package-rooted `Package/Folder/Name` path that `searchElements()` returns as each result's `path` — so when a name is duplicated, paste that `path` in verbatim instead of hunting for a guid.
- `designer` and `type` narrow the match further. A reference that is still ambiguous after all of them deliberately does NOT resolve — the chip says so, rather than guessing.
- The chip shows the element's CURRENT name and its own designer-defined type icon (a Class looks like a Class, a Command like a Command), and its state: struck out if the element has since been deleted, and a warning triangle if it did not resolve. The tooltip says WHY — the element is gone, the name is ambiguous, or no application was named — so the author knows which to fix. Clicking a resolved chip opens the designer at that element.
- For an element that does not exist **yet** (something this document is proposing), do not reach for `<ModelRef>` — it will read as broken, correctly. Name it in a `` ```mermaid `` fence, a `<DataModel>` or a `<ModelChanges>` entry instead.
- In plain `.md`, `[OrderLine](intent://element/OrderLine?designer=Domain&application=MyApp.Sales)` renders as the identical chip — so an imported spec can be linked to the model without being rewritten as MDX.

### `<FileRef>` (an inline chip for a codebase file)

```mdx
Wired up in <FileRef path="src/handlers/CreateOrder.cs" line={42} />.
```

- Write `path` **relative to the repository root** — the form Source Control shows. A path relative to the document itself, or to the application's output location, also works; an absolute path is used as-is. What does NOT work is a path relative to some project folder you happened to be reading the file from: `wwwroot/App/Foo.ts` for a file that lives at `MyApp.Web/wwwroot/App/Foo.ts` matches nothing.
- A path that resolves nowhere is **reported back to you when the document is written**, the same way a broken ```mermaid fence is — so fix the ones a write names rather than leaving the reader a dead chip. Use `change="added"` for a file the document is proposing; that is how you say "not there yet" instead of "I got the path wrong". `added`, `removed` and `deleted` are exempt from the check, because their file is meant to be missing.
- Optional `line` / `anchor` deep-link into the file. Add `endLine` to quote a BLOCK — `line={6} endLine={13}` opens the file with lines 6–13 selected, and the chip reads `:6-13`. An `endLine` that is not past `line` is ignored. `change="modified"` makes the chip open the file's diff instead of the file, the same changed-vs-read distinction the AI chat's file pills make. `label` overrides the displayed text.
- Prose gets this for free: writing `WizardAutomation.ts:6-13` in a sentence (or in an AI reply, a `.md` file, or terminal output) links the whole reference and reveals that range. Use an ASCII hyphen — an en dash is left as plain text.
- The chip shows red when the path doesn't exist — unless it is one of the exempt changes above — so a wrong path is visible rather than a dead link.
- In plain `.md`, an ordinary relative link to a source file (`[handler](../src/Foo.cs)`) upgrades to the same chip automatically.

### Linking documents

Link to another document with a wiki-link. It works the same in `.md` and `.mdx`, in prose, in `<Table>` cells and in `<Checklist>` labels:

```mdx
Overlaps with [[git-view-snapshots]] for snapshot restore.
See [[git-view-snapshots/design#snapshot-restore|the restore flow]] and [[Electron/README]].
```

- **Forms:** `[[target]]`, `[[target|label]]`, `[[target#heading]]` and `[[target#heading|label]]`. The heading may be written as its text or its id (`#Snapshot restore` or `#snapshot-restore`). Clicking opens the document rendered, scrolled to that heading.
- **What a target resolves to, in order:**
  1. A spec slug. `slug` opens the spec's requirements, and `slug/design` or `slug/tasks` opens that artifact. A plan slug (the part of `YYYY-MM-DD-slug` after the date) opens the newest plan with that slug. A spec wins over a plan with the same slug.
  2. A path, relative to the document's folder, an application's output, the solution folder or the repository root, exactly as `<FileRef>` resolves one. `.md` / `.mdx` may be left off.
  3. A path suffix matched against every `.md` / `.mdx` in the workspace, gitignored files excluded. `[[README]]` matches any `README.md`, and `[[Electron/README]]` narrows it.
- **Ambiguity is never guessed.** A suffix that matches more than one document renders as a broken link that lists the candidates, and the write is reported back with an `ambiguous-link` warning. Qualify it with a folder. A target that matches nothing gets an `unresolved-link` warning. Fix both the way you fix a dead `<FileRef>`.
- **Inside a GFM pipe table** (plain `.md`), escape the label separator: `[[target\|label]]`. `<Table>` rows in MDX need no escape.
- A backticked `` `[[x]]` `` stays literal. A target with a source-file extension (`[[Foo.cs]]`) renders as a `<FileRef>` chip instead.
- An ordinary relative markdown link (`[notes](../notes.mdx#setup)`) behaves the same way: it opens rendered at the heading.
- **`[[…]]` or `<FileRef>`?** Use `[[…]]` for a document the reader should read, and above all for a spec, which you should always link by slug and never by a path into `intent/.specs`. Use `<FileRef>` for source code, or for a specific line of a document.

### Showing the shape of the model

**For the SHAPE of the model — entities, their members and how they relate — draw a `` ```mermaid `` `classDiagram` or `erDiagram` fence**, and scope it to the elements the change touches plus their immediate neighbours. That covers what exists and what is proposed in the same picture, which is what a reader needs. Then: `<DataModel>` for one entity's fields, `<ModelChanges>` for a proposed delta, and `<ModelRef>` inline wherever the prose names an element that already exists. Never paste a screenshot of a designer diagram — it is stale the next time the model changes.

### `<ApprovalGate>` (approve or reject a spec phase from inside the document)

```mdx
<ApprovalGate id="approve-design" phase="design" title="Approve this design?">

Optional Markdown body stating exactly what is being approved.

</ApprovalGate>
```

- Only meaningful inside a spec artifact (a document under `intent/.specs/<slug>/`): approving here advances the spec exactly as the Approve button in the Specs panel does. Elsewhere it renders as a note pointing at the panel, so it is not worth adding to a general document.

## Writing plain `.md` instead

`.md` files render through this same viewer, progressively enhanced: fenced code blocks become the same themed `Code` block, ```mermaid fences render as diagrams, `intent://element/...` and relative source-file links become live `<ModelRef>` / `<FileRef>` chips, and a `requirements` / `tasks` artifact under `intent/.specs/<slug>/` gains live coverage chips and wave progress either way. What `.md` cannot have is the structured blocks above — no `<Diagram>`, `<DataModel>`, `<ApiEndpoint>`, `<Table>`, `<Diff>`, `<QuestionForm>` (a pipe table is fine).

Write `.md` when the user asked for plain markdown, when the document is genuinely all prose, or when the content is dense with characters MDX would choke on (heavy inline generics, template syntax, embedded markup) and backticking it all would hurt more than the blocks help. `tasks` is always `.md`.

## Reference files

Two blocks carry more authoring rules than fit here, and both are easy to get subtly wrong. `use_skill` reports this skill's directory; `read_file` these from it **before** authoring the block, not after:

- `references/wireframe.md` — the quality bar for any `<Wireframe>` block or `<Canvas>` artboard body.
- `references/canvas.md` — artboard-per-state discipline, lane layout, annotation and connector rules.

## Known gaps — do not use these yet

- **Interactive prototypes.** There is no click-through/hotspot tier: no `Prototype` tag, no `data-goto`. It needs a real renderer sandbox that does not exist in this app yet. A `<Canvas>` of the states, with connectors for the transitions, is the supported way to show a flow.
- **Design-fidelity mockups.** `<Wireframe>` is a structural sketch by design — no brand colours, imagery or pixel-accurate styling. Do not try to force fidelity through inline `style`.

Do not reference `WireframeBlock`, `Screen` or `Prototype` — they do not exist in this renderer and will fail to render. The block is `<Wireframe>`.
