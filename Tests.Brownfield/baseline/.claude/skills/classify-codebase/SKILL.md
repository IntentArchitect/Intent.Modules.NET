---
description: Discover an application's own review-risk areas from its code (the subsystems, flows, data and partners where a wrong change hurts in this system) and set them up as File Classifications in Intent Architect, so Change Review shows which changes a developer must review and which can be left to AI review. Agrees the classifications and their Review Importance with the user, writes the entries onto the Codebase Structure designer, proves every file resolves as intended, and produces an MDX page to review. Also updates or redoes an existing map (add an area, reclassify a folder, raise or lower importance, rename, split, merge or drop classifications, catch up with new code). Use when the user asks to classify, categorise, tag or risk-map a codebase, set up File Classifications, Review Importance or the Needs Attention filter, or decide which files reviewers should focus on, or runs /classify-codebase, even if they only say "which files matter for review" or "flag the risky parts of this repo".
short-description: "Discover a codebase's own review-risk areas, agree them with the user, and set them up as File Classifications with an MDX page to review."
argument-hint: "[application name] [areas or concerns to cover]"
intent-updates: automatic
requiredTools:
  - get_applications
  - run_solution_query
  - get_designer_rules
  - run_designer_query
  - run_designer_script
  - ask_user_question
  - create_sub_agent
  - write_file
  - patch_file
  - apply_staged_file_changes
  - run_software_factory
  - get_file_diffs
  - bash
group: Review and pull requests
---

# Classify a codebase for review

**Goal.** Reviewers filter Change Review by classification and Review Importance. This skill gives them a map of _this_ system: where a change needs a developer's eyes, where AI review is enough, and why. You end with:

- File Classifications derived from this codebase and agreed with the user;
- entries on the right elements, so that **every file** resolves to the classification and importance you intended, proven by a dry run;
- a Software Factory run that proposes no changes, since classifications are metadata only;
- **an MDX page the user reviews**, showing every classification, the files it covers and the defects the scan found;
- a config file that re-creates all of it, a snapshot of what was applied, and a change history, so the map can be updated later.

**Under an orchestrating skill.** When the skill that loaded this one records a **User decision** to skip questions, ask none: take the option you would recommend, and skip phase 6's spot-check, which only the user can do.

$ARGUMENTS

## Start: new map, update or redo

Decide this before anything else, from who made the existing classifications and what the user asked for.

**Who made them.** The config file is the signal:

- **This skill.** `{solutionFolder}/file-classifications/{Application}.classify.json` exists. The map came from this app's own analysis, and the team agreed it.
- **Something else.** There are classifications or entries in the Codebase Structure designer but no config. They are module defaults, which are generic, or hand edits. Neither is a baseline worth keeping.
- **Nothing.** There are no classifications yet.

**What to do:**

| Existing classifications | The request                                                                 | Do this                                                                                                 |
| ------------------------ | --------------------------------------------------------------------------- | ------------------------------------------------------------------------------------------------------- |
| None                     | anything                                                                    | A new map: phases 1–8.                                                                                  |
| No config                | anything, even "update"                                                     | A new map that replaces them (see below). Say so up front; don't ask. Fold in anything specific the user asked for. |
| This skill's config      | names a change ("add Reporting", "Reports is low risk now")                 | **Updating an existing map.**                                                                           |
| This skill's config      | says redo, start over, or from scratch                                      | **Redoing an existing map.**                                                                            |
| This skill's config      | unclear ("classify the codebase", a bare `/classify-codebase`, "re-run it") | Ask, as below.                                                                                          |

**Replacing entries that came without a config.** They are module defaults or hand edits. The apply overwrites or empties every Template Output in the config, and phase 5 reports the classifications the modules created (such as "Business Logic") as `unused`. Ask whether to retire them, since other applications' modules may match classifications by name. If entries look hand-made rather than module defaults (names specific to this app), list them in phase 3 so the user can say which should carry over.

**Asking.** Use one `ask_user_question`, giving the facts that make the choice easy:

- the number of classifications;
- the date of the last `history` entry;
- how many files changed since, from `sinceLastRun` in a quick `generate.js` run on the existing config.

Offer two options:

- **Update it:** keep the agreed classifications, catch up with the code changed since, and make any changes the user describes.
- **Redo from scratch:** analyse the code afresh. The current classifications are replaced, and the review document shows old against new.

