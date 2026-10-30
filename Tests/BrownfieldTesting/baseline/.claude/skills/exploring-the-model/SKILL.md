---
description: How to inspect an Intent Architect designer model — the discovery order, one run_designer_query call per area (getDesignerSchema, searchElements, getDesignerModelStructure, getElementDetails) plus run_solution_query for application settings, and how to read the element/association notation the snapshots emit. Load before exploring or discovering any part of the model.
short-description: Inspect the model with run_designer_query (schemas, structure, elements, settings) and read the snapshot notation.
intent-updates: automatic
requiredTools:
  - run_designer_query
  - get_designer_rules
  - run_solution_query
  - get_applications
group: Modelling
---

# Exploring the Model

The **model is the source of truth**; the codebase is a generated artifact — never infer model structure, naming, or hierarchy from files on disk. Discover the relevant part of the model thoroughly before you design or answer: keep inspecting until you would make no assumptions.

Every model read goes through **`run_designer_query`** — a JavaScript query whose `return` value comes back as `result`. Load the **scripting-the-model** skill for the API (the tool refuses to run until you have); this skill is the *workflow* around it. Solution reads (modules, architectures, application settings) are **`run_solution_query`**, which needs no skill.

## Discovery order

1. **Locate the application & designers.** The workspace context you were given already lists the solution's applications, their designers, output locations, and codebase roots — start there, and call `get_applications` when it was elided or you need the full list. Each designer has an id you pass as `designerId`.
2. **`getDesignerSchema()` once per designer you are going to EDIT** — compact by default: the containment block, association types with their allowed ends, packages, stereotype names with what they apply to, current diagram. The schema does **not** change as you edit elements: reuse the result, do not re-read it after mutating. **Do NOT read it for every designer you discover** — orienting in a designer you never touch wastes the call. To orient, `getDesignerModelStructure()` (scoped) tells you what a designer holds. Add the slice for the detail you will need in the same query: `getStereotypeDefinitions()` (property values and hints, before you set any), `getReferenceTypes()` (what can I `setType` to), `getSettings()` (long mappings). In the **same response**, call `get_designer_rules` for each designer you will change — its module rules are binding and live only there; `run_designer_script` refuses a designer with rules until you have. `getDesignerSchema({ full: true })` returns all of it at once — tens of thousands of characters, so only when you genuinely need most of it.
3. **Find elements** — `searchElements(regex, { fields?, specializations? })` when you know what you're after. Use `getDesignerModelStructure(options)` when you genuinely need topology ("show me all packages and their classes"). An unscoped call is safe — it is bounded and says what it left out — then narrow with `elementId` / `packageId` / `specializations`.
4. **`getElementDetails(pathOrNameOrId)`** on a specific element before referencing or modifying it, to confirm its properties, type references, stereotypes, mappings, and parent constraints.

**One query per area, not one per read.** The point of the query tool is that a single call can do steps 2–4 together:

```js
const found = await searchElements("^Order", { specializations: ["Class"] });
const first = found.matches[0];                                    // may be undefined — searchElements can match nothing
return {
    elementTypes: getElementTypes(),                               // the slice, not the whole schema
    matches: found.matches.map(m => m.path),                       // project — don't echo whole snapshots back
    order: first ? await getElementDetails(first.path) : null,
};
```

Combining reads is free, but **returning** them is not: the result is charged in full and truncated past ~200,000 characters. Return the fields you will act on, not the snapshots they came from.

Independent queries against **different** designers may still be issued in parallel in one turn.

## The snapshot notation (shared legend)

`getDesignerModelStructure` and `getElementDetails` render elements and associations in one shared notation:

