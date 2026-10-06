# Stereotypes and their properties

```js
const st = entity.ensureStereotype("Http Settings");  // by NAME (from the schema; a definition id also works)
st.setProperty("Verb", "POST");                        // set a property value in one call
```

**Prefer `ensureStereotype`** — it is idempotent (applies the stereotype, or returns it if already applied), so re-running a script is safe. Pass the stereotype NAME (what `getDesignerSchema()` lists; a definition id also works) — the same name `getStereotype` / `hasStereotype` take. An off-name throws, listing the stereotypes applicable to that element. `addStereotype` is the lower-level form and THROWS if the stereotype is already applied.

If a stereotype's apply mode is `Always` it is applied automatically — don't add it.

## Property values must be valid

`getDesignerSchema()` lists each property as `Name: allowed`. Use a listed value **exactly**; an off-list value throws listing the allowed ones.

- `ref to <Types>` → set the **id** of an element of one of those types.
- `(multiple)` → pass a JSON array of ids, e.g. `st.setProperty("Security Roles", [roleId])`.
- `true/false` → boolean.
- A ref id that isn't a valid option (wrong type, or in a package this one doesn't reference) THROWS naming the allowed type — resolve the element by name and add a package reference if needed.
- Read a value back with `st.getProperty(name).getValue()` (an ARRAY of elements for a `(multiple)` ref) — **not** the captured `.value` snapshot.
- The text after ` — ` on a property's schema line is the module author's own **hint**: what the value means, what a path is relative to, how it is spelt. It is authoritative and frequently un-guessable — read it before setting the value.

## Item lists

A property that holds a repeating list of ROWS rather than one value (e.g. one glob rule per row, each with its own severity). The schema shows one as `Name: item-list of '<Row Type>'` and lists that row type's own properties under the stereotype's `itemTypes`.

`setValue` / `setProperty` do **NOT** work on one: its `value` is only a cache of the row labels, so writing it stores labels and no rows — and reading it back gives you the stale label cache, not the rows. Use the item members:

```js
const prop = st.getProperty("Entries");
prop.isItemList()                                 // true for this shape; the members below throw on any other
const row = prop.addItem();                       // append a row, returns its handle
row.setProperty("Glob", "**/*.cs");               // the row's own properties (schema: `itemTypes`)
row.setProperty("Severity", "3 - High");
prop.getItems()                                   // the rows, in order
prop.getItems()[0].getProperty("Glob").getValue() // read one back
row.remove();  prop.removeItem(0);  prop.clearItems();   // drop one row (handle or index) / all rows
prop.moveItem(2, 0);  row.moveTo(0);              // re-order a row to a new position
```

A row handle's `delete()` drops the ROW, not the owning element's stereotype.

Rows are **ORDERED and the order is significant**: where several rows match, the **LAST matching row wins**. Add the broad rule first and the specific overrides after it. (For a stereotype applicable to nested containers — e.g. `Custom File Classification` on a project and again on a folder inside it — the DEEPEST container holding the file wins outright, and only then does last-row-wins decide between that container's own rows. A module states any rule of its own in the stereotype's `description`.)

## Removing

Stereotypes live on the OWNER (element **or** package) and you add/remove them from that owner — there is **no `st.remove()`**:

```js
entity.removeStereotype("Audit");                    // by name or definition id
lookupPackage("Domain")?.ensureStereotype("Name").setProperty("Key", "v"); // packages carry stereotypes too
entity.getStereotypes() / entity.getStereotype(name) // read what's applied
st.getProperty(name).getValue() / st.getProperties()  // read one / every property; each has getName() + getValue()
```

A held stereotype handle also has `st.delete()`, which just calls the owner's `removeStereotype`.

**Associations** carry stereotypes PER END, with the same `ensureStereotype` / `addStereotype` / `removeStereotype` / `getStereotype` members — call them on the end that owns the stereotype (`a.getSourceEnd()` / `a.getTargetEnd()`).