Recommend one and say why. Recommend updating when the map is recent and little has changed, and redoing when the code has moved a lot or the map is old. Don't hard-code thresholds.

**Redoing an existing map.** Run phases 1–8 as for a new map, with four differences:

1. **Analyse blind.** Don't give the scan sub-agents, or yourself while synthesising, the old classification names. Otherwise the redo is an update in disguise.
2. **Carry forward the user's decisions, not the classifications.** In phase 3, ask again about each entry in the old config's `decisions`, such as what High means and areas the user asked to cover or ignore: "Last time you asked us to cover X; is that still wanted?"
3. **Compare with the old map.** Before writing the new config, copy the old one to `<scratch>/baseline.classify.json`. Then pass `--baseline` to `generate.js` and `--changes` to `gen-mdx.js`, so the document opens with what the redo changed.
4. **Retire old classifications.** List every old classification that doesn't come back in `retiredClassifications`. If a new one is clearly the same concept as an old one, list the old name in its `formerNames` instead, so the element and its history carry over.

**There is no default set of classifications.** Each system's are discovered from its code and named in its own vocabulary: its subsystems, flows, data and the partners it talks to. Some will resemble concerns other systems have, and that's fine, but the list comes out of the analysis, never out of a template, an earlier run or another codebase. `references/finding-risk.md` explains how to get from code to themes.

Load the Intent skills as you reach each phase: **scripting-the-model** and **exploring-the-model** before reading the designer, **working-with-modules** if the module needs upgrading, **changing-the-model** before the apply step, **generating-and-applying-code** before the Software Factory run, and **authoring-rich-documents** before producing the MDX. If the host offers `get_designer_rules`, call it for the Codebase Structure designer before the first `run_designer_script`. The scripts below are in this skill's folder (`use_skill` reports it as the base directory) under `scripts/`. They need Node.js 18+ and git.

## How classification resolves

Read this before planning anything. Each point changes what a correct setup looks like.

- **One stereotype, three kinds of host.** The `File Classification` stereotype (`Intent.Modelers.CodebaseStructure` 1.2.0-pre.0 and later) has an `Entries` item list. Each row has `Glob`, `Review Importance` and `Classification`.

  | Element it sits on                  | Files it classifies                                                                                                                                                | Globs are relative to                                                           |
  | ----------------------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------ | ------------------------------------------------------------------------------- |
  | Template Output                     | Files its template generates, including Merge-mode files with hand-written bodies                                                                                  | The folder containing the Template Output                                       |
  | C# Project, Folder                  | Hand-written files under it, files the project element itself writes (`.csproj`, `launchSettings.json`), and generated files whose template has no Template Output | That element's directory                                                        |
  | Root Folder (1.2.0-pre.1 and later) | The same as a folder, for everything under the application's output folder: repository-root files and folders with no project                                      | The output folder                                                               |

- **Templates don't need a Template Output.** Most have one, but some don't, such as a project-level `Program.cs`. Their files are classified by the folder, project or Root Folder entries above them. That is normal, not a gap.
- **A Template Output owns its template's files even with no entries.** If it has no entries, or none match, those files are unclassified. The folder above never takes over, and neither does another Template Output of the same template. The generator models this.
- **A project or folder with no entries hands its files to the element above.** The apply removes the stereotype from a project or folder that gets no rows, so the next project, folder or Root Folder up classifies its files. The generator models this too, so a `!path` carve-out gives the same result whether or not the inner host gets rows.
- **Solution Folders aren't used as hosts.** Their globs are relative to each contained project's root, which generated rows can't express. The projects inside them are hosts themselves, and the apply refuses a Solution Folder.
- **A template can have Template Outputs in several projects.** Each one classifies the template's files under its own folder: Intent picks the deepest Template Output of the template that contains the file. A file outside all of them goes to the first one Intent loads that has entries, which matches it on its whole path. That is rare. `generate.js` then gives every Template Output of the template rows for the file, so the result doesn't depend on load order, and lists those files under `filesOutsideTemplateOutputs`. Only their `atRisk` files, whose whole path equals another file's folder-relative path, can show differently.
- **Modules stamp defaults onto Template Outputs.** Installing a module writes a single `**/*` entry onto each of its Template Outputs and creates any classification it names (such as "Business Logic"). An upgrade re-stamps only Template Outputs still carrying exactly what the previous version wrote, so entries this skill writes survive. A Template Output with _no_ stereotype is different: Change Review offers **Apply missing classifications**, which puts the default back. So the scripts never remove the stereotype from a Template Output; they leave it with an empty list.
- **Only one row classifies a file.** The deepest element containing the file wins, and within it the _last matching row_ wins. Overlapping entries neither merge nor take the highest importance. So the row a file lands on must carry _all_ its themes at its highest importance. `generate.js` builds rows that way and checks every file, which is why you write a theme-based plan and never hand-place rows.
- **Glob rules.** These are Intent's own matcher rules, and the scripts reproduce them exactly. They are _not_ `.gitignore` rules.
  - Every pattern is anchored to the host's folder. `README.md`, `*` and `*.json` match the top level only.
  - Reach deeper with `**`: `**/README.md` matches at every depth, and `src/**` matches everything under `src`. A bare folder name (`src`) does not match its contents.
  - `*` and `?` stay within one folder. Matching ignores case.
  - `!` excludes, `#` starts a comment, and within one row the last matching line wins. A row with no lines matches everything.
  - A **leading `/` matches nothing**, and braces `{a,b}` and classes `[ab]` aren't supported. The scripts reject all three.
