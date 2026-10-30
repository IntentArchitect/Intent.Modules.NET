// Designer script for the Codebase Structure designer: clears File Classification entries so a re-run starts clean.
// Run with run_designer_script, includedScriptPaths = [<this file>] and a script that calls it, e.g.
//   return clearClassifications({ dryRun: true })                                  // report only, change nothing
//   return clearClassifications({ deleteClassifications: true, deleteFolders: ["root/.github"] })
//
// - Empties the File Classification entries on the Root Folder and every element carrying them, including module
//   defaults on Template Outputs. Template Outputs keep the stereotype with an empty list: with no stereotype at all,
//   Change Review offers "Apply missing classifications", which puts the module's default back. Others lose it.
// - deleteClassifications: also delete the File Classification elements and their container. Ask first: other
//   applications' modules may match classifications by name.
// - deleteFolders: Folder elements to delete because they existed only to carry entries. Each must be empty;
//   `emptyHostFolders` in the result lists the candidates. Ask before deleting any.
function clearClassifications(options) {
    const o = Object.assign({ dryRun: false, deleteClassifications: false, deleteFolders: [] }, options || {});
    const STEREOTYPE = "File Classification";
    const HOST_TYPES = ["Folder", "Template Output", "Solution Folder", "C# Project (.NET)"];
    const specOf = p => (typeof p.getSpecialization === "function" ? p.getSpecialization() : p.specialization);
    const nameOf = p => (typeof p.getName === "function" ? p.getName() : p.name);
    const rootPkg = getPackages().find(p => specOf(p) === "Root Folder");
    if (!rootPkg) throw new Error("No Root Folder package found: run this against the Codebase Structure designer.");
    const pathOf = el => (el === rootPkg ? nameOf(rootPkg) : (() => { const parts = []; for (let cur = el; cur; cur = cur.getParent ? cur.getParent() : null) parts.unshift(nameOf(cur)); return parts.join("/"); })());
    const carries = x => (typeof x.hasStereotype === "function"
        ? x.hasStereotype(STEREOTYPE)
        : (x.getStereotypes ? x.getStereotypes() : []).some(s => nameOf(s) === STEREOTYPE));

    const rowsOn = h => h.getStereotype(STEREOTYPE).getProperty("Entries").getItems().length;
    const isTemplateOutput = h => h !== rootPkg && specOf(h) === "Template Output";
    // Template Outputs already holding an empty list are already clear.
    const hosts = [rootPkg, ...findElements({ specialization: HOST_TYPES })].filter(carries).filter(h => !isTemplateOutput(h) || rowsOn(h) > 0);
    const cleared = hosts.map(h => ({ element: pathOf(h), rows: rowsOn(h) }));
    const emptyHostFolders = hosts.filter(h => h !== rootPkg && specOf(h) === "Folder" && h.getChildren().length === 0).map(pathOf);
    const containers = findElements({ specialization: "File Classifications" });
    const classifications = containers.flatMap(c => c.getChildren("File Classification").map(x => x.getName()));
    if (o.dryRun) return { dryRun: true, wouldClear: cleared, classifications, emptyHostFolders };

    for (const h of hosts) {
        if (isTemplateOutput(h)) h.getStereotype(STEREOTYPE).getProperty("Entries").clearItems();
        else h.removeStereotype(STEREOTYPE);
    }
    const deletedClassifications = [];
    if (o.deleteClassifications) for (const c of containers) { deletedClassifications.push(...c.getChildren("File Classification").map(x => x.getName())); c.delete(); }
    const deletedFolders = [];
    for (const p of o.deleteFolders) {
        const el = lookupByPath(p);
        if (!el) continue;
        if (specOf(el) !== "Folder" || el.getChildren().length) throw new Error(`${p} is not an empty Folder; not deleting it.`);
        el.delete();
        deletedFolders.push(p);
    }
    return { cleared, deletedClassifications, deletedFolders, emptyHostFolders: emptyHostFolders.filter(p => !deletedFolders.includes(p)) };
}
