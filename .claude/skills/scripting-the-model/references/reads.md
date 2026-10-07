# Reads — the `run_designer_query` globals

Every global here is also available inside `run_designer_script`, so a mutation script can inspect before it writes. The reverse is not true: a query script has no mutation surface at all (every setter, `createElement*`, `createAssociation`, `processChangeOps`, `dialogService` and the navigation/clipboard/module-task globals throw).

Solution-level reads are a different tool with a disjoint API — `run_solution_query`, documented at the end of this file. Nothing here exists there, and nothing there exists here.

## Result rules

- **`return` plain data.** Names, paths, ids, and the snapshot objects below are all fine. `JSON.stringify` of an element handle yields a function-less husk and `getPackages()` yields near-empty objects, so returning handles tells you nothing; a circular structure fails the whole call.
- Select what you need. A result over 200,000 characters is truncated with a note stating the original length.

## Model globals

```ts
/** EVERYTHING below in one object: settings, stereotype definitions, reference types, current diagram, packages.
 *  Read it for a designer you are about to EDIT and are unfamiliar with — not for every designer you discover. */
declare function getDesignerSchema(): Promise<DesignerSchemaSnapshot>;

/** The containment block (what each element type accepts as children, and which require a type), headed by the
 *  designer's module rules. Those rules are binding — e.g. which associations a Command/Query must carry. */
declare function getElementTypes(): string;

/** Just the type NAMES `setType` accepts (generics carry their `<...>` signature). */
declare function getReferenceTypes(): string[];

/** Terse indented tree per package (packages → folders → elements) plus a compact association list.
 *  Self-bounding: it spends a CHARACTER budget breadth-first — the top level first (roots that don't fit are counted in `omitted`), then it
 *  goes as deep as fits. Whatever it didn't expand is counted on its parent — `Catalog/ [Folder] (176 Class)`.
 *  Drill with `elementId`, or raise `maxChars` (default 20,000, max 60,000); re-issuing the same call returns the same thing. */
declare function getDesignerModelStructure(options?: {
    elementId?: string, maxChars?: number, packageId?: string,
    specializations?: string[], includeComments?: boolean, includeAssociations?: boolean,
}): Promise<DesignerStructureSnapshot>;

/** Every validation error/warning in the designer, each addressed by `path`, `elementId` or `associationId`. */
declare function getDesignerValidationErrors(): Promise<DesignerValidationErrorsSnapshot>;

/** Just the stereotype DEFINITIONS (applicability + property contracts) — a slice of the schema. */
declare function getStereotypeDefinitions(): Promise<any>;

/** Current package references, per package (all packages when packageId is omitted). Name/module/path only —
 *  no GUIDs, since a reference is addressed by name everywhere else. See packages.md. */
declare function getPackageReferences(packageId?: string): any[];

/** Case-insensitive regex search over element text. Never throws. Each match carries `path` — the unambiguous address to reuse. Caps at 100. */
declare function searchElements(query: string, options?: {
    fields?: ("name" | "specialization" | "comment" | "value" | "typeReference" | "stereotype")[],
    specializations?: string[],
}): Promise<{ totalMatches: number, limitApplied: boolean, matches: any[] }>;

/** ONE element (or package) in full: properties, type reference, members tree, applied stereotypes, associations, mappings, errors. */
declare function getElementDetails(pathOrNameOrId: string): Promise<ElementDetailsSnapshot>;

/** The current diagram: visuals with their positions/sizes and the routed association edges. */
declare function getDiagramSnapshot(): Promise<any>;

/** The designer's own settings block (a slice of the schema): rules, elementTypes, mappings, associationTypes. */
declare function getSettings(): Promise<any>;
```

**Composing the slices costs ONE call, same as the bare schema** — this is a script, not a tool per read:

```js
return { types: getElementTypes(), stereos: await getStereotypeDefinitions() };
```

