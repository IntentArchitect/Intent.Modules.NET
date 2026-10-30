// Shared helpers for the classify-codebase scripts. Node 18+, no dependencies.
'use strict';
const fs = require('fs');
const path = require('path');
const { execSync, execFileSync } = require('child_process');

const LEVELS = ['none', 'low', 'medium', 'high'];
const RANK = { none: 0, low: 1, medium: 2, high: 3 };
const IMPORTANCE = { none: '0 - None', low: '1 - Low', medium: '2 - Medium', high: '3 - High' };
const LABEL = { none: 'None', low: 'Low', medium: 'Medium', high: 'High' };

// Glob -> RegExp with the same semantics as Intent Architect's own matcher (GlobLineSet), verified against it:
// - every pattern is anchored to the host element's folder: `README.md` and `*.xml` match the top level only;
// - `**` matches any number of folders (`**/x` reaches every depth; `dir/**` is everything under dir);
// - `*` and `?` stay within one folder; matching ignores case; `\` is read as `/`;
// - a leading `/` matches nothing, and braces / character classes are not supported.
const reCache = new Map();
function toRe(glob) {
    let re = reCache.get(glob);
    if (re) return re;
    const g = glob.trim().replace(/\\/g, '/');
    if (g.startsWith('/')) re = /(?!)/;
    else {
        const segs = g.split('/');
        let src = '^';
        segs.forEach((s, i) => {
            const last = i === segs.length - 1;
            if (s === '**') { src += last ? '.*' : '(?:[^/]*/)*'; return; }
            src += [...s].map(c => (c === '*' ? '[^/]*' : c === '?' ? '[^/]' : c.replace(/[.+^${}()|[\]\\]/g, '\\$&'))).join('');
            if (!last) src += '/';
        });
        re = new RegExp(src + '$', 'i');
    }
    reCache.set(glob, re);
    return re;
}

// The lines of a row or plan entry that are patterns: not blank, not '#' comments.
const patternLines = lines => lines.map(l => l.trim()).filter(l => l && !l.startsWith('#'));

// One row's glob lines: blank and '#' lines ignored, '!' excludes, the last matching line wins.
// A row with no pattern lines matches everything, as in Intent.
function linesMatch(lines, relPath) {
    let hit = false;
    let any = false;
    for (const raw of lines) {
        const l = raw.trim();
        if (!l || l.startsWith('#')) continue;
        any = true;
        const neg = l.startsWith('!');
        if (toRe(neg ? l.slice(1) : l).test(relPath)) hit = !neg;
    }
    return any ? hit : true;
}

// Patterns Intent's matcher would silently never match.
function globProblem(line) {
    const l = line.trim().replace(/^!/, '');
    if (!l || line.trim().startsWith('#')) return null;
    if (l.startsWith('/')) return 'starts with "/" (Intent matches nothing; patterns are already anchored to the host folder)';
    if (/[{}[\]]/.test(l)) return 'uses braces or brackets (not supported; write one pattern per line)';
    return null;
}

const asList = (cfg, key, fail) => {
    if (cfg[key] === undefined) cfg[key] = [];
    if (!Array.isArray(cfg[key])) fail(`"${key}" must be a list`);
    return cfg[key];
};

