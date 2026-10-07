# Worked recipes

**Bind, then guard — never chain onto a search.** `lookupByName` / `lookupByPath` / `lookupById` / `lookupPackage` / `el.getChild(...)` / `el.getType()` all return `null` when nothing matches, and `find*` / `.find(...)` return `[]` / `undefined`. Chaining straight off one (`lookupByName("X").addChild(...)`, `el.getChild(...).ensureStereotype(...)`) fails with `Cannot read properties of null` and tells you nothing about which lookup missed. Assign to a variable, check it, then use it. The one handle that can never miss is the one a `create*` / `add*` just returned — so pass that along rather than looking the thing up again.

## (a) One query instead of four read calls — `run_designer_query`

```js
const orders = await searchElements("^Order", { specializations: ["Class"] });
const first  = orders.matches[0];                          // searchElements can return NO matches
return {
    elementTypes: getElementTypes(),                        // carries the designer's rules — don't drop them
    matches: orders.matches.map(m => m.path),
    order: first ? await getElementDetails(first.path) : null,
};
```

**Return only the fields you will act on.** A bare `return { schema, orders }` is tens of thousands of characters of payload you pay for and mostly discard; anything over ~200,000 is truncated outright.

## (b) Idempotently add an `Email` string property to the `Customer` entity

```js
const customer = lookupByName("Customer");
if (!customer) throw new Error(`No "Customer" element in this designer.`);
const existing = customer.getChildren("Attribute").find(a => a.getName().toLowerCase() === "email");
const email = existing ?? customer.addChild("Attribute", "Email");   // `.find` misses as undefined — the `??` is what makes this safe
email.setType("string");
```

## (c) Create a `CreateOrder` command with an `OrderDto` field

```js
const command = createElementUnder("Services/Commands", "Command", "CreateOrder");
const dto     = createElementUnder("Services/Contracts", "DTO", "OrderDto");
const field   = command.addChild("Parameter", "order");
field.setType("OrderDto");
```

## (d) Apply a stereotype and set its properties (idempotent)

```js
const customer = lookupByName("Customer");
const email = customer?.getChild(c => c.getName() === "Email");
if (!email) throw new Error(`No "Email" child under Customer — check the name the designer actually stored.`);
const col = email.ensureStereotype("Column");
col.setProperty("Name", "email_address");
```

## (e) Wire an association between two existing entities by name, WITH cardinality, and confirm it

```js
const customer = lookupByName("Customer");
const order = lookupByName("Order");
if (!customer || !order) throw new Error(`Missing endpoint — Customer: ${!!customer}, Order: ${!!order}`);
// a Customer has zero-or-many Orders; each Order has exactly one Customer:
const a = createAssociation("Association", customer.id, order.id, { targetMultiplicity: "0..*", sourceMultiplicity: "1" });
return { name: a.getName(), target: a.getMultiplicity(), source: a.getSourceEnd().getMultiplicity() };
```

## (f) Find work within a subtree (never throws)

```js
return lookupByName("Customer")?.findChildren({ specialization: "Attribute" }).map(a => a.getName()) ?? [];
```

## (g) Create two entities + an association — one script, one undo step. NO layout here

```js
const customer = createElementUnder("Domain", "Class", "Customer");
const order    = createElementUnder("Domain", "Class", "Order");
createAssociation("Association", customer.id, order.id, { targetMultiplicity: "0..*", sourceMultiplicity: "1" });
// ...then lay the diagram out as a SEPARATE step with apply_change_diagram_layout — not in this script.
```

After running, inspect the returned `errors[]` (scoped to what you changed). If non-empty, fix the issues in a follow-up script and re-run until clear.

## (h) Advanced-map a command's fields onto a domain entity (discover → create → map → verify)

```js
const command = lookupByName("CreateCustomer");
if (!command) throw new Error(`No "CreateCustomer" element in this designer.`);
const m = command.createAdvancedMapping("Mapping Type Name");   // the REAL name comes from the schema
m.mapEnd(["Name"], ["Name"]);
m.mapEnd(["Email"], ["Email"]);
return m.getMappedEnds().map(e => `${e.sourcePath.map(p => p.name).join(".")} -> ${e.targetPath.map(p => p.name).join(".")}`);
```

## (i) Basic-map a DTO field to a domain attribute

```js
const field     = lookupByName("CustomerDto")?.getChild(c => c.getName() === "Name");
const attribute = lookupByName("Customer")?.getChild(c => c.getName() === "Name");
if (!field || !attribute) throw new Error(`Missing end — field: ${!!field}, attribute: ${!!attribute}`);
field.setBasicMapping(attribute.id);
```