So prefer the slices. If you need "what can I create where", `getElementTypes()` is the call — and it carries the designer's rules, so read and follow them before you edit. When you will create associations, add `(await getSettings()).associationTypes` for their allowed sources and targets (not the whole `getSettings()`, which repeats the rules).

Notes:

- **An empty field is OMITTED, not returned empty — this holds for EVERY optional field of `getElementDetails`** (`members`, `stereotypes`, `associations`, `comment`, `value`, `mapping`, `mappings`, `suggestions`, `codeFiles`, …). A missing `members` means "no children"; a missing `stereotypes` means "none applied". It is never "not reported". Don't read absence as "I couldn't see it" — a small response IS the answer.
- `getElementDetails` throws for an element that lives in a **referenced** package owned by another designer; the message names the owning application and designer, so re-issue the query against that `designerId`.
- `getDiagramSnapshot()` returns no elements until something has been PLACED — a freshly scripted designer has nothing on its diagram and no sizes yet. Don't read it before the first layout; call `apply_change_diagram_layout`, which reports the resulting positions and sizes.
- `searchElements`'s `matches[].path` is package-rooted (`Package/Folder/Name`). Prefer it over a bare name when feeding `getElementDetails` or `lookupByPath`.
- The finders (`findElements`, `lookupByName`, `lookupByPath`, `lookupById`, `lookupPackage`, `getPackages`, `resolveType`) are documented in `elements.md` / `packages.md` / `types.md` and are equally available in a query.

## `run_solution_query` globals

```ts
/** Settings for the applicationId passed to run_solution_query: basic fields + module setting groups.
 *  Throws when no applicationId was given. `search` is a case-insensitive REGEX narrowing the groups —
 *  ALWAYS pass it: the unfiltered payload is tens of thousands of characters. */
declare function getApplicationSettings(options?: { search?: string }): {
    id: string, name: string, description: string,
    metadataLocation: string, outputLocation: string,
    moduleSettingGroups: { id: string, title: string, module: string, settings: any[] }[],
};

declare function listInstalledModules(applicationId: string, searchString?: string):
    Promise<{ id: string, version: string, summary: string, isCompatible: boolean }[]>;

declare function searchAvailableModules(searchString: string, options?: { includePrerelease?: boolean, repositoryUrl?: string }):
    Promise<{ id: string, summary: string, authors: string, version: string, isCompatible: boolean, repository: string }[]>;

declare function searchArchitectures(query?: string, options?: { includePrerelease?: boolean }): Promise<any[]>;

declare function getArchitectureDetails(architectureId: string, options?: { includePrerelease?: boolean }): Promise<any>;
```

Notes:

- `outputLocation` is the resolved absolute on-disk path for generated code, `metadataLocation` the resolved absolute metadata path — use them directly rather than guessing the solution root.
- `search` matches every text field at both levels — a group's `title`/`module`, and a setting's `id`, `title`, `hint`, `controlType`, `value` and its `options[]`. Matching a GROUP returns that group entire; matching a SETTING returns just it. So you can find a setting by what it DOES, without knowing its name:

```js
getApplicationSettings({ search: "Domain" })          // the whole Domain group
getApplicationSettings({ search: "^Key Creation" })   // anchored — just that setting
getApplicationSettings({ search: "connection string" })  // matches a hint, wherever it lives
```

  Matching `value` and the option descriptions is deliberately broad, so a common word can drag back more than you expected — anchor with `^` when you know the name. No match returns `moduleSettingGroups: []` (the key is always present).
- Every module setting has an `id` unique within the application; pass that exact id to `update_application_settings`. `controlType` says how the value reads: `Checkbox`/`Switch` → `"true"`/`"false"`; `Select`/`MultiSelect` → one of `options[].value`; `Number` → a numeric string; the rest are free text.
- `searchArchitectures` with no `query` lists everything — do that rather than retrying different search terms.
- A solution global throws "this conversation is not working against an Intent Architect solution" when there is none; a query that touches none of them runs regardless.
