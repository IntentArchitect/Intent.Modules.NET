---
description: "How to mutate an Intent Architect model — run_designer_script (the primary mutation path), update_application_settings, apply_change_diagram_layout, and the validate-until-clean loop on each script result's errors. Load before making any model, settings, or diagram change: covers operation ordering, containment, type/package references, associations & cardinality, mapping, showing your changes, and diagram layout on request."
short-description: Mutate the model with run_designer_script (containment, ordering, associations & cardinality, mapping), settings, diagram layout, and the validate-until-clean loop.
intent-updates: automatic
requiredTools:
  - run_designer_script
  - run_designer_query
  - get_designer_rules
  - run_solution_query
  - apply_change_diagram_layout
  - update_application_settings
  - get_scripts
group: Modelling
---

# Changing the Model

The **model is the source of truth**. Apply changes through the designer tools — `run_designer_script` is the primary mutation path. Write **one cohesive script per designer**: every aggregate, entity, enum and association that designer needs, in dependency order, in a single `run_designer_script` — not one script per aggregate. Its result is your verification: fix whatever its `errors[]` reports in a short follow-up script, then move to the next designer (Domain before the Services designer that references it). Split a designer's work only when it is genuinely too large to write as one script. A script changes the **model only** — never lay out a diagram from a script (that's a separate step, below).

Before your first script, load the **scripting-the-model** skill for the API (`run_designer_script` refuses to run until you have) and read each designer's `getDesignerSchema()` once with `run_designer_query` (see the **exploring-the-model** skill) — compact by default; add `getStereotypeDefinitions()` in the same query when you will set stereotype properties. In the **same response**, call `get_designer_rules` for every designer you will change: its module rules are binding, and `run_designer_script` refuses a designer with rules until you have.

## `run_designer_script` — the primary mutation path

