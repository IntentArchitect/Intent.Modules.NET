// Turns a classify config (plan per classification + host elements) into readable, last-match-wins
// File Classification rows per host, and proves every file resolves to what the plan intends.
//
// Usage: node generate.js <config.json> <outDir> [--baseline <previous config.json>]
// Writes <outDir>/rows.json (rows + per-file resolution) and <outDir>/designer-data.js (include for the apply script).
// With --baseline it also resolves the previous config against the same files and writes <outDir>/changes.json:
// what the update changes, for the user to approve and for gen-mdx.js --changes.
'use strict';
const fs = require('fs');
const path = require('path');
const { execFileSync } = require('child_process');
const L = require('./lib');

const USAGE = 'Usage: node generate.js <config.json> <outDir> [--baseline <previous config.json>]';
const args = process.argv.slice(2);
const option = name => {
    const i = args.indexOf(name);
    if (i < 0) return null;
    const v = args[i + 1];
    if (!v || v.startsWith('--')) { console.error(`${name} needs a file path. ${USAGE}`); process.exit(2); }
    if (!fs.existsSync(v)) { console.error(`${name}: ${v} does not exist.`); process.exit(2); }
    args.splice(i, 2);
    return v;
};
const baselinePath = option('--baseline');
const [configPath, outDir] = args;
if (!configPath || !outDir || args.length > 2) { console.error(USAGE); process.exit(2); }
const cfg = L.loadConfig(configPath);
const files = L.listFiles(cfg.appRoot);