- **Row values.** `Review Importance` is the full option text, `0 - None` up to `3 - High`. `Classification` is a list of File Classification element ids. `Entries` is an item list: add rows with `prop.addItem()`, never `setProperty`.
- **The classifications themselves.** They are `File Classification` elements under a `File Classifications` container in the Root Folder package. Each one's colour is set on its `File Classification Settings` stereotype (`Color`).
- **How Change Review shows it.** Classification pills always show. The importance flag shows only where the change touched hand-written code. The agent-side `get_change_review` does _not_ report pills, so the final check has to be the user looking at the Change Review tab.

## Phase 1 — Orient (read only, except an agreed upgrade)

1. **Pick the application.** Use the workspace context or `get_applications`. If there are several, ask which; each application gets its own config. Read its `outputLocation` with `run_solution_query`: `getApplicationSettings({ search: "outputLocation" })`.
2. **Check the module version.** Use `listInstalledModules` for `Intent.Modelers.CodebaseStructure`.
   - **Below 1.2.0-pre.1** (1.2.0-pre.0, or any 1.1.x), recommend upgrading before planning anything, using `ask_user_question` with the upgrade as the recommended option. From that version the Root Folder can carry entries, so repository-root files and folders without a project are covered without extra Folder elements. Check `searchAvailableModules` for the latest version, upgrade with **working-with-modules** if the user agrees, then re-read `getStereotypeDefinitions()`.
   - **1.1.x or earlier** has only the old stereotype (`Custom File Classification` / `Severity`), which can't reach generated files. Don't proceed without the upgrade.
3. **Probe the Root Folder.** Do this even after an upgrade, leaving no trace. Run `run_designer_script` with `const p = lookupPackage("<root package name>"); const had = p.getStereotypes().some(s => (s.getName ? s.getName() : s.name) === "File Classification"); p.ensureStereotype("File Classification"); if (!had) p.removeStereotype("File Classification"); return true;`.
   - If it succeeds, the Root Folder becomes a custom host with `dir: ""`.
   - If it throws, the user declined the upgrade. Repository-root files are then unreachable, and a top-level folder without a project needs a new Folder element (ask first).
4. **Record existing state.**
   - Note any File Classification elements, and keep their names and colours if they still fit.
   - Note every element that already carries entries. Single `**/*` entries on Template Outputs are module defaults, and the apply replaces or empties them; "Start" above covers what to tell the user. Ask before overwriting anything that looks hand-made.
5. **List the candidate hosts.** Use `searchElements` over `Template Output`, `Folder` and `C# Project (.NET)`, plus the Root Folder package. Leave out Solution Folders (see "How classification resolves").
   - For each, note the path, its directory relative to the output folder, and for Template Outputs the template id (the element name) and its Registration Filter.
   - Put **every** Template Output in the config, even ones that may get no rows. The generator uses them to know which templates have one.
