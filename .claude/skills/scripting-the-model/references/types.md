# Types — resolving and setting

Set the type **directly on the element**; there is no need to reach through `typeReference`.

```js
el.setType("string")                              // primitive by name
el.setType("Customer")                            // user type by name
el.setType({ name: "Order", isCollection: true }) // a COLLECTION of Order
el.setType({ name: "Order", isNullable: true })   // optional / nullable
el.setType("Dictionary<string,int>")              // generic sugar — only for a type declaring generic params
el.setType({ name: "PagedResult", genericArgs: ["OrderSummaryDto"] })  // the same, as an object
el.setType("<a-guid>")                            // an existing id
el.setType("void")                                // CLEAR the type (same as setType(null))
el.getType()                                      // read it back (the type as an element, or null)
```

A non-GUID string resolves by name; a GUID keeps exact existing behaviour. `"void"` / `null` / `""` clear the type (an operation or command with no return type is "void"). `setType` throws when the element has no type reference — check `el.hasType`.

The object form takes exactly `name`, `isCollection`, `isNullable` and `genericArgs`; any other key throws. Each `genericArgs` entry is a name string or a nested object of the same shape, e.g. `{ name: "OrderDto", isCollection: true }`.

**Order matters.** A name resolves to an id at call time, so the type must already exist. If one script both creates a type and references it, create it first.

**You are MODELLING, not writing C#.** The only valid types are the ones the designer defines: primitives, your own elements, and the reference types the schema lists (`referenceTypes` / referenced packages). If a name does not resolve it is not a modelling type — consult the schema, don't invent one.

- **Collections:** there is no `List` / `IEnumerable` / array type. "Many" is the `isCollection` flag on the element type itself — `setType({ name: "OrderLine", isCollection: true })`. `setType("List<OrderLine>")` is a coding habit, not a model. `isNullable` is "optional" the same way.
- **Generics:** generic arguments apply only to a type that itself declares generic parameters, and you must supply exactly as many as it declares — an unset or unresolvable generic argument is a validation error. The schema's `referenceTypes` shows generic types with their parameter signature (`Dictionary<TKey, TValue>`, `Task<T>`), so you can tell which need arguments and how many; a bare name takes none. To make your own type generic (e.g. a `Result<T>` Type-Definition or DTO), call `el.addGenericType("T")` on it first (it returns the parameter; `.id` is a valid `setType` id) — `genericTypes` is a read-only display string. Members of that element then type themselves with it by name: `field.setType("T")`, `{ name: "T", isCollection: true }`, or as an argument (`{ name: "List", genericArgs: ["T"] }`); the element's own parameters win over a same-named type elsewhere.

For finer control the lower-level `el.typeReference` (`ITypeReference`) is still there, only when `el.hasType`:

```js
tr.setType(...)   tr.getType()   tr.isTypeFound()
tr.getIsCollection() / tr.setIsCollection(b)
tr.getIsNullable()   / tr.setIsNullable(b)
tr.getIsNavigable()  / tr.setIsNavigable(b)
```

```ts
/**
 * Resolves a type NAME to its element id. Type-Definitions (primitives like `string`, `int`, `Guid`,
 * `datetime`, plus reference type-defs) win over user classes sharing the name. null if unknown;
 * THROWS if a user-element name is ambiguous.
 */
declare function resolveType(name: string): string | null;
```
