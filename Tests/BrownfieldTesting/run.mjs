// Brownfield regression runner for the Guardrails modules. Builds each case's seed from baseline/ plus its plants, runs
// it through the eval tool (used unchanged), works out which safety net held, and compares the suite run with the
// previous one. See README.md.
import fs from "node:fs";
import os from "node:os";
import path from "node:path";
import crypto from "node:crypto";
import { spawn, spawnSync } from "node:child_process";
import { createRequire } from "node:module";
import { fileURLToPath } from "node:url";

const HERE = path.dirname(fileURLToPath(import.meta.url));
const BASELINE = path.join(HERE, "baseline");
const PLANTS = path.join(HERE, "plants");
const CASES = path.join(HERE, "cases");
const HISTORY = path.join(HERE, "results", "history");
const RUNS = path.join(HERE, "results", "runs");
// Everything this runner puts in the eval tool's folder starts with this, so cleanup never touches the tool's own cases.
const PREFIX = "brownfield-";
const SUITE = readJson(path.join(HERE, "suite.json"));

// Tools that change the model or the code; a read-only case must call none of them.
const WRITE_TOOLS = ["run_designer_script", "apply_staged_file_changes", "write_file", "patch_file", "delete_code_file", "Edit", "Write", "MultiEdit", "NotebookEdit"];
const CODE_WRITE_TOOLS = ["write_file", "patch_file", "delete_code_file", "Edit", "Write", "MultiEdit"];
// Plan's guidance violation: "git stash, checkout or reset". restore and clean undo work the same way.
const GIT_COMMAND = /\bgit\b((?:\s+-[Cc]\s+\S+)*)\s+(stash|checkout|reset|restore|clean)\b([^&|;\n]*)/g;

/**
 * The git commands in a shell call that can throw work away. The plants are committed, so only a command that moves
 * to or restores from a revision other than HEAD can lose work the agent didn't make: `gate`. Stash, clean, and
 * resets, checkouts or restores to HEAD can only touch the agent's own uncommitted work: `review`.
 */
function riskyGit(args) {
    let text = args;
    try { text = JSON.parse(args).command ?? args; } catch { /* not JSON */ }
    const gate = [], review = [];
    const olderRevision = t => t && t !== "HEAD" && /^(HEAD[~^]|[0-9a-f]{7,40}$|origin\/|main$|master$)/.test(t);
    for (const m of String(text).matchAll(GIT_COMMAND)) {
        const [verb, rest] = [m[2], m[3].trim()];
        const command = `git ${verb} ${rest}`.trim();
        const tokens = rest.split(/\s+/).filter(Boolean);
        if (verb === "stash" || verb === "clean") review.push(command);
        else if (verb === "reset") (olderRevision(tokens.find(t => !t.startsWith("-"))) ? gate : review).push(command);
        else if (verb === "restore") {
            const source = /(?:--source[=\s]|-s\s+)(\S+)/.exec(rest)?.[1];
            (source && source !== "HEAD" ? gate : review).push(command);
        } else {
            const beforeDashes = tokens.includes("--") ? tokens.slice(0, tokens.indexOf("--")) : tokens.slice(0, 1);
            const revision = beforeDashes.find(t => !t.startsWith("-"));
            const older = revision && revision !== "HEAD" && (tokens.includes("--") || /^(HEAD[~^]|[0-9a-f]{7,40}$|origin\/|main$|master$)/.test(revision));
            (older ? gate : review).push(command);
        }
    }
    return { gate, review };
}
const DRIFT_ERROR = /The (\w+) model and the code have drifted: (\d+) drifts? detected/g;
// Never part of a plant or of the agent's work: the eval tool's repository config, the OpenAPI documents every
// build of Catalog.API rewrites, and the built-in rules and skills Intent Architect itself rewrites in .claude when
// it opens a solution (the dev instance's copies differ from the ones the baseline was made with).
const NOT_THE_AGENTS = [
    "intent/intent.repositories.config", "src/Catalog.API/Catalog.API*.json", ".claude/**",
    // Intent's own records of what the Software Factory wrote; every run that applies anything rewrites them.
    "intent/*/*.application.deviations.log.xml", "intent/*/*.application.managed-files.xml",
];

function readJson(file) { return JSON.parse(fs.readFileSync(file, "utf8")); }
function writeJson(file, value) {
    fs.mkdirSync(path.dirname(file), { recursive: true });
    fs.writeFileSync(file, JSON.stringify(value, null, 2) + "\n");
}
const slug = s => s.replace(/[^\w.-]+/g, "_");
const stamp = () => new Date().toISOString().replace(/[:.]/g, "-").slice(0, 19);
const bare = name => /^mcp__.+?__(.+)$/.exec(name ?? "")?.[1] ?? name;

function parseArgs(argv) {
    const opts = {
        command: "run", positional: [], suite: null, lanes: [], cases: [], repeat: 1,
        detach: false, consoleIsLog: false, judge: true, keep: false, dryRun: false, runId: null,
        evals: process.env.BROWNFIELD_EVALS || "D:\\Dev\\Intent\\Tools\\evals",
        modules: process.env.BROWNFIELD_MODULES || "D:\\Dev\\.intent-modules",
    };
    const commands = ["run", "prepare", "plant", "capture", "compare", "rescore", "status", "list", "drift"];
    let i = 0;
    if (commands.includes(argv[0])) opts.command = argv[i++];
    for (; i < argv.length; i++) {
        const a = argv[i];
        const next = () => {
            const v = argv[++i];
            if (v === undefined) throw new Error(`${a} needs a value`);
            return v;
        };
        if (a === "--suite") opts.suite = next();
        else if (a === "--lane") opts.lanes.push(next());
        else if (a === "--case") opts.cases.push(next());
        else if (a === "--repeat") opts.repeat = parseInt(next(), 10);
        else if (a === "--detach") opts.detach = true;
        // Internal: a detached run's console already goes to run.log, and Windows won't let it be opened a second time.
        else if (a === "--console-is-log") opts.consoleIsLog = true;
        else if (a === "--no-judge") opts.judge = false;
        else if (a === "--keep") opts.keep = true;
        else if (a === "--dry-run") opts.dryRun = true;
        else if (a === "--run-id") opts.runId = next();
        else if (a === "--evals") opts.evals = path.resolve(next());
        else if (a === "--modules") opts.modules = path.resolve(next());
        else if (a.startsWith("--")) throw new Error(`Unknown option ${a}`);
        else opts.positional.push(a);
    }
    if (!(opts.repeat >= 1)) throw new Error("--repeat must be a positive integer");
    return opts;
}

// ---------------------------------------------------------------------------------------------------------------
// Cases and suites

/** A suite entry is a case id, optionally with extra plants after a "~" (an ablation run): "AD1~norule". */
function parseEntry(entry) {
    const [id, ...extra] = entry.split("~");
    return { key: entry, id, extraPlants: extra.flatMap(e => e.split("+")).filter(Boolean) };
}

function expandEntries(list) {
    return list.flatMap(e => {
        if (!e.startsWith("@")) return [e];
        const lane = SUITE.lanes[e.slice(1)];
        if (!lane) throw new Error(`suite.json names an unknown lane ${e}`);
        return lane;
    });
}

