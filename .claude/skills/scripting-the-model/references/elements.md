# Elements — finding, creating, navigating, editing

## Finding (top-level globals; they search the whole designer)

```ts
/** Finds many. Never throws — returns []. The primitive to reach for first. */
declare function findElements(criteria: {
    name?: string, nameContains?: string, specialization?: string | string[],
    parentPath?: string, includeReferences?: boolean,
}): IElementApi[];

/** Many by exact (case-insensitive) name, optionally filtered by specialization. Never throws. */
declare function findElementsByName(name: string, specialization?: string | string[]): IElementApi[];

/** A SINGLE element by exact name. null when none; THROWS on >1, counting a referenced element of another type (pass a specialization, or use lookupByPath). */
declare function lookupByName(name: string, specialization?: string | string[]): IElementApi | null;

/** A SINGLE element by `/`-separated, package-rooted, folder-aware path. null when none; THROWS on ambiguity. */
declare function lookupByPath(path: string): IElementApi | null;

/** A SINGLE element by id — only when you already hold one from a tool result. */
declare function lookupById(id: string): IElementApi | null;
```

All of these are editable-first with a reference fallback (see SKILL.md). None of them ever returns a **package** — use `lookupPackage` / `getPackages` (see `packages.md`).

## Creating

```ts
/** Creates an element under a parent: an editable-package NAME, a folder/element path, an element/package handle, or an id. */
declare function createElementUnder(parent: string | IElementApi | IPackageApi, specialization: string, name: string): IElementApi;
```

```js
const attr   = entity.addChild("Attribute", "Email");                               // element-relative (preferred)
const folder = createElementUnder("Domain", "Folder", "Ordering");                   // bare package name
const cmd    = createElementUnder("Services/Commands", "Command", "CreateOrder");    // package-rooted path
const cmd2   = createElementUnder(folder, "Command", "CreateOrder");                 // a handle
```

**The designer may rewrite a name you supply** to its own convention (`addChild("Attribute", "SKU")` can store `Sku`), so **never look an element back up by the literal you passed** — keep the handle `addChild` / `createElementUnder` returned and use that (`addUniqueIndex(entity.addChild("Attribute", "SKU"))` cannot fail), or match case-insensitively.

Before creating children or setting types, consult `getDesignerSchema()`'s **"Element types"** block: per type it lists the exact child specialization names accepted, marks any type that REQUIRES a type reference with `!` — including on a child reference (e.g. `Class: Attribute!`), so set a type wherever you see it — and `¹` for max-one children. Use those exact names; `addChild` throws if the parent does not accept the specialization.

## The `IElementApi` surface

These are the real members. Accessors are METHODS, not bare properties.

**Identity / read**

```js
el.getId()                // the element's id (or bare `el.id`) — the only thing you echo back to tools
el.getSpecialization()    // human type name ("Class", "Attribute", …); el.specializationId = its id
el.getName()              // NOT a `.name` property
el.getDisplay()           // full display label
el.getComment()   el.getValue()   el.getIsAbstract()   el.getIsStatic()   el.getOrder()
el.hasStereotype(n)   el.hasErrors()   el.hasWarnings()
el.isReference()          // true = from a referenced (read-only) package
```

**Navigate (read)** — the global finders are NOT element methods; to search inside an element:

```js
el.getChildren(spec?)     // ARRAY of direct children (+ association ends), optionally filtered
el.findChildren({ name?, nameContains?, specialization?, includeReferences? })  // descendants at ANY depth; never throws
el.getChild(c => c.getName() === "Email", searchHierarchy?)                     // first MATCHING child, or null
el.getParent(spec?)   el.getPackage()   el.getAssociations(spec?)
// `getChildren()` is the ARRAY; `el.children` is a legacy METHOD — don't use it.
```

**Mutate**

```js
el.setName(name)          // optional 2nd arg `true` to auto-deduplicate
el.setComment(text)   el.setValue(v)   el.setIsAbstract(b)   el.setIsStatic(b)   el.setOrder(i)
el.setType("string")      // see types.md
el.getType()              // the referenced type as an element (null if none)
el.addChild(spec, name)   // create a child; `spec` is resolved against what THIS element accepts
el.setParent(parentId)    // MOVE under a new parent — don't delete + recreate to move something
el.delete()
el.ensureStereotype("Name")   // see stereotypes.md
el.removeStereotype(nameOrId)
el.setMetadata(key, val)   el.getMetadata(key)   el.hasMetadata(key)
```

Mappings (`setBasicMapping`, `createAdvancedMapping`, …) are in `mappings.md`.

When moving children to a new parent, move them **all** before deleting the old parent.