// `lenient` is for a baseline (the previously applied config): defects, notes and templateFiles aren't checked,
// since they don't affect how files resolve. Everything resolving needs is still checked.
function loadConfig(file, { lenient = false } = {}) {
    const cfg = JSON.parse(fs.readFileSync(file, 'utf8'));
    const fail = msg => { throw new Error(`${file}: ${msg}`); };
    if (!cfg.application) fail('"application" (the Intent application name) is required');
    if (!cfg.appRoot) fail('"appRoot" (the application output folder) is required');
    cfg.appRoot = cfg.appRoot.replace(/\\/g, '/').replace(/\/$/, '');
    for (const key of ['classifications', 'plan', 'hosts', 'defects', 'notes', 'history', 'retiredClassifications', 'outOfScope']) asList(cfg, key, fail);

    // Names are matched case-insensitively in the model, so names differing only in case would share one element.
    const names = new Set();
    const lower = new Set();
    for (const c of cfg.classifications) {
        if (!c.name || typeof c.name !== 'string' || !c.name.trim()) fail(`classification ${JSON.stringify(c)} needs a name`);
        if (/[|\r\n]/.test(c.name)) fail(`classification name "${c.name}" contains "|" or a line break`);
        const k = c.name.trim().toLowerCase();
        if (lower.has(k)) fail(`duplicate classification "${c.name}" (names are compared ignoring case)`);
        names.add(c.name);
        lower.add(k);
        if (c.color !== undefined && !/^#[0-9a-f]{6}$/i.test(c.color)) fail(`classification "${c.name}" has color "${c.color}" (use six-digit hex, e.g. #C62828)`);
    }
    // formerNames: names a classification had before a rename or merge. retiredClassifications: names to delete.
    const former = new Set();
    for (const c of cfg.classifications) for (const f of c.formerNames || []) {
        const k = String(f).trim().toLowerCase();
        if (k !== c.name.trim().toLowerCase() && lower.has(k)) fail(`"${c.name}" lists "${f}" in formerNames, but "${f}" is still a classification`);
        if (former.has(k)) fail(`"${f}" appears in more than one formerNames list`);
        former.add(k);
    }
    for (const r of cfg.retiredClassifications) {
        const k = String(r).trim().toLowerCase();
        if (lower.has(k)) fail(`retiredClassifications lists "${r}", which is still a classification`);
        if (former.has(k)) fail(`retiredClassifications lists "${r}", which is also in a formerNames list`);
    }
    for (const h of cfg.history) {
        if (!h.date || !h.change) fail(`history entry ${JSON.stringify(h)} needs date and change`);
        if (h.commit && !/^[0-9a-f]{4,40}$/i.test(h.commit)) fail(`history commit "${h.commit}" is not a commit hash`);
    }
    if (cfg.decisions && (!Array.isArray(cfg.decisions) || cfg.decisions.some(d => typeof d !== 'string' || !d.trim()))) fail('"decisions" must be a list of non-empty strings');
    for (const d of lenient ? [] : cfg.defects) {
        if (!d || typeof d.title !== 'string' || !d.title.trim()) fail(`defect ${JSON.stringify(d)} needs a title`);
        if (d.path !== undefined && d.where === undefined) fail(`defect "${d.title}" uses "path"; the field is "where"`);
        if (d.where !== undefined) {
            const w = [].concat(d.where);
            if (w.some(x => typeof x !== 'string' || !x.trim())) fail(`defect "${d.title}" has a "where" that isn't a path or a list of paths`);
            d.where = w.map(x => x.trim().replace(/\\/g, '/').replace(/^\.\//, ''));
        }
    }
    for (const n of lenient ? [] : cfg.notes) if (!n || typeof n.body !== 'string') fail(`note ${JSON.stringify(n)} needs a body`);
    for (const e of cfg.plan) {
        if (!names.has(e.classification)) fail(`plan entry uses unknown classification "${e.classification}"`);
        if (!(e.importance in RANK)) fail(`plan entry for "${e.classification}" has importance "${e.importance}" (use none/low/medium/high)`);
        if (!Array.isArray(e.globs) || !e.globs.length) fail(`plan entry for "${e.classification}" (${e.importance}) has no globs`);
        // An entry with no pattern lines would match every file, and one with only exclusions matches nothing.
        if (!patternLines(e.globs).some(l => !l.startsWith('!'))) fail(`plan entry for "${e.classification}" (${e.importance}) has no pattern line that includes files`);
        for (const g of e.globs) { const p = globProblem(g); if (p) fail(`plan glob "${g}" for "${e.classification}" ${p}`); }
    }
    // Like a plan entry, a list with only comments or exclusions would match everything or nothing.
    if (cfg.outOfScope.length && !patternLines(cfg.outOfScope).some(l => !l.startsWith('!'))) fail('"outOfScope" has no pattern line that includes files');
    for (const g of cfg.outOfScope) {
        if (typeof g !== 'string') fail('"outOfScope" must be a list of globs');
        const p = globProblem(g); if (p) fail(`outOfScope glob "${g}" ${p}`);
    }
    for (const [f, t] of Object.entries(lenient ? {} : cfg.templateFiles || {})) if (typeof t !== 'string' || !t) fail(`templateFiles entry "${f}" needs a template id`);
    for (const h of cfg.hosts) {
        if (!h.element || h.dir === undefined || !['custom', 'template'].includes(h.kind)) fail(`host ${JSON.stringify(h)} needs element, dir and kind (custom|template)`);
        if (h.kind === 'template' && !h.templateId) fail(`template host ${h.element} needs templateId`);
        h.dir = h.dir.replace(/\\/g, '/').replace(/^\/|\/$/g, '');
    }
    return cfg;
}

// Tracked plus untracked-but-not-ignored files under the application output folder.
function listFiles(appRoot) {
    let out;
    try {
        out = execFileSync('git', ['ls-files', '-z', '--cached', '--others', '--exclude-standard'], { cwd: appRoot, maxBuffer: 1 << 28, stdio: ['ignore', 'pipe', 'pipe'] }).toString();
    } catch (e) {
        throw new Error(`git ls-files failed in appRoot ${appRoot}: it must exist and be inside a git repository. ${String(e.stderr || e.message).trim()}`);
    }
    return [...new Set(out.split('\0').filter(Boolean))].filter(f => fs.existsSync(path.join(appRoot, f))).sort();
}

function repoRoot(appRoot) {
    return execSync('git rev-parse --show-toplevel', { cwd: appRoot }).toString().trim().replace(/\\/g, '/');
}

// appRoot relative to the repository root ('' when they are the same), in git's own spelling. Unlike a
// path.relative of the two absolute paths, this survives drive-letter case and 8.3 short names on Windows.
function repoPrefix(appRoot) {
    return execFileSync('git', ['rev-parse', '--show-prefix'], { cwd: appRoot }).toString().trim().replace(/\\/g, '/').replace(/\/$/, '');
}

// Files the config deliberately leaves out of the map, such as Intent's own metadata folder.
const outOfScopeOf = (cfg, f) => patternLines(cfg.outOfScope).some(l => !l.startsWith('!')) && linesMatch(cfg.outOfScope, f);

// The template that generated a file: an explicit "templateFiles" entry, else the template-id attribute
// Intent writes near the top of the file. Only a real template id counts, so documentation that merely
// mentions the attribute (with a placeholder like <id>) is not mistaken for generated code.
const TEMPLATE_ATTR = new RegExp('IntentTemplate' + '\\(\\s*["\']([A-Za-z_][\\w.\\-]*)["\']');
function templateOf(cfg, rel) {
    if (cfg.templateFiles && cfg.templateFiles[rel]) return cfg.templateFiles[rel];
    let fd;
    try {
        fd = fs.openSync(path.join(cfg.appRoot, rel), 'r');
        const buf = Buffer.alloc(16384);
        const n = fs.readSync(fd, buf, 0, buf.length, 0);
        const m = buf.toString('utf8', 0, n).match(TEMPLATE_ATTR);
        return m ? m[1] : null;
    } catch { return null; } finally { if (fd !== undefined) fs.closeSync(fd); }
}

const under = (dir, f) => dir === '' || f.startsWith(dir + '/');
const relTo = (dir, f) => (dir === '' ? f : f.slice(dir.length + 1));
const absFrom = (dir, r) => (dir === '' ? r : dir + '/' + r);

// Which host classifies a file, as Intent decides it:
// - A generated file whose template has a Template Output belongs to that Template Output: the deepest one
//   containing the file, else the template's first one, which matches it on its full path. (Intent takes the
//   first it loads that has entries; generate.js gives each of them rows for such a file, so either agrees.)
//   It stays there even if the Template Output has no entries: Intent leaves those files unclassified, with
//   no folder fallback.
// - Only a template with no Template Output element at all is classified like a hand-written file, by the
//   deepest project, folder or Root Folder above it.
// `hosts` defaults to every config host; generate.js passes a narrower list once it knows which projects and
// folders end up with no entries (the apply removes their stereotype, so the element above takes over).
function assignHost(cfg, f, templateId, hosts = cfg.hosts) {
    const deepest = list => list.filter(h => under(h.dir, f)).sort((a, b) => b.dir.length - a.dir.length)[0] || null;
    if (templateId) {
        const outputs = hosts.filter(h => h.kind === 'template' && h.templateId === templateId);
        if (outputs.length) return deepest(outputs) || outputs[0];
    }
    return deepest(hosts.filter(h => h.kind === 'custom'));
}
// The path a host's globs see: relative to its folder, or the whole path for a file outside it
// (only possible for a Template Output, whose template can write outside its folder).
const remainderOf = (h, f) => (under(h.dir, f) ? relTo(h.dir, f) : f);

// What the plan says about a file: every classification whose entry matches it, at the highest importance among them.
function intended(cfg, f) {
    const hits = cfg.plan.filter(e => linesMatch(e.globs, f));
    if (!hits.length) return null;
    const top = Math.max(...hits.map(e => RANK[e.importance]));
    return { level: LEVELS[top], classes: [...new Set(hits.map(e => e.classification))].sort() };
}
const sigOf = r => (r ? `${r.level} | ${r.classes.join(', ')}` : '(none)');

// Last matching row wins.
function resolveRows(rows, rel) {
    let hit = null;
    for (const row of rows) if (linesMatch(row.globs, rel)) hit = row;
    return hit;
}

module.exports = { LEVELS, RANK, IMPORTANCE, LABEL, toRe, patternLines, linesMatch, globProblem, loadConfig, listFiles, repoRoot, repoPrefix, outOfScopeOf, templateOf, assignHost, remainderOf, intended, sigOf, resolveRows, under, relTo, absFrom };
