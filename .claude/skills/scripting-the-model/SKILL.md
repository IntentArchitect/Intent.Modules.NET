---
description: The designer scripting API — the map for reading a model with run_designer_query and mutating it with run_designer_script. Naming conventions, editable-first lookup, the result channel, and a per-aspect index into references/ for the real signatures (elements, types, stereotypes, associations, mappings, packages, reads, recipes). REQUIRED before calling either script tool.
short-description: "The designer scripting API: conventions, which tool to use, and an index into the per-aspect signature references."
requiredTools:
  - run_designer_query
  - run_designer_script
  - read_file
---

# Scripting the Model

Two tools share one JavaScript API. Both tools **refuse to run until this skill is loaded**.

| Tool | Use it for | Reports |
| --- | --- | --- |
| `run_designer_query` | **Every model read** — schemas, structure, elements, diagrams, validation errors, package references. | your `return` value as `result` |
| `run_designer_script` | **Every mutation** — create/rename/move/delete, types, stereotypes, associations, mappings, package references. | `changes`, `errors`, and your `return` value as `result` |

Scripts are plain JavaScript with top-level `await`. Hold references in local variables; never invent or echo GUIDs.

## Both tools

- **`return` your answer.** The return value comes back as `result`, parsed as JSON. Return **plain data** — names, paths, the snapshot objects. An API handle is not serialisable in any useful way and a circular structure fails outright. Over ~200,000 characters the result is truncated, so select what you need rather than returning whole subtrees.
- **Call exactly the members in `references/`.** They are the real ones. Do NOT guess (there is no `el.findElements`, no `node.children.find`, no `stereotype.remove`). If a member you need genuinely is not documented, the full declarations are on disk under `wwwroot/App/Api/TypeDefinitions/` (`designer-macro.api.d.ts`, `designer-common.api.d.ts`, `core.context.types.d.ts`) — read those rather than inventing a method, but not otherwise.
- **Bind, then guard.** Every lookup can miss: `lookup*` and `getChild` / `getType` answer `null`, `find*` answers `[]`, `.find(...)` answers `undefined`. Never chain onto one — assign it, check it, then use it. The handle a `create*` / `add*` returned is the only one that cannot miss.
- **Never re-find something by the name you just supplied.** The designer may normalise it (you pass `"SKU"`, it stores `Sku`), so keep the returned handle instead of looking it up again.
- **`console.log` / `warn` / `error`** are captured and returned as `output`. A throw comes back once, as the failure message, with source-mapped stack frames and `executed: false`.

## Naming conventions — learn these once, they hold across the whole API

| Prefix | Behaviour | Examples |
| --- | --- | --- |
| `find*` | returns an **ARRAY**, never throws (`[]` when none) | `findElements`, `el.findChildren` |
| `lookup*` | returns a **SINGLE** item, `null` when none, **THROWS on ambiguity** | `lookupByName`, `lookupByPath`, `lookupById`, `lookupPackage` |
| `create*` / `add*` | performs the change and **returns the created object** | `createElementUnder`, `el.addChild`, `prop.addItem` |
| `get*` / `set*` | always **METHODS** — `el.getName()`, not `el.name`. A plain field getter always answers | `el.getComment()`, `el.setType(...)` |
| `getChild` / `getType` | a **SEARCH** wearing a `get*` name — `null` when nothing matches, so guard it | `el.getChild(pred)`, `el.getType()` |
| `resolve*` | returns ids / type-reference data | `resolveType` |

- The finders (`findElements` / `lookupByName` / `lookupByPath` / `lookupById`) are **top-level globals** that search the whole designer. They are NOT methods on an element. To search *within* an element or package use that object's `findChildren(...)` / `getChildren(...)` / `getChild(...)`.
- Lookups are **editable-first with a reference fallback**: editable packages are searched first (so your own types win and never cause false "ambiguous" errors), and if nothing editable matches they retry across referenced, read-only packages. So `lookupByName("Order")` finds a Domain type referenced into a Services designer with no extra flag. Such results have `el.isReference() === true` — usable as association endpoints, operation/attribute types and mapping ends, but not renameable or restructurable. `findElements` takes `{ includeReferences: true }` to force references into an array result even when editable matches exist.
- **A package is NOT an element.** `lookupByName("Domain")` is `null`. Address a package with `getPackages()` or `lookupPackage(name)`.
- **Duplicate names throw.** `lookupByName("Address")` throws when several match; address those by path — `lookupByPath("Package/Folder/Address")`. `findElements({ name: "Address" })` returns them all without throwing.

## Mutating: statement order IS operation order

- **Read the designer's rules first.** Call `get_designer_rules` for every designer you will change, in the same response as your first schema read. `run_designer_script` refuses a designer with rules until you have.

- Create a **parent before its children**; both **endpoints before an association** between them; the **element before a stereotype**, and the stereotype before setting its properties.
- A **name resolves to an id at call time**, so a type must already exist before it is referenced. If one script both creates and references a type, create it first.
- The whole script runs inside **ONE undo composite** (Ctrl+Z reverts it as one action). A mid-script throw still commits the steps before it — read `changes` to see what landed before writing the follow-up.
- Write **one cohesive script per designer**, not one per aggregate. After running, read `errors[]` — what your change introduced (on what you touched first, then elsewhere in the designer, capped); fix those in a short follow-up script — the result is the verification, so don't follow it with a validation or structure query.
- **Never lay out a diagram from a script.** No `layoutVisuals`, no positioning visuals, no opening a diagram to place things. `run_designer_script` shows your changes on an Explorer Diagram automatically; saved diagrams change only when the user asks.

## Where the signatures are

For a typical build, `read_file` **`references/common-path.md` first**. It covers packages, Domain classes, enums, value objects, associations, constructors and operations, then Services DTOs, commands and queries with HTTP endpoints, entity actions and mappings, the result channel and common pitfalls, in one example script per designer. Read the files below only for what it doesn't cover, never all of them.

| Aspect | Reference |
| --- | --- |
| **The common path: one end-to-end Domain + Services build, plus pitfalls — read this first** | `references/common-path.md` |
| Reads: the query globals and what each returns (incl. `getElementTypes`, `getReferenceTypes`, `getApplicationSettings({ search })`) | `references/reads.md` |
| Finding, creating, navigating and editing elements (the `IElementApi` surface) | `references/elements.md` |
| Setting types, collections, nullability, generics | `references/types.md` |
| Stereotypes and their properties, including item lists | `references/stereotypes.md` |
| Associations: creation, cardinality, direction, reading links | `references/associations.md` |
| Basic and advanced mappings | `references/mappings.md` |
| Packages (creating, loading and unloading one) and package references | `references/packages.md` |
| Worked end-to-end recipes | `references/recipes.md` |

For the **workflow** around these — discovery order, the verify-until-clean loop, diagram layout as its own step — see the **exploring-the-model** and **changing-the-model** skills.