6. **Take the file inventory.** Run `git ls-files` in the output folder.
   - A generated C# file carries Intent's template-id attribute near the top; `generate.js` reads it.
   - Generated non-C# files (static content, markdown, JSON) need a `templateFiles` entry only when their template _has_ a Template Output. Without one, they classify through their folder like hand-written files. Module-written markdown often carries a `contentHash:` line in its front matter; that and the Template Output's folder are the usual ways to find these files.
   - **Leave out what no one reviews.** If the Intent solution folder (the one holding the `.isln`) sits inside the output folder, add it to `outOfScope` (for example `intent/**`), with anything else that isn't the team's code, such as `.intent/**` user settings. Out-of-scope files are left out of every count and get one line in the document. `generate.js` hints when the solution folder is missing.
   - Brownfield code adopted into Intent is mostly "generated" with hand-written bodies. Classify those files by what their code does.

## Phase 2 — Discover the risks (read only, critical)

Read `references/finding-risk.md` first: section 2 for the scan lenses, 3 for turning findings into themes, 4 for importance, 5 for evidence.

1. **Learn the system first.** Before scanning for risk, understand what the system is: its services and projects, its core entities and flows, who calls it and what it calls, and how it is deployed. Theme names come from this vocabulary.
2. **Fan out.** Dispatch read-only sub-agents in parallel, using `discovery` via `create_sub_agent` or native Explore agents. Give each a non-overlapping slice, such as a project or a top-level folder, and the scan brief from section 7 of the reference. The brief contains **no theme names**. Ask for findings in the system's own terms, each with a failure mode and `file:line` evidence.
3. **Verify.** A file name is not a risk. Read the code behind every proposed High yourself before you trust a sub-agent's claim.
4. **Collect defects.** A scan like this surfaces real bugs, such as an authorization check that always passes or a committed signing key. They raise the importance of the files involved. Record each in the config's `defects` list (`title`, a one-line `detail`, and `where`: a path, `path:line` or `path:line-endLine`, or a list of them) so it appears in the review document. Don't fix them.
5. **Synthesise the themes** (section 3 of the reference):
   - Cluster the pooled findings by the question a reviewer would ask, and name each cluster after this system's concept.
   - Write each theme's reviewer question in this system's terms.
   - Give each a colour from the reference palette.
   - Run the self-test: if the names could describe any application, regroup and rename before going on.
6. **Draft the plan.** Write entries per theme and importance, each with a `why`; every High entry needs one. Add a Medium catch-all for every High cluster.

## Phase 3 — Engage the requester

The user knows things the code doesn't: audit scopes, the incident last quarter, which integration is fragile, what the team already ignores. Bring them in before anything is written, and keep asking until nothing material is open.

1. Show the draft in chat as one compact table:
   - each theme and the question it answers;
   - _why it exists in this system_, in one line of evidence;
   - two or three example High files;
   - file counts per importance.
2. Then ask with `ask_user_question`, bundled into one call:
   - **The themes.** Are these the right areas for this system? Offer concrete merge or split alternatives drawn from the findings, plus free text to rename, add or drop themes.
   - **Anything else to cover.** A multi-select of concrete areas the scan touched but didn't make into themes (a regulatory scope, a background worker, a deployment pipeline), plus free text for the user's own.
   - **What High means.** Recommend the review-centric definition in section 4 of the reference: a developer must review an AI's change here, rather than leaving it to AI review. Offer stricter and broader readings, each with the number of High files it produces. Don't steer towards a percentage.
   - **Host placement.** Ask this only if the Root Folder can't host entries.
3. If the user names files or areas, find them in the code, decide their theme and importance with evidence, and confirm.
4. If the user rejects the themes, go back to the findings and re-synthesise. Don't patch the names.
5. Fold the answers in. Don't proceed on a guess about the user's priorities. When an answer opens a new question, ask it.
6. Record the answers that shaped the map in the config's `decisions`, one line each: what High means, areas the user asked to cover or ignore, and merges or splits they chose. Updates and redos ask about them again, and the MDX lists them.

## Phase 4 — Write the config and dry-run

1. **Write the config** to `{solutionFolder}/file-classifications/{Application}.classify.json`, where `{solutionFolder}` is the folder holding the `.isln`. The format is in `references/config.md`.
   - Configs usually run to tens of kilobytes. It's fine to build one with a scratch script that writes straight to that path, rather than retyping it through `write_file`.
   - If you use `write_file` and the folder sits inside an application's output, it _stages_ the file. Apply it with `apply_staged_file_changes` before running the scripts, since they read from disk.
   - To lower one file inside a broader entry, add a `!path` line to the broader entry. A separate lower-importance entry can't lower it, because the plan takes every matching theme at its highest importance.