- Load the **scripting-the-model** skill ONCE before your first script — both script tools refuse to run until it is loaded. Read `getDesignerSchema()` ONCE per designer for its containment, associations and stereotype names — do NOT re-read it. It is compact: property values come from `getStereotypeDefinitions()`, type names from `getReferenceTypes()`.
- `getDesignerSchema()`'s **"Element types"** block (`elementTypes`, also `getElementTypes()`) is the source of truth for CONTAINMENT: per type it lists the exact child specialization names you may `addChild`/`createElementUnder`, marks any type that REQUIRES a type reference with `!` — INCLUDING on a child reference (e.g. `Class: Attribute!`), so set a type wherever you see `!` — and `¹` for max-one children. Use those exact names; don't guess a child type or skip a required type. (Valid type-reference targets aren't enumerated — resolve them by name with `resolveType`/`setType`.)
- Don't assume package names (e.g. "Domain"): get exact names from `getDesignerSchema()` or list them at runtime with `getPackages()`. Name/path lookups are **EDITABLE-FIRST with a REFERENCE FALLBACK**: they match your own editable types first (no false "ambiguous" errors), and if nothing editable matches they automatically find types from REFERENCED designers/packages — so in e.g. a Services designer, `lookupByName("Order")` or `lookupByPath("Domain/Ordering/Order")` resolves the referenced Domain type with no extra flag. Those results are READ-ONLY (use them as association endpoints / operation & attribute types / mapping ends; you cannot rename or restructure them). If a type you KNOW exists comes back null, it is almost always a name/path typo — verify with `getPackages()` / the schema, not by guessing a flag.
- **A package is not an element.** Element finders (`lookupByName` / `findElements` / `lookupByPath`) search only the children of editable packages — `lookupByName("Domain")` returns `null`. Address a package via `getPackages()` or `lookupPackage(nameOrId)`. A package that exists on disk but isn't loaded is listed by `getUnloadedPackages()` — bring it back with `await loadPackage(id)`, never `createPackage`; take one out with `await unloadPackage(nameOrId)`, never `pkg.delete()`.
- **Remove a stereotype owner-side:** `owner.removeStereotype(nameOrDefinitionId)` (on both element and package handles) — there is no `stereotype.remove()`.
- Console output (`log`/`warn`/`error`) is captured and returned as `output`; a thrown error comes back once, as the call's failure message, with source-mapped stack frames (`(main script)` for your inline `script`, else the included path) and `executed: false`.
- The result reports **`changes`** — the elements actually changed, grouped `created` / `updated` / `deleted`, each entry `{ id, name }`. This is populated even when `executed` is false: a script mutates incrementally and a mid-script throw STILL applies the steps before it to the **in-memory** model (not yet saved to disk — see "Persisting to disk" below). So on a failure do NOT assume nothing happened — read `changes` to see what already succeeded and write the follow-up to continue from there (don't blindly re-create elements that already exist). When recording spec traceability, forward the entries verbatim — they are already the `{ id, name }` shape `record_spec_traceability` takes.
- The result also reports **`errors`** — what your change introduced: validation errors/warnings on the elements you touched first, then (capped at 20, with a closing entry counting the rest) new ones elsewhere in the designer, each addressed by `path` (the package-rooted `Package/Folder/Name` you can feed back to lookups/finders), `associationId` or `elementId`. Problems already in the designer before the script are not listed, only counted in `preexistingProblems`; leave them unless the user asks. An empty `errors` means your change introduced none — no separate validation query is needed. A non-empty `errors` after a "successful" run means your change left the model invalid — fix it before moving on.

### Operation ordering (statement order is operation order)

- Create **parents before children**.
- Create **both endpoints before creating an association** between them. ALWAYS set cardinality: `createAssociation(spec, sourceId, targetId, { targetMultiplicity: "0..*", sourceMultiplicity: "1" })` (UML strings `"1"`, `"0..1"`, `"1..*"`, `"0..*"`; `"*"` is a synonym for `"0..*"`, anything else throws). Omitting it defaults to 1-to-1, usually wrong. Read the applied cardinality back with `getMultiplicity()` — it is stored as flags, not as the string you passed. Associations are **unidirectional by default** (only the target end navigable) — do NOT pass `isBidirectional`; set `isBidirectional: true` (or `assoc.setBidirectional(true)`) ONLY when two-way navigation is an explicit part of the design.
- To INSPECT existing links, `el.getAssociations()` returns one handle per connected element, each oriented AWAY from `el`: `getAssociatedElement()` is the connected neighbour, `getName()` is `el`'s name for the link, `getMultiplicity()` is how many, `getSpecialization()` is the association's kind (what `getAssociations(type)` filters on). Find a link by the connected element (`...find(a => a.getAssociatedElement()?.getName() === "Payment")`), not by guessing the end name. For the reverse name/cardinality back toward `el`, use `a.getThisElementsEnd()`. (Avoid `getOtherEnd()` — it reads as "the neighbour's end" but hops back to `el` itself.)
- Create the element **before adding stereotypes**; add a stereotype before updating its properties. If a stereotype's apply mode is `Always`, it is created/applied automatically — don't add it. Stereotype property values must be valid: `getStereotypeDefinitions()` (or `getDesignerSchema({ full: true })`) lists each as `Name: allowed` — set with `setProperty("Verb", "POST")`; `ref to <Types>` = an element id of one of those types; `(multiple)` = a JSON array of ids (e.g. `setProperty("Security Roles", [roleId])`). Off-list values fail listing the allowed values; a ref id that isn't a valid option (wrong type, or in a package this designer doesn't reference) fails naming the allowed type — resolve the element by name, and add the package reference if it lives elsewhere. Read a value back with `getProperty(name).getValue()` (an array of elements for a `(multiple)` ref), not the `.value` snapshot. A property listed as `item-list of '<Row Type>'` is NOT set with `setProperty` — call `prop.addItem()` per row and set that row handle's own properties (the schema lists them under the stereotype's `itemTypes`); where several rows match, the LAST matching row wins, so add the broad rule first. Honour a property's authored hint (the text after ` — ` on its schema line).
- You are **MODELLING, not writing C#** — don't import language idioms.
- When moving children to a new parent, move them ALL before deleting the old parent. Don't delete-and-recreate to move — update the parent reference.
- To **map** elements (e.g. a Command's fields to a Domain entity), create both endpoints and the child fields first, then map. `getDesignerSchema()`'s "Mappings" block (or `(await getSettings()).mappings` when the compact schema points there) says which element/association types support mapping and the mapping-type names (designer-specific — never assume a universal name like "Map to Domain"); `getElementDetails()` shows existing mappings. Basic: `el.setBasicMapping(targetId)`. Advanced: `const m = el.createAdvancedMapping("Mapping Type Name")` then `m.mapEnd(["SourceField"], ["TargetField"])` (the mapping type is optional and the last arg — omit it; a mapping almost always has one used automatically), verifying with `m.getMappedEnds()`. Path segments take names or ids; a wrong one throws and lists what was available.

If a designer rule is violated, the operation fails — adjust the design and retry.

### Package references

- PACKAGE REFERENCES (a package depending on another package's types) are ALSO changed via a script — there is no add/remove tool. Discover candidates with `await getAvailablePackageReferences()` (a read-only query global), then on the owning package handle call `await pkg.addReference(absolutePath, module?)` (async; resolves once the reference's types are usable) — pass a candidate's `absolutePath`, plus its `source` as `module` when `sourceType === "module"` (omit `module` for a 'solution' package). Remove with `pkg.removeReference(idOrNameOrPath)`; list current ones with `pkg.getReferences()` (or the `getPackageReferences(packageId?)` global). Get the package handle via `lookupPackage(name)`/`getPackages()`.

### Persisting to disk — in-memory until saved, and saves are per-application

`run_designer_script` mutates only the target application's **in-memory** model — nothing is written to disk by the call itself (unless `saveOnSuccess` is set, see below), no matter how "final" a successful run or a validated `errors[]` looks. `update_application_settings` is the opposite: it **always** writes the application's settings to disk immediately, but touches no designer or package, so it neither saves nor discards unsaved designer changes — except a `persistenceFormatVersion` change, which rewrites every one of the application's packages in the new format. The model reaches disk automatically as the first step of a `run_software_factory` run, but **only for that same application** — a Software Factory run for Application A saves only A's own dirty designers, never a different application B's.

This matters whenever one application references another's package. Application A's reference to Application B resolves against **B's saved-to-disk model**, not B's live in-memory edits. So after modelling changes in B, running Software Factory for A does **not** make A see them — the new/changed types will look missing or stale. If you've just modelled B and now need A (which references B) to pick up those changes: run Software Factory for B **first** (this saves B), then model/generate in A. Don't assume a designer's changes are visible cross-application just because its own script run reported success or clean validation.

Both `run_designer_script` and `apply_change_diagram_layout` accept an optional `saveOnSuccess` flag to persist that one designer to disk immediately, **without** running the Software Factory — useful for pure-modelling work (e.g. spec/traceability recording, documentation-only modelling) where no generation step is needed yet. Only set it `true` on the **final** call of an already-verified sequence: the save happens unattended and does **not** gate on validation errors, so saving mid-fix would persist a half-corrected model.

### A designer tool refused because a dialog is showing

If the refusal lists choices, call `resolve_designer_dialog` with one (e.g. `discard-and-reload` discards the tab's unsaved edits, so re-query before editing; `keep-my-changes` leaves it out of sync with disk). Check git to see who changed the file, never loop on the same refusal, and tell the user if resolving fails. A refusal that lists no choices needs the user to answer the dialog.

Treat `saved:false` as work at risk: pass `saveOnSuccess` once `errors[]` is clean. A module install or reinstall reloads the open designers and discards unsaved edits, and a designer with validation errors cannot be saved, so fix a module's errors before modelling against it.

### Reusing saved application scripts (`includedScriptPaths`)

- Call `get_scripts` first to discover available scripts; use the returned `path` values directly. Each entry is an absolute path or a path relative to the `.application.config` file. Scripts are prepended in order — a library loaded before your main body. To run a saved script with no extra logic, pass its `path` in `includedScriptPaths` and an empty string for `script`.

### Comments

Add comments to elements for non-obvious design decisions — express the purpose of the element and/or its expected behaviour if it represents an operation or function. This is passed on to coding agents that implement the logic.

## Verify — confirm it landed, then validate until clean

After every mutation confirm **both**, and don't proceed while either fails:

1. **It landed as intended.** Reconcile the returned `changes[]` against your plan: each new element appears **once** as `created`, edits show as `updated` (a stray `created` where you meant to modify is a **duplicate** — e.g. a second `FirstName` → `FirstName1`). Only when you retried a throwing script (pre-throw steps already committed) and `changes[]` cannot settle what exists, re-read the container with `getElementDetails()` / a scoped `getDesignerModelStructure()`.
2. **It's error-free.** The result's `errors[]` lists what your change introduced anywhere in the designer; fix those — do NOT follow a script with a `getDesignerValidationErrors()` or `getDesignerModelStructure()` query just to verify. Resolve what it lists in a follow-up script and read that result's `errors[]`.

A script's own `return` value also comes back, as `result` (parsed JSON) — end a mutation script with a `return` of what you want to confirm (names, multiplicities, mapped ends) instead of a follow-up read, but keep `changes[]` as the authoritative report of what the mutation did. Return plain data, never API handles. Broader reads go through `run_designer_query`; `run_designer_script` is for mutations.

`addChild` / `createElementUnder` / `createElement` never dedupe. To _ensure an element exists_, look it up first (`lookupByName` / `findElements` / `el.getChildren()`) and create only when absent.

## Showing your changes — the Explorer Diagram, not the saved diagrams

`run_designer_script` shows your changes on an Explorer Diagram automatically; leave saved diagrams alone unless asked. Never lay out from a script (no `layoutVisuals`, no positioning visuals in-script).

## Saved diagrams — only when the user asks

Add elements to, or rearrange, a saved diagram **only when the user asks you to**. Then use `apply_change_diagram_layout` as a separate step after that designer's changes are verified, and **you own the result: it must be tidy with NO overlapping elements.**

- **Read first.** Call `getDiagramSnapshot()` to see the diagram's current nodes (each with x/y/width/height) and edges. Also honour any layout rules on the diagram's type from `getDesignerSchema()`.
- **Add the new elements.** Address each node by its **id, name, or package-rooted PATH** (e.g. `Domain/Orders/Order`) — whatever a tool result gave you; never invent a GUID (an ambiguous name errors with the candidate paths). Give clear `x, y, width, height`. Place a new element next to its most-connected related element. Some nodes `autoSized` — their width/height is computed, so position by their actual size. The check is per diagram type: an element type with no visual configuration on *this* diagram fails the call with an explanatory error (e.g. a DTO on a Domain diagram — DTOs belong on Services diagrams).
- **Place what the user asked for.** When they want your new elements on a diagram, include every top-level one that diagram type can show: enums, value objects and DTOs, not only classes and commands. The layout result lists new elements on no diagram (`ON NO DIAGRAM: …`); place only the ones the user asked to see.
- **Route the associations** you created so their lines render. Address edges by `associationId` (from `getDiagramSnapshot()`, `getElementDetails()`, or `getDesignerModelStructure()` — whose edges share its character budget with the tree, so scope it with `elementId` when you need a specific one). An edge routes only when **both** its endpoint nodes are placed in the same call — add both endpoints as nodes alongside the edge; an edge to an element you didn't place won't appear. Routing is automatic — don't set waypoints unless asked.
- **Spacing & stability:** keep ≥150px horizontal and vertical gaps; nodes must never overlap; don't move existing elements unnecessarily — only include a node if you intend to move or resize it. Moving an existing element (to make room / clear an overlap) is exactly what this tool is for.
- The result reports final geometry and any overlaps, each with a collision-checked move to clear it. **Re-call applying those moves until no overlaps remain**, then verify a clean layout.

## `update_application_settings`

- Changes the per-application settings shown in the Solution Explorer "Settings…" dialog. Read `getApplicationSettings({ search })` with `run_solution_query` first to discover current values and the exact `id`s of module settings before patching.
- **Partial patch** — only the parameters you supply change; everything else is untouched.
- Changing `name` ALSO renames the on-disk folder and metadata files (the solution is modified).
- `moduleSettings` is a flat list of `{ settingId, value }`; each `settingId` must match an `id` returned by `getApplicationSettings()` (the tool resolves the group). Stringly-typed values: checkboxes/switches expect `"true"`/`"false"`; selects must use an option `value`; numbers are numeric strings.
- `persistenceFormatVersion` ∈ `V1Xml` (legacy), `V2Xml`, `V3Xml` (nested associations), `V3Yaml`. `iconType` ∈ `UrlImagePath`, `RelativeImagePath`, `FontAwesome`, `CharacterBox`.
