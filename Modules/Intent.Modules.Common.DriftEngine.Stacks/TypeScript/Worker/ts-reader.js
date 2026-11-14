// The TypeScript language's worker: TypeScript's own parser (typescript.js, beside this file), run by Node.js. It parses each
// file of a request, syntax only, and writes each one's tree as JSON for the TypeScript reader (TypeScriptReader.cs).
//
//   node ts-reader.js <request.json> <response.json>
//
// Request:  { "files": [{ "path": "src/api/client.ts", "text": "...", "kind": "ts" }] }, kind one of ts, tsx, js or jsx.
// Response: { "version": "6.0.2", "kinds": ["Unknown", ...], "files": [{ "path", "tree", "tokens", "diagnostics" }] },
//           or { "error": "..." } when the request couldn't be read.
//   kinds:  each SyntaxKind's name, by its number.
//   tree:   a node is { k: kind, s: start, e: end, t: text, o: operator kind, d: declared as, b: [true flags],
//           c: { property: node or [node] } }; t, o, d, b and c only when it has them. Positions are UTF-16 offsets.
//   tokens: [start, end, start, end, ...]: every token, without comments or whitespace, for canonical text.
//   diagnostics: [{ s: start, m: message }]: what the parser couldn't read.
'use strict';
const fs = require('fs');
const path = require('path');
const ts = require(path.join(__dirname, 'typescript.js'));

const kinds = [];
for (const name of Object.keys(ts.SyntaxKind)) {
    const value = ts.SyntaxKind[name];
    // First* and Last* name ranges, not kinds.
    if (typeof value === 'number' && !/^(First|Last)/.test(name) && kinds[value] === undefined) {
        kinds[value] = name;
    }
}

const scriptKinds = { ts: ts.ScriptKind.TS, tsx: ts.ScriptKind.TSX, js: ts.ScriptKind.JS, jsx: ts.ScriptKind.JSX };

// What a node holds besides its syntax: the binder's and checker's state, positions, and what the tree already says.
const skipped = new Set(['parent', 'original', 'symbol', 'localSymbol', 'flowNode', 'emitNode', 'locals', 'nextContainer',
    'endFlowNode', 'returnFlowNode', 'jsDoc', 'jsDocCache', 'id', 'modifierFlagsCache', 'transformFlags', 'pos', 'end',
    'kind', 'flags', 'text', 'escapedText', 'rawText', 'operator', 'keywordToken', 'token']);

function read(file) {
    const source = ts.createSourceFile(file.path, file.text,
        { languageVersion: ts.ScriptTarget.Latest, jsDocParsingMode: ts.JSDocParsingMode.ParseNone }, true, scriptKinds[file.kind] ?? ts.ScriptKind.TS);

    function node(n) {
        const o = { k: n.kind, s: n.getStart(source), e: n.end };
        if (ts.isIdentifier(n) || ts.isPrivateIdentifier(n)) {
            o.t = ts.idText(n);
        } else if (n.kind !== ts.SyntaxKind.SourceFile && typeof n.text === 'string') {
            // A literal's or template part's value, its escapes resolved.
            o.t = n.text;
        }

        // A prefix or postfix operator, keyof or readonly, import.meta's keyword, extends or implements.
        const operator = n.operator ?? n.keywordToken ?? (ts.isHeritageClause(n) ? n.token : undefined);
        if (typeof operator === 'number') {
            o.o = operator;
        }

        if (ts.isVariableDeclarationList(n)) {
            const f = n.flags;
            o.d = (f & ts.NodeFlags.AwaitUsing) === ts.NodeFlags.AwaitUsing ? 'await using'
                : f & ts.NodeFlags.Using ? 'using' : f & ts.NodeFlags.Const ? 'const' : f & ts.NodeFlags.Let ? 'let' : 'var';
        } else if (ts.isModuleDeclaration(n)) {
            o.d = n.flags & ts.NodeFlags.GlobalAugmentation ? 'global' : n.flags & ts.NodeFlags.Namespace ? 'namespace' : 'module';
        }

        if (n.kind === ts.SyntaxKind.SourceFile) {
            o.c = { statements: n.statements.map(node) };
            return o;
        }

        for (const key in n) {
            if (skipped.has(key)) {
                continue;
            }

            const v = n[key];
            if (v === true) {
                (o.b || (o.b = [])).push(key);
            } else if (v && typeof v === 'object' && typeof v.pos === 'number') {
                if (Array.isArray(v)) {
                    (o.c || (o.c = {}))[key] = v.map(node);
                } else if (typeof v.kind === 'number') {
                    (o.c || (o.c = {}))[key] = node(v);
                }
            }
        }

        return o;
    }

    const tokens = [];
    (function leaves(n) {
        const children = n.getChildren(source);
        if (children.length === 0) {
            const start = n.getStart(source);
            if (n.kind !== ts.SyntaxKind.EndOfFileToken && n.end > start) {
                tokens.push(start, n.end);
            }

            return;
        }

        for (const child of children) {
            leaves(child);
        }
    })(source);

    return {
        path: file.path,
        tree: node(source),
        tokens,
        diagnostics: source.parseDiagnostics.map(x => ({ s: x.start, m: ts.flattenDiagnosticMessageText(x.messageText, '\n') }))
    };
}

// The request and response are the last two arguments: Intent Architect's Node.js (Electron run as Node.js) passes its own
// before them (<exe> <script> --node <script> <request> <response>).
const [requestPath, responsePath] = process.argv.slice(-2);
try {
    const request = JSON.parse(fs.readFileSync(requestPath, 'utf8'));
    const files = request.files.map(file => {
        try {
            return read(file);
        } catch (e) {
            return { path: file.path, error: String(e && e.stack || e) };
        }
    });
    fs.writeFileSync(responsePath, JSON.stringify({ version: ts.version, kinds, files }));
} catch (e) {
    // The response says why, as a host may not pass on what is written to the error output.
    fs.writeFileSync(responsePath, JSON.stringify({ error: String(e && e.stack || e) }));
    process.exitCode = 1;
}

// Intent Architect's Node.js (Electron run as Node.js) doesn't end the process when the script ends.
process.exit(process.exitCode ?? 0);
