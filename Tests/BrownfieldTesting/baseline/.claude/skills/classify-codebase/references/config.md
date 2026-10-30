# The classify config

One JSON file per application, kept at `{solutionFolder}/file-classifications/{Application}.classify.json`. It is the source of truth for that application's classifications. The scripts read it, and you change the map by editing it and re-running them, never by editing rows in the designer.

Two companions live beside it:

- **`{Application}.applied.js`** is a copy of the `designer-data.js` that was last applied. `check-drift.designer.js` compares the model with it, so an update notices edits made in the designer since.
- **`{Application}.mdx`** is the review document.

## Shape

```json
{
  "application": "Sales.Api",
  "appRoot": "C:/src/sales",
  "title": "Sales.Api file classifications",
  "classifications": [
    { "name": "Refunds & Payouts", "color": "#C62828", "question": "Can money leave the business twice, to the wrong account, or for more than was paid?" }
  ],
  "plan": [
    {
      "classification": "Refunds & Payouts",
      "importance": "high",
      "why": "RefundHandler trusts the amount in the request, so a client can refund more than it paid (RefundHandler.cs:42).",
      "globs": ["src/Sales.Api/Refunds/RefundHandler.cs", "src/Sales.Api/Payouts/**"]
    },
    { "classification": "Refunds & Payouts", "importance": "medium", "globs": ["src/Sales.Api/Refunds/**"] }
  ],
  "hosts": [
    { "element": "root", "dir": "", "kind": "custom" },
    { "element": "root/Sales/src/Sales.Api", "dir": "src/Sales.Api", "kind": "custom" },
    { "element": "root/Sales/src/Sales.Api/Intent.AspNetCore.Controllers.Controller", "dir": "src/Sales.Api", "kind": "template", "templateId": "Intent.AspNetCore.Controllers.Controller" }
  ],
  "templateFiles": { "docs/agents/refunds.md": "Intent.Some.Module.RefundsGuide" },
  "outOfScope": ["intent/**"],
  "defects": [
    { "title": "Refunds aren't capped.", "detail": "RefundHandler trusts the amount in the request.", "where": "src/Sales.Api/Refunds/RefundHandler.cs:42" }
  ],
  "decisions": [
    "High means a developer must review an AI's change to the file, rather than leaving it to AI review.",
    "The user asked for the nightly payout batch to be covered, and for generated API clients to be left unclassified."
  ],
  "notes": [
    { "tone": "info", "title": "What High means here", "body": "A developer must review an AI's change to these files rather than leave it to AI review." }
  ],
  "history": [
    { "date": "2026-03-02", "commit": "4f1c2ab", "change": "Initial classification: 9 classifications." }
  ]
}
```

After an update that renamed "Refunds" to "Refunds & Payouts", merged "Payouts" into it, and dropped "Legacy Export", the same config also has:

```json
{
  "classifications": [
    { "name": "Refunds & Payouts", "formerNames": ["Refunds", "Payouts"], "color": "#C62828", "question": "…" }
  ],
  "retiredClassifications": ["Legacy Export"],
  "history": [
    { "date": "2026-03-02", "commit": "4f1c2ab", "change": "Initial classification: 9 classifications." },
    { "date": "2026-05-19", "commit": "9be07d1", "change": "Merged Refunds and Payouts; retired Legacy Export." }
  ]
}
```

## Fields

