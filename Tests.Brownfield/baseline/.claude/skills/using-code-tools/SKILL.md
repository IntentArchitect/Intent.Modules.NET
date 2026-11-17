---
description: How to search, read, and edit source files with Intent Architect's code tools — grep, glob, read_file, write_file, patch_file, delete_code_file, and import_code. Load before a search→read→edit pass over the codebase, when choosing between patch_file and write_file, or when importing existing C# into a designer.
short-description: Search & read & edit source files and import existing code.
intent-updates: automatic
requiredTools:
  - grep
  - glob
  - read_file
  - write_file
  - patch_file
  - delete_code_file
  - import_code
group: Code and generation
---

# Using the Code Tools

> **Not available over plain MCP.** An external MCP client has none of these tools; use your host's own file tools there.

These tools operate on the **source files** of an application in the open solution. The **model is the source of truth** — use them to understand implementation logic, runtime behaviour, and existing patterns, and to write the bespoke code the Software Factory can't generate. **Never** use them to infer model structure, hierarchy, naming, or file paths — that comes from the designers.

They are for **bespoke** logic only. During the Software-Factory / generation phase, do NOT use them to search, read, or patch **generated** output to verify what the SF did — inspect that with `get_file_diffs` instead (see the **generating-and-applying-code** skill). If generated code is wrong or won't compile, fix the model and re-run the SF; don't hand-edit generated files. Grepping/reading generated source to "check" generation is the biggest source of wasted turns — skip it.

All file paths are **ABSOLUTE** and must sit inside the open workspace — usually inside an application, but a file the workspace contains outside every application (a repo-root README, a build script, a docs folder, or *any* file when the workspace was opened as a plain folder with no solution in it) is editable too. Such a file has no Software Factory that could stage it, so a write/patch/delete of one always lands straight on disk; the user's own source control is the review surface. Get real paths from `getElementDetails()`, `glob`, `grep`, or a prior `read_file` — never invent one.

## Find → read → edit

**Search first, read narrowly, then edit surgically.**

**Both tools show you version-controlled source by default.** `glob` finds files by name, `grep` by content; both honour the enclosing repository's `.gitignore` / `.ignore` / `.rgignore` exactly as ripgrep does, so build output, package caches and generated artifacts stay out of results. `glob` reports what it left out and takes `includeIgnored: true` to include it — so if a file you expect is missing, read the skip line rather than concluding it doesn't exist. `Intent.Metadata` and VCS metadata are never listed by either tool. Neither returns directory NAMES: use the shell (`bash` / `powershell` — `ls`, `Get-ChildItem`) when you need directories, an empty folder, or per-entry size and last-modified.

### `glob` — find files by name/path pattern

