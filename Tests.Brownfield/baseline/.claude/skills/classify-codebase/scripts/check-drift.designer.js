// Designer script for the Codebase Structure designer. Read only: it changes nothing.
// Compares the model's File Classification entries with the snapshot saved after the last apply
// ({Application}.applied.js, a copy of the designer-data.js that was applied), so an update never silently
// overwrites edits someone made in the designer since.
// Run with run_designer_script, includedScriptPaths = [<the snapshot>, <this file>] and the script
//   return checkDrift();
function checkDrift() {
    if (typeof HOSTS === "undefined" || typeof CLASSIFICATIONS === "undefined") throw new Error("Include the applied snapshot ({Application}.applied.js) before this script.");
    const STEREOTYPE = "File Classification";
    const P = { entries: "Entries", glob: "Glob", importance: "Review Importance", classification: "Classification" };
    const HOST_TYPES = ["Folder", "Template Output", "Solution Folder", "C# Project (.NET)"];
    const specOf = p => (typeof p.getSpecialization === "function" ? p.getSpecialization() : p.specialization);
    const nameOf = p => (typeof p.getName === "function" ? p.getName() : p.name);
    const lc = s => String(s || "").trim().toLowerCase();
    const rootPkg = getPackages().find(p => specOf(p) === "Root Folder");
    if (!rootPkg) throw new Error("No Root Folder package found: run this against the Codebase Structure designer.");
    const pathOf = el => (el === rootPkg ? nameOf(rootPkg) : (() => { const parts = []; for (let cur = el; cur; cur = cur.getParent ? cur.getParent() : null) parts.unshift(nameOf(cur)); return parts.join("/"); })());
    const carries = (o, st) => (typeof o.hasStereotype === "function"
        ? o.hasStereotype(st)
        : (o.getStereotypes ? o.getStereotypes() : []).some(s => nameOf(s) === st));
    const keyOf = r => JSON.stringify([r.globs, r.importance, r.classifications]);
    const show = r => `${r.importance} | ${r.classifications.join(", ")} | ${r.globs.replace(/\n/g, "  ")}`;

    // Classifications: added, removed or recoloured in the designer.
    const model = findElements({ specialization: "File Classifications" }).flatMap(c => c.getChildren("File Classification")).map(x => ({
        name: x.getName(),
        color: carries(x, "File Classification Settings") ? x.getStereotype("File Classification Settings").getProperty("Color").getValue() : null,
    }));
    const snapNames = new Set(CLASSIFICATIONS.map(c => lc(c.name)));
    const classifications = {
        addedInDesigner: model.filter(m => !snapNames.has(lc(m.name))).map(m => m.name),
        missingFromModel: CLASSIFICATIONS.filter(c => !model.some(m => lc(m.name) === lc(c.name))).map(c => c.name),
        recoloured: CLASSIFICATIONS.map(c => ({ c, m: model.find(m => lc(m.name) === lc(c.name)) }))
            .filter(x => x.m && x.c.color && lc(x.m.color) !== lc(x.c.color)).map(x => ({ name: x.c.name, applied: x.c.color, now: x.m.color })),
    };

    // Entries, host by host. An empty list (how the apply clears a Template Output) is the same as none.
    const hasRows = e => carries(e, STEREOTYPE) && e.getStereotype(STEREOTYPE).getProperty(P.entries).getItems().length > 0;
    const now = new Map([rootPkg, ...findElements({ specialization: HOST_TYPES })].filter(hasRows).map(e => [pathOf(e),
        e.getStereotype(STEREOTYPE).getProperty(P.entries).getItems().map(it => ({
            globs: it.getProperty(P.glob).getValue() || "",
            importance: String(it.getProperty(P.importance).getValue()),
            classifications: (it.getProperty(P.classification).getValue() || []).map(c => c.getName()).sort(),
        }))]));
    const then = new Map(HOSTS.map(h => [h.element, h.rows.map(r => ({ globs: r.globs.join("\n"), importance: r.importance, classifications: [...r.classifications].sort() }))]));
    const hosts = [];
    for (const element of new Set([...now.keys(), ...then.keys()])) {
        const a = now.get(element), b = then.get(element);
        if (!b) { hosts.push({ element, change: "entries added in the designer", rows: a.map(show) }); continue; }
        if (!a) { hosts.push({ element, change: "entries removed in the designer", appliedRows: b.length }); continue; }
        if (JSON.stringify(a.map(keyOf)) === JSON.stringify(b.map(keyOf))) continue;
        const bKeys = b.map(keyOf), aKeys = a.map(keyOf);
        const added = a.filter(r => !bKeys.includes(keyOf(r))).map(show);
        const removed = b.filter(r => !aKeys.includes(keyOf(r))).map(show);
        hosts.push({ element, change: added.length || removed.length ? "rows edited in the designer" : "rows reordered in the designer", added, removed });
    }
    const clean = !hosts.length && !classifications.addedInDesigner.length && !classifications.missingFromModel.length && !classifications.recoloured.length;
    return { clean, classifications, hosts };
}
