// Designer script for the Codebase Structure designer. Run with run_designer_script, passing
// includedScriptPaths = [<outDir>/designer-data.js, <this file>] and an empty script.
// designer-data.js defines CLASSIFICATIONS, HOSTS, CLEAR and RETIRED (written by generate.js).
//
// 1. Ensures the File Classifications container and one File Classification per entry in CLASSIFICATIONS
//    (matched case-insensitively, so existing ones are reused), and sets each Color. A classification whose
//    formerNames match an existing element takes that element over and renames it, so its id survives; when
//    several former names exist (a merge), the others are deleted once no entries point at them.
// 2. For every host in HOSTS: finds the element by path (creating a Folder when the host says
//    createFolderUnder), applies the File Classification stereotype, REPLACES its Entries with the
//    generated rows, and reads them back.
// 3. Empties CLEAR hosts (config hosts with no rows this run), so entries from an earlier run or a module's
//    default can't keep classifying their files. A Template Output keeps the stereotype with an empty Entries
//    list: Intent skips an empty list, while a Template Output with no stereotype at all makes Change Review
//    offer "Apply missing classifications", which would put the module's default back. Other hosts lose it.
// 4. Deletes RETIRED classifications, and reports elements that carry entries outside the config (stale),
//    classifications nothing in the config accounts for (unused), and Template Outputs the config doesn't
//    list (templateOutputsNotInConfig: the generator assumed their files are classified by folders).

const STEREOTYPE = "File Classification";
const P = { entries: "Entries", glob: "Glob", importance: "Review Importance", classification: "Classification" };
const HOST_TYPES = ["Folder", "Template Output", "Solution Folder", "C# Project (.NET)"];
const CLEAR_HOSTS = typeof CLEAR === "undefined" ? [] : CLEAR;
const RETIRED_NAMES = typeof RETIRED === "undefined" ? [] : RETIRED;

const specOf = p => (typeof p.getSpecialization === "function" ? p.getSpecialization() : p.specialization);
const nameOf = p => (typeof p.getName === "function" ? p.getName() : p.name);
const lc = s => String(s).trim().toLowerCase();
const rootPkg = getPackages().find(p => specOf(p) === "Root Folder");
if (!rootPkg) throw new Error("No Root Folder package found: run this against the Codebase Structure designer.");
const pathOf = el => (el === rootPkg ? nameOf(rootPkg) : (() => { const parts = []; for (let cur = el; cur; cur = cur.getParent ? cur.getParent() : null) parts.unshift(nameOf(cur)); return parts.join("/"); })());
const elementAt = p => (p === nameOf(rootPkg) ? rootPkg : lookupByPath(p));
const carries = o => (typeof o.hasStereotype === "function"
    ? o.hasStereotype(STEREOTYPE)
    : (o.getStereotypes ? o.getStereotypes() : []).some(s => nameOf(s) === STEREOTYPE));

// 0. Check everything that could stop the apply halfway, before changing anything: every host resolves to one
// element that can carry entries (or has a parent to create its Folder under), and every row names a
// classification in CLASSIFICATIONS. A Solution Folder's globs are relative to each contained project, which
// the generated rows can't express, so its projects must be the hosts instead.
const blockers = [];
const lookup = p => { try { return elementAt(p); } catch (e) { blockers.push(`${p}: ${e.message || e}`); return undefined; } };
const known = new Set(CLASSIFICATIONS.map(c => c.name));
for (const h of HOSTS) {
    const el = lookup(h.element);
    if (el === undefined) continue;
    if (!el && !h.createFolderUnder) blockers.push(`${h.element}: not found. Re-read the paths with searchElements and fix the config.`);
    else if (!el && h.createFolderUnder && !lookup(h.createFolderUnder)) blockers.push(`${h.element}: parent ${h.createFolderUnder} for the new Folder not found.`);
    else if (el && el !== rootPkg && specOf(el) === "Solution Folder") blockers.push(`${h.element}: a Solution Folder can't host generated rows; use the projects inside it as hosts.`);
    else if (el && el !== rootPkg && !HOST_TYPES.includes(specOf(el))) blockers.push(`${h.element}: a ${specOf(el)} can't carry the ${STEREOTYPE} stereotype.`);
    for (const r of h.rows) for (const n of r.classifications) if (!known.has(n)) blockers.push(`${h.element}: a row uses classification "${n}", which is not in CLASSIFICATIONS.`);
}
if (blockers.length) throw new Error(`Nothing was changed. Fix these and regenerate:\n- ${[...new Set(blockers)].join("\n- ")}`);