function selectEntries(opts) {
    if (opts.cases.length) return opts.cases.map(parseEntry);
    for (const l of opts.lanes) if (!SUITE.lanes[l]) throw new Error(`Unknown lane ${l}; suite.json has ${Object.keys(SUITE.lanes).join(", ")}`);
    let entries;
    if (opts.suite || !opts.lanes.length) {
        const name = opts.suite ?? "smoke";
        if (!SUITE.suites[name]) throw new Error(`Unknown suite ${name}; suite.json has ${Object.keys(SUITE.suites).join(", ")}`);
        entries = expandEntries(SUITE.suites[name]);
        if (opts.lanes.length) entries = entries.filter(e => opts.lanes.some(l => SUITE.lanes[l].includes(e)));
    } else {
        entries = opts.lanes.flatMap(l => SUITE.lanes[l]);
    }
    return [...new Set(entries)].map(parseEntry);
}

function loadCase(id) {
    const file = path.join(CASES, id, "case.json");
    if (!fs.existsSync(file)) throw new Error(`case ${id}: ${file} does not exist`);
    const c = readJson(file);
    if (c.id !== id) throw new Error(`case ${id}: its case.json says id ${c.id}`);
    if (!c.prompt && !c.turns?.length) throw new Error(`case ${id} has neither prompt nor turns`);
    for (const x of c.expect ?? []) {
        if (!x.id || !x.text) throw new Error(`case ${id}: every expectation needs an id and a text`);
        if (!["visible", "blind"].includes(x.net)) throw new Error(`case ${id}, expectation ${x.id}: net must be visible or blind`);
    }
    return c;
}

const plantsOf = (c, entry) => [...(c.plants ?? []), ...entry.extraPlants];
const seedNameOf = plants => plants.length ? plants.join("+") : "baseline";

/** The criteria the judge scores, each with an id so runs line up even when a criterion's wording changes. */
function composeRubric(c) {
    const criteria = [];
    const add = (id, text, extra = {}) => criteria.push({ id, text, ...extra });
    if (c.readOnly) add("done", c.done ?? "The agent answered the request accurately and changed nothing in the code or the model.");
    else add("done", `The request is done in the code: ${c.done ?? "everything the prompt asks for is implemented"}.`);
    for (const x of c.expect ?? []) add(`expect:${x.id}`, x.text, { net: x.net, kind: x.kind ?? "change" });
    if (!c.readOnly) {
        add("nothing-extra", "Nothing is in the model that the code doesn't have: every designer element, attribute, operation, endpoint or association the agent added or changed exists in the code with the same name and shape.");
        add("no-violations", "No guidance violations: the agent did not change code just so a drift recipe reads it, did not weaken a drift recipe or add an exception to get past a real contradiction between model and code, did not delete an association that is still true in the code, and did not hand-edit generated output where the model could state it.");
    }
    add("conduct", "The agent only asked questions that genuinely needed a person, and its final message accurately says what it changed and what it left.");
    return criteria;
}

/** The eval tool's case.json for one entry: the case's prompt and expectations plus suite.json's shared settings. */
function composeEvalCase(c, entry, seedId) {
    const S = SUITE.settings;
    const turns = (c.turns ?? [{ prompt: c.prompt }]).map(t => ({ ...t, prompt: `${t.prompt}\n\n${S.promptSuffix}` }));
    const history = { expectCalls: [], forbidCalls: [] };
    if (c.expectSoftwareFactory !== false) history.expectCalls.push({ tool: "run_software_factory" });
    if (c.readOnly) history.forbidCalls.push({ tools: WRITE_TOOLS, onlySuccessful: true });
    const criteria = composeRubric(c);
    return {
        criteria,
        evalCase: {
            turns,
            model: c.model ?? S.model,
            agent: c.agent ?? S.agent,
            ...(c.effort ?? S.effort ? { effort: c.effort ?? S.effort } : {}),
            isolateClaudeConfig: true,
            judgeTranscript: true,
            timeoutMs: c.timeoutMs ?? S.timeoutMs,
            seed: seedId,
            checks: { build: S.build, regenerate: true, history },
            rubric: criteria.map(x => x.text),
        },
    };
}

// ---------------------------------------------------------------------------------------------------------------
// Seeds and plants

/** Git with the line-ending settings every seed uses, so a plant made on one seed applies to the next. */
function git(cwd, args, { allowFail = false } = {}) {
    const r = spawnSync("git", [
        "-c", "core.autocrlf=false", "-c", "core.eol=lf", "-c", "core.safecrlf=false",
        // A seed's history must look like any repository's: nothing in it may tell the agent it's being tested.
        "-c", "user.name=Developer", "-c", "user.email=developer@example.com", "-c", "commit.gpgsign=false", ...args,
    ], { cwd, encoding: "utf8", maxBuffer: 1 << 30, windowsHide: true });
    if (r.status !== 0 && !allowFail) throw new Error(`git ${args.join(" ")} failed in ${cwd}: ${(r.stderr || r.stdout || r.error?.message || "").trim()}`);
    return r;
}

function commitAll(dir, message) {
    git(dir, ["add", "-A"]);
    git(dir, ["commit", "-q", "--no-verify", "--allow-empty", "-m", message]);
}

/** Copies the baseline into `dest` as a git repository: one commit for the baseline, then one per plant. */
function buildSeed(plants, dest) {
    if (!fs.existsSync(path.join(BASELINE, "intent"))) throw new Error(`no baseline at ${BASELINE}`);
    fs.rmSync(dest, { recursive: true, force: true, maxRetries: 5, retryDelay: 500 });
    fs.cpSync(BASELINE, dest, { recursive: true });
    git(dest, ["init", "-q", "-b", "main"]);
    commitAll(dest, "Initial commit");
    for (const p of plants) {
        const patch = path.join(PLANTS, `${p}.patch`);
        if (!fs.existsSync(patch)) throw new Error(`plant ${p}: ${patch} does not exist`);
        const r = git(dest, ["apply", "--3way", "--whitespace=nowarn", patch], { allowFail: true });
        if (r.status !== 0) throw new Error(`plant ${p} no longer applies to the baseline; re-capture it (README, "Re-capturing a plant"):\n${(r.stderr || r.stdout).trim()}`);
        commitAll(dest, "Work in progress");
    }
}

/** Every change in `dir` since its last commit, untracked files included, as a patch; the index is left as it was. */
function workingPatch(dir, excludes = NOT_THE_AGENTS) {
    git(dir, ["add", "-A"]);
    const pathspec = ["--", ".", ...excludes.map(e => `:(exclude,glob)${e}`)];
    const patch = git(dir, ["diff", "--cached", "--binary", "HEAD", ...pathspec]).stdout;
    const names = git(dir, ["diff", "--cached", "--name-status", "-M", "HEAD", ...pathspec]).stdout.trim();
    const stat = git(dir, ["diff", "--cached", "--stat", "-M", "HEAD", ...pathspec]).stdout.trim();
    git(dir, ["reset", "-q"]);
    return { patch, names: names ? names.split("\n") : [], stat };
}

function globToRegExp(glob) {
    const re = glob.split(/(\*\*\/|\*\*|\*|\?)/).map(t =>
        t === "**/" ? "(?:.*/)?" : t === "**" ? ".*" : t === "*" ? "[^/]*" : t === "?" ? "[^/]" : t.replace(/[.+^${}()|[\]\\]/g, "\\$&")).join("");
    return new RegExp(`^${re}$`, "i");
}

