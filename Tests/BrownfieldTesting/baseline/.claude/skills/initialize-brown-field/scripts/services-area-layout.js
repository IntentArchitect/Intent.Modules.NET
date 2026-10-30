// Left-to-right layout for Services area diagrams. See ../references/diagrams.md.
//
// Run it with run_designer_script on the Services designer (run_designer_query can't include files):
//   includedScriptPaths: ["<this skill>/scripts/services-area-layout.js"]
//   script: return servicesAreaLayout({ AREAS: { ... }, ONLY: [ ... ] });
// It changes nothing (the result's `changes` stays empty): it returns, per diagram, the `nodes` and `edges`
// to pass to apply_change_diagram_layout.
//
//   - Entry points (controllers, HTTP-exposed services, and the area's elements nothing on the diagram calls)
//     take column 0. The message or domain event an event handler subscribes to sits in column 0, left of it.
//   - Everything an element invokes or acts on sits to its right: one column right of its deepest caller.
//   - Within a column, the area's own elements and entities come first, aligned with the operations
//     that call them; services from other areas go below them.
//   - A service calling its own operations is routed around its right-hand edge, in lanes.
//
// config:
//   AREAS   Diagram name -> the area's own elements (services, controllers, commands, queries, event handlers).
//           Either a list of names, looked up across the designer, or { package, elements } to look the names
//           (and the diagram) up in one package, for when names repeat across packages.
//   EXTRAS  Diagram name -> names of other elements to stack under column 0 (e.g. an enum no line reaches),
//           looked up in the area's package when it names one.
//   ONLY    Diagram names to emit in this run, or null for all. Keep each result small enough to read.
//   HEIGHTS Height the designer reported, wherever it differs from the estimates below. Keyed by element id
//           (always unambiguous), "Specialization:Name" (e.g. "Service:Invoice") or a specialization alone
//           (e.g. "Command"). Bare names are not used: a service and an entity often share one.
//   isEntry el => true for the elements that take column 0. Defaults to controllers and services with an
//           HTTP-exposed operation.
function servicesAreaLayout(config) {
    const { AREAS = {}, EXTRAS = {}, ONLY = null, HEIGHTS = {} } = config || {};
    const isEntry = (config && config.isEntry) || (el =>
        /Controller$/.test(el.getName()) || el.getChildren("Operation").some(o => o.hasStereotype("Http Settings")));

    const W = 200, GAP_Y = 100, MIN_GAP_X = 200, LANE0 = 16, LANE_STEP = 8;
    const OWN_KINDS = ["Service", "Command", "Query", "Integration Event Handler", "Domain Event Handler"];
    // The end of a handler that names what it subscribes to.
    const ENDS = ["Subscribe Integration Event", "Domain Event Handler Association"];
    // Heights measured with current modules, used unless HEIGHTS says otherwise.
    const DEFAULT_HEIGHTS = { "Message": 70, "Domain Event": 70, "Integration Event Handler": 90, "Domain Event Handler": 90 };
    const INTENT = /Entity Action|Perform Invocation|Publish Integration Event|Send Integration Command/;
    const problems = [];

    const pkgOf = el => { let e = el; while (e.getParent?.()) e = e.getParent(); return e.getName(); };
    const lookup = (name, pkg, kinds, what) => {
        const hits = findElements({ name }).filter(e => !e.isReference() && (!pkg || pkgOf(e) === pkg) && (!kinds || kinds.includes(e.getSpecialization())));
        if (hits.length !== 1) problems.push(`${hits.length} ${what} named "${name}"${pkg ? ` in ${pkg}` : ""}`);
        return hits.length === 1 ? hits[0] : null;
    };
    const diagrams = findElements({ specialization: "Diagram" });
    const diagramOf = (name, pkg) => {
        const hits = diagrams.filter(d => d.getName() === name && (!pkg || pkgOf(d) === pkg));
        if (hits.length !== 1) problems.push(`${hits.length} diagrams named "${name}"${pkg ? ` in ${pkg}` : ""}`);
        return hits.length === 1 ? hits[0] : null;
    };

    // Geometry from the Services modeler's visual settings: a Service's operation rows start 56px below its
    // top and are 18px tall (36px with Http Settings); a line anchors at a row's centre-left or centre-right.
    // The box is 60px plus its rows, rounded up to 10. Anything else is DEFAULT_HEIGHTS or 60px (set HEIGHTS).
    const rowCache = {};
    const rows = el => (rowCache[el.id] ??= el.getSpecialization() !== "Service" ? null : (() => {
        let y = 56;
        return el.getChildren("Operation").map(o => {
            const h = o.hasStereotype("Http Settings") ? 36 : 18;
            const row = { id: o.id, mid: y + h / 2, h };
            y += h;
            return row;
        });
    })());
    const height = el => HEIGHTS[el.id] ?? HEIGHTS[`${el.getSpecialization()}:${el.getName()}`] ?? HEIGHTS[el.getSpecialization()]
        ?? DEFAULT_HEIGHTS[el.getSpecialization()] ?? (rows(el) ? Math.ceil((60 + rows(el).reduce((s, r) => s + r.h, 0)) / 10) * 10 : 60);
    const anchor = (node, el) => rows(node)?.find(r => r.id === el.id)?.mid ?? height(node) / 2;
    const nodeOf = el => (["Operation", "Class Constructor", ...ENDS].includes(el.getSpecialization()) ? el.getParent() : el);
    const handlerEnds = el => el.getChildren().filter(c => ENDS.includes(c.getSpecialization()));
    const sources = el => (el.getSpecialization() === "Service" ? el.getChildren("Operation")
        : handlerEnds(el).length ? handlerEnds(el) : el.getChildren("Operation").length ? el.getChildren("Operation") : [el]);

    // A handler's subscription end doesn't expose its associations, so index every intent association by its
    // source once, walking every package (one side of each intent association lives in this designer).
    const IDX = new Map(), seen = new Set();
    const index = el => {
        for (const c of el.getChildren()) {
            for (const a of c.getAssociations?.() ?? []) {
                const as = a.getAssociation();
                if (seen.has(as.id)) continue;
                seen.add(as.id);
                if (!INTENT.test(a.getSpecialization())) continue;
                const src = as.getSourceEnd().getAssociatedElement?.(), tgt = as.getTargetEnd().typeReference?.getType?.();
                if (!src || !tgt) { problems.push(`unresolved ${a.getSpecialization()} ${as.id}`); continue; }
                if (!IDX.has(src.id)) IDX.set(src.id, []);
                IDX.get(src.id).push({ spec: a.getSpecialization(), assocId: as.id, t: tgt });
            }
            index(c);
        }
    };
    for (const p of getPackages()) index(p);
    const outgoing = el => sources(el).flatMap(s => (IDX.get(s.id) ?? []).map(x => ({ s, ...x })));

    const r10 = v => Math.round(v / 10) * 10;
    // Weighted isotonic regression: the non-decreasing sequence closest to the targets.
    const pav = (target, weight) => {
        const blocks = [];
        target.forEach((v, i) => {
            blocks.push({ v, w: weight[i], n: 1 });
            while (blocks.length > 1 && blocks[blocks.length - 2].v > blocks[blocks.length - 1].v) {
                const b = blocks.pop(), a = blocks.pop(), w = a.w + b.w;
                blocks.push({ v: (a.v * a.w + b.v * b.w) / w, w, n: a.n + b.n });
            }
        });
        return blocks.flatMap(b => Array(b.n).fill(b.v));
    };

    const out = [];
    for (const [name, area] of Object.entries(AREAS)) {
        if (ONLY && !ONLY.includes(name)) continue;
        const pkg = !area || Array.isArray(area) ? null : area.package;
        const ownNames = Array.isArray(area) ? area : (area && area.elements) || [];
        const diagram = diagramOf(name, pkg);
        if (!diagram) continue;
        const own = ownNames.map(n => lookup(n, pkg, OWN_KINDS, "services, commands, queries or handlers")).filter(Boolean);
        if (!own.length) { problems.push(`no elements found for "${name}"`); continue; }
        const ownIds = new Set(own.map(e => e.id));

        // The diagram shows the area's own elements, what its handlers subscribe to, and everything they invoke or act on.
        const triggers = [], edges = [];
        const nodes = [...own], onDiagram = new Set(ownIds);
        for (const n of own) for (const end of handlerEnds(n)) {
            const t = end.typeReference?.getType?.();
            if (!t) { problems.push(`${n.getName()}: subscription with no type`); continue; }
            if (!onDiagram.has(t.id)) { onDiagram.add(t.id); nodes.push(t); triggers.push(t); }
            edges.push({ id: null, from: t.id, to: n.id, y1: height(t) / 2, y2: anchor(n, end), own: true, invocation: false });
        }
        for (const n of own) for (const { t } of outgoing(n)) {
            const tn = nodeOf(t);
            if (!onDiagram.has(tn.id)) { onDiagram.add(tn.id); nodes.push(tn); }
        }
        const order = new Map(nodes.map((n, i) => [n.id, i]));

        // The designer draws every association between two elements on a diagram, so lay out every invocation
        // between them, plus the area's own entity actions, published events and sent commands.
        for (const n of nodes) for (const { s, spec, assocId, t } of outgoing(n)) {
            const tn = nodeOf(t);
            const invocation = spec === "Perform Invocation";
            if (!onDiagram.has(tn.id) || (!invocation && !ownIds.has(n.id))) continue;
            edges.push({ id: assocId, from: n.id, to: tn.id, y1: anchor(n, s), y2: anchor(tn, t), own: ownIds.has(n.id), invocation });
        }
        const hasIncoming = id => edges.some(e => e.to === id && e.from !== id);
        const entries = nodes.filter(n => triggers.includes(n) || (ownIds.has(n.id) && (isEntry(n) || !hasIncoming(n.id))));

        // Columns: longest path from the entry points; cycles (mutual calls) are broken keeping the area's own direction.
        const succ = new Map(nodes.map(n => [n.id, []]));
        for (const e of [...edges].sort((p, q) => q.own - p.own)) {
            if (e.from !== e.to && !succ.get(e.from).includes(e.to)) succ.get(e.from).push(e.to);
        }
        const state = new Map(), back = new Set();
        const dfs = id => {
            state.set(id, 1);
            for (const m of succ.get(id)) { if (state.get(m) === 1) back.add(`${id}>${m}`); else if (!state.get(m)) dfs(m); }
            state.set(id, 2);
        };
        for (const n of [...entries, ...own, ...nodes]) if (!state.get(n.id)) dfs(n.id);
        const depth = new Map(nodes.map(n => [n.id, entries.includes(n) ? 0 : 1]));
        for (let k = 0; k < nodes.length; k++) for (const n of nodes) for (const m of succ.get(n.id)) {
            if (!back.has(`${n.id}>${m}`) && depth.get(m) < depth.get(n.id) + 1) depth.set(m, depth.get(n.id) + 1);
        }
        const maxDepth = Math.max(...nodes.map(n => depth.get(n.id)));

        const isArea = n => ownIds.has(n.id) || ["Class", "Message", "Domain Event"].includes(n.getSpecialization());
        const top = new Map(), place = [], routed = [], colX = [0], columns = [];
        for (let d = 0; d <= maxDepth; d++) {
            const col = nodes.filter(n => depth.get(n.id) === d);
            // Where each element would like to sit: level with the operations that call it (mean over its callers).
            const want = new Map(), weight = new Map();
            for (const n of col) {
                const ys = d === 0 ? [] : edges.filter(e => e.to === n.id && e.from !== n.id && top.has(e.from))
                    .map(e => top.get(e.from) + e.y1 - e.y2);
                want.set(n.id, ys.length ? ys.reduce((s, v) => s + v, 0) / ys.length : null);
                weight.set(n.id, ys.length || 0.001);
            }
            if (d === 0) col.sort((p, q) => triggers.includes(p) - triggers.includes(q) || order.get(p.id) - order.get(q.id));
            else col.sort((p, q) => (want.get(p.id) === null) - (want.get(q.id) === null)
                || isArea(q) - isArea(p)
                || (want.get(p.id) ?? 0) + height(p) / 2 - ((want.get(q.id) ?? 0) + height(q) / 2)
                || order.get(p.id) - order.get(q.id));
            // Pack the column top to bottom, GAP_Y apart, as close to those positions as the order allows.
            let acc = 0;
            const offsets = col.map(n => { const o = acc; acc += height(n) + GAP_Y; return o; });
            const z = d === 0 ? col.map(() => 0) : pav(col.map((n, i) => (want.get(n.id) ?? 0) - offsets[i]), col.map(n => weight.get(n.id)));
            let bottom = 0;
            col.forEach((n, i) => {
                const y = r10(Math.max(z[i], 0)) + offsets[i];
                top.set(n.id, y);
                bottom = y + height(n);
                place.push({ elementId: n.id, x: colX[d], y });
            });
            if (d === 0) for (const extra of EXTRAS[name] ?? []) {
                const el = lookup(extra, pkg, null, "elements");
                if (!el || onDiagram.has(el.id)) continue;
                onDiagram.add(el.id);
                const y = r10(bottom + GAP_Y);
                place.push({ elementId: el.id, x: 0, y });
                bottom = y + height(el);
            }

            // Calls that stay in this column (a service calling its own operations) go round the right-hand edge:
            // out of the caller's row, down a vertical lane, into the callee's row. Short spans get the inner lanes.
            const same = edges.filter(e => e.invocation && depth.get(e.from) === d && depth.get(e.to) === d)
                .map(e => ({ ...e, a: top.get(e.from) + e.y1, b: top.get(e.to) + e.y2 }))
                .sort((p, q) => Math.abs(p.b - p.a) - Math.abs(q.b - q.a) || Math.min(p.a, p.b) - Math.min(q.a, q.b));
            const lanes = [];
            for (const e of same) {
                const lo = Math.min(e.a, e.b) - 5, hi = Math.max(e.a, e.b) + 5;
                let k = 0;
                while (lanes[k]?.some(([p, q]) => p < hi && lo < q)) k++;
                (lanes[k] ??= []).push([lo, hi]);
                const x = colX[d] + W + LANE0 + k * LANE_STEP;
                const [ya, yb] = e.a === e.b ? [e.a - 4, e.b + 4] : [e.a, e.b];
                routed.push({ associationId: e.id, waypoints: [{ x, y: ya }, { x, y: yb }] });
            }
            // The gap to the next column widens when this column needs many lanes.
            colX.push(colX[d] + W + Math.max(MIN_GAP_X, LANE0 + lanes.length * LANE_STEP + 60));
            columns.push(col.map(n => n.getName()).join(", "));
        }
        const left = edges.filter(e => e.id && depth.get(e.to) < depth.get(e.from)).length;
        out.push([
            `### ${name} | ${diagram.id}`,
            `COLUMNS ${columns.map((c, i) => `[${i}] ${c}`).join(" | ")}`,
            ...(left ? [`LEFT-POINTING ${left} (mutual calls)`] : []),
            `NODES ${JSON.stringify(place)}`,
            `EDGES ${JSON.stringify(routed)}`,
        ].join("\n"));
    }
    return (problems.length ? `PROBLEMS\n${problems.join("\n")}\n` : "") + out.join("\n");
}