2. **Generate** into a scratch folder (OS temp, `classify-codebase/{Application}`):

   ```bash
   node <skill>/scripts/generate.js <config.json> <scratch>
   ```

   Read the summary before going further:
   - **`wrong` must be empty.** A non-empty list means a host overlaps wrongly. Check the host `dir` values.
   - **`unreachable`.** These are files the plan classifies that no host can reach. Add a host, or accept the gap and record it in `notes`.
   - **`deadGlobs`.** These plan lines match nothing. Remember the glob rules: did a top-level pattern need `**/`?
   - **`generatedViaFolderHosts`.** These are generated files classified through folder entries, because their template has no Template Output. That's normal. Don't report them as gaps.
   - **`filesOutsideTemplateOutputs`.** Usually absent. These are generated files outside every one of their template's several Template Outputs, covered as described under "How classification resolves". Each template's `atRisk` list should be empty; report any files in it as one gap line in phase 8.
   - **`hint` and `plannedButOutOfScope`.** A hint says the Intent solution folder is counted as unclassified: add it to `outOfScope`. `plannedButOutOfScope` lists files the plan classifies but `outOfScope` excludes: drop one or the other.
   - **`highShareOfClassified`.** Check that it matches the definition of High the user chose.
   - **Rows per host.** More than about 25 rows on one host reads badly. Add folder catch-alls to the plan so fewer exact files are needed.
3. Show the user the summary: counts per theme and importance, High share, gaps. Show the rows of the two or three most important hosts from `rows.json` too, then ask whether to apply or adjust. This is the last stop before the model changes.

## Phase 5 — Apply to the designer

Run the bundled designer script with the generated data, as a single `run_designer_script` against the Codebase Structure designer:

- `includedScriptPaths`: `["<scratch>/designer-data.js", "<skill>/scripts/apply-classifications.designer.js"]`
- `script`: `""`

It first checks every host. If it throws `Nothing was changed`, a host is missing, is a Solution Folder, or has a row naming a classification the config doesn't have: fix the config and regenerate. Otherwise it reuses or creates the classifications and sets their colours. It creates any `createFolderUnder` hosts, replaces each host's entries, reads every row back, and returns:

- **`problems`.** These are rows that didn't store as generated, and the list must be empty. A failure here usually means the stereotype or property names differ from the ones this skill expects. Re-read `getStereotypeDefinitions()`.
- **`stale`.** These are elements carrying entries that this run didn't write. Ask the user whether to clear them or keep them.
- **`cleared`.** These are hosts in the config that had entries, from an earlier run or a module default, but get no rows now, so their entries were removed. Template Outputs keep the stereotype with an empty list (`templateOutputsLeftEmpty` counts them), so Change Review won't offer to put the module default back. Other hosts lose the stereotype.
- **`templateOutputsNotInConfig` must be empty.** The generator only knows about Template Outputs listed in `hosts`. For any other Template Output, it assumed the files were classified by folders, but Intent leaves them to that Template Output, which may carry a module default. Add these to `hosts`, then regenerate and re-apply.
- **`deleted` and `kept`.** These are classifications removed by a merge (`formerNames`) or by `retiredClassifications`. A `kept` one is still used by stale entries, and goes on a later apply once those are resolved.
- **`unused`.** These are classifications in the model that the config doesn't account for. Ask whether to add them to `retiredClassifications` or to the config.

Then run `getDesignerValidationErrors()` until it is clean, and save. A trivial follow-up `run_designer_script` with `saveOnSuccess: true` does it.

**Record the run.** The next update depends on both of these:

1. Copy `<scratch>/designer-data.js` to `{solutionFolder}/file-classifications/{Application}.applied.js`. This is the snapshot that the drift check compares the model with.
2. Append a `history` entry to the config. It has `date`, `commit` (`git rev-parse --short HEAD` in the output folder; leave it out if the repository has no commits yet) and `change`, a one-line summary such as "Initial classification: 17 classifications" or "Added Reporting; merged AI into External Integrations".

Record the run again after any re-apply, such as one that fixes a spot-check.

## Phase 6 — Verify

1. **Software Factory.** Run it once. Classifications are metadata, so expect zero changes.
   - If it reports any change, staged or already applied depending on the Software Factory's mode, inspect it with `get_file_diffs`. Typical causes are a module refreshing its own static content, or a drift that was already there. Neither comes from the classifications.
   - Report what it is. Leave unrelated staged changes for the user unless they ask you to apply them.