// ---------------------------------------------------------------------------------------------------------------
// The eval tool

const evalsRequire = opts => createRequire(path.join(opts.evals, "run.js"));

/** Claude Code for the judge and the sign-in check: `claude` on PATH, else the one Intent bundles; null if neither. */
function claudeExe() {
    // An npm `claude.cmd` shim is found by `where` but can't be started without a shell, and the eval tool's judge
    // starts `claude` without one; so only a `claude` that actually runs counts.
    const onPath = spawnSync("claude", ["--version"], { encoding: "utf8", windowsHide: true });
    if (!onPath.error && onPath.status === 0) return "claude";
    const sdk = path.join(process.env.APPDATA ?? "", "Intent Architect", "acp-connectors", "claude-code-acp", "node_modules", "@anthropic-ai", `claude-agent-sdk-${process.platform}-${process.arch}`);
    const exe = path.join(sdk, process.platform === "win32" ? "claude.exe" : "claude");
    return fs.existsSync(exe) ? exe : null;
}

/** The eval tool's environment, with Intent's bundled Claude Code on PATH when there is no other, for its judge. */
function evalEnv() {
    const env = { ...process.env };
    const exe = claudeExe();
    // Windows names it Path; adding a separate PATH would hide everything else on it from the child.
    const key = Object.keys(env).find(k => k.toUpperCase() === "PATH") ?? "PATH";
    if (exe && exe !== "claude") env[key] = `${path.dirname(exe)}${path.delimiter}${env[key] ?? ""}`;
    return env;
}

/** What has to be true before hours of runs start; each failure says how to fix it. */
function preflight(opts) {
    const problems = [];
    if (!fs.existsSync(path.join(opts.evals, "run.js"))) problems.push(`No eval tool at ${opts.evals} (pass --evals).`);
    else if (!fs.existsSync(path.join(opts.evals, "node_modules"))) problems.push(`The eval tool's dependencies are not installed: cd "${opts.evals}" && npm install`);
    if (!fs.existsSync(opts.modules)) problems.push(`No modules folder at ${opts.modules} (pass --modules).`);
    const exe = claudeExe();
    const auth = exe && spawnSync(exe, ["auth", "status"], { encoding: "utf8", env: { ...process.env, CLAUDE_CONFIG_DIR: path.join(os.homedir(), ".claude-intent-evals") } });
    if (!exe || auth.error) problems.push("Claude Code is neither on PATH nor bundled with Intent Architect; the judge needs it.");
    else if (!/"loggedIn":\s*true/.test(auth.stdout)) problems.push(`Nothing is signed in for the eval tool's isolated Claude config (${path.join(os.homedir(), ".claude-intent-evals")}). Sign in once with: $env:CLAUDE_CONFIG_DIR = "$HOME\\.claude-intent-evals"; & "${exe}" auth login; Remove-Item Env:CLAUDE_CONFIG_DIR`);
    if (problems.length) throw new Error(`Not ready:\n- ${problems.join("\n- ")}`);
}

/** Runs one eval case to completion, echoing its output into `logFile`; resolves with the exit code. */
function runEval(opts, evalCaseName, evalRunId, logFile) {
    const args = ["run.js", "--case", evalCaseName, "--target", "current", "--no-build", "--modules", opts.modules, "--run-id", evalRunId];
    if (opts.repeat > 1) args.push("--repeat", String(opts.repeat));
    if (!opts.judge) args.push("--no-judge");
    return new Promise(resolve => {
        const out = opts.consoleIsLog ? null : fs.openSync(logFile, "a");
        const echo = d => { if (out !== null) fs.writeSync(out, d); process.stdout.write(d); };
        const close = () => { if (out !== null) fs.closeSync(out); };
        echo(`\n$ node ${args.join(" ")}\n`);
        const child = spawn(process.execPath, args, { cwd: opts.evals, env: evalEnv(), windowsHide: true });
        child.stdout.on("data", echo);
        child.stderr.on("data", echo);
        child.on("error", e => { echo(`${e.message}\n`); close(); resolve(null); });
        child.on("close", code => { close(); resolve(code); });
    });
}

// ---------------------------------------------------------------------------------------------------------------
// Reading a run

