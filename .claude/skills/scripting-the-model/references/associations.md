# Associations

```ts
/**
 * Creates an association of `specialization` from a source element id, optionally to a target element id.
 * Pass `options` to set each end's cardinality (`sourceMultiplicity` / `targetMultiplicity`: "1",
 * "0..1", "1..*", "0..*") and end names. Omitting multiplicity defaults to 1-to-1.
 * Associations are UNIDIRECTIONAL by default (only the target end navigable) — leave `isBidirectional`
 * unset for the normal case. Set `isBidirectional: true` ONLY when two-way navigation is an explicit
 * intention in the design.
 */
declare function createAssociation(specialization: string, sourceElementId: string, targetElementId?: string, options?: {
    sourceMultiplicity?: string, targetMultiplicity?: string, sourceName?: string, targetName?: string, isBidirectional?: boolean,
}): IAssociationApi;
```

```js
const order = lookupByName("Order");
const orderLine = lookupByName("OrderLine");
if (!order || !orderLine) throw new Error(`Missing endpoint — Order: ${!!order}, OrderLine: ${!!orderLine}`);
// an Order has one-or-many OrderLines; each OrderLine has exactly one Order:
const assoc = createAssociation("Association", order.id, orderLine.id, { targetMultiplicity: "1..*", sourceMultiplicity: "1" });
// CONFIRM what was applied:
console.log(`${assoc.getName()}: target=${assoc.getMultiplicity()}, source=${assoc.getSourceEnd().getMultiplicity()}`);
```

**ALWAYS set cardinality via `options`** — an association created without it defaults to 1-to-1, which is usually wrong. UML multiplicity strings: `"1"` required-single, `"0..1"` optional-single, `"1..*"` one-or-many, `"0..*"` zero-or-many. `"*"` is a synonym for `"0..*"` (NOT `"1..*"`); anything else throws, naming the accepted set. `targetMultiplicity` is "how many targets per source" — the side you usually care about.

**ALWAYS read the applied cardinality back**, as the snippet above does — cardinality is stored as two flags rather than the string you passed, so the read-back is what proves the model says what you meant.

Change cardinality on an EXISTING end with `setMultiplicity`. Address an end by ROLE — `getSourceEnd()` / `getTargetEnd()` return the same end regardless of which handle you hold:

```js
assoc.getTargetEnd().setMultiplicity("0..*"); // target end → zero-or-many (the handle's own end, so assoc.setMultiplicity("0..*") is equivalent)
assoc.getSourceEnd().setMultiplicity("1");    // source end → exactly one
```

## Direction (navigability)

A bidirectional association has BOTH ends navigable; a unidirectional one has only the TARGET end navigable. New associations are unidirectional by default — only make one bidirectional when the design explicitly needs two-way navigation. Read and change direction at the **association** level; do not reach for navigability on the end directly:

```js
assoc.isBidirectional()        // true ⟺ both ends navigable
assoc.setBidirectional(false); // make unidirectional (only target end navigable)
assoc.setBidirectional(true);  // make bidirectional (only when the design calls for it)
```

(Lower-level: `a.getIsNavigable()` / `a.setIsNavigable(b)` toggle a SINGLE end — prefer `setBidirectional`.)

## Reading links

`x.getAssociations()` returns one handle per connected NEIGHBOUR. On each handle:

- `a.getAssociatedElement()` — the element across the link (the NEIGHBOUR, the element `x` connects to).
- `a.getName()` — `x`'s navigation name for that neighbour (an Order's link to Payment is "Payments").
- `a.getMultiplicity()` — cardinality toward the neighbour.
- `a.getSpecialization()` — the association's kind (e.g. `"Association"`), the same value `getAssociations(type)` filters on.
- `a.getThisElementsEnd()` — `x`'s OWN end; use it for the REVERSE name/cardinality back toward `x` (e.g. how many Orders per Payment).

```js
for (const a of order.getAssociations()) {
    // "Payments -> Payment (0..*)"
    console.log(`${a.getName()} -> ${a.getAssociatedElement()?.getName()} (${a.getMultiplicity()}) bidi=${a.isBidirectional()}`);
}
// Find a specific link ROBUSTLY by the connected element:
const toPayment = order.getAssociations().find(a => a.getAssociatedElement()?.getName() === "Payment");
```

**Advanced:** `getSourceEnd()` / `getTargetEnd()` address the two ends by ABSOLUTE ROLE; `isSourceEnd()` / `isTargetEnd()` tell you which role THIS handle is. `getOtherEnd()` exists only for macro back-compat and hops BACK to `x` itself after `getAssociations()` — never use it to reach the neighbour.

## The `IAssociationApi` surface

An association is modelled as its two ENDS; a handle is one end oriented AWAY from the element you got it from. `a.getSpecialization()` is the ASSOCIATION's type (not the end's own).

```js
a.id   a.specialization / a.getSpecialization()  // the ASSOCIATION's type
a.getName() / a.setName(v)                       // THIS end's name; the other via getSourceEnd()/getTargetEnd().setName(v)
a.getMultiplicity() / a.setMultiplicity("0..*")  // THIS end's cardinality
a.isBidirectional() / a.setBidirectional(b)
a.getIsNavigable() / a.setIsNavigable(b)         // THIS single end (low-level)
a.getThisElementsEnd()                           // the queried element's OWN end
a.getSourceEnd()  a.getTargetEnd()               // the two ends by ABSOLUTE ROLE
a.getOtherEnd()                                  // (back-compat only) the opposite end
a.isSourceEnd() / a.isTargetEnd()
a.getAssociatedElement() / a.getAssociatedElementId()  // the NEIGHBOUR; getElement()/getElementId() are aliases
a.getAssociation()                               // the canonical association handle (target end)
a.typeReference                                  // this end's type-ref (.getType(), .getIsCollection()/setIsCollection(b), …)
a.getParent()   a.getPackage()   a.delete()
a.getComment() / a.setComment(v)                 // THIS end's comment
a.ensureStereotype(n) / a.removeStereotype(n)    // on THIS end (see stereotypes.md); addStereotype(n) throws if already applied
a.getStereotypes() / a.getStereotype(n) / a.hasStereotype(n)
```

Associations can carry mappings — see `mappings.md`.
