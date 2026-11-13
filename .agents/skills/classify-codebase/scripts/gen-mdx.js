// Renders the review document: every classification as a coloured pill with its live model chip, a file tree
// per classification listing every file it covers (from the rows actually applied), the defects the scan found,
// the decisions, notes and change history.
//
// Usage: node gen-mdx.js <config.json> <outDir>/rows.json <out.mdx> [--changes <outDir>/changes.json]
// With --changes (from generate.js --baseline) the page opens with what changed since the previous map.
'use strict';
const fs = require('fs');
const path = require('path');
const L = require('./lib');

const USAGE = 'Usage: node gen-mdx.js <config.json> <rows.json> <out.mdx> [--changes <changes.json>]';
const args = process.argv.slice(2);
const ci = args.indexOf('--changes');
let changesPath = null;
if (ci >= 0) {
    changesPath = args[ci + 1];
    if (!changesPath || changesPath.startsWith('--')) { console.error(`--changes needs a file path. ${USAGE}`); process.exit(2); }
    if (!fs.existsSync(changesPath)) { console.error(`--changes: ${changesPath} does not exist.`); process.exit(2); }
    args.splice(ci, 2);
}
const [configPath, rowsPath, outFile] = args;
if (!configPath || !rowsPath || !outFile || args.length > 3) { console.error(USAGE); process.exit(2); }
const changes = changesPath ? JSON.parse(fs.readFileSync(changesPath, 'utf8')) : null;
const cfg = L.loadConfig(configPath);
const result = JSON.parse(fs.readFileSync(rowsPath, 'utf8'));
const files = result.files;
const appRel = L.repoPrefix(cfg.appRoot);
const repoPath = f => (appRel ? `${appRel}/${f}` : f);
const js = s => JSON.stringify(s);
// A JSX attribute. A quoted string has no escapes in JSX, so anything with a quote or backslash goes in braces.
const attr = s => (/["\\\r\n]/.test(String(s)) ? `{${js(String(s))}}` : js(String(s)));
// Prose is parsed as MDX: a bare '<' or '{' breaks the whole document, a paragraph starting with
// `import`/`export` is read as an ES module statement, and a backslash escapes what follows it.
// Code spans are left alone, since nothing in them is parsed; a span closes only on a backtick run of the
// same length, as in Markdown, and an unclosed run is written as literal backticks.
const prose = t => t.replace(/\\/g, '\\\\').replace(/[<{}]/g, c => ({ '<': '&lt;', '{': '&#123;', '}': '&#125;' }[c]))
    .replace(/^(\s*)(import|export)\b/gm, '$1&#8203;$2');
const md = s => {
    const str = String(s ?? '');
    let out = '', i = 0;
    while (i < str.length) {
        const open = str.indexOf('`', i);
        if (open < 0) { out += prose(str.slice(i)); break; }
        let n = open;
        while (str[n] === '`') n++;
        let close = -1;
        for (let k = n; k < str.length;) {
            const p = str.indexOf('`', k);
            if (p < 0) break;
            let q = p;
            while (str[q] === '`') q++;
            if (q - p === n - open) { close = p; break; }
            k = q;
        }
        if (close < 0) { out += prose(str.slice(i, open)) + '\\`'.repeat(n - open); i = n; continue; }
        out += prose(str.slice(i, open)) + str.slice(open, close + n - open);
        i = close + n - open;
    }
    return out;
};
// A GFM table cell: '|' splits cells even inside a code span, so escape every one.
const cell = s => md(s).replace(/\|/g, '\\|');
const usedIds = new Set();
const uniqueId = base => { let id = base, n = 2; while (usedIds.has(id)) id = `${base}-${n++}`; usedIds.add(id); return id; };
const slug = s => uniqueId('fc-' + (s.toLowerCase().replace(/[^a-z0-9]+/g, '-').replace(/^-|-$/g, '') || 'classification'));
const ORDER = ['high', 'medium', 'low', 'none'];
const app = cfg.application;

const textOn = hex => {
    const [r, g, b] = [1, 3, 5].map(i => parseInt(hex.slice(i, i + 2), 16) / 255).map(c => (c <= 0.03928 ? c / 12.92 : ((c + 0.055) / 1.055) ** 2.4));
    return 0.2126 * r + 0.7152 * g + 0.0722 * b > 0.4 ? '#1F1F1F' : '#FFFFFF';
};
const pill = c => (c.color
    ? `<span title=${attr(c.color)} style={{ background: ${js(c.color)}, color: ${js(textOn(c.color))}, padding: "2px 10px", borderRadius: "999px", fontWeight: 600, whiteSpace: "nowrap" }}>${md(c.name)}</span>`
    : `**${md(c.name)}**`);
const pillOf = n => { const c = cfg.classifications.find(x => x.name === n); return c ? pill(c) : `**${md(n)}**`; };
const ref = c => `<ModelRef application=${attr(app)} designer="Codebase Structure" type="File Classification" name=${attr(c.name)} />`;
const plural = (n, word) => `${n} ${word}${n === 1 ? '' : 's'}`;

const all = Object.keys(files).sort();
const scoped = all.filter(f => !files[f].outOfScope);
const outOfScope = all.filter(f => files[f].outOfScope);
const classified = scoped.filter(f => files[f].level);
const unclassified = scoped.filter(f => !files[f].level);
const count = level => classified.filter(f => files[f].level === level).length;
const counts = {};
for (const c of cfg.classifications) counts[c.name] = { high: 0, medium: 0, low: 0, none: 0, total: 0 };
for (const f of classified) for (const c of files[f].classes) if (counts[c]) { counts[c][files[f].level]++; counts[c].total++; }
const example = classified.find(f => files[f].classes.length > 1 && files[f].level === 'high');

// Deleting these three lines with patch_file makes Intent compile the whole document and check every path.
const STUB = '{/* classify-codebase: compile-check stub.\nDelete these three lines with patch_file to compile the whole document and check every file path.\n*/}';
const out = [];
out.push(`${STUB}
# ${md(cfg.title || `${app} file classifications`)}


The ${cfg.classifications.length} File Classifications on <ModelRef application=${attr(app)} designer="Codebase Structure" name="File Classifications" /> tell a reviewer *what a changed file is for* (the coloured pills in Change Review) and *how closely to look* (its Review Importance). This page lists every file each classification covers. Paths are relative to the repository root.

The chips in the *In the model* column are live links into the designer. The file lists are a snapshot, computed on ${result.generatedAt.slice(0, 10)} from the entries applied to the model, against the ${scoped.length} files under \`${appRel || '.'}\`${outOfScope.length ? ` (${outOfScope.length} more are out of scope)` : ''}. ${classified.length} of them are classified: **${count('high')} High**, ${count('medium')} Medium, ${count('low')} Low and ${count('none')} None.

<Callout id="one-row" tone="warning" title="Importance belongs to the file, not to each classification">

Only one entry classifies a file, so a file shows every classification it belongs to at the highest importance among them${example ? `. For example, \`${repoPath(example)}\` is ${L.LABEL[files[example].level]} and shows ${md(files[example].classes.join(', '))} at that importance` : ''}. A reviewer filters by classification and by importance separately, so the file still surfaces under each classification.

</Callout>
`);

// Generated files outside every Template Output of their template that no glob can tell apart from another file (see generate.js).
const atRisk = [...new Set((result.filesOutsideTemplateOutputs || []).flatMap(t => t.atRisk || []))];
if (atRisk.length) {
    out.push(`<Callout id="outside-template-outputs" tone="warning" title=${attr(`${plural(atRisk.length, 'file')} may show a different classification`)}>

These generated files sit outside every Template Output of their template, so Intent classifies them with the first of those Template Outputs it loads, on their whole path. That path equals another file's path inside one of them, so no entry can tell the two apart: ${atRisk.slice(0, 10).map(f => `\`${repoPath(f)}\``).join(', ')}${atRisk.length > 10 ? ', …' : ''}.

</Callout>
`);
}

// A re-run (update or redo) opens with what it changed, compared with the previous config on the same files.
if (changes) {
    const C = changes.classifications;
    const level = l => (l ? L.LABEL[l] : 'Unclassified');
    const b = changes.baseline || {};
    const since = b.date ? `the map applied on ${md(b.date)}${b.commit ? ` (commit \`${b.commit}\`)` : ''}` : 'the previous config';
    const lines = [
        ...(C.added.length ? [`- **Added:** ${C.added.map(pillOf).join(' ')}`] : []),
        ...C.renamed.map(r => `- **Renamed:** ${md(r.from)} → ${pillOf(r.to)}`),
        ...C.merged.map(m => `- **Merged:** ${m.from.map(md).join(' + ')} → ${pillOf(m.into)}`),
        ...(C.retired.length ? [`- **Retired:** ${C.retired.map(md).join(', ')}`] : []),
        ...(C.removedNotRetired.length ? [`- **Removed from the plan:** ${C.removedNotRetired.map(md).join(', ')}`] : []),
        ...(C.recoloured.length ? [`- **Recoloured:** ${C.recoloured.map(r => pillOf(r.name)).join(' ')}`] : []),
        ...(C.reworded.length ? [`- **Question reworded:** ${C.reworded.map(pillOf).join(' ')}`] : []),
    ];
    const n = changes.fileCount;
    const highs = changes.becameHigh.length || changes.noLongerHigh.length ? `: ${changes.becameHigh.length} became High and ${changes.noLongerHigh.length} are no longer High` : '';
    const transitions = Object.entries(changes.transitions).sort((x, y) => y[1] - x[1]);
    out.push(`## What changed since the previous map

Compared with ${since}, **${plural(n, 'file')}** change${n === 1 ? 's' : ''} classification or importance${highs}.
${lines.length ? '\n' + lines.join('\n') + '\n' : ''}${transitions.length ? `
| Change | Files |
| --- | --- |
${transitions.map(([k, v]) => `| ${cell(k)} | ${v} |`).join('\n')}
` : ''}`);
    if (changes.files.length) {
        out.push(`<FileTree
  id=${attr(uniqueId('fc-update'))}
  title=${attr(`Changed: ${plural(n, 'file')}`)}
  entries={[
${changes.files.map(c => {
            const note = [c.from === c.to ? level(c.to) : `${level(c.from)} → ${level(c.to)}`, ...c.added.map(x => '+ ' + x), ...c.removed.map(x => '− ' + x)].join(' · ');
            return `    { path: ${js(repoPath(c.file))}, note: ${js(note)} },`;
        }).join('\n')}
  ]}
/>
`);
    }
}

out.push(`## The classifications

| Classification | In the model | Question it answers | High | Medium | Low | None | Files |
| --- | --- | --- | --- | --- | --- | --- | --- |
${cfg.classifications.map(c => `| ${pill(c)} | ${ref(c)} | ${cell(c.question)} | ${counts[c.name].high || ''} | ${counts[c.name].medium || ''} | ${counts[c.name].low || ''} | ${counts[c.name].none || ''} | ${counts[c.name].total} |`).join('\n')}
`);

for (const c of cfg.classifications) {
    const own = classified.filter(f => files[f].classes.includes(c.name));
    const lv = ORDER.filter(l => counts[c.name][l]).map(l => `${counts[c.name][l]} ${L.LABEL[l]}`).join(', ');
    const whys = cfg.plan.filter(e => e.classification === c.name && e.why).sort((a, b) => L.RANK[b.importance] - L.RANK[a.importance]);
    out.push(`## ${md(c.name)}

${pill(c)} ${md(c.question)} ${plural(own.length, 'file')}${lv ? ` (${lv})` : ''}.
${whys.length ? '\n' + whys.map(e => `- **${L.LABEL[e.importance]}:** ${md(e.why)}`).join('\n') + '\n' : ''}`);
    if (own.length) {
        out.push(`<FileTree
  id=${attr(slug(c.name))}
  title=${attr(`${c.name}: ${plural(own.length, 'file')}`)}
  entries={[
${own.map(f => {
            const others = files[f].classes.filter(x => x !== c.name);
            return `    { path: ${js(repoPath(f))}, note: ${js(L.LABEL[files[f].level] + (others.length ? ' · also ' + others.join(', ') : ''))} },`;
        }).join('\n')}
  ]}
/>
`);
    }
}

if (unclassified.length) {
    const noHost = unclassified.filter(f => !files[f].host);
    out.push(`## Not classified

<Callout id="unclassified" tone="warning" title=${attr(`${plural(unclassified.length, 'file')} carry no classification`)}>

${noHost.length ? `${noHost.length} of them sit under no element that can carry a classification. ` : ''}The rest were deliberately left out of the plan. Change Review lists unclassified files under their own row, so a reviewer can still include or exclude them.

</Callout>

<FileTree
  id=${attr(uniqueId('fc-unclassified'))}
  title=${attr(`Unclassified: ${plural(unclassified.length, 'file')}`)}
  entries={[
${unclassified.map(f => `    { path: ${js(repoPath(f))}${files[f].host ? '' : ', note: "no host element"'} },`).join('\n')}
  ]}
/>
`);
}

if (outOfScope.length) {
    const byGlob = cfg.outOfScope.map(g => ({ g, n: outOfScope.filter(f => L.toRe(g.replace(/^!/, '')).test(f)).length })).filter(x => x.n && !x.g.startsWith('!'));
    out.push(`## Out of scope

${plural(outOfScope.length, 'file')} are deliberately outside the map and left out of every count above: ${byGlob.map(x => `\`${x.g}\` (${x.n})`).join(', ')}.
`);
}

// Defects the scan found. They live here, in the review document, rather than in the chat report.
if (cfg.defects.length) {
    // "where" is a path relative to appRoot, optionally with ":line" or ":line-endLine", or a list of them.
    const where = d => [].concat(d.where || []).map(w => {
        const m = String(w).match(/^(.*?):(\d+)(?:-(\d+))?$/);
        if (!m) return `<FileRef path=${attr(repoPath(w))} />`;
        return `<FileRef path=${attr(repoPath(m[1]))} line={${Number(m[2])}}${m[3] ? ` endLine={${Number(m[3])}}` : ''} />`;
    }).join(', ');
    out.push(`## Defects found during the scan

<Callout id="defects" tone="risk" title=${attr(`${plural(cfg.defects.length, 'defect')} found, not fixed by this work`)}>

The scan reads the code closely, so it surfaces real bugs and security holes. They raise the importance of the files involved, but fixing them is a separate piece of work.

</Callout>

${cfg.defects.map((d, i) => `${i + 1}. **${md(d.title)}**${d.detail ? ` ${md(d.detail)}` : ''}${d.where && d.where.length ? ` (${where(d)})` : ''}`).join('\n')}
`);
}

if ((cfg.decisions || []).length) {
    out.push(`## Decisions agreed with the team

${cfg.decisions.map(d => `- ${md(d)}`).join('\n')}
`);
}

cfg.notes.forEach((n, i) => out.push(`<Callout id=${attr(uniqueId(`note-${i + 1}`))} tone=${attr(['info', 'decision', 'risk', 'warning', 'success'].includes(n.tone) ? n.tone : 'info')}${n.title ? ` title=${attr(n.title)}` : ''}>

${md(n.body)}

</Callout>
`));

if (cfg.history.length) {
    out.push(`## Change history

| Date | Commit | Change |
| --- | --- | --- |
${[...cfg.history].reverse().map(h => `| ${cell(h.date)} | ${h.commit ? `\`${h.commit}\`` : ''} | ${cell(h.change)} |`).join('\n')}
`);
}

fs.mkdirSync(path.dirname(outFile), { recursive: true });
fs.writeFileSync(outFile, out.join('\n'));
console.log(JSON.stringify({
    out: outFile, bytes: fs.statSync(outFile).size, files: scoped.length, outOfScope: outOfScope.length, classified: classified.length, unclassified: unclassified.length,
    next: 'Delete the 3-line compile-check stub at the top with patch_file (empty replaceBlock) to compile and path-check the document.',
    stub: STUB,
}, null, 1));
