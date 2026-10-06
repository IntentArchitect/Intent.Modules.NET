# Mappings — connecting an element (or association) to another model's elements

A mapping links a source element to target element(s) — e.g. a Command's fields onto a Domain entity's attributes. **Not every designer/type supports mapping.** There are two kinds:

- **BASIC** (projection): one element maps to a single target via a path. `el.setBasicMapping(...)`
- **ADVANCED** (element-to-element): a mapping TYPE holding many source→target "mapped ends". `el.createAdvancedMapping(type)` then `mapping.mapEnd(...)`

## Discover first

The mapping-type name is **DESIGNER-SPECIFIC**. There is no universal name — it is NOT always "Map to Domain"; one designer's types might be "Query Entity Mapping" / "Update Entity Mapping". `"Mapping Type Name"` below is a PLACEHOLDER, not a literal to copy.

Get the real names from `getDesignerSchema()`'s **"Mappings"** block — it lists, per element AND association type, the exact names to pass to `createAdvancedMapping` / `getAdvancedMapping`. Passing one that doesn't exist THROWS, listing the valid names. `getElementDetails(...)` shows what an element is ALREADY mapped to (`mapping` = basic, `mappings` = advanced). Create both endpoints (and any child fields you map) BEFORE mapping between them.

## Basic (projection) — a single path to one target

```js
field.isBasicMapped()                        // is it mapped?
field.setBasicMapping(attribute.id)          // map to one element by id
field.setBasicMapping([entity.id, attr.id])  // map THROUGH a path (entity → its attribute)
field.getBasicMapping()                      // read: .getElement(), .getPath() (each .name/.getElement()), .mappingSettingsId
field.clearBasicMapping()                    // remove the mapping
```

## Advanced (element-to-element) — create once, then add mapped ends

```js
const m = command.createAdvancedMapping();                     // omit the type when the element has exactly one
const m = command.createAdvancedMapping("Mapping Type Name");  // else pass the mapping-type NAME (or id)
m.getAvailableMappingTypes();                                  // the mapping-type names (only needed when >1 exists)
m.mapEnd(["Name"], ["Name"]);                                  // connect a source path → a target path (the DEFAULT)
m.mapEnd(["Total"], ["Order", "Total"]);                       // paths descend with extra segments
m.mapEnd(["Name"], ["Name"], "Data Mapping");                  // mapping type is OPTIONAL + LAST — pass it only to be explicit
m.mapExpression(["IsActive"], "true");                         // a LITERAL/expression on a target — when no source maps to it
m.mapExpression(["DisplayName"], "{FirstName} {LastName}");     // or an interpolated expression of source elements
m.removeMappedEnd(["Address"]);                                // remove ONE mapped end (inverse of mapEnd) by its target path
m.getMappedEnds();                                             // read back what's mapped — VERIFY after mapping
command.getAdvancedMapping("Mapping Type Name");               // fetch an existing one (null when none yet)
m.delete();                                                    // delete the WHOLE mapping (inverse of createAdvancedMapping)
```

**REPLACE-or-CREATE idiom** — `getAdvancedMapping` returns null when absent, so delete-if-present then recreate:

```js
const existing = command.getAdvancedMapping("Mapping Type Name");
if (existing) { existing.delete(); }
const m = command.createAdvancedMapping("Mapping Type Name");
```

## Paths

Paths take element NAMES *or* ids per segment, are rooted at the mapped element, and **OMIT that root**: `["Name"]` is the mapped element's `Name` child; `["Order", "Total"]` descends two levels.

`mapEnd` / `mapExpression` / `removeMappedEnd` THROW a descriptive error naming the segment that didn't resolve and listing what was available there — read it and correct the path.

`mapEnd` is **IDEMPOTENT**: mapping an end that already exists is a no-op, not a duplicate error, so re-running a script is safe. `removeMappedEnd(targetPath)` deletes just the one end at that target — e.g. drop a whole-object `["Address"]` mapping while leaving `["Address","Line1"]` in place. To remove the mapping ENTIRELY, call `m.delete()`.

## `mapEnd` vs `mapExpression`

Pick **ONE** per target end, never both — they are mutually exclusive ways to fill the same target. `mapEnd` is the DEFAULT: a direct source → target connection, used whenever a source element/path exists to map from. Reach for `mapExpression` ONLY when NO source maps to that target — a literal or constant (`"true"`, `"false"`, `"0"`, `"Active"`) or a value derived from an expression. If you can `mapEnd` it, do that.

## Associations can carry mappings too

It works exactly like an element — the source & target are the association's own ends, so just pass the mapping TYPE (discovered from the schema's "Mappings" block, same as elements):

```js
const action = createAssociation("Update Entity Action", command.id, entity.id);
const m = action.getAdvancedMapping("Update Entity Mapping") ?? action.createAdvancedMapping("Update Entity Mapping");
m.mapEnd(["Email"], ["Email"]);
```
