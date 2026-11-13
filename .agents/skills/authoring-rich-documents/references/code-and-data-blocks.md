# Code and data blocks: `<DataModel>`, `<ApiEndpoint>`, `<ModelChanges>`, `<Diff>`, `<AnnotatedCode>`, `<Code>`, `<TabsBlock>`

Props and examples for the blocks that show a shape, a contract or code. The general MDX rules in `SKILL.md` (escaping, validation, stable `id`s, the two-space continuation-line gotcha for multi-line attributes) apply to every block here.

## `<DataModel>` (structured field/type/change rows for an entity or schema)

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
- `type` and `note` render inline markdown, same as a table cell.

## `<ApiEndpoint>` (structured method/path/params rows for a route)

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
- `params`: each row is `name`/`type`/`required`/optional `note` (inline markdown, same as a table cell).

## `<ModelChanges>` (the designer changes a document PROPOSES)

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

## `<Diff>` (a split before/after code comparison)

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

## `<AnnotatedCode>` (a single code block with line-numbered annotations, no diff)

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

## `<Code>` (a plain snippet — no diff, no annotations)

```mdx
<Code id="snippet" filename="src/config.ts" language="typescript" caption="Optional caption" maxLines={20}
  code={`export const config = { ... };`}
/>
```

- `maxLines`: optional; truncates with a "N more lines" footer instead of dumping a very long file.

## `<TabsBlock>` (group several diffs under one tab strip)

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