| Field               | Meaning                                                                                                                                                                                                                                                                        |
| ------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------ |
| `application`       | The Intent application's name. It is used for the live model chips in the MDX.                                                                                                                                                                                                 |
| `appRoot`           | The application's output location: `outputLocation` from `getApplicationSettings` via `run_solution_query`. All `plan` globs and `defects` paths are relative to it.                                                                                                           |
| `title`             | Optional heading for the MDX.                                                                                                                                                                                                                                                  |
| `classifications[]` | `name` (shown on the pill; matched case-insensitively against existing File Classifications, so existing ones are reused, which is why two names differing only in case are rejected; no pipe character or line break), `color` (six-digit hex such as `#C62828`), `question` (what it asks a reviewer).                                                                                                |
| `plan[]`            | One entry per classification and importance. `importance` is `none`, `low`, `medium` or `high`. `globs` are relative to `appRoot` and follow the glob rules below. Every entry needs at least one line that includes files. `why` is optional and shown under the classification in the MDX; write one for every High entry.            |
| `hosts[]`           | The Codebase Structure elements that can carry entries (see below).                                                                                                                                                                                                            |
| `templateFiles`     | Optional. Maps a generated file (relative to `appRoot`) to its template id. Only needed when the file has no template-id attribute _and_ its template has a Template Output, as with static content, markdown or JSON written by a Template Output.                            |
| `defects[]`         | Optional. Real bugs or security holes the scan found: `title`, `detail` (one line) and `where`. `where` is a path relative to `appRoot`, `path:line`, `path:line-endLine`, or a list of them (`loadConfig` rejects the old field name `path`). They get their own section in the MDX, with file links, so the chat report doesn't have to list them. |
| `outOfScope`        | Optional. Globs, relative to `appRoot` and following the glob rules below, for files the map deliberately leaves out, such as the Intent solution's own metadata (`intent/**`) when it sits inside the output folder. They are left out of every count and of the unclassified list, and the document mentions them in one line. The plan shouldn't classify them; `generate.js` reports any it does as `plannedButOutOfScope`. |
| `notes[]`           | Optional callouts appended to the MDX: `tone` (`info`, `warning`, `risk`, `success`), `title` and `body`. Use them for gaps and decisions the reader should know about.                                                                                                        |
| `classifications[].formerNames` | Optional. Earlier names of this classification. The apply takes over the first existing element it finds among `name` and these, renames it, and keeps its id. Any other former names still in the model were merged in, so they are deleted. Keep the list; it is harmless once the old elements are gone. |
| `decisions[]`       | Optional. The user's answers that shaped the map, one line each: what High means, areas they asked to cover or ignore, merges or splits they chose. Updates and redos ask about them again, and the MDX lists them. |
| `retiredClassifications` | Optional. Names of dropped classifications for the apply to delete. Leave them listed; a name that no longer exists is skipped. A name can't also be a current classification or a former name. |
| `history[]`         | One entry per applied run: `date`, `commit` (short hash of `HEAD` in the output folder when applied) and `change` (one line). `generate.js` reports the files changed since the last entry's commit, and the MDX ends with the history as a table. |

## Glob rules

These are Intent's own matcher rules, verified against it, and `lib.js` reproduces them. They differ from `.gitignore` rules.

- A pattern is anchored to the folder it's evaluated from. That is `appRoot` for plan globs, and the host's `dir` for the rows the generator writes. `README.md` and `*.json` match the top level only. Use `**/README.md` for every depth, and `src/**` for everything under `src`.
- `*` and `?` stay within one folder, and matching ignores case.
- `!` excludes, `#` is a comment, and within one entry the last matching line wins.
- A leading `/`, braces `{a,b}` and classes `[ab]` don't work in Intent, and `loadConfig` rejects them.

`plan` entries overlap freely, so write them by theme as a reviewer would. A catch-all row keeps its entry's `!` carve-outs, so a carved-out folder stays carved out for files added there later. A file gets every theme whose entry matches it, at the highest importance among them, and `generate.js` works out the one row it actually lands on. Because of that, a lower-importance entry can't lower a file that a broader entry already covers. To carve a file out, add `!path` to the broader entry.

## Hosts

A host is an element in the Codebase Structure designer that carries a `File Classification` stereotype.

- **`kind: "custom"`**: the Root Folder (`dir: ""`, element is the root package name, available from module 1.2.0-pre.1), a C# Project or a Folder. It classifies everything under its directory that isn't classified through a Template Output:
  - hand-written files;
  - files the project element writes itself (`.csproj`, `launchSettings.json`);
  - generated files whose template has no Template Output.

  The deepest custom host containing a file owns it. A custom host that gets no rows loses the stereotype in the apply, so the host above it classifies its files, and the generator reassigns them the same way. Solution Folders aren't hosts: their globs are relative to each contained project, which generated rows can't express, and the apply refuses one.
