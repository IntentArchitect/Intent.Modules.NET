// Re-applies a code plant's edit list to a prepared seed, for re-making the plant after a baseline refresh.
// An edit is one of: { file, find, replace, all? } (`find` must occur exactly once, or at least once with `all`),
// { file, create } (writes a new file), { file, delete: true }, or { file, renameTo } (moves a file).
// Usage: node plants/_code/edit.mjs <prepared folder> plants/_code/<plant>.json
import fs from "node:fs";
import path from "node:path";

const [dir, editsFile] = process.argv.slice(2);
if (!dir || !editsFile) {
    console.error("usage: node plants/_code/edit.mjs <prepared folder> <edits.json>");
    process.exit(2);
}
let failed = 0;
for (const e of JSON.parse(fs.readFileSync(editsFile, "utf8"))) {
    const file = path.join(dir, e.file);
    const done = what => console.log(`${what} ${e.file}${e.note ? ` (${e.note})` : ""}`);
    if (e.create !== undefined) {
        fs.mkdirSync(path.dirname(file), { recursive: true });
        fs.writeFileSync(file, e.create);
        done("created");
        continue;
    }
    if (e.delete) {
        fs.rmSync(file, { recursive: true });
        done("deleted");
        continue;
    }
    if (e.renameTo) {
        fs.mkdirSync(path.dirname(path.join(dir, e.renameTo)), { recursive: true });
        fs.renameSync(file, path.join(dir, e.renameTo));
        done(`renamed to ${e.renameTo}:`);
        continue;
    }
    const text = fs.readFileSync(file, "utf8");
    // The edit lists use \n; some eShop files are CRLF, so follow the file.
    const eol = text.includes("\r\n") ? "\r\n" : "\n";
    const find = e.find.replace(/\n/g, eol), replace = e.replace.replace(/\n/g, eol);
    const count = text.split(find).length - 1;
    if (e.all ? count < 1 : count !== 1) {
        console.error(`${e.file}: expected ${e.all ? "at least one match" : "one match"}, found ${count}: ${JSON.stringify(e.find).slice(0, 160)}`);
        failed++;
        continue;
    }
    fs.writeFileSync(file, text.split(find).join(replace));
    done("edited");
}
process.exit(failed ? 1 : 0);