// 1. Classifications
const containers = findElements({ specialization: "File Classifications" });
const container = containers.length ? containers[0] : createElementUnder(rootPkg, "File Classifications", "File Classifications");
const existing = () => container.getChildren("File Classification");
const ids = {};
const classificationReport = [];
const toDelete = [];
for (const c of CLASSIFICATIONS) {
    const names = [c.name, ...(c.formerNames || [])].map(lc);
    // The element already carrying the new name wins, then the former names in the order listed.
    const matches = existing().filter(x => names.includes(lc(x.getName()))).sort((a, b) => names.indexOf(lc(a.getName())) - names.indexOf(lc(b.getName())));
    const el = matches[0] || container.addChild("File Classification", c.name);
    const was = el.getName();
    if (was !== c.name) el.setName(c.name);
    if (c.color) el.ensureStereotype("File Classification Settings").setProperty("Color", c.color);
    ids[c.name] = el.getId();
    toDelete.push(...matches.slice(1).map(x => ({ el: x, name: x.getName(), reason: `merged into ${c.name}` })));
    classificationReport.push({ name: c.name, reused: !!matches[0], ...(was !== c.name && matches[0] ? { renamedFrom: was } : {}) });
}
for (const r of RETIRED_NAMES) for (const x of existing()) if (lc(x.getName()) === lc(r) && !Object.values(ids).includes(x.getId())) toDelete.push({ el: x, name: x.getName(), reason: "retired" });

// 2. Hosts
const hostReport = [];
const problems = [];
for (const h of HOSTS) {
    // The Root Folder is a package, not an element, so path lookups never return it.
    let el = elementAt(h.element);
    if (!el && h.createFolderUnder) {
        const parent = elementAt(h.createFolderUnder);
        if (!parent) throw new Error(`Parent ${h.createFolderUnder} for new folder ${h.element} not found.`);
        el = createElementUnder(parent, "Folder", h.element.split("/").pop());
    }
    if (!el) throw new Error(`Host element not found: ${h.element}. Re-read the paths with searchElements and fix the config.`);
    const prop = el.ensureStereotype(STEREOTYPE).getProperty(P.entries);
    prop.clearItems();
    for (const r of h.rows) {
        const row = prop.addItem();
        row.setProperty(P.glob, r.globs.join("\n"));
        row.setProperty(P.importance, r.importance);
        row.setProperty(P.classification, r.classifications.map(n => {
            if (!ids[n]) throw new Error(`Row on ${h.element} uses classification "${n}" that is not in CLASSIFICATIONS.`);
            return ids[n];
        }));
    }
    const items = prop.getItems();
    items.forEach((it, i) => {
        const want = h.rows[i];
        const glob = it.getProperty(P.glob).getValue();
        const imp = String(it.getProperty(P.importance).getValue());
        const cls = (it.getProperty(P.classification).getValue() || []).map(c => c.getName()).sort();
        if (glob !== want.globs.join("\n") || imp !== want.importance || JSON.stringify(cls) !== JSON.stringify([...want.classifications].sort())) {
            problems.push({ host: h.element, row: i, stored: { glob, imp, cls } });
        }
    });
    if (items.length !== h.rows.length) problems.push({ host: h.element, expectedRows: h.rows.length, storedRows: items.length });
    hostReport.push(`${items.length} rows  ${h.element}`);
}

// 3. Config hosts with no rows this run
const cleared = [];
let templateOutputsLeftEmpty = 0;
const rowsOn = el => (carries(el) ? el.getStereotype(STEREOTYPE).getProperty(P.entries).getItems().length : 0);
for (const p of CLEAR_HOSTS) {
    const el = elementAt(p);
    if (!el) continue;
    const had = rowsOn(el);
    if (el !== rootPkg && specOf(el) === "Template Output") {
        el.ensureStereotype(STEREOTYPE).getProperty(P.entries).clearItems();
        templateOutputsLeftEmpty++;
    } else if (carries(el)) {
        el.removeStereotype(STEREOTYPE);
    }
    if (had) cleared.push({ element: p, rowsRemoved: had });
}

// 4. Elements with entries this run did not write, then retired and merged classifications
const written = new Set([...HOSTS.map(h => h.element), ...CLEAR_HOSTS]);
const staleEls = [rootPkg, ...findElements({ specialization: HOST_TYPES })].filter(e => !written.has(pathOf(e)) && rowsOn(e) > 0);
const stale = staleEls.map(e => ({ element: pathOf(e), rows: e.getStereotype(STEREOTYPE).getProperty(P.entries).getItems().length }));
const staleRefs = new Set(staleEls.flatMap(e => e.getStereotype(STEREOTYPE).getProperty(P.entries).getItems()
    .flatMap(it => (it.getProperty(P.classification).getValue() || []).map(c => c.getId()))));
const deleted = [];
const kept = [];
for (const d of toDelete) {
    if (staleRefs.has(d.el.getId())) { kept.push({ name: d.name, reason: `${d.reason}, but stale entries still use it` }); continue; }
    d.el.delete();
    deleted.push({ name: d.name, reason: d.reason });
}
const unused = existing().map(x => x.getName()).filter(n => !(n in ids) && !kept.some(k => k.name === n));
const templateOutputsNotInConfig = findElements({ specialization: "Template Output" }).map(pathOf).filter(p => !written.has(p));

return { classifications: classificationReport, hosts: hostReport, problems, cleared, templateOutputsLeftEmpty, stale, deleted, kept, unused, templateOutputsNotInConfig };