- **`kind: "template"`**: a Template Output, which classifies the files its template generates. `templateId` is the Template Output element's name, which is the template id. `dir` is the folder containing the Template Output, relative to `appRoot`. The same template can have Template Outputs in several projects, each with a Registration Filter; list each one with its own `dir`. Each classifies the template's files under its own folder (see below). **List every Template Output,** including ones that will get no rows. The generator needs them all to know which files a Template Output owns, and the apply reports any it finds in the model that aren't listed (`templateOutputsNotInConfig`).
- **`createFolderUnder`**: for a custom host that doesn't exist yet. The apply script creates a Folder under this parent path (the root package name means the Root Folder). Rarely needed once the Root Folder can host entries. Add one only after the user agrees, since it changes the Codebase Structure tree.

To get `element` paths, run `searchElements(".*", { specializations: ["Template Output", "Folder", "C# Project (.NET)"] })` with `run_designer_query` against the Codebase Structure designer, and copy each `path` verbatim. To get `dir`, check where that element's files actually land on disk. A Visual Studio Solution element adds no folder of its own.

## How a file is matched to a host

1. A file is **generated** if `templateFiles` names it, or if its first 16 KB carries Intent's template-id attribute with a real template id. Documentation that merely mentions the attribute doesn't count.
2. A generated file whose template has a Template Output belongs to that Template Output: the deepest one of its template containing it. It stays there even when the Template Output gets no rows; Intent then leaves the file unclassified and never falls back to a folder or to another Template Output of the template. A file outside every Template Output of its template goes to the first one Intent loads that has entries, which matches it on its whole output-relative path. The generator assigns it to the template's first in the config, and when the template has several, gives each of them rows for it, so the result doesn't depend on load order (`filesOutsideTemplateOutputs`).
3. Every other file belongs to the deepest **custom** host containing it that gets rows. That includes hand-written files and generated files whose template has no Template Output element at all.
4. A file with no host at all can't be classified. `generate.js` lists it under `unreachable` when the plan wanted it classified.

## Outputs of generate.js

- `rows.json`: every host with its generated rows (`globs`, `importance`, `classifications`), and `files`, the classification and importance each file resolves to (`outOfScope: true` on excluded files). A Template Output that also carries rows for files outside every Template Output of its template shows `outsideFiles` and `outsideAtRisk`. `gen-mdx.js` reads this.
- `designer-data.js`, for `apply-classifications.designer.js`:
  - `CLASSIFICATIONS`, with any `formerNames`;
  - `HOSTS`, the hosts with at least one row;
  - `CLEAR`, the config hosts with no rows. The apply leaves Template Outputs among them with an empty list, so Change Review won't offer to restore a module default, and removes the stereotype from the rest;
  - `RETIRED`.

  After a clean apply it is copied to `{Application}.applied.js`.
- `changes.json`, only with `--baseline <previous config>`. Both configs are resolved against the same files, and the file lists:
  - the classifications added, renamed, merged, retired or recoloured;
  - `removedNotRetired`, which must be empty;
  - every file whose classifications or importance change;
  - the transitions;
  - `becameHigh` and `noLongerHigh`.

  `gen-mdx.js --changes` renders it.
- On stdout:
  - `wrong`: files whose row result differs from the plan. It must be empty.
  - `unreachable`: files the plan classifies that no host can reach.
  - `deadGlobs`: plan lines that match no file.
  - `generatedViaFolderHosts`: generated files whose template has no Template Output element. This is normal, and listed so you don't mistake it for a gap.
  - `filesOutsideTemplateOutputs`: templates with several Template Outputs that write files outside all of them, with those files. Each of the template's Template Outputs gets rows for them, so the planned result shows whichever one Intent loads first. `atRisk` lists the files that can't be covered that way (a whole path equal to another file's folder-relative path), and should be empty. The list is also saved in `rows.json`, and the MDX warns about any `atRisk` files.
  - `outOfScope`: how many files `outOfScope` excludes. `plannedButOutOfScope` lists any the plan classifies anyway.
  - `hint`: shown when the Intent solution folder sits inside `appRoot` and isn't in `outOfScope`.
  - `highShareOfClassified`
  - rows per host
  - `sinceLastRun`, when `history` has a commit. It gives counts of the files added, renamed, deleted and modified since that commit, and lists the new files that are unclassified. The full lists are in `rows.json`. `untracked` counts the added files that aren't committed. It is approximate: files that were uncommitted when the map was applied count as added. Out-of-scope files are left out.
  - `changes`, the summary of `changes.json`, when `--baseline` is given.