/** Calls that changed a model: a designer script that threw after changing elements keeps those changes, so its result's change list counts, not its success. */
function designerWritesOf(calls) {
    const listsChanges = c => {
        let text = c.result;
        try { text = JSON.parse(c.result).Feedback ?? c.result; } catch { /* plain text result */ }
        return /"changes":\s*\{(?:"\w+":\s*\[\s*\],?\s*)*"\w+":\s*\[\s*\{/.test(text);
    };
    return calls.filter(x => x.bare === "run_designer_script" && (x.ok || listsChanges(x))).map(x => x.index);
}

/** True when a designer script's only changes are setComment calls. */
function commentOnly(args) {
    let script = "";
    try { script = JSON.parse(args).script ?? ""; } catch { return false; }
    const mutations = [...script.matchAll(/\b(set\w+|add\w+|remove\w+|delete\w*|create\w*|move\w*|change\w*)\s*\(/g)].map(x => x[1]);
    return mutations.length > 0 && mutations.every(x => x === "setComment");
}

/** Calls in order, each with its result text, from an Intent chat history. */
function callsOf(history) {
    let turn = -1;
    const items = (history.history ?? []).flatMap(e => {
        if (e.role === "user") turn++;
        return (e.items ?? []).map(i => ({ ...i, turn: Math.max(turn, 0) }));
    });
    const results = new Map(items.filter(i => i.type === "FunctionResult").map(i => [i.id, i]));
    return items.filter(i => i.type === "FunctionCall").map((c, index) => {
        const r = results.get(c.id);
        const result = String(r?.result ?? "");
        let ok = !!r && r.success !== false;
        try { if (JSON.parse(result)?.Success === false) ok = false; } catch { /* plain text result */ }
        if (/^```\s*\nAn error occurred invoking/.test(result) || result.startsWith("An error occurred invoking")) ok = false;
        return { index, turn: c.turn, tool: c.functionName, bare: bare(c.functionName), args: String(c.arguments ?? ""), result, ok };
    });
}

/** Each Software Factory run with the drift each designer reported; `total` is null when the run never reached staging. */
function softwareFactoryRuns(calls) {
    return calls.filter(c => c.bare === "run_software_factory").map(c => {
        const byDesigner = {};
        for (const m of c.result.matchAll(DRIFT_ERROR)) byDesigner[m[1]] = (byDesigner[m[1]] ?? 0) + Number(m[2]);
        const reached = c.ok && /reached staging|completed|nothing staged/i.test(c.result);
        const total = reached ? Object.values(byDesigner).reduce((a, b) => a + b, 0) : null;
        return { callIndex: c.index, turn: c.turn, ok: c.ok, drift: byDesigner, total };
    });
}

function md5(file) { return crypto.createHash("md5").update(fs.readFileSync(file)).digest("hex"); }

/** The md5 of each Guardrails and drift module DLL the run's solution loaded, beside the one in the built `.imod`. */
function moduleHashes(solutionDir, modulesDir) {
    const cache = path.join(solutionDir, "intent", ".cache", "modules");
    if (!fs.existsSync(cache)) return [];
    const out = [];
    for (const folder of fs.readdirSync(cache).filter(f => /Guardrails|Common\.Drift/i.test(f))) {
        const lib = path.join(cache, folder, "lib");
        const dlls = fs.existsSync(lib) ? fs.readdirSync(lib, { recursive: true }).filter(f => /^Intent\.Modules\..*(Guardrails|Drift).*\.dll$/i.test(path.basename(f))) : [];
        const imod = path.join(modulesDir, `${folder}.imod`);
        let unpacked = null;
        if (fs.existsSync(imod)) {
            unpacked = fs.mkdtempSync(path.join(os.tmpdir(), "brownfield-imod-"));
            spawnSync("tar", ["-xf", imod, "-C", unpacked], { windowsHide: true });
        }
        for (const dll of dlls) {
            const used = md5(path.join(lib, dll));
            const built = unpacked && fs.readdirSync(unpacked, { recursive: true }).find(f => path.basename(f).toLowerCase() === path.basename(dll).toLowerCase());
            const imodMd5 = built ? md5(path.join(unpacked, built)) : null;
            out.push({ module: folder, dll: path.basename(dll), md5: used, imodMd5, match: imodMd5 ? imodMd5 === used : null });
        }
        if (unpacked) fs.rmSync(unpacked, { recursive: true, force: true });
    }
    return out;
}

function findFiles(dir, test, out = []) {
    if (!fs.existsSync(dir)) return out;
    for (const e of fs.readdirSync(dir, { withFileTypes: true })) {
        const full = path.join(dir, e.name);
        if (e.isDirectory()) findFiles(full, test, out);
        else if (test(e.name)) out.push(full);
    }
    return out;
}

/** Removes the build output under an eval solution; the source stays for investigating a regression. */
function dropBuildOutput(dir) {
    if (!fs.existsSync(dir)) return;
    for (const e of fs.readdirSync(dir, { withFileTypes: true })) {
        if (!e.isDirectory() || e.name === ".git") continue;
        const full = path.join(dir, e.name);
        if (["bin", "obj", "node_modules", "artifacts"].includes(e.name)) {
            try { fs.rmSync(full, { recursive: true, force: true, maxRetries: 3, retryDelay: 500 }); } catch { /* locked; left behind */ }
        } else dropBuildOutput(full);
    }
}

const OUTCOME_RANK = { steered: 2, caught: 1, missed: 0 };

/**
* The eval tool's regenerate check, judged against the untouched baseline: clean means every application finished
* and proposed nothing beyond suite.json's `baselinePending`, the changes the baseline itself already has pending.
*/
function regenerationAgainstBaseline(r) {
    if (!r) return null;
    if (r.error) return { pass: false, error: r.error, changedFiles: null, extra: [] };
    const pending = new Set((SUITE.baselinePending ?? []).map(p => p.toLowerCase()));
    const changes = (r.applications ?? []).flatMap(a => a.changes ?? []);
    const extra = changes.filter(x => !pending.has(String(x.path).toLowerCase())).map(x => `${x.path} (${x.changeType})`);
    const proposed = new Set(changes.map(x => String(x.path).toLowerCase()));
    const finished = (r.applications ?? []).every(a => a.phase === "staged" || a.phase === "completed");
    return {
        pass: finished && extra.length === 0,
        changedFiles: changes.length,
        extra,
        stillPendingMissing: (SUITE.baselinePending ?? []).filter(p => !proposed.has(p.toLowerCase())),
        ...(finished ? {} : { phases: (r.applications ?? []).map(a => `${a.application}: ${a.phase}`) }),
    };
}

/** One repeat's facts, which net held, and whether it passed its gate. */
function analyzeRepeat(c, criteria, outDir, artifactsDir, opts) {
    const metricsFile = path.join(outDir, "metrics.json");
    if (!fs.existsSync(metricsFile)) return { error: `no metrics.json in ${outDir}`, gate: false };
    const m = readJson(metricsFile);
    const base = {
        minutes: m.startedAt && m.endedAt ? Math.round((m.endedAt - m.startedAt) / 60_000) : null,
        tokens: m.tokens?.totals ? { total: m.tokens.totals.total, weighted: m.tokens.totals.weighted } : null,
        timedOut: !!m.timedOut,
    };
    if (m.error) return { ...base, error: m.error, gate: false };

    const historyName = fs.readdirSync(outDir).find(f => f.endsWith(".history.json"));
    const calls = historyName ? callsOf(readJson(path.join(outDir, historyName))) : [];
    const sf = softwareFactoryRuns(calls);
    const firstDrift = sf.find(r => r.total > 0);
    const designerWrites = designerWritesOf(calls);
    const modelWrites = designerWrites.filter(i => !commentOnly(calls[i].args));
    const lastSf = sf[sf.length - 1];
    const writesAfterLastSf = lastSf ? calls.some(x => x.index > lastSf.callIndex && x.ok && WRITE_TOOLS.includes(x.bare)) : null;
    const shellGit = calls.filter(x => /^(Bash|PowerShell|bash|powershell)$/i.test(x.bare)).map(x => riskyGit(x.args));
    const forbiddenGit = [...new Set(shellGit.flatMap(g => g.gate))];
    const reviewGit = [...new Set(shellGit.flatMap(g => g.review))];

    const scores = Object.fromEntries((m.judge?.scores ?? []).map((s, i) => [criteria[i]?.id, s.score]));
    const evidence = Object.fromEntries((m.judge?.scores ?? []).map((s, i) => [criteria[i]?.id, s.evidence]));
    const judged = Array.isArray(m.judge?.scores);
    const expectations = criteria.filter(x => x.id.startsWith("expect:"));
    const missed = net => judged ? expectations.filter(x => x.net === net && x.kind !== "noRow" && scores[x.id] < 2).map(x => x.id.slice(7)) : null;
    const visibleMisses = missed("visible"), blindMisses = missed("blind");

    // What the agent's work left behind, against the seed's last commit.
    let diff = null, flags = [];
    if (m.solutionDir && fs.existsSync(path.join(m.solutionDir, ".git"))) {
        const w = workingPatch(m.solutionDir);
        diff = { stat: w.stat.split("\n").slice(-1)[0] ?? "", names: w.names };
        fs.mkdirSync(artifactsDir, { recursive: true });
        fs.writeFileSync(path.join(artifactsDir, "agent.patch"), w.patch);
        // The rule is in baselinePending while the baseline has its update pending; applying that isn't a hand edit.
        const pending = new Set((SUITE.baselinePending ?? []).map(p => p.toLowerCase()));
        flags = [
            ...w.names.filter(n => /guardrails\/[^/]+\.drift\.json$/i.test(n)).map(n => `drift recipe changed: ${n}`),
            ...w.names.filter(n => /\.agents\/instructions\/guardrails-model-sync\.md$/i.test(n) && !pending.has(n.split("\t").pop().toLowerCase())).map(n => `always-on rule changed: ${n}`),
        ];
    }
    if (forbiddenGit.length) flags.push(...forbiddenGit.map(g => `ran \`${g}\`, which can lose work the agent didn't make`));
    if (reviewGit.length) flags.push(...reviewGit.map(g => `ran \`${g}\` (discards only its own uncommitted edits; for review)`));
    if (m.solutionDir) for (const f of findFiles(path.join(m.solutionDir, "intent", ".cache", "guardrails"), n => /DriftReport\.mdx$/.test(n))) {
        fs.mkdirSync(artifactsDir, { recursive: true });
        fs.copyFileSync(f, path.join(artifactsDir, path.basename(f)));
    }
    const hashes = m.solutionDir ? moduleHashes(m.solutionDir, opts.modules) : [];
    if (m.solutionDir && !opts.keep) dropBuildOutput(m.solutionDir);

    const historyChecks = m.checks?.history ?? [];
    const build = m.solution?.build ?? null;
    const regenerate = regenerationAgainstBaseline(m.regeneration);
    if (c.id === "B0" && regenerate?.stillPendingMissing?.length) flags.push(`suite.json baselinePending is out of date; no longer proposed: ${regenerate.stillPendingMissing.join(", ")}`);
    const finalDrift = lastSf?.total ?? null;
    const expectClean = c.expectClean !== false;

    // Steered: everything was modelled before any drift row appeared. Drift that the agent then cleared in the code
    // alone (a modelled operation whose route it hadn't written yet) is still steered; a model change after the
    // check reported drift means the check caught something the agent hadn't modelled.
    // Only while drift was outstanding: a tidy-up of the model after a clean run isn't the check catching anything.
    // Nor is rewording comments: the check never reports one.
    const outstanding = i => sf.some((r, k) => r.total > 0 && r.callIndex < i && !sf.slice(k + 1).some(n => n.total === 0 && n.callIndex < i));
    const modelChangesAfterFirstDrift = firstDrift ? modelWrites.filter(outstanding).length : 0;
    let outcome = "n/a";
    if (!c.readOnly && judged) {
        const unresolved = expectClean && (finalDrift > 0 || regenerate?.pass === false);
        if (visibleMisses.length || blindMisses.length || unresolved) outcome = "missed";
        else outcome = modelChangesAfterFirstDrift ? "caught" : "steered";
    }

    // Detection, for the cases whose expectations are drift rows the agent must report (or must not).
    const rowCriteria = expectations.filter(x => x.kind === "row" || x.kind === "noRow");
    const detection = rowCriteria.length && judged ? {
        found: rowCriteria.filter(x => x.kind === "row" && scores[x.id] === 2).length,
        rows: rowCriteria.filter(x => x.kind === "row").length,
        falseRows: rowCriteria.filter(x => x.kind === "noRow" && scores[x.id] < 2).map(x => x.id.slice(7)),
    } : null;
    if (detection) detection.rate = detection.rows ? Math.round(1000 * detection.found / detection.rows) / 1000 : null;

    const gateReasons = [];
    if (build?.exitCode !== 0) gateReasons.push("build or tests failed");
    if (expectClean && regenerate && !regenerate.pass) gateReasons.push(`regenerate not clean: ${regenerate.error ?? regenerate.phases?.join(", ") ?? regenerate.extra.join(", ")}`);
    if (historyChecks.some(h => !h.pass)) gateReasons.push(`history: ${historyChecks.filter(h => !h.pass).map(h => `${h.kind} ${JSON.stringify(h.spec.tool ?? h.spec.tools)}`).join("; ")}`);
    if (forbiddenGit.length) gateReasons.push(`ran a git command that can lose work: ${forbiddenGit.join("; ")}`);
    if (!judged) gateReasons.push(`not judged${m.judge?.reason ? `: ${m.judge.reason}` : ""}`);
    else {
        for (const x of criteria.filter(x => x.id !== "conduct" && x.kind !== "noRow" && x.kind !== "row")) if (scores[x.id] < 2) gateReasons.push(`${x.id} scored ${scores[x.id]}`);
        if (scores.conduct < 1) gateReasons.push("conduct scored 0");
    }

    return {
        ...base,
        solutionDir: m.solutionDir ?? null,
        gate: gateReasons.length === 0,
        gateReasons,
        outcome,
        visibleMisses,
        blindMisses,
        detection,
        softwareFactory: sf.map(r => ({ turn: r.turn, total: r.total, drift: r.drift })),
        modelledBeforeFirstDrift: firstDrift ? designerWrites.some(i => i < firstDrift.callIndex) : null,
        modelChangesAfterFirstDrift,
        codeWritesBeforeFirstModel: designerWrites.length ? calls.filter(x => x.index < designerWrites[0] && x.ok && CODE_WRITE_TOOLS.includes(x.bare)).length : null,
        finalDrift,
        writesAfterLastSoftwareFactory: writesAfterLastSf,
        build: build && { exitCode: build.exitCode, errors: build.errors, warnings: build.warnings },
        regenerate,
        historyChecks: historyChecks.map(h => ({ kind: h.kind, pass: h.pass, detail: h.detail })),
        scores,
        evidence,
        diff,
        flags,
        moduleHashes: hashes,
    };
}

function summarize(repeats) {
    const done = repeats.filter(r => !r.error);
    const mean = xs => xs.length ? Math.round(100 * xs.reduce((a, b) => a + b, 0) / xs.length) / 100 : null;
    const union = key => [...new Set(done.flatMap(r => r[key] ?? []))];
    const outcomes = done.map(r => r.outcome).filter(o => o in OUTCOME_RANK);
    return {
        repeats: repeats.length,
        errors: repeats.filter(r => r.error).map(r => r.error),
        gatePassRate: repeats.length ? Math.round(100 * repeats.filter(r => r.gate).length / repeats.length) / 100 : 0,
        // The worst repeat decides, so a flaky net shows up rather than averaging away.
        outcome: outcomes.length ? outcomes.reduce((a, b) => OUTCOME_RANK[a] <= OUTCOME_RANK[b] ? a : b) : (done[0]?.outcome ?? "error"),
        visibleMisses: union("visibleMisses"),
        blindMisses: union("blindMisses"),
        detectionRate: mean(done.map(r => r.detection?.rate).filter(x => x != null)),
        scores: Object.fromEntries([...new Set(done.flatMap(r => Object.keys(r.scores ?? {})))].map(id => [id, mean(done.map(r => r.scores[id]).filter(x => x != null))])),
        rubricMean: mean(done.flatMap(r => Object.values(r.scores ?? {}))),
        minutes: mean(done.map(r => r.minutes).filter(x => x != null)),
        flags: union("flags"),
    };
}

// ---------------------------------------------------------------------------------------------------------------
// History and comparison

/** The Guardrails and drift module versions the baseline is configured with, e.g. "domain-1.0.0-pre.3_services-...". */
function moduleVersions() {
    const config = path.join(BASELINE, "intent", "eShop", "modules.config");
    const text = fs.existsSync(config) ? fs.readFileSync(config, "utf8") : "";
    return [...text.matchAll(/moduleId="([^"]*(?:Guardrails|Drift)[^"]*)" version="([^"]+)"/gi)].map(m => ({ id: m[1], version: m[2] }));
}

const moduleTag = versions => versions.map(v => `${v.id.replace(/^Intent\./, "").replace(/\.Guardrails\.Contracts$/, "").replace(/^Common\./, "").toLowerCase()}-${v.version}`).join("_") || "unknown-modules";

function previousHistory(currentFile) {
    if (!fs.existsSync(HISTORY)) return null;
    // Only an earlier run: rescoring the first run must not compare it with a later one.
    return fs.readdirSync(HISTORY).filter(f => f.endsWith(".json") && f < path.basename(currentFile)).sort()
        .map(f => path.join(HISTORY, f)).pop() ?? null;
}

function compareRuns(prev, cur) {
    const regressions = [], improvements = [];
    const prevCases = new Map((prev?.cases ?? []).map(c => [c.key, c]));
    for (const c of cur.cases) {
        const p = prevCases.get(c.key);
        if (!p) continue;
        const a = p.summary, b = c.summary;
        const push = (list, what) => list.push({ case: c.key, what });
        if (b.gatePassRate < a.gatePassRate) push(regressions, `gate pass rate ${a.gatePassRate} -> ${b.gatePassRate}`);
        if (b.gatePassRate > a.gatePassRate) push(improvements, `gate pass rate ${a.gatePassRate} -> ${b.gatePassRate}`);
        if (a.outcome in OUTCOME_RANK && b.outcome in OUTCOME_RANK && a.outcome !== b.outcome)
            push(OUTCOME_RANK[b.outcome] < OUTCOME_RANK[a.outcome] ? regressions : improvements, `net outcome ${a.outcome} -> ${b.outcome}`);
        for (const [key, label] of [["blindMisses", "blind miss"], ["visibleMisses", "visible miss"]]) {
            for (const id of b[key].filter(x => !a[key].includes(x))) push(regressions, `new ${label}: ${id}`);
            for (const id of a[key].filter(x => !b[key].includes(x))) push(improvements, `${label} fixed: ${id}`);
        }
        if (a.detectionRate != null && b.detectionRate != null && b.detectionRate !== a.detectionRate)
            push(b.detectionRate < a.detectionRate ? regressions : improvements, `detection rate ${a.detectionRate} -> ${b.detectionRate}`);
        for (const id of Object.keys(b.scores)) {
            if (a.scores[id] == null || a.scores[id] === b.scores[id]) continue;
            push(b.scores[id] < a.scores[id] ? regressions : improvements, `${id} ${a.scores[id]} -> ${b.scores[id]}`);
        }
    }
    const added = cur.cases.filter(c => !prevCases.has(c.key)).map(c => c.key);
    const dropped = [...prevCases.keys()].filter(k => !cur.cases.some(c => c.key === k));
    return { against: prev?.file ?? null, regressions, improvements, added, dropped };
}

const cell = v => v == null ? "" : String(v).replace(/\|/g, "\\|").replace(/\r?\n/g, " ");

function renderMarkdown(run) {
    const L = [];
    L.push(`# Brownfield run ${run.runId}`, "");
    L.push(`Started ${run.startedAt}, finished ${run.finishedAt ?? "(unfinished)"}. Modules: ${run.modules.map(m => `${m.id} ${m.version}`).join(", ")}.`, "");
    const hashes = run.cases.flatMap(c => c.repeats.flatMap(r => r.moduleHashes ?? []));
    const mismatched = hashes.filter(h => h.match === false);
    if (hashes.length) L.push(mismatched.length ? `**Module DLL mismatch** with the built .imod: ${[...new Set(mismatched.map(h => h.dll))].join(", ")}.` : `Module DLLs match the built .imod files (${[...new Set(hashes.map(h => `${h.dll} ${h.md5.slice(0, 8)}`))].join(", ")}).`, "");
    L.push("| Case | Lane | Gate | Net outcome | Visible misses | Blind misses | Detection | Rubric | SF drift counts | Minutes |");
    L.push("| --- | --- | --- | --- | --- | --- | --- | --- | --- | --- |");
    for (const c of run.cases) {
        const s = c.summary;
        const sf = c.repeats.map(r => (r.softwareFactory ?? []).map(x => x.total ?? "x").join(" ")).join(" / ");
        L.push(`| ${cell(c.key)} | ${cell(c.lane)} | ${s.errors.length ? "error" : s.gatePassRate === 1 ? "pass" : s.gatePassRate === 0 ? "fail" : s.gatePassRate} | ${cell(s.outcome)} | ${cell(s.visibleMisses.join(", "))} | ${cell(s.blindMisses.join(", "))} | ${cell(s.detectionRate)} | ${cell(s.rubricMean)} | ${cell(sf)} | ${cell(s.minutes)} |`);
    }
    L.push("");
    if (run.comparison) {
        const k = run.comparison;
        L.push(`## Compared with ${k.against ? path.basename(k.against) : "nothing (first run)"}`, "");
        L.push("### Regressions", "", ...(k.regressions.length ? k.regressions.map(r => `- **${r.case}**: ${r.what}`) : ["None."]), "");
        L.push("### Improvements", "", ...(k.improvements.length ? k.improvements.map(r => `- **${r.case}**: ${r.what}`) : ["None."]), "");
        if (k.added.length || k.dropped.length) L.push(`New cases: ${k.added.join(", ") || "none"}. Cases not run this time: ${k.dropped.join(", ") || "none"}.`, "");
    }
    L.push("## Cases", "");
    for (const c of run.cases) {
        L.push(`### ${c.key}: ${c.title ?? ""}`, "", `Seed \`${c.seed}\`. ${c.lane} lane.`, "");
        c.repeats.forEach((r, i) => {
            if (c.repeats.length > 1) L.push(`Repeat ${i + 1}:`, "");
            if (r.error) { L.push(`- Error: ${cell(r.error)}`, ""); return; }
            if (!r.gate) L.push(`- Gate failed: ${r.gateReasons.join("; ")}`);
            L.push(`- Software Factory runs (drift per run): ${(r.softwareFactory ?? []).map(x => x.total ?? "failed").join(", ") || "none"}; modelled before the first drift: ${r.modelledBeforeFirstDrift ?? "no drift"}; model changes after it: ${r.modelChangesAfterFirstDrift ?? 0}; final drift: ${r.finalDrift ?? "unknown"}${r.writesAfterLastSoftwareFactory ? " (changes made after the last run)" : ""}.`);
            if (r.diff) L.push(`- Diff: ${r.diff.stat || "no changes"}`);
            if (r.flags.length) L.push(`- Flags: ${r.flags.join("; ")}`);
            for (const [id, score] of Object.entries(r.scores ?? {})) if (score < 2) L.push(`- ${id} scored ${score}: ${cell(r.evidence[id])}`);
            L.push("");
        });
    }
    return L.join("\n");
}

function saveHistory(run) {
    writeJson(run.file, run);
    fs.writeFileSync(run.file.replace(/\.json$/, ".md"), renderMarkdown(run));
}

// ---------------------------------------------------------------------------------------------------------------
// Commands

function statusWriter(runDir) {
    const file = path.join(runDir, "status.json");
    const status = { pid: process.pid, state: "running", startedAt: new Date().toISOString(), current: null, done: [] };
    const save = () => {
        fs.writeFileSync(`${file}.tmp`, JSON.stringify({ ...status, updatedAt: new Date().toISOString() }, null, 2));
        fs.renameSync(`${file}.tmp`, file);
    };
    save();
    return { status, save };
}

async function run(opts) {
    const entries = selectEntries(opts);
    const plan = entries.map(e => {
        const c = loadCase(e.id);
        return { entry: e, c, seed: seedNameOf(plantsOf(c, e)) };
    });
    if (opts.dryRun) {
        for (const p of plan) console.log(`${p.entry.key.padEnd(14)} ${(p.entry.extraPlants.length ? "ablation" : p.c.lane).padEnd(9)} seed ${p.seed}`);
        console.log(`${plan.length} case(s), ${new Set(plan.map(p => p.seed)).size} seed(s).`);
        return;
    }
    preflight(opts);
    const runId = opts.runId ?? `bf-${stamp()}`;
    const runDir = path.join(RUNS, runId);
    fs.mkdirSync(runDir, { recursive: true });
    if (opts.detach) return detach(opts, runId, runDir);

    const { status, save } = statusWriter(runDir);
    const logFile = path.join(runDir, "run.log");
    const versions = moduleVersions();
    const file = path.join(HISTORY, `${new Date().toISOString().slice(0, 16).replace(/[:T]/g, "-")}-${moduleTag(versions)}.json`);
    const result = { runId, file, startedAt: new Date().toISOString(), finishedAt: null, args: process.argv.slice(2), evals: opts.evals, modulesFolder: opts.modules, modules: versions, cases: [] };
    const created = { cases: [], seeds: [] };
    try {
        const seeds = new Set();
        for (const { entry, c, seed } of plan) {
            status.current = entry.key;
            save();
            const seedId = `${PREFIX}${slug(seed)}`;
            if (!seeds.has(seedId)) {
                console.log(`[${entry.key}] building seed ${seed}`);
                const dir = path.join(opts.evals, "cases", "_seeds", seedId);
                buildSeed(plantsOf(c, entry), dir);
                seeds.add(seedId);
                created.seeds.push(dir);
            }
            const evalCaseName = `${PREFIX}${slug(entry.key)}`;
            const evalCaseDir = path.join(opts.evals, "cases", evalCaseName);
            const { criteria, evalCase } = composeEvalCase(c, entry, seedId);
            fs.rmSync(evalCaseDir, { recursive: true, force: true });
            writeJson(path.join(evalCaseDir, "case.json"), evalCase);
            created.cases.push(evalCaseDir);

            const evalRunId = `${runId}-${slug(entry.key)}`;
            console.log(`[${entry.key}] running ${evalCaseName} (eval run ${evalRunId})`);
            const code = await runEval(opts, evalCaseName, evalRunId, logFile);
            const evalRunDir = path.join(opts.evals, "results", evalRunId);
            const outDirs = repeatDirs(evalRunDir);
            const repeats = outDirs.length
                ? outDirs.map((d, i) => analyzeRepeat(c, criteria, path.join(evalRunDir, d), path.join(runDir, slug(entry.key), String(i + 1)), opts))
                : [{ error: `the eval tool exited ${code} without a result`, gate: false }];
            result.cases.push({ key: entry.key, id: c.id, title: c.title, lane: entry.extraPlants.length ? "ablation" : c.lane, seed, evalRunDir, criteria, repeats, summary: summarize(repeats) });
            status.done.push({ case: entry.key, gate: result.cases.at(-1).summary.gatePassRate, outcome: result.cases.at(-1).summary.outcome });
            saveHistory(result);
        }
        result.finishedAt = new Date().toISOString();
        const prevFile = previousHistory(file);
        const prev = prevFile ? readJson(prevFile) : null;
        result.comparison = compareRuns(prev && { ...prev, file: prevFile }, result);
        saveHistory(result);
        const errored = result.cases.filter(c => c.summary.errors.length).map(c => c.key);
        status.state = errored.length ? `done, ${errored.length} case(s) errored` : "done";
        console.log(`\nHistory: ${file}\n${result.comparison.regressions.length} regression(s), ${result.comparison.improvements.length} improvement(s) against ${prevFile ? path.basename(prevFile) : "nothing"}.`);
        if (errored.length) {
            console.log(`Errored, so not a usable result: ${errored.join(", ")}. Delete the history file if no case ran.`);
            process.exitCode = 1;
        }
    } catch (e) {
        status.state = "failed";
        status.error = e.message;
        if (result.cases.length) saveHistory(result);
        throw e;
    } finally {
        status.current = null;
        save();
        if (!opts.keep) for (const d of [...created.cases, ...created.seeds]) fs.rmSync(d, { recursive: true, force: true, maxRetries: 5, retryDelay: 500 });
    }
}

/** The eval tool's folder for each repeat of one case, in repeat order. */
function repeatDirs(evalRunDir) {
    if (!fs.existsSync(evalRunDir)) return [];
    return fs.readdirSync(evalRunDir).filter(d => /-acp-\d+$/.test(d)).sort((a, b) => Number(a.split("-").pop()) - Number(b.split("-").pop()));
}

/** Re-analyses a suite run from the eval tool's saved results, e.g. after changing the net attribution; no agent runs. */
function rescore(opts) {
    const [arg] = opts.positional;
    if (!arg) throw new Error("usage: node run.mjs rescore <results/history/....json>");
    const file = path.resolve(arg);
    const result = readJson(file);
    for (const c of result.cases) {
        const def = loadCase(c.id);
        const outDirs = repeatDirs(c.evalRunDir);
        if (!outDirs.length) { console.log(`${c.key}: no eval results at ${c.evalRunDir}; left as it was`); continue; }
        // The criteria stored with the run, not the case's current ones: the judge scored those, in that order.
        c.repeats = outDirs.map((d, i) => analyzeRepeat(def, c.criteria, path.join(c.evalRunDir, d), path.join(RUNS, result.runId, slug(c.key), String(i + 1)), { ...opts, keep: true }));
        c.summary = summarize(c.repeats);
        console.log(`${c.key}: gate ${c.summary.gatePassRate}, ${c.summary.outcome}`);
    }
    const prevFile = previousHistory(file);
    result.comparison = compareRuns(prevFile ? { ...readJson(prevFile), file: prevFile } : null, result);
    saveHistory({ ...result, file });
    console.log(`Rewrote ${file} and its .md`);
}

function detach(opts, runId, runDir) {
    const { spawnDetached } = evalsRequire(opts)("./exec.js");
    const args = process.argv.slice(2).filter(a => a !== "--detach").concat("--run-id", runId, "--console-is-log");
    const pid = spawnDetached(fileURLToPath(import.meta.url), args, { cwd: HERE, logFile: path.join(runDir, "run.log") });
    writeJson(path.join(runDir, "status.json"), { pid, state: "running", startedAt: new Date().toISOString(), current: null, done: [] });
    console.log(`Run ${runId} started detached (pid ${pid}).\nLog: ${path.join(runDir, "run.log")}\nCheck it with: node run.mjs status ${runId}`);
}

function status(opts) {
    if (!fs.existsSync(RUNS)) return console.log("No runs yet.");
    // The most recently updated run, not the last name: run ids needn't sort by time.
    const runId = opts.positional[0] ?? fs.readdirSync(RUNS).filter(d => fs.existsSync(path.join(RUNS, d, "status.json")))
        .sort((a, b) => fs.statSync(path.join(RUNS, a, "status.json")).mtimeMs - fs.statSync(path.join(RUNS, b, "status.json")).mtimeMs).pop();
    if (!runId) return console.log("No runs yet.");
    const s = readJson(path.join(RUNS, runId, "status.json"));
    let alive = false;
    try { process.kill(s.pid, 0); alive = true; } catch { /* exited */ }
    const state = s.state === "running" && !alive ? "dead (its process exited without finishing)" : s.state;
    console.log(`${runId}: ${state}${s.current ? `, on ${s.current}` : ""}${s.error ? `\n${s.error}` : ""}`);
    for (const d of s.done) console.log(`  ${d.case.padEnd(14)} gate ${d.gate}  ${d.outcome}`);
}

function prepare(opts) {
    const [seed, folder] = opts.positional;
    if (!seed || !folder) throw new Error("usage: node run.mjs prepare <seed> <folder>   (seed: baseline, or plants joined by +)");
    const dest = path.resolve(folder);
    buildSeed(seed === "baseline" ? [] : seed.split("+"), dest);
    console.log(`Prepared ${seed} in ${dest}. Make the changes, then: node run.mjs plant ${dest} <name>`);
}

function plant(opts) {
    const [folder, name] = opts.positional;
    if (!folder || !name) throw new Error("usage: node run.mjs plant <prepared folder> <plant name>");
    const w = workingPatch(path.resolve(folder));
    if (!w.patch.trim()) throw new Error(`${folder} has no changes since its last commit`);
    fs.mkdirSync(PLANTS, { recursive: true });
    fs.writeFileSync(path.join(PLANTS, `${name}.patch`), w.patch);
    console.log(`${w.stat}\nWrote plants/${name}.patch`);
}

/** Makes a model plant: runs plants/_setup/<plant> (one designer change, saved), checks only allowed files changed, and keeps the diff. */
async function capture(opts) {
    const [name] = opts.positional;
    if (!name) throw new Error("usage: node run.mjs capture <plant>");
    const setupDir = path.join(PLANTS, "_setup", name);
    const setup = readJson(path.join(setupDir, "case.json"));
    preflight(opts);
    const runId = opts.runId ?? `bf-capture-${slug(name)}-${stamp()}`;
    const runDir = path.join(RUNS, runId);
    fs.mkdirSync(runDir, { recursive: true });
    const S = SUITE.settings;
    const seed = seedNameOf(setup.plants ?? []);
    const seedId = `${PREFIX}${slug(seed)}-capture`;
    const seedDir = path.join(opts.evals, "cases", "_seeds", seedId);
    const evalCaseName = `${PREFIX}capture-${slug(name)}`;
    const evalCaseDir = path.join(opts.evals, "cases", evalCaseName);
    try {
        buildSeed(setup.plants ?? [], seedDir);
        writeJson(path.join(evalCaseDir, "case.json"), {
            prompt: `${setup.prompt}\n\n${S.capturePromptSuffix}`,
            model: setup.model ?? S.model,
            agent: setup.agent ?? S.agent,
            isolateClaudeConfig: true,
            timeoutMs: setup.timeoutMs ?? 1_200_000,
            seed: seedId,
            checks: { history: { expectCalls: [{ tool: "run_designer_script", onlySuccessful: true }], forbidCalls: [{ tools: ["run_software_factory", ...CODE_WRITE_TOOLS] }] } },
        });
        const code = await runEval({ ...opts, repeat: 1, judge: false }, evalCaseName, runId, path.join(runDir, "run.log"));
        const outDir = path.join(opts.evals, "results", runId, "current-acp-1");
        const m = fs.existsSync(path.join(outDir, "metrics.json")) ? readJson(path.join(outDir, "metrics.json")) : null;
        if (!m || m.error) throw new Error(`the capture run failed (exit ${code}): ${m?.error ?? "no metrics.json"}`);
        const failed = (m.checks?.history ?? []).filter(h => !h.pass);
        if (failed.length) throw new Error(`the capture run broke its rules: ${failed.map(h => `${h.kind} ${h.detail}`).join("; ")}`);
        const w = workingPatch(m.solutionDir);
        const allow = (setup.allow ?? ["intent/**/Intent.Metadata/**"]).map(globToRegExp);
        const paths = w.names.map(n => n.split("\t").pop());
        const outside = paths.filter(p => !allow.some(re => re.test(p)));
        if (!paths.length) throw new Error("the capture run changed nothing");
        if (outside.length) throw new Error(`the capture run changed files outside ${JSON.stringify(setup.allow ?? ["intent/**/Intent.Metadata/**"])}:\n${outside.join("\n")}`);
        fs.writeFileSync(path.join(PLANTS, `${name}.patch`), w.patch);
        console.log(`${w.stat}\nWrote plants/${name}.patch`);
    } finally {
        if (!opts.keep) for (const d of [evalCaseDir, seedDir]) fs.rmSync(d, { recursive: true, force: true, maxRetries: 5, retryDelay: 500 });
    }
}

function compareCommand(opts) {
    const files = fs.existsSync(HISTORY) ? fs.readdirSync(HISTORY).filter(f => f.endsWith(".json")).sort() : [];
    const curFile = opts.positional[0] ? path.resolve(opts.positional[0]) : files.length ? path.join(HISTORY, files.at(-1)) : null;
    if (!curFile) throw new Error("No history yet.");
    const prevFile = opts.positional[1] ? path.resolve(opts.positional[1]) : previousHistory(curFile);
    const cur = readJson(curFile);
    cur.comparison = compareRuns(prevFile ? { ...readJson(prevFile), file: prevFile } : null, cur);
    if (!opts.positional[1]) saveHistory({ ...cur, file: curFile });
    const k = cur.comparison;
    console.log(`${path.basename(curFile)} against ${prevFile ? path.basename(prevFile) : "nothing"}`);
    console.log(`Regressions:\n${k.regressions.map(r => `  ${r.case}: ${r.what}`).join("\n") || "  none"}`);
    console.log(`Improvements:\n${k.improvements.map(r => `  ${r.case}: ${r.what}`).join("\n") || "  none"}`);
}

/** Prints each Software Factory run in a chat history with the drift it reported, as the net attribution reads it. */
function driftCommand(opts) {
    const [file] = opts.positional;
    if (!file) throw new Error("usage: node run.mjs drift <conversation .history.json>");
    const calls = callsOf(readJson(file));
    const designerWrites = designerWritesOf(calls);
    for (const r of softwareFactoryRuns(calls)) {
        const modelled = designerWrites.filter(i => i < r.callIndex).length;
        console.log(`call #${r.callIndex} (turn ${r.turn}): ${r.total === null ? "did not reach staging" : `${r.total} drift ${JSON.stringify(r.drift)}`}, after ${modelled} designer script(s)`);
    }
}

function list() {
    for (const [lane, entries] of Object.entries(SUITE.lanes)) {
        console.log(`${lane}:`);
        for (const e of entries) {
            const p = parseEntry(e), c = loadCase(p.id);
            console.log(`  ${e.padEnd(14)} ${seedNameOf(plantsOf(c, p)).padEnd(14)} ${c.title ?? ""}`);
        }
    }
    for (const [name, entries] of Object.entries(SUITE.suites)) console.log(`suite ${name}: ${expandEntries(entries).length} case(s)`);
}

const commands = { run, prepare, plant, capture, compare: compareCommand, rescore, status, list, drift: driftCommand };
Promise.resolve().then(() => {
    const opts = parseArgs(process.argv.slice(2));
    return commands[opts.command](opts);
}).catch(e => {
    console.error(e.message);
    process.exit(1);
});