2. **Spot-check in Change Review.** You can't see the pills, so ask the user to check about four files with `ask_user_question` ("All match" / "Something's off").
   - Pick files that currently have changes in the working tree; only those appear in Change Review. Use `git status`.
   - Cover different hosts: a Template Output file, a project file, a Root Folder file, and a generated file classified through its folder if there is one.
   - If a template has Template Outputs in several projects, include files of it from two of those projects. Each should match `rows.json`, and its **Go to classification setting** should name the Template Output in its own project.
   - Name the expected classifications and importance for each.
   - The file row's **Go to classification setting** shows which element classified it. If something is off, compare it with `rows.json`, fix the config, then regenerate and re-apply.

## Phase 7 — Produce the review document

The page often runs to hundreds of kilobytes, so don't copy it through `write_file`. Generate it in place and let a one-line patch do the checking:

1. Generate it straight to its destination:

   ```bash
   node <skill>/scripts/gen-mdx.js <config.json> <scratch>/rows.json {solutionFolder}/file-classifications/{Application}.mdx
   ```

   It writes:
   - a summary table with each classification as a pill in its own colour, the live model chip, and per-importance counts;
   - one file tree per classification with each file's importance and its other themes, plus the `why` lines;
   - the unclassified files, and one line for the out-of-scope ones;
   - the defects the scan found, with links to the files;
   - the decisions agreed with the user;
   - the config's `notes` as callouts;
   - the change history.

   For an update or a redo, add `--changes <scratch>/changes.json`. The page then opens with **What changed since the previous map**: classifications added, renamed, merged or retired, the importance transitions, and a file tree of every file whose classification changed.
2. **Compile-check it.** The file opens with a three-line `{/* classify-codebase: compile-check stub … */}` comment. Delete exactly those three lines with `patch_file`, passing them as `searchBlock` and an empty `replaceBlock`. That patch compiles the whole document and checks every file path. Fix anything it reports as `WILL NOT RENDER` or as a dead path, using further targeted patches.
3. If the result says the change was staged, because the document is inside an application's output, apply it with `apply_staged_file_changes`.

## Phase 8 — Report

The review document is what the user reads, so the chat report is short and **ends with the document**. Keep it to about ten lines, in this shape:

```markdown
Classified **{Application}**: {N} classifications on {E} elements. {C} of {F} files are classified: **{H} High**, {M} Medium, {L} Low, {X} None.

- {gap, one line each, only if there are any}
- {D} defects found during the scan are listed in the document.

**Review the classifications:** `{path of the .mdx relative to the repository root}`
```

For an update or a redo, the first line says what changed instead, and the last line points to the same document:

```markdown
Updated **{Application}**: {classification changes in one line, e.g. "added Reporting, merged AI into External Integrations"}. {n} files change classification or importance: {b} became High, {l} are no longer High.

**Review the update:** `{path of the .mdx relative to the repository root}`
```

Don't list the defects, the themes or process notes in chat; they are all in the document. Save memory notes silently. To change the map later, run this skill again with the change you want, and it follows "Updating an existing map". Don't hand-edit rows in the designer: the next apply overwrites them, and they can silently change other files' results.

## Updating an existing map

Use this when the config exists and the user wants to change the map rather than start over. They might want to:

- add an area;
- reclassify a folder;
- raise or lower importance;
- rename, split, merge or drop classifications;
- catch up with code added since the last run.

The user's request is the brief, and the config is the agreed baseline.

1. **Orient.**
   - Copy the config to `<scratch>/baseline.classify.json` before changing anything. It is the "before" that the preview and the document compare with.
   - Load the config. Check the module version as in phase 1; from 1.2.0-pre.1 the Root Folder can replace any `createFolderUnder` folders, so offer the upgrade.
   - Re-list the candidate hosts (phase 1, step 5) and reconcile them with `hosts`. Add new projects, folders and Template Outputs, and remove ones that no longer exist.
   - If `loadConfig` rejects the config, it was written by an older version of this skill. Fix it in both the config and the baseline copy: drop a plan line's leading `/` (old versions read those lines from `appRoot`, which is what a plain path means now), and rename a defect's `path` to `where`. The old generator read those lines from `appRoot`, which is what a plain path means now.