const isWild = g => /[*?]/.test(g);
const probeOf = g => g.replace(/\/\*\*$/, '/zzq/zzq.cs').replace(/^\*\*\/\*$/, 'zzq/zzq.cs').replace(/\*\*\//g, 'zzq/').replace(/\*/g, 'zzq');
// Broad before narrow: fewer literal folders first, '**' before '*', then alphabetical.
const spec = g => { const lit = g.split(/[*?]/)[0]; return [lit.split('/').length - 1, lit.length, g.includes('**') ? 0 : 1]; };
const cmpSpec = (a, b) => { const x = spec(a), y = spec(b); for (let i = 0; i < x.length; i++) if (x[i] !== y[i]) return x[i] - y[i]; return a.localeCompare(b); };

// Builds every host's rows for one config and resolves every file through them.
function run(cfg) {
    // Inventory: what generated each file, and what the plan wants for it.
    const inv = files.map(f => ({ f, templateId: L.templateOf(cfg, f), want: L.intended(cfg, f) }));

    // own: the files the host classifies. foreign: for a Template Output, its template's files that none of the
    // template's Template Outputs contains and another of them owns (see `outside` below). Intent classifies such
    // a file with the first of them it loads that has entries, on its whole path, so each of them also gets rows
    // for those files, seen the same way: the result is then right whichever one Intent loads first.
    function build(h, own, foreign) {
        // What the host's globs see for each of its files; a file outside a Template Output's folder is seen whole.
        const rel = own.map(f => L.remainderOf(h, f));
        const fileOf = new Map(own.map((f, i) => [rel[i], f]));
        const clashes = rel.filter((r, i) => rel.indexOf(r) !== i);
        const sig = f => L.sigOf(L.intended(cfg, f));
        // A foreign file seen at the same path as one of this host's own can't be told apart from it.
        const foreignAtRisk = [], foreignRel = [];
        for (const f of foreign) {
            const r = L.remainderOf(h, f);
            if (fileOf.has(r)) { if (sig(fileOf.get(r)) !== sig(f)) foreignAtRisk.push(f); continue; }
            fileOf.set(r, f);
            foreignRel.push(r);
        }
        const want = r => sig(fileOf.has(r) ? fileOf.get(r) : L.absFrom(h.dir, r));

        // Candidate catch-alls: the plan's wildcard lines, rebased onto this host, that reach at least one of its
        // files; and for foreign files seen whole, the plan lines as written. Each keeps its entry's later
        // exclusions, so a carved-out folder stays carved out for files added there later.
        const rebase = l => {
            let p = null;
            if (h.dir === '' || l.startsWith('**/')) p = l;
            else if (l.startsWith(h.dir + '/')) p = l.slice(h.dir.length + 1);
            else if (l.endsWith('/**') && (h.dir + '/').startsWith(l.slice(0, -2))) p = '**/*';
            return p === '**' ? '**/*' : p;
        };
        const pats = new Map();
        const probeSet = new Set();
        for (const e of cfg.plan) {
            const lines = L.patternLines(e.globs);
            lines.forEach((l, i) => {
                if (l.startsWith('!') || !isWild(l)) return;
                const later = lines.slice(i + 1).filter(x => x.startsWith('!')).map(x => x.slice(1));
                const p = rebase(l);
                if (p && rel.some(r => L.toRe(p).test(r))) {
                    const excl = later.map(rebase).filter(Boolean);
                    const globs = [p, ...excl.map(x => '!' + x)];
                    pats.set(JSON.stringify(globs), { globs, probe: L.absFrom(h.dir, probeOf(p)) });
                    probeSet.add(probeOf(p));
                    for (const x of excl) if (isWild(x)) probeSet.add(probeOf(x));
                }
                if (foreignRel.some(r => L.toRe(l).test(r))) {
                    const globs = [l, ...later.map(x => '!' + x)];
                    const key = JSON.stringify(globs);
                    if (!pats.has(key)) pats.set(key, { globs, probe: probeOf(l) });
                }
            });
        }
        const probes = [...probeSet].filter(p => !fileOf.has(p));
        const targets = [...rel, ...foreignRel, ...probes];
        const wants = new Map(targets.map(t => [t, want(t)]));
        const got = (rows, t) => { const r = L.resolveRows(rows, t); return r ? r.sig : '(none)'; };
        // Files the plan leaves alone are fixed with exclusions later, so they don't count against a catch-all here.
        const dropTargets = [...rel, ...foreignRel].filter(t => wants.get(t) !== '(none)').concat(probes);
        const score = rows => dropTargets.reduce((n, t) => n + (got(rows, t) === wants.get(t) ? 1 : 0), 0);
        const allRight = rows => targets.every(t => got(rows, t) === wants.get(t));
        // Within a row: folder patterns, then exact files A-Z, then exclusions (only '!' lines are order-sensitive).
        const kind = g => (g.startsWith('!') ? 2 : isWild(g) ? 0 : 1);
        const tidy = globs => [...new Set(globs)].sort((a, b) => kind(a) - kind(b) || (kind(a) === 0 ? cmpSpec(a, b) : a.localeCompare(b)));

        let rows = [...pats.values()].sort((a, b) => cmpSpec(a.globs[0], b.globs[0])).map(c => {
            const r = L.intended(cfg, c.probe);
            return r && { globs: c.globs, sig: L.sigOf(r), r };
        }).filter(Boolean);

        // Drop catch-alls that change nothing.
        let best = score(rows);
        for (let i = rows.length - 1; i >= 0; i--) {
            const trial = rows.filter((_, j) => j !== i);
            const s = score(trial);
            if (s >= best) { rows = trial; best = s; }
        }

        // Exact rows for existing files the catch-alls get wrong, grouped by outcome; exclusions for files the plan leaves alone.
        const fixes = new Map();
        for (const r of [...rel, ...foreignRel]) {
            const w = wants.get(r);
            if (got(rows, r) === w) continue;
            if (w === '(none)') { for (const row of rows) if (L.linesMatch(row.globs, r)) row.globs = [...row.globs, '!' + r]; continue; }
            if (!fixes.has(w)) fixes.set(w, { globs: [], sig: w, r: L.intended(cfg, fileOf.get(r)) });
            fixes.get(w).globs.push(r);
        }
        rows.push(...[...fixes.values()].sort((a, b) => L.RANK[b.r.level] - L.RANK[a.r.level] || a.sig.localeCompare(b.sig)));
        rows = rows.map(r => ({ ...r, globs: tidy(r.globs) }));

        // Merge rows with the same outcome wherever every file and probe keeps its result, judged in the
        // order the lines will actually be written.
        for (let changed = true; changed;) {
            changed = false;
            outer: for (let i = 0; i < rows.length; i++) for (let j = i + 1; j < rows.length; j++) {
                if (rows[i].sig !== rows[j].sig) continue;
                for (const [keep, drop] of [[i, j], [j, i]]) {
                    const trial = rows.map((r, k) => (k === keep ? { ...r, globs: tidy([...r.globs, ...rows[drop].globs]) } : r)).filter((_, k) => k !== drop);
                    if (allRight(trial)) { rows = trial; changed = true; break outer; }
                }
            }
        }

        return {
            rows,
            // A file whose whole path equals another file's folder-relative path can't be told apart by any glob.
            wrong: [...new Set([...rel.filter(r => got(rows, r) !== wants.get(r)), ...clashes])],
            foreignCount: foreignRel.length,
            foreignAtRisk: [...foreignAtRisk, ...foreignRel.filter(r => got(rows, r) !== wants.get(r)).map(r => fileOf.get(r))],
        };
    }

    // Generated files outside every one of their template's several Template Outputs. Intent classifies such a file
    // with the first of them it loads that has entries, matching its whole path; every other file of the template
    // is classified by the deepest Template Output containing it alone. Template Outputs are never dropped below.
    const outputsOf = t => cfg.hosts.filter(h => h.kind === 'template' && h.templateId === t);
    const outside = inv.filter(i => i.templateId && outputsOf(i.templateId).length > 1 && !outputsOf(i.templateId).some(o => L.under(o.dir, i.f)));

    // A project or folder that ends up with no rows loses the stereotype in the apply, so the element above it
    // classifies its files. Drop such hosts and assign their files again until every remaining one has rows.
    // Template Outputs always stay: they own their template's files even with an empty list.
    let active = cfg.hosts.slice();
    let hostFiles, built;
    for (; ;) {
        hostFiles = new Map(cfg.hosts.map(h => [h, []]));
        for (const i of inv) {
            i.host = L.assignHost(cfg, i.f, i.templateId, active);
            if (i.host) hostFiles.get(i.host).push(i.f);
        }
        built = new Map();
        for (const h of active) {
            const foreign = h.kind === 'template' ? outside.filter(i => i.templateId === h.templateId && i.host !== h).map(i => i.f) : [];
            built.set(h, build(h, hostFiles.get(h), foreign));
        }
        const drop = active.filter(h => h.kind === 'custom' && !built.get(h).rows.length && hostFiles.get(h).length);
        if (!drop.length) break;
        active = active.filter(h => !drop.includes(h));
    }

    const hosts = cfg.hosts.map(h => {
        const b = built.get(h) || { rows: [], wrong: [], foreignCount: 0, foreignAtRisk: [] };
        return {
            element: h.element, dir: h.dir, kind: h.kind, templateId: h.templateId, createFolderUnder: h.createFolderUnder,
            fileCount: hostFiles.get(h).length, wrong: b.wrong,
            ...(b.foreignCount || b.foreignAtRisk.length ? { outsideFiles: b.foreignCount, outsideAtRisk: b.foreignAtRisk } : {}),
            rows: b.rows.map(r => ({ globs: r.globs, importance: L.IMPORTANCE[r.r.level], level: r.r.level, classifications: r.r.classes })),
        };
    });
    const resolution = {};
    for (const i of inv) {
        const h = i.host && hosts.find(x => x.element === i.host.element);
        const row = h && L.resolveRows(h.rows, L.remainderOf(h, i.f));
        resolution[i.f] = {
            host: h ? h.element : null, generated: !!i.templateId, level: row ? row.level : null, classes: row ? row.classifications : [],
            ...(L.outOfScopeOf(cfg, i.f) ? { outOfScope: true } : {}),
        };
    }
    return { inv, hosts, resolution, outside };
}

const lc = s => String(s).trim().toLowerCase();
const label = l => (l ? L.LABEL[l] : 'Unclassified');
const sameList = (a, b) => a.length === b.length && a.every((x, i) => x === b[i]);

// What the update changes: classifications renamed, merged, added or removed, and every file whose
// classifications or importance differ. Renames alone don't count as a file change.
function compare(base, before, after) {
    const now = new Map(cfg.classifications.map(c => [lc(c.name), c]));
    const formerTo = new Map();
    for (const c of cfg.classifications) for (const f of c.formerNames || []) formerTo.set(lc(f), c.name);
    const mapName = n => (now.has(lc(n)) ? now.get(lc(n)).name : formerTo.get(lc(n)) || n);
    const retired = new Set(cfg.retiredClassifications.map(lc));
    const baseByName = new Map(base.classifications.map(c => [lc(c.name), c]));

    const classifications = { added: [], renamed: [], merged: [], retired: [], removedNotRetired: [], recoloured: [], reworded: [] };
    for (const c of cfg.classifications) {
        const sources = [c.name, ...(c.formerNames || [])].filter(n => baseByName.has(lc(n))).map(n => baseByName.get(lc(n)));
        if (!sources.length) { classifications.added.push(c.name); continue; }
        if (sources.length > 1) classifications.merged.push({ from: sources.map(s => s.name), into: c.name });
        else if (lc(sources[0].name) !== lc(c.name)) classifications.renamed.push({ from: sources[0].name, to: c.name });
        const prev = sources.find(s => lc(s.name) === lc(c.name)) || sources[0];
        if (lc(prev.color || '') !== lc(c.color || '')) classifications.recoloured.push({ name: c.name, from: prev.color, to: c.color });
        if ((prev.question || '') !== (c.question || '')) classifications.reworded.push(c.name);
    }
    for (const b of base.classifications) {
        if (now.has(lc(b.name)) || formerTo.has(lc(b.name))) continue;
        (retired.has(lc(b.name)) ? classifications.retired : classifications.removedNotRetired).push(b.name);
    }

    const changed = [];
    for (const f of Object.keys(after)) {
        const a = after[f], b = before[f] || { level: null, classes: [] };
        if (a.outOfScope) continue;
        const was = [...new Set(b.classes.map(mapName))].sort();
        if (a.level === b.level && sameList(a.classes, was)) continue;
        changed.push({ file: f, from: b.level, to: a.level, classes: a.classes, added: a.classes.filter(c => !was.includes(c)), removed: was.filter(c => !a.classes.includes(c)) });
    }
    const transitions = {};
    for (const c of changed) {
        const k = c.from === c.to ? `${label(c.to)} (classifications only)` : `${label(c.from)} → ${label(c.to)}`;
        transitions[k] = (transitions[k] || 0) + 1;
    }
    const lastRun = [...base.history].reverse()[0] || null;
    return {
        baseline: lastRun ? { date: lastRun.date, commit: lastRun.commit || null } : {},
        classifications,
        fileCount: changed.length,
        transitions,
        becameHigh: changed.filter(c => c.to === 'high' && c.from !== 'high').map(c => c.file),
        noLongerHigh: changed.filter(c => c.from === 'high' && c.to !== 'high').map(c => c.file),
        files: changed,
    };
}

// Files added, renamed, deleted or modified since the commit recorded in the config's last history entry,
// so an update can focus its reading on code the map hasn't seen. Untracked files are counted as added but
// also listed apart: some may already have existed, uncommitted, when the map was applied.
function sinceLastRun(resolution) {
    const last = [...cfg.history].reverse().find(h => h.commit);
    if (!last) return null;
    const git = a => execFileSync('git', a, { cwd: cfg.appRoot, maxBuffer: 1 << 28, stdio: ['ignore', 'pipe', 'ignore'] }).toString();
    try { git(['cat-file', '-e', `${last.commit}^{commit}`]); } catch { return { commit: last.commit, date: last.date, error: 'commit not found in this repository' }; }
    const inScope = f => resolution[f] && !resolution[f].outOfScope;
    const added = new Set(), untracked = new Set(), renamed = [], deleted = [], modified = [];
    const tok = git(['diff', '--name-status', '-z', '-M', '--relative', last.commit, '--', '.']).split('\0').filter(Boolean);
    for (let i = 0; i < tok.length;) {
        const st = tok[i++];
        if (st[0] === 'R' || st[0] === 'C') { const from = tok[i++], to = tok[i++]; renamed.push({ from, to }); continue; }
        const f = tok[i++];
        if (st[0] === 'A') added.add(f); else if (st[0] === 'D') deleted.push(f); else modified.push(f);
    }
    for (const f of git(['ls-files', '-z', '--others', '--exclude-standard']).split('\0').filter(Boolean)) { added.add(f); untracked.add(f); }
    const arrived = [...added, ...renamed.map(r => r.to)].filter(inScope).sort();
    const byLevel = { high: 0, medium: 0, low: 0, none: 0, unclassified: 0 };
    for (const f of arrived) byLevel[resolution[f].level || 'unclassified']++;
    return {
        commit: last.commit, date: last.date,
        added: arrived, addedByLevel: byLevel, addedUnclassified: arrived.filter(f => !resolution[f].level),
        untracked: arrived.filter(f => untracked.has(f)),
        renamed: renamed.filter(r => inScope(r.to)), deleted: deleted.filter(f => !L.outOfScopeOf(cfg, f)), modified: modified.filter(inScope).sort(),
    };
}

const cap = (list, n, where) => (list.length > n ? [...list.slice(0, n), `…and ${list.length - n} more (full list in ${where})`] : list);

const cur = run(cfg);
const { inv, hosts, resolution } = cur;

// Checks the agent must read before applying.
const deadGlobs = [];
for (const e of cfg.plan) for (const g of e.globs) {
    const l = g.trim();
    if (!l || l.startsWith('!') || l.startsWith('#')) continue;
    if (!files.some(f => L.toRe(l).test(f))) deadGlobs.push(`${e.classification} (${e.importance}): ${l}`);
}
const unreachable = inv.filter(i => i.want && !i.host).map(i => `${i.f}  ->  ${L.sigOf(i.want)}  [no Root Folder, project or folder host above it]`);
const plannedOutOfScope = inv.filter(i => i.want && resolution[i.f].outOfScope).map(i => i.f);
// Generated files classified through folder entries because their template has no Template Output element.
// This is normal: templates aren't required to have one.
const viaFolder = inv.filter(i => i.templateId && i.host && i.host.kind === 'custom');
// Templates with several Template Outputs that write files outside all of them (see run: each of them carries rows
// for those files). atRisk: files whose whole path equals another file's folder-relative path; no glob tells them apart.
const outsideOutputs = [...new Set(cur.outside.map(i => i.templateId))].map(t => {
    const list = hosts.filter(h => h.kind === 'template' && h.templateId === t);
    return {
        template: t,
        files: cur.outside.filter(i => i.templateId === t).map(i => i.f),
        outputs: list.map(h => h.dir || '(output root)'),
        atRisk: [...new Set(list.flatMap(h => h.outsideAtRisk || []))],
    };
});
// The Intent solution folder (two levels above the config) sitting inside appRoot: its metadata is usually out of scope.
const solutionRel = (() => {
    const sol = path.resolve(path.dirname(configPath), '..').replace(/\\/g, '/');
    const root = cfg.appRoot;
    return sol.toLowerCase().startsWith(root.toLowerCase() + '/') ? sol.slice(root.length + 1) : null;
})();
const solutionHint = solutionRel && files.some(f => L.under(solutionRel, f) && !resolution[f].outOfScope && !resolution[f].level)
    ? `The Intent solution folder ${solutionRel}/ is inside appRoot and its files are counted as unclassified. Add "${solutionRel}/**" to outOfScope unless the map should cover it.`
    : null;
const inScope = Object.keys(resolution).filter(f => !resolution[f].outOfScope);
const byLevel = { high: 0, medium: 0, low: 0, none: 0, unclassified: 0 };
for (const f of inScope) byLevel[resolution[f].level || 'unclassified']++;
const classified = inScope.length - byLevel.unclassified;
const since = sinceLastRun(resolution);
const changes = baselinePath ? (() => { const base = L.loadConfig(baselinePath, { lenient: true }); return compare(base, run(base).resolution, resolution); })() : null;

fs.mkdirSync(outDir, { recursive: true });
const result = { application: cfg.application, appRoot: cfg.appRoot, generatedAt: new Date().toISOString(), sinceLastRun: since, filesOutsideTemplateOutputs: outsideOutputs, hosts, files: resolution };
fs.writeFileSync(path.join(outDir, 'rows.json'), JSON.stringify(result, null, 1));
if (changes) fs.writeFileSync(path.join(outDir, 'changes.json'), JSON.stringify({ generatedAt: result.generatedAt, ...changes }, null, 1));
const designerHosts = hosts.filter(h => h.rows.length).map(h => ({ element: h.element, createFolderUnder: h.createFolderUnder, rows: h.rows.map(r => ({ globs: r.globs, importance: r.importance, classifications: r.classifications })) }));
fs.writeFileSync(path.join(outDir, 'designer-data.js'),
    `// Generated by classify-codebase/scripts/generate.js — include before apply-classifications.designer.js.\n` +
    `// After a clean apply, a copy is kept as {Application}.applied.js: the snapshot check-drift.designer.js compares the model with.\n` +
    `const CLASSIFICATIONS = ${JSON.stringify(cfg.classifications.map(c => ({ name: c.name, color: c.color, ...(c.formerNames ? { formerNames: c.formerNames } : {}) })), null, 1)};\n` +
    `const HOSTS = ${JSON.stringify(designerHosts, null, 1)};\n` +
    `// Config hosts that get no rows this run. Template Outputs are left with an empty Entries list; other hosts lose the stereotype.\n` +
    `const CLEAR = ${JSON.stringify(hosts.filter(h => !h.rows.length).map(h => h.element), null, 1)};\n` +
    `const RETIRED = ${JSON.stringify(cfg.retiredClassifications)};\n`);

const outOfScopeCount = Object.keys(resolution).length - inScope.length;
console.log(JSON.stringify({
    files: files.length,
    ...(outOfScopeCount ? { outOfScope: outOfScopeCount } : {}),
    classified,
    byLevel,
    highShareOfClassified: classified ? `${Math.round((100 * byLevel.high) / classified)}%` : 'n/a',
    hosts: hosts.map(h => `${h.rows.length} rows / ${h.fileCount} files / wrong ${h.wrong.length}  ${h.element}`),
    wrong: hosts.filter(h => h.wrong.length).map(h => ({ host: h.element, files: h.wrong })),
    unreachable,
    deadGlobs,
    ...(plannedOutOfScope.length ? { plannedButOutOfScope: { note: 'the plan classifies these, but outOfScope excludes them from the counts and the document: drop one or the other', files: cap(plannedOutOfScope, 15, 'rows.json') } } : {}),
    ...(solutionHint ? { hint: solutionHint } : {}),
    generatedViaFolderHosts: { count: viaFolder.length, note: 'normal: their template has no Template Output element', examples: viaFolder.slice(0, 5).map(i => `${i.f} (${i.templateId}) -> ${i.host.element}`) },
    ...(outsideOutputs.length ? {
        filesOutsideTemplateOutputs: {
            note: 'These templates write files outside every one of their Template Outputs. Intent classifies such a file with the first of them it loads that has entries, so each of them carries rows for these files; only atRisk files can show differently.',
            templates: outsideOutputs.map(t => ({ ...t, files: cap(t.files, 10, 'rows.json'), atRisk: cap(t.atRisk, 10, 'rows.json') })),
        },
    } : {}),
    ...(since ? {
        sinceLastRun: since.error ? since : {
            commit: since.commit, date: since.date,
            added: since.added.length, untracked: since.untracked.length, addedByLevel: since.addedByLevel, addedUnclassified: cap(since.addedUnclassified, 30, 'rows.json'),
            renamed: since.renamed.length, deleted: since.deleted.length, modified: since.modified.length,
        },
    } : {}),
    ...(changes ? {
        changes: {
            classifications: Object.fromEntries(Object.entries(changes.classifications).filter(([, v]) => v.length)),
            files: changes.fileCount, transitions: changes.transitions,
            becameHigh: cap(changes.becameHigh, 15, 'changes.json'), noLongerHigh: cap(changes.noLongerHigh, 15, 'changes.json'),
            ...(changes.classifications.removedNotRetired.length ? { warning: 'removedNotRetired: these stay in the model as unused classifications unless listed in retiredClassifications' } : {}),
        },
    } : {}),
    outputs: [path.join(outDir, 'rows.json'), path.join(outDir, 'designer-data.js'), ...(changes ? [path.join(outDir, 'changes.json')] : [])],
}, null, 1));
