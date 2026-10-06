# Packages and package references

```ts
/** The editable packages currently loaded into the designer. */
declare function getPackages(): IPackageApi[];

/** A SINGLE editable package by name (or id). null when none; THROWS on >1 (pass the id). The only way to address a package. */
declare function lookupPackage(nameOrId: string): IPackageApi | null;

/**
 * Creates a package, as the designer's "New Package" does: placed beside the existing ones (written on save), with
 * the package type's default references loaded. `packageType` is a package specialization name or id (default: the
 * designer's first). The name is trimmed. THROWS on a duplicate, an invalid file name or an unknown type. AWAIT it.
 */
declare function createPackage(name: string, packageType?: string): Promise<IPackageApi>;
```

An empty designer needs a package before anything can be created in it — create it yourself; don't ask the user to:

```js
const pkg = lookupPackage("<package type>") ?? await createPackage("<package type>"); // Use correct package type
return pkg.getName();
```

Don't assume a package is called "Domain" — get exact names from `getDesignerSchema()`, or list them at runtime:

```js
const pkgNames = getPackages().map(p => p.getName());
const domain   = lookupPackage("Domain");
```

## A package is NOT an element

The element finders (`findElements` / `lookupByName` / `lookupByPath`) NEVER return a package — `lookupByName("Domain")` is `null`.

A package handle supports the same container/identity members as an element:

```js
pkg.addChild(spec, name)                       // same as createElementUnder(pkg, spec, name)
pkg.getChild(fn, searchHierarchy?)   pkg.getChildren(spec?)   pkg.findChildren({ ... })
pkg.getName() / pkg.setName(v)       pkg.getComment() / pkg.setComment(v)
pkg.ensureStereotype(n) / pkg.addStereotype(n) / pkg.removeStereotype(n) / pkg.getStereotype(n) / pkg.getStereotypes()
pkg.setMetadata(k, v) / pkg.getMetadata(k) / pkg.hasMetadata(k)
pkg.isReference()   pkg.delete()
```

It does NOT have element-only members: no type reference, value, mappings, associations, abstract/static.

## Package references

A package handle also manages its package references — the packages it depends on, what the "Package Reference Manager" shows. **This is the ONLY way to add or remove a reference; there is no mutation tool for it.**

```js
pkg.getReferences()                    // current references
pkg.addReference(absolutePath, module?) // add one
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
pkg.addReference(dep.absolutePath, dep.sourceType === "module" ? dep.source : undefined);
```

Reading the *current* references is `getPackageReferences(packageId?)` — see `reads.md`.