- Glob syntax: `**/*.cs`, `src/**/*.ts`, `**/*Service*.cs`.
- `path` must be an ABSOLUTE directory (an application's output directory searches the whole app).
- Every pattern recurses, matching ripgrep: one with no `/` matches the FILE NAME at any depth (`*.cs` finds every `.cs` in the tree), while one containing `/` or `**` is matched against the path relative to `path` (`src/**/*.ts`).
- Returns paths sorted by last-modified (most recent first), so the `headLimit` cap keeps recently-touched matches. There is no size cap — glob reports names, not contents.
- Ignored paths are excluded and **reported**: a trailing `Skipped N directories (…) and M files.` line names what was left out. Counts are of excluded *entries* — a pruned directory counts once, however many files sit inside it.
- `includeIgnored: true` returns ignored files too — use it to find generated output, something under `bin/`/`obj/`, or a file the skip line suggests is being hidden.
- Use it to locate files by name/extension/path; use `grep` for contents.

### `grep` — search file contents by regex

- Full .NET regex (e.g. `log.*Error`, `class\s+\w+`).
- `path` must be ABSOLUTE (a file or directory).
- Targeting a single file defaults to `content` mode (matching lines + line numbers); a directory defaults to `files_with_matches`.
- Filter with `glob` (e.g. `*.cs`, `**/*.tsx`) or `type` (extension, e.g. `cs`, `ts`). Output modes: `files_with_matches`, `content`, `count`. Use `afterContext`/`beforeContext`/`context` in content mode. Case-insensitive by default (`caseInsensitive: false` for exact case). `headLimit` caps output (default 250).
- Ignored paths are skipped: whatever the repository's ignore files exclude, plus VCS metadata and `Intent.Metadata`. Outside a git repository there is no ignore stack, so build and package directories are excluded by name instead.

### `read_file` — read a source file

- Always pass the ABSOLUTE path (from `getElementDetails()`, `glob`, `grep`, or a prior `read_file`).
- Read only the lines you need via `startLine`/`endLine` — target the relevant range (e.g. around a `grep` hit) rather than the whole file. Reads cap at 2000 lines; the summary reports the shown range and where to continue.
- A **staged-change note** in the summary means the content isn't on disk yet (a run held back by a destructive change) — see **generating-and-applying-code**.

## Editing: `patch_file` vs `write_file`

**Prefer `patch_file` for partial edits; use `write_file` only to create a file or fully replace one.** Rule of thumb: `patch_file` when changing less than ~30% of a file's lines. Read the file first if you intend to modify rather than replace.

**Batch writes to different files.** Emit every independent `write_file` / `patch_file` / `delete_code_file` for **different** files in ONE response — each extra response costs a full turn. Never put two calls touching the same file (a read included), or a call that depends on another's result, in the same response.

**What these three tools may never write.** `.intent` folders (the Software Factory's runtime state), `Intent.Metadata` folders (the designer model store) and `*.application.config` are owned by the designers and the Software Factory — a write there is refused, and the fix is to change the model and re-run the Software Factory. A spec's `spec.yaml` and `requirements.json` are likewise off-limits: `write_spec`, `advance_spec_phase`, `record_spec_*` and `complete_spec_task` maintain them (requirement hashes, phase state), and a hand edit desyncs the spec from the Specs panel. Writing an `.mdx` through these tools compiles it and folds the verdict into the feedback — see the **authoring-rich-documents** skill for the block set.

### `patch_file` — surgical search-and-replace

- Replaces one specific block without rewriting the file.
- `searchBlock` is matched with **fuzzy-normalized** comparison (leading/trailing per-line whitespace ignored during search; the file's original formatting is preserved in the output). It must be **at least 3 lines** long to anchor uniquely.
- `replaceBlock` indentation is auto-adjusted to the matched block — do **not** hand-adjust indentation.
- If `searchBlock` appears more than once you get an `ambiguous_match` error listing occurrence line numbers — re-call with the 0-based `occurrenceIndex`. **Never** pass `occurrenceIndex` without first receiving `ambiguous_match` — let the tool enforce uniqueness.
- If not found you get a `no_match` error with the first failing line and nearest actual content — correct and retry once before falling back to `read_file`.
- `searchBlock`/`replaceBlock` are raw source only (no markdown fences/backticks).

### `write_file` — create or fully overwrite

- Use only when creating from scratch or replacing an entire file; it's more expensive than `patch_file` because it rewrites everything.
- `content` is the exact file body only — no markdown fences, backticks, or commentary.
- `createDirectories: true` creates missing parent directories.
- Be explicit in your `intention` about whether you're creating or replacing.
- For C#/TypeScript/JavaScript in this repo, use **4 spaces** per indent, never tabs; match existing indentation and formatting when updating.

### `delete_code_file` — stage a file deletion

- Deletes source files, tracked through the virtual codebase. ABSOLUTE path, inside an app in this solution.
- Double-check the path first; be explicit in `intention` about why.

## Staging vs write-through

`write_file` / `patch_file` / `delete_code_file` write to disk in either mode. The one exception: in Staging mode, a file that already has a staged Software Factory change pending gets your edit staged into it, so reads and a later apply stay consistent (see the **generating-and-applying-code** skill).

## `import_code` — import existing C# contracts into a designer

Non-destructive: `import_code` parses existing C# and populates a designer's model (Domain entities, Services DTOs/Commands/Queries, …); it never edits code.

- `sourceRoot` is REQUIRED — the ABSOLUTE root of the codebase to import; `includeGlobs` are relative to it. The codebase is usually EXTERNAL (a brownfield repo). To re-import the app's own output, pass its output path explicitly.
- **First** call `getDesignerSchema()` for the TARGET designer to learn its exact element specialization names (e.g. `Class`, `DTO`, `Command`, `Enum`, `Attribute`, `Operation`, `Parameter`, `Generalization`) and package names. Use those exact names in `mappings` and `package`.
- Work **ONE** designer/area per call. Survey within `sourceRoot` (e.g. with `glob`) and partition the codebase into coherent scoped calls: each call's `includeGlobs` selects files of ONE architectural role; `mappings` (keyed by C# kind — class/record/interface/struct/enum) says how each maps, naming the element `specialization` plus optionally how `property`/`method`/`parameter`/`constructor`/`generalization`/`literal` constructs map (omit one to skip it). Example (Domain): `sourceRoot: "C:/src/MyApp.Server"`, `includeGlobs: ["**/Domain/**/*.cs"]`, `mappings: { class: { specialization: "Class", property: "Attribute", generalization: "Generalization" }, enum: { specialization: "Enum", literal: "Literal" } }`.
- Be **selective and accurate** — this categorization is the part only you can get right. One coherent role per call; never blanket-glob (`*.cs`) or lump unrelated folders. Use `excludeGlobs` to drop unwanted files. When unsure whether a type belongs, leave it out — import is additive, so refine the rules and re-run.
- By default the on-disk folder structure (relative to `sourceRoot`) is recreated as designer folders. Pass `targetFolder` to nest under a base folder, or `folderStructure: "flat"` for no folders.
- The tool is deterministic about everything except your categorization (it extracts members, resolves type references, decides attribute-vs-association from the designer's rules, derives cardinality/nullability from the C#). Enum literals are extracted automatically. Set `generalization` to model inheritance; an unimported base is deferred and wired on a later re-import.
- Import is **idempotent** on each type's fully-qualified name (re-running updates, never duplicates). A reference to a not-yet-imported type is wired to a placeholder under "Unresolved Imports" and reported in `placeholders`; import those types and re-run to reconcile. Only `stillUnresolved` names need manual mapping.
- After importing, check the designer for validation errors and review the created elements.