```
Shared notation for model elements and associations:
- Element line: `Name [Type] (memberCounts) +Stereotype(prop=value) — comment [#id]`. `[Type]` is the specialization; a trailing `/` on the name marks a container (folder / an element with members); `(3 Attribute, 1 Operation)`, when present, COUNTS what this node contains but does not list here — its child members, plus any nested elements a depth cut collapsed; `+Stereotype(prop=value)` lists APPLIED stereotypes showing only set / non-default property values (`+Name` alone when it has none); `— comment` is the description; a trailing `#id` appears ONLY where you need it to act (see the notes below).
- Association line: `Source →"navName" Target (srcMult→tgtMult) [Type] #id`. `→` reads source→target (`↔` = bidirectional, with the back-navigation name after `/`); `"navName"` is the Source's name for the link; `Source`/`Target` are the element's bare NAME, or its full `Package/Folder/Name` PATH when that name is duplicated or cross-package (address either verbatim via lookupByPath / getElementDetails); `(srcMult→tgtMult)` are the multiplicities; `[Type]` is shown only for non-default association types; the trailing `#id` is the association's id — an association has NO name/path address, so this is its only handle (and the `associationId` apply_change_diagram_layout needs to route the edge). In a script `Source.getAssociations()` returns this link — `getName()` = navName, `getElement()` = Target (the connected element, NOT Source), `getMultiplicity()` = the target multiplicity.
```

## Per-read notes

### `getDesignerSchema()`

- Returns the designer's schema only (no elements, associations, or errors). The default is COMPACT, one line per item: `elementTypes` (containment), `associationTypes` (`Type: Sources → Targets`), `designerRules` (a pointer to `get_designer_rules`, only when the designer has rules), `packages` (`Name [Type]`), `stereotypes` (`Name → applicable types`), and `mappings` when short. The notes below describe the detail that lives in the slices (`getStereotypeDefinitions()`, `getReferenceTypes()`) and in `getDesignerSchema({ full: true })`.
- A reference type shown with a `<...>` signature is GENERIC and needs that many type arguments (e.g. `Dictionary<TKey, TValue>` → `setType("Dictionary<string, Order>")`); a plain name takes none.
- Stereotype definitions list `applicableTo` and, when present, `applicableToReferencedTypes` — or `appliesTo` (a filter-function rule resolved to the element types it accepts) or `applicability` (`any element`, or `dynamic` when a per-element filter function decides and we could not resolve it; read the description for where it applies); apply one by its `id` (names can collide). `availableIn` appears only when a stereotype is limited to certain packages. A stereotype applies at most ONCE per element unless it carries `allowMultipleApplies: true`.
- A property line's text after ` — ` is the module author's own **hint** — what the value means, what a path is relative to, how it is spelt. It is authoritative and frequently un-guessable; read it before setting the value rather than inferring from the property name.
- A property shown as `item-list of '<Row Type>'` is a list of ROWS, not a value: `setProperty` on it stores labels and no rows, and reading its value gives you that stale label cache. Add rows with `prop.addItem()` and set the returned handle's own properties — which are listed under the stereotype's `itemTypes`, keyed by row type name — and read them back with `prop.getItems()`.
- Applied stereotypes on elements are NOT here — use `getElementDetails` for per-element values. Validation is in each `run_designer_script` result's `errors` (or `getDesignerValidationErrors()` when you have not just run one), never in the schema.
- `getStereotypeDefinitions()`, `getElementTypes()`, `getReferenceTypes()` and `getSettings()` each return one slice. Because a query is a script, composing the compact schema with a slice costs one call — so reach for that by default, and keep `{ full: true }` for when you need most of the detail at once.

### `searchElements` (preferred for targeted lookups)

- Searches element text with a **case-insensitive regex**. `fields` restricts it: `name`, `specialization`, `comment`, `value`, `typeReference`, `stereotype` (all stereotype names + property values); omit for all fields.
- `specializations` filters to matching element types (e.g. `["Class"]`, `["Repository"]`); omit for all types. Searches every element (top-level, children, package references).
- Each result carries `matchedOn` (which fields matched) and a **`path`** — its package-rooted `Package/Folder/Name` address. **Prefer passing this `path`** (not the bare name) to `getElementDetails` and `lookupByPath` — it's unambiguous, so duplicate names never collide. A bare name is safe only when exactly one match came back.
- Results cap at 100 — if `limitApplied` is true, narrow the regex, restrict `fields`, or add `specializations`.

### `getDesignerModelStructure` (topology only)

- A terse indented TEXT tree per package (packages → folders → elements) plus a compact association list. Stereotype DEFINITIONS/rules/settings are NOT here (that's the schema) — but each element's APPLIED stereotypes are shown inline.
- The overview carries NO element ids — address an element by its NAME or PATH (the indentation IS the path, e.g. `Catalog/Product`). `(memberCounts)` advertise child members NOT shown — call `getElementDetails` for those.
- A line shown with `@ Package/Folder/Name` has a DUPLICATED name — address it by THAT exact path; names without an `@` qualifier are unique, so the bare name is safe.
- It is **bounded and breadth-first**: the top level first, then as deep as its character budget reaches (`maxChars`, default 20,000); detail goes to the first branches in model order, so drill into the one you need. Whatever it did not expand is counted on its nearest shown parent — `Catalog/ [Folder] (176 Class, 12 Enum)`. `truncated: true` means some of the model is a number rather than a line; re-issuing the same call returns the same thing. `omitted` counts what isn't even a number, closed by a `… +212 more elements (198 Class, 14 Folder)` line.
- **Drill in with `{ elementId }`** — an id, a `Package/Folder/Name` path, or a unique name — to give one subtree the whole budget. It is the level `getElementDetails` can't give you: folder contents are top-level elements, not members. It gains nothing on a node the unscoped call already spent everything on, and there is no paging within one container — for "list everything in X" use `searchElements`.
- Narrow further with `{ packageId }`, `{ specializations: ["Class"] }` (a folder still counts what the filter hid, and an association stays when either end matches), or `{ includeAssociations: false }` to spend the budget on the tree alone.

### `getElementDetails`

- Reads ONE element in full — pass its `Folder/Name` PATH, its exact NAME, or its id. Returns the element plus a terse `members` tree, applied `stereotypes` (current values), `associations`, any `mapping`/`mappings`, and `suggestions`/`codeFiles`/`errors` when present.
- A PACKAGE name resolves here too (you get its applied stereotypes; the element-only surfaces — members tree, associations, mappings — are omitted).
- `members` are children indented by containment (each with a trailing `#id`); a member's own associations and stereotype values are not expanded — call it again on that member to drill in.
- Existing mappings: `mapping` is a BASIC projection (`→ targetPath [Mapping Setting]`); `mappings` are ADVANCED — a `[Mapping Type]` header then one indented `sourcePath → targetPath [= expression]` per mapped end (name-based paths). An association's advanced mappings show in the same notation beneath its `associations` line.
- `stereotypes` shows applied stereotypes with CURRENT values only — the allowed values/types per property live in the schema. Change one with `run_designer_script`: `el.getStereotype("Name").setProperty("prop", value)`.
- It **throws** for an element that lives in a referenced package owned by another designer; the message names the owning application and designer, so re-issue the query against that `designerId`.

### `getApplicationSettings({ search })` (`run_solution_query`)

- Per-application settings + architecture configuration: basic fields (`name`, `description`, `metadataLocation`, `outputLocation`) and dynamic `moduleSettingGroups` from installed modules. Read it with `run_solution_query`, passing the `applicationId` and a `search` regex.
- `outputLocation` is the resolved absolute on-disk path for generated code; `metadataLocation` the resolved absolute metadata path — use these directly when you need an absolute path; don't guess the solution root.
- Every module setting has a unique `id` (per application); pass that exact id to `update_application_settings`. `controlType` says how the value is read: `Checkbox`/`Switch` → `"true"`/`"false"`; `Select`/`MultiSelect` → one of `options[].value`; `Number` → numeric string; the rest are free text.

## Designer Quick Reference

- **User Interface designer** — Pages, Components, Layouts.
- **Services designer** — Commands, Queries, DTOs, Services, Operations (CQRS / API surface).
- **Domain designer** — Entities, Value Objects, Aggregates, Repositories.
- **Codebase Structure designer** — Folder/project layout and template output anchors.

Folder names in a designer map to namespaces or output paths — they may not match disk folders. Trust the designer.