2. **Check for edits made in the designer.** Run `run_designer_script` with `includedScriptPaths: ["{solutionFolder}/file-classifications/{Application}.applied.js", "<skill>/scripts/check-drift.designer.js"]` and the script `return checkDrift();`. It changes nothing.
   - `clean: true`: carry on.
   - Otherwise, someone has changed entries or classifications since the last apply, and applying the config would overwrite those changes. Show the differences, then ask whether to fold each one into the config as a plan entry or discard it.
   - No snapshot means the map predates snapshots. Skip the check, and tell the user the apply rewrites the entries on every host in the config.
3. **See what changed in the code.**
   - Run `generate.js` on the baseline copy.
   - `sinceLastRun` lists files added, renamed, deleted and modified since the commit in the last `history` entry, with how each new file classifies today. The full lists are in `rows.json`. Untracked files count as added and are also listed under `untracked`: some may have existed, uncommitted, when the map was applied.
   - `deadGlobs` shows plan lines whose files have gone.
4. **Analyse what the request touches.** Read the code behind the request, and behind the new and heavily modified files, using phase 2's evidence standard:
   - verify every High yourself and write its `why`;
   - add new defects, and remove any the code now shows are fixed.

   Fan out sub-agents only when the request is broad, such as "re-assess everything". In that case, run phase 2 in full and present the result as changes to the existing classifications (keep, rename, split, merge, add, drop), not as a fresh list.
   - **Keep the agreed vocabulary.** Don't rename, regroup or recolour classifications the request doesn't touch. They came out of this system's own analysis and the team agreed them. Churning them breaks the filters reviewers already use. (The rule against predetermined classifications is about other systems' lists, not this map.)
5. **Agree the changes** with `ask_user_question`, as in phase 3. Show only what changes and why. Ask about whatever the request leaves open, such as the importance of a new area, or where files go once a dropped classification no longer covers them.
6. **Edit the config.**
   - **Rename:** change `name` and add the old name to `formerNames`. The apply renames the element in place, so its id survives.
   - **Merge:** keep one classification and point the others' plan entries at it. Remove the others, and list their names in the survivor's `formerNames`. The apply deletes them.
   - **Split:** keep the original under one of the new names (with `formerNames` if it changed), add the other, and divide the plan entries between them.
   - **Drop:** remove the classification and its plan entries, and list its name in `retiredClassifications`.
   - Hosts that end up with no rows stay in `hosts`; the apply clears their old entries.
7. **Preview.** Run `node <skill>/scripts/generate.js <config.json> <scratch> --baseline <scratch>/baseline.classify.json`.
   - Read the phase 4 checks first.
   - Then read `changes`: the classification changes, how many files change, the transitions (such as `Medium → High`), and which files became High or stopped being High. A rename alone doesn't count as a file change.
   - `removedNotRetired` means a classification left the config without being retired, so it would stay in the model unused. Fix that before going on.
   - Show the user the preview and ask whether to apply.
8. **Apply, record and verify** with phases 5 and 6. Spot-check files whose classification changed.
9. **Document and report** with phases 7 and 8, passing `--changes`.

## Clearing a previous run

A redo doesn't need this, because the apply rewrites or empties every host in the config. Use it when the user wants the classifications removed altogether, or when old entries sit on elements the new config won't list. It empties entries, leaving Template Outputs with an empty list for the reason given in "How classification resolves". Include it via `includedScriptPaths`:

1. Run it with the script `return clearClassifications({ dryRun: true });`. It lists every element carrying entries, the classifications, and `emptyHostFolders`: Folder elements with no children that probably existed only to hold entries.
2. Confirm with the user, then run `return clearClassifications({ deleteClassifications: true, deleteFolders: [<the agreed empty folders>] });`.
3. Validate and save. If the classifications are being removed altogether, delete the config, its `.applied.js` snapshot and the MDX. For a redo, keep the old config as the baseline, but forget memory notes about the previous run's themes so they can't leak into the new analysis.

## Pitfalls

- **The scripts' glob rules mirror Intent's matcher.** If a spot-check still disagrees with `rows.json`, trust Change Review and report the mismatch.
- **`write_file` boundaries.** It only writes inside the workspace, and stages anything inside an application's output.
- **Scratch files on Windows.** Bash heredocs containing quotes can break in Git Bash. Write scratch files, such as a config builder, with a file-writing tool that can reach the OS temp folder, or with a short Node script. Intent's `write_file` only writes inside the workspace.
- **Out of scope.** Files outside the application's output folder can't be classified from this application; note them in `notes`. Files inside it that no one reviews, such as the Intent solution's own metadata, go in `outOfScope`.
