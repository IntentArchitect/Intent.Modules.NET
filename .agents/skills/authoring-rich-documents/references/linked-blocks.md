# Model- and file-linked blocks: `<ModelRef>`, `<FileRef>`, `[[wiki-links]]`, `<ApprovalGate>`

These are resolved against the LIVE designer model and workspace every time the document is opened, so a document cannot silently go stale. The general MDX rules in `SKILL.md` apply here too.

## `<ModelRef>` (an inline chip for a designer element)

```mdx
The order total is calculated on <ModelRef application="MyApp.Sales" name="OrderLine" designer="Domain" type="Class" />.
```

- **`application` is REQUIRED** — the application's display NAME (what you and the user call it) or its id. Resolution never scans across applications: a solution with four applications has four designers called `Domain`, and guessing which one you meant sends the reader to the wrong element while looking confident. With no application in scope the chip renders as a warning rather than resolving against a guess.
- Address the element by `name` or by `elementId` (durable, but unreadable in prose). `name` takes **either** the bare element name **or** the package-rooted `Package/Folder/Name` path that `searchElements()` returns as each result's `path` — so when a name is duplicated, paste that `path` in verbatim instead of hunting for a guid.
- `designer` and `type` narrow the match further. A reference that is still ambiguous after all of them deliberately does NOT resolve — the chip says so, rather than guessing.
- The chip shows the element's CURRENT name and its own designer-defined type icon (a Class looks like a Class, a Command like a Command), and its state: struck out if the element has since been deleted, and a warning triangle if it did not resolve. The tooltip says WHY — the element is gone, the name is ambiguous, or no application was named — so the author knows which to fix. Clicking a resolved chip opens the designer at that element.
- For an element that does not exist **yet** (something this document is proposing), do not reach for `<ModelRef>` — it will read as broken, correctly. Name it in a `` ```mermaid `` fence, a `<DataModel>` or a `<ModelChanges>` entry instead.
- In plain `.md`, `[OrderLine](intent://element/OrderLine?designer=Domain&application=MyApp.Sales)` renders as the identical chip — so an imported spec can be linked to the model without being rewritten as MDX.

## `<FileRef>` (an inline chip for a codebase file)

```mdx
Wired up in <FileRef path="src/handlers/CreateOrder.cs" line={42} />.
```

- Write `path` **relative to the repository root** — the form Source Control shows. A path relative to the document itself, or to the application's output location, also works; an absolute path is used as-is. What does NOT work is a path relative to some project folder you happened to be reading the file from: `wwwroot/App/Foo.ts` for a file that lives at `MyApp.Web/wwwroot/App/Foo.ts` matches nothing.
- A path that resolves nowhere is **reported back to you when the document is written**, the same way a broken ```mermaid fence is — so fix the ones a write names rather than leaving the reader a dead chip. Use `change="added"` for a file the document is proposing; that is how you say "not there yet" instead of "I got the path wrong". `added`, `removed` and `deleted` are exempt from the check, because their file is meant to be missing.
- Optional `line` / `anchor` deep-link into the file. Add `endLine` to quote a BLOCK — `line={6} endLine={13}` opens the file with lines 6–13 selected, and the chip reads `:6-13`. An `endLine` that is not past `line` is ignored. `change="modified"` makes the chip open the file's diff instead of the file, the same changed-vs-read distinction the AI chat's file pills make. `label` overrides the displayed text.
- Prose gets this for free: writing `WizardAutomation.ts:6-13` in a sentence (or in an AI reply, a `.md` file, or terminal output) links the whole reference and reveals that range. Use an ASCII hyphen — an en dash is left as plain text.
- The chip shows red when the path doesn't exist — unless it is one of the exempt changes above — so a wrong path is visible rather than a dead link.
- In plain `.md`, an ordinary relative link to a source file (`[handler](../src/Foo.cs)`) upgrades to the same chip automatically.

## Linking documents

Link to another document with a wiki-link. It works the same in `.md` and `.mdx`, in prose, in table cells and in `<Checklist>` labels:

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
- **Inside a pipe table**, in `.md` and `.mdx` alike, escape the label separator: `[[target\|label]]`.
- A backticked `` `[[x]]` `` stays literal. A target with a source-file extension (`[[Foo.cs]]`) renders as a `<FileRef>` chip instead.
- An ordinary relative markdown link (`[notes](../notes.mdx#setup)`) behaves the same way: it opens rendered at the heading.
- **`[[…]]` or `<FileRef>`?** Use `[[…]]` for a document the reader should read, and above all for a spec, which you should always link by slug and never by a path into `intent/.specs`. Use `<FileRef>` for source code, or for a specific line of a document.

## `<ApprovalGate>` (approve or reject a spec phase from inside the document)

```mdx
<ApprovalGate id="approve-design" phase="design" title="Approve this design?">

Optional Markdown body stating exactly what is being approved.

</ApprovalGate>
```

- Only meaningful inside a spec artifact (a document under `intent/.specs/<slug>/`): approving here advances the spec exactly as the Approve button in the Specs panel does. Elsewhere it renders as a note pointing at the panel, so it is not worth adding to a general document.
