# Packages and package references

```ts
/** The editable packages currently loaded into the designer. */
declare function getPackages(): IPackageApi[];

/** A SINGLE editable package by name (or id). null when none; THROWS on >1 (pass the id). The only way to address a package. */
declare function lookupPackage(nameOrId: string): IPackageApi | null;

/**
 * Creates a package as "New Package" does, with the type's default references loaded. `packageType`
 * (name or id) may be omitted only if the designer has one type or its packages share one. THROWS on a bad name or type. AWAIT it.
 */
declare function createPackage(name: string, packageType?: string): Promise<IPackageApi>;
```

An empty designer needs a package before anything can be created in it — create it yourself; don't ask the user to:

```js
const pkg = lookupPackage("<package name>") ?? await createPackage("<package name>", "<package type>"); // type from getDesignerSchema()
return pkg.getName();
```

Don't assume a package is called "Domain" — get exact names from `getDesignerSchema()`, or list them at runtime:

```js
const pkgNames = getPackages().map(p => p.getName());
const domain   = lookupPackage("Domain");
```

## Loading and unloading packages

A package file can exist on disk without being loaded into the designer, e.g. after someone unloaded it. **Load it rather than creating a new one** — `createPackage` throws when the file already exists.

```ts
/** This designer's packages on disk (under its folder) that aren't loaded. Read-only, so it works in run_designer_query too. */
declare function getUnloadedPackages(): { id: string, name: string, specialization: string, absolutePath: string }[];

/** Loads one, as "Add existing package..." does: an id from getUnloadedPackages(), or a .pkg.config path (absolute or relative to the designer folder). THROWS if already loaded or it fails to load. AWAIT it. */
declare function loadPackage(pathOrId: string): Promise<IPackageApi>;

/** Unloads a package by name or id; the file stays. Asks the user first if it has unsaved changes and THROWS if they decline, unless discardChanges. AWAIT it. */
declare function unloadPackage(nameOrId: string, options?: { discardChanges?: boolean }): Promise<void>;
```

```js
const name = "<package name>";
let pkg = lookupPackage(name); // already loaded, e.g. on a re-run
if (!pkg) {
  const unloaded = getUnloadedPackages().find(p => p.name === name); // .find misses as undefined
  if (!unloaded) throw new Error(`Not on disk: ${getUnloadedPackages().map(p => p.name).join(", ")}`);
  pkg = await loadPackage(unloaded.id);
}
return pkg.getName();
```

A package in another folder isn't listed — pass its `.pkg.config` path to `loadPackage` instead.

`createPackage`, `loadPackage` and `unloadPackage` save the designer themselves, so the change is already on disk. The tool result's `saved: false` only reports the optional `saveOnSuccess` save.

## A package is NOT an element

The element finders (`findElements` / `lookupByName` / `lookupByPath`) NEVER return a package — `lookupByName("Domain")` is `null`.

A package handle supports the same container/identity members as an element:

```js
pkg.addChild(spec, name)                       // same as createElementUnder(pkg, spec, name)
pkg.getChild(fn, searchHierarchy?)   pkg.getChildren(spec?)   pkg.findChildren({ ... })
pkg.getName() / pkg.setName(v)       pkg.getComment() / pkg.setComment(v)
pkg.ensureStereotype(n) / pkg.addStereotype(n) / pkg.removeStereotype(n) / pkg.getStereotype(n) / pkg.getStereotypes()
pkg.setMetadata(k, v) / pkg.getMetadata(k) / pkg.hasMetadata(k)
pkg.isReference()   pkg.getAbsolutePath()   pkg.isLoading()   pkg.hasLoadError()
await pkg.unload(options?)                     // same as unloadPackage(pkg.getId(), options?)
```

`pkg.delete()` does NOT delete the file — it unloads without asking about unsaved changes. Use `pkg.unload()`.

It does NOT have element-only members: no type reference, value, mappings, associations, abstract/static.

## Package references

A package handle also manages its package references — the packages it depends on, what the "Package Reference Manager" shows. **This is the ONLY way to add or remove a reference; there is no mutation tool for it.**

```js
pkg.getReferences()                    // current references
await pkg.addReference(absolutePath, module?) // add one; async, resolves once its types are usable; idempotent (an existing reference is returned)
pkg.removeReference(idOrNameOrPath)     // remove one
```

Discover what can be added with the `getAvailablePackageReferences()` global:

```ts
/**
 * Packages available to ADD as a reference (app-wide: installed-module + other-application packages),
 * each classified by `sourceType` ('module' | 'solution'). Async — AWAIT it. Pass an item's
 * `absolutePath` (and, for a module package, its `source` as the module id) to `pkg.addReference`.
 */
declare function getAvailablePackageReferences(): Promise<{
    packageId: string, name: string, packageType: string,
    sourceType: "module" | "solution", source: string,
    absolutePath: string, isExternal: boolean,
}[]>;
```

```js
const cands = await getAvailablePackageReferences();
const dep   = cands.find(c => c.name === "WTWDemo.Ordering.Domain");   // .find misses as undefined
const pkg   = lookupPackage("WTWDemo.Ordering.Services");              // lookupPackage misses as null
if (!dep || !pkg) throw new Error(`Missing — dependency: ${!!dep}, package: ${!!pkg}`);
await pkg.addReference(dep.absolutePath, dep.sourceType === "module" ? dep.source : undefined);
```

Reading the *current* references is `getPackageReferences(packageId?)` — see `reads.md`.
