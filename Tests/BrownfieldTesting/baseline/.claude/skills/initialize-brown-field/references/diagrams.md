# Optimising the diagrams

The reverse-engineer skills each leave one diagram per designer. On a real codebase those are unreadable: one Services diagram held 70 elements and hundreds of lines. A reviewer wants a page they can read in a minute: what an area's endpoints call, and what each operation does to which entity. So replace the all-in-one diagrams with **area diagrams**.

Load **changing-the-model** before you start (diagram layout is a separate step from model changes, done with `apply_change_diagram_layout`).

## How the designers behave

These behaviours shape everything below.

- **Every association between two placed elements is drawn.** Placing an element on a diagram makes the designer draw all its associations to elements already there, including calls between services from other areas (on one area's diagram, `Invoice` calling itself 17 times and calling `Customer` 9 times). `edges: []` keeps those lines. So lay out against every line that will be drawn, not just the area's own.
- **Placing a node adds it to the diagram.** `apply_change_diagram_layout` can place and move elements but never remove them. To take one off a diagram, see "Removing an element from a diagram" below.
- **`getDiagramSnapshot()` reads only the diagram open in the designer.** To read another diagram's nodes and sizes without changing anything, call `apply_change_diagram_layout` on it with `nodes: []` and `edges: []`. Its result lists every node with its real size.
- **Diagram names revert on save.** A diagram created by script takes its parent's name (the folder's or package's) when the designer is saved. Create the diagrams, save, then rename them by id in a separate script, save again, and re-query the names.
- **Saves time out.** `saveOnSuccess` sometimes fails with "the designer did not settle within 120s". Retry the no-op save; it once took three attempts.

## Services designer

### 1. Decide the areas

- **One diagram per feature area**, usually a top-level code folder. Controllers and business classes in the same `Orders` folders form the "Orders" diagram. Prefix sub-areas of a large folder: "Billing - Invoices", "Billing - Payment Matching".
- **Every service, controller, command and query belongs to exactly one area**, its home diagram. Its outgoing lines are laid out there. It also appears, without its own lines, on any other area's diagram that calls it.
- **Split an area that's too big to read.** As a guide, split when a diagram would carry more than about 20 elements or 150 lines. Split by controller or by aggregate. Give a service with very many operations (50 or more) its own diagram, and put the controller in front of it on another. For example, an `Invoice` service with 66 operations gets "Billing - Invoice Service", and `InvoiceController` gets "Billing - Invoice Endpoints".
- **Shared base classes and helpers** get a "Shared" diagram.
- **Measure before you create.** With `run_designer_query`, count each proposed area's own elements, the services it calls, its entities and its lines. Adjust the grouping until no diagram is oversized.
- An element no line reaches, such as an unused enum, still needs a home: list it in `EXTRAS` for its area.

### 2. Create the diagrams

- Create a `Diagrams` folder in the package and one `Diagram` per area inside it, then delete the all-in-one diagrams. The user asked for no all-in-one diagrams.
- Then rename the new diagrams by id after the first save (see above).

### 3. Lay them out: `scripts/services-area-layout.js`

The script turns the area list into ready layout payloads. It defines one function, `servicesAreaLayout(config)`, and its header describes the config: `AREAS`, and where needed `EXTRAS`, `ONLY`, `HEIGHTS` and `isEntry`. Run it with `run_designer_script` on the Services designer, passing the file in `includedScriptPaths` and `return servicesAreaLayout({ ... });` as the script (`run_designer_query` can't include a file). It only reads: the result's `changes` stays empty. When element names repeat across packages, give an area as `{ package, elements }`; the script reports a name it can't resolve to exactly one element rather than guessing. Use `ONLY` to emit four or five diagrams per run so each result stays readable. For each diagram it returns:

- `COLUMNS`: what went in each column, so you can sanity-check it;
- `NODES` and `EDGES`: pass them unchanged as `nodes` and `edges` to `apply_change_diagram_layout`;
- `LEFT-POINTING n`, when some lines must point left: two services that call each other can't both be to the right of the other.

The layout it produces is what the user approved:

- **Left to right by intent.** Entry points (controllers, HTTP-exposed services, and the area's elements nothing on the diagram calls) are in column 0. Everything an element invokes or acts on goes one column to the right of its deepest caller. That includes calls between other areas' services, because they are drawn too. Entities sit one column right of the operations that act on them.
- **Event handlers.** An integration or domain event handler is one of the area's elements. The message or domain event it subscribes to sits in column 0, left of the handler, and the handler's intent lines start at its subscription.
- **Order within a column.** The area's own elements and its entities come first, each level with the mean height of the operation rows that call it. Other areas' services go below them. Without this, a 1,250px-tall `Invoice` service takes the top of a column and pushes the area's own entities far down the page. The column is packed 100px apart, as close to those heights as the order allows.
- **Self-calls on the right-hand side.** When a service calls its own operations, the line leaves the caller's row on the right edge, runs down a vertical lane, and enters the callee's row from the right. Lanes start 16px right of the service and are 8px apart, with short spans nearest the box. Columns are 200px apart, wider when a column needs more lanes.
- **Geometry**, from the Services modeler's visual settings:
  - An operation row starts 56px below the service's top and is 18px tall, or 36px when the operation has `Http Settings`.
  - A line anchors at a row's centre-left or centre-right.
  - A service box is 60px plus its rows, rounded up to 10.
  - Messages and domain events are assumed to be 70px tall, event handlers 90px, and other elements 60px (measured with current modules).

After applying each payload, compare the heights in the tool's result with the script's assumptions. A module update can change the visual settings, and commands and queries may not be 60px. Where a height differs, add it to `HEIGHTS`, keyed by element id, by `Specialization:Name` or by a whole specialization, and run that diagram again. The result must report no overlaps.

If the script doesn't fit the codebase, lay out by hand to the same principles.

### 4. Removing an element from a diagram

When an element loses all its lines on a diagram, take it off. This happens, for example, after removing plumbing invocations at the user's request. The layout tool can't do this; a script can, by opening the diagram:

```js
const diagram = lookupById(diagramId);
if (!diagram) throw new Error(`No diagram ${diagramId}`);
await diagram.openDiagram();
const cd = getCurrentDiagram();
if (cd.isVisual(elementId)) cd.hideVisual(cd.getVisual(elementId));
return cd.isVisual(elementId); // false once removed
```

This is a deliberate exception to changing-the-model's rule against driving diagrams from a script: `apply_change_diagram_layout` can't remove an element, and there is no other way. It switches the user's open tab. Do all the removals in one script, then reopen a sensible diagram at the end. Then re-run the layout script for the affected diagrams.

## Domain designer

- **One diagram per area**: a code folder, or a cluster of aggregates that reference each other. Keep each to about 15 classes. Keep the enums on their own diagram, laid out as a grid. Give each separate persistence context (e.g. a second DbContext or package) its own diagram.
- **Lay out each one to the designer's own rules.** Read them from the diagram type in `getDesignerSchema()` on the Domain designer, as changing-the-model describes, and follow them as written. In current modules they describe a snowflake around the most-connected class, with minimum spacings.
- **Include the classes the area's associations point at**, even from other areas, on the outer ring, so every relationship line can be drawn.
- **Check coverage.** Every entity class must be on at least one diagram, and every association must be drawn on some diagram. Check both with `run_designer_query` before you finish.
