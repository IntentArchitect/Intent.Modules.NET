---
description: Bring an existing (brownfield) codebase into Intent Architect from nothing. Create the solution and an Empty Application whose output is the existing code (with the Visual Studio component when the code has a .sln/.csproj), and mirror the existing solution and projects so Intent writes into the files already there. Run the reverse-engineer skills that fit (reverse-engineer-domain, reverse-engineer-services) so the designers model the code as it is. Then replace the generated all-in-one diagrams with focused, left-to-right area diagrams, and classify the codebase for Change Review with the classify-codebase skill. Use when the user has opened a folder of existing code with no Intent solution and asks to initialize, onboard, adopt, set up or reverse-engineer it in Intent Architect, or runs /initialize-brown-field, even if they only say "get this repo into Intent".
short-description: "Bring an existing codebase into Intent: solution, in-place application, reverse-engineered designers, readable diagrams, review classifications."
argument-hint: "[folder or .sln to initialize]"
intent-updates: automatic
requiredTools:
  - run_solution_query
  - ask_user_question
  - create_solution
  - create_application
  - update_application_settings
  - install_or_update_modules
  - get_designer_rules
  - run_designer_query
  - run_designer_script
  - apply_change_diagram_layout
  - run_software_factory
  - get_file_diffs
  - run_task
  - bash
group: Modelling
---

# Initialize a brownfield codebase

**Goal.** Start from a folder of existing code and no Intent solution. End with:

- a solution and one application whose output is the existing code, in place;
- designers that model the code as it is;
- a Software Factory (SF) run that proposes **zero** changes;
- area diagrams a reviewer can read;
- if the user opts in (step 2), File Classifications that tell Change Review where a change needs a developer's eyes, with a document for the user to review.

**This skill orchestrates; it doesn't repeat other skills.** Each phase's how-to lives in another skill. Load it before that phase and follow it. Where this skill records a decision the user made while it was being written, that decision applies unless the user now says otherwise. Those decisions are marked **User decision**.

**User decision: no questions after step 2.** Step 2's answer is the last thing the run asks; the user reviews the result at the end. From step 3 on:

- Where this skill or a skill it loads would ask (reverse-engineer-domain, reverse-engineer-services, configure-drift-check, classify-codebase), don't: take the option it would recommend. Skip configure-drift-check's step 9 approval and classify-codebase's phase 6 spot-check.
- A file whose adoption would change behaviour beyond headers and formatting is excluded, not asked about (step 5).
- Stop only for a real blocker.
- No end-of-run report: the skills this one loads write no final report, and step 8's message is a few lines.

Skills used:

- **creating-solutions** and **creating-applications**: creating the solution and the application.
- **scripting-the-model**, **exploring-the-model** and **changing-the-model**: required before any designer script. Where the host offers `get_designer_rules`, call it for each designer before its first `run_designer_script`, which refuses a designer with rules until you have.
- **generating-and-applying-code** and **resolve-destructive-changes**: SF runs and adoption.
- **working-with-modules**: installing modules.
- **reverse-engineer-domain** and **reverse-engineer-services**: modelling the existing code.
- **classify-codebase**: classifying the codebase for review.

The work is long: hundreds of elements across several sessions. Keep a task list with one item per phase, and save facts the next session needs (decisions, blockers, workarounds) to memory as you go.

## 1. Survey the codebase (read-only)

Use the shell, and read-only sub-agents for a large tree. Find out:

- **Is there already an Intent solution?** Look for `*.isln`, usually under `intent/`. If there is one, this isn't a fresh initialization: ask whether to add an application to it or stop.
- **Is it a Visual Studio codebase?** A `.sln` or `.slnx` file with `.csproj` projects means yes. For each project, record:
  - its path relative to the solution;
  - SDK, target framework(s) (`TargetFramework`, or `TargetFrameworks` for a multi-target project), configurations, `Nullable`, `LangVersion`, `UserSecretsId`, and any `RootNamespace`, `AssemblyName`, `ImplicitUsings` or `OutputType`;
  - project references.

  Also record the solution file format, whether package versions are managed centrally (`Directory.Packages.props` with `ManagePackageVersionsCentrally`) and which projects opt out, and which solution folders exist on disk.
- **Which layers exist?**
  - A persistence and domain layer (DbContext, `IEntityTypeConfiguration`, entity folders, repositories) is a job for **reverse-engineer-domain**.
  - An application layer (DTOs, services or business classes, controllers or endpoints, MediatR requests, messaging) is a job for **reverse-engineer-services**.

  Both skills are for C#. For any other stack, no reverse-engineer skill applies yet.
- **How big is each layer?** Count files and types per layer and per feature folder. You'll use this to plan extraction sub-agents and diagrams.
- **What will block a build?** Look for a `nuget.config` with private feeds and anything else that needs credentials, so a failed build is no surprise.
- **Can the codebase be classified (step 7)?** classify-codebase needs the code to be in a git repository and Node.js 18 or later on the path. Check both with `git rev-parse --show-toplevel` and `node --version`, and note either gap for step 2.

## 2. Confirm the plan in one question

This skill fixes the architecture: **Empty Application**. Its id is currently `Intent.ApplicationTemplate.EmptyVisualStudioSolution`; confirm it with `searchArchitectures()`. Read `getArchitectureDetails()` for it first. Because the architecture is already decided, creating-applications' two questions fold into a single `ask_user_question`:

1. **Solution name.** Suggest the repository folder's name (creating-solutions).
2. **Application name.** Suggest the existing `.sln` name.
3. **Output target.** Offer the architecture's optional components:
   - `Visual Studio`, recommended when step 1 found a `.sln`/`.csproj`;
   - `Folder Structure` otherwise.

   Say in the option text that the `.gitignore` setting will be turned off when the repository already has a `.gitignore`, and that `Visual Studio` also authorizes updating `Intent.VisualStudio.Projects` (step 3).
4. **What to set up.** Use `multiSelect`, with one option per layer and one for classification:
   - **The layers step 1 detected.** Say that each one installs its Guardrails module (`Intent.Domain.Guardrails.Contracts`, `Intent.Services.Guardrails.Contracts`) and the metadata modules its skill detects. Search for them with `includePrerelease: true` (working-with-modules): the Guardrails modules may exist only as pre-releases, and when one does, say so in the option text. The answer is the user's authorization for those installs.
   - **Classify for Change Review** (step 7). Say it runs classify-codebase after the diagrams, and asks its own questions then. If step 1 found no git repository or no Node.js 18+, say so in the option text.

   The tool allows one recommended option per question, so mark the most important detected layer as recommended, and name the others in the question text. When classification is the only option (no layer applies), ask it as a yes/no question of its own: the tool needs at least two options.

## 3. Create the solution and the application

1. **Create the solution** if the folder has none (creating-solutions).
2. **Create the application** (creating-applications):
   - pass the chosen component in `componentOverrides` (the other one `"false"`);
   - turn the `.gitignore` setting off by its id, not by title: the architecture calls it `Generate Git ignore file` while the installed setting is `Generate .gitignore file`, and an override by the wrong title is silently ignored. Right after creation, read its id with `getApplicationSettings({ search: "gitignore" })`, set it to `"false"` with `update_application_settings`, and check it before step 4's first SF run;
   - skip creating-applications' steps that finish and build the new application: the existing code already has its entrypoints, and step 4 builds it.
3. **Don't keep what the SF run that `create_application` starts produces.** It targets a new folder next to the metadata, not the existing code, and nothing should be written until step 4 points the application at the code. If its changes are only staged, don't apply them. If they reached disk anyway (see "The Software Factory may apply changes itself" in the Gotchas), delete the folder it created next to the metadata; `git status` shows it as new.
4. **Update `Intent.VisualStudio.Projects`** to its latest version, pre-release included (4.1.12+ adds `Generate Program File`), before step 4's first script: an install reloads the designers and drops unsaved edits.

## 4. Point the application at the code and mirror the solution

1. **Output location.** Set it with `update_application_settings` (`relativeLocation`).
   - The path is relative to the application's metadata folder, where its `.application.config` lives, not to the solution.
   - Check that the `outputLocation` the tool returns is exactly the folder that holds the `.sln`. A first attempt is easily one level off.
2. **Mirror the solution** in the Codebase Structure designer with one script (changing-the-model), then verify it with `getDesignerModelStructure`.
   - **The `Visual Studio Solution` element.** Name it after the `.sln` file, and set `Solution File Format` to match (Classic `.sln` or `.slnx`).
   - **The default project.** The architecture creates one, and it carries the **Default Output Anchor**. Rename it to the host project (the web or API project) rather than deleting it.
   - **One `C# Project (.NET)` per `.csproj`.** Name it exactly as the file and place it in the folder that mirrors its path. Copy the code's values into the stereotypes:
   - `.NET Settings`: `SDK`, `Target Framework` (an element reference: look up the `.NET Core Version` elements; for platform-specific frameworks such as `net10.0-android`, model only the base framework, `net10.0`), `Configurations`, `User Secrets Id`;
   - `C# Project Options`: `Nullable` (`(unspecified)` when the project doesn't set it), `Language Version`;
     - every other property the project file sets: `Target Multiple Frameworks` with `Target Frameworks` for a multi-target project, and `Root Namespace`, `Assembly Name`, `Implicit Usings` and `Output Type`. Read the full property lists with `getStereotypeDefinitions()`.
   - **Solution-wide values.** Set `Manage Package Versions Centrally` on the solution when the code uses `Directory.Packages.props` (and false on each project that opts out), and `Materialize Folder` on each Solution Folder that exists on disk.
   - **Hands-off startup files.** Turn off `Generate Program File`, `Generate AppSettings File` and `Generate LaunchSettings File` on every project that shows them, so the existing files stay untouched. Don't flip `Use top-level statements` to clear a `Program.cs` merge error.
   - **`Project Reference` associations.** Add one per project reference, with both multiplicities `"1"`. Model nothing the code doesn't have.
3. **Generate, inspect, apply** (generating-and-applying-code).
   - Save the designer and run the SF. Read every diff before applying. If the run applied changes itself, read them with `get_file_diffs` straight away, and undo any you didn't want before going on (see the Gotchas).
   - The `.sln` and `.csproj` files should be untouched, or changed harmlessly. A diff that drops a package reference, property or item is a gap in the model: fix the model (resolve-destructive-changes).
   - **Startup files.** If one still shows a diff with its `Generate … File` property off, ask the user whether to apply it.
   - **Package references.** Apply additions and upgrades, then align their versions (see "NuGet package versions").
4. **Build.** Run the build. If it fails for an environmental reason (e.g. a private NuGet feed returning 401), skip builds for the rest of the work, save that to memory, and say so in step 8's message. Never patch around it. A package version conflict isn't environmental: fix it (see "NuGet package versions").
5. **Re-run the SF.** It must propose zero changes.

For a **Folder Structure** application, set the output location the same way, and add only the folders later Template Outputs need. If the code isn't C#, no reverse-engineer skill applies: skip steps 5 and 6, and go on to step 7 if the user chose classification.

## 5. Reverse-engineer the designers

- **Only the layers the user chose in step 2.**
- The `reverse-engineer*` skills are introduced by the `Intent.Domain.Guardrails.Contracts` and `Intent.Service.Guardrails.Contracts`, you will need to install the modules you need and run the Software Factory to get the skills.
- **Domain first, then Services.** Commands, DTOs and entity actions reference Domain types. For each skill, install its preconditions (working-with-modules, searching with `includePrerelease: true`), then follow it from §0 to its zero-change finish. Skip its diagram section (reverse-engineer-domain §5, reverse-engineer-services §6): step 6 replaces those diagrams.
- **Services in phases**, each adopted with a zero-change SF run before the next, in the skill's own order:
  1. the contracts the rest depend on (§3): DTOs, enums, messages and eventing DTOs;
  2. the rest of §3: commands, queries, services or business classes, and event handlers;
  3. the endpoint surface, with HTTP and security (§4);
  4. intent (§5).

  For a large layer, run the skill's §1 extraction with parallel read-only sub-agents, one per folder slice.

Pass on these decisions:

- **Names verbatim.** Adopting files in place needs the model to use the code's names, legacy spelling and all (`orderlines`, `custNo`). Before creating any element, set the naming conventions to `manual` with `update_application_settings`:
  - the Domain's in group `Domain Settings`;
  - the Services' `Entity`, `Operation`, `Parameter` and `Property Naming Convention` in group `Service Settings`.

  See the Gotchas for a designer that doesn't pick this up.
- **Writer settings stay at their defaults**: Auto Format, Usings Sorting and Parameter Placement. The reverse-engineer skills already forbid turning Auto Format off to shrink diffs. **User decision:** don't change any of the three without asking. Doing so once without asking flush-lefted every attributed declaration, and the user objected strongly.
- **Intent is for a reviewer, not a call graph.** **User decision:** leave out plumbing a visual reviewer doesn't need: audit and event logging, and configuration or feature-flag lookups. For other cross-cutting helpers (role checks, notification payload builders, caching), list each category with its call count in reverse-engineer-services §5's single question, and let the user choose.
- **No mappings** on intent associations (reverse-engineer-services §5). See the Gotchas for the mapping the designer creates anyway.
- **When adopting a file would change behaviour beyond headers and formatting, exclude it; don't ask.** For example, the Guardrails template can't emit `[FromBody]`/`[FromRoute]`, so adopting those controllers would strip them. The element stays modelled; exclude its file before the run with the Template Output's `Registration Filter`, rather than leaving it unapplied (see the Gotchas), and list it in step 8's message. Package references and versions don't count (see "NuGet package versions").

## 6. Optimise the diagrams

Read `references/diagrams.md` and do what it says. In short:

- replace the all-in-one diagrams with one diagram per feature area;
- lay out the Services area diagrams left to right with `scripts/services-area-layout.js`, which also routes self-calls round each service's right-hand side;
- lay out the Domain area diagrams to the designer's own layout rules;
- remove elements left with no lines.

**User decision:** no all-in-one diagrams, and service-to-service lines on the right-hand side of the service.

## 7. Classify the codebase for review

Do this step if the user chose **Classify for Change Review** in step 2. Load **classify-codebase** and follow it from its "Start" section through its phase 7, which produces the review document. Skip its phase 8 report: step 8 still has to run, and its message gives the document's path.

Let its "Start" section decide what kind of run this is. Don't assume nothing is classified: the modules installed in step 5 can stamp default classifications onto their Template Outputs, which is its "No config" case, a new map that replaces them.

It comes after modelling and adoption on purpose. By now every Template Output this initialization created is in place, and the adopted files carry the template ids the classification reads. Run earlier, it would classify most generated files as hand-written and miss the Template Outputs added later.

Hand it what this initialization already knows, so it doesn't rediscover it:

- the application and its output folder (step 4);
- where the Intent solution folder is, in case it sits inside the output folder;
- the survey from step 1: the projects, layers and their sizes, which help it slice its scan. The classifications themselves still come from its own analysis.

Leave its questions to it: the classifications, what High means, and any upgrade of `Intent.Modelers.CodebaseStructure` it recommends. The answer in step 2 authorized the Guardrails, metadata and Visual Studio modules only, so that upgrade needs its own agreement, which classify-codebase asks for. If the user declines an upgrade it can't do without, stop this step and say so in the report.

## 8. Verify and finish

- **Validation.** Run `getDesignerValidationErrors` on every designer. It should be clean.
- **Save and regenerate.** Save every designer, then do a final SF run: zero changes.
- **Build**, or note that builds were skipped (step 4).
- **Message.** **User decision:** no longer report. Write a few lines:
  - the status: done, or what is left and why, with any file excluded or left unapplied;
  - the diagrams to review;
  - the path of classify-codebase's review document, or why step 7 was skipped.

## NuGet package versions

The SF raises a package to the version a module requires but never lowers it, so a version you raise by hand stays.

- Apply package additions and upgrades; don't ask about them or leave them unapplied.
- Align each added package with the version its family already uses in the repo, in `Directory.Packages.props` when versions are central, otherwise on the `PackageReference`.
- Fix version conflicts at restore or build (`NU1109`, `NU1605`, `NU1608`, `NU1107`) the same way, by raising the lower version.
- Never go below the module's version; if a package must stay lower or untouched, ask about `IntentIgnore="true"` on its `PackageReference`.
- These edits are allowed even though generated files are otherwise off-limits: a version isn't structure.

## Gotchas

Seen while building this skill on a real codebase:

- **The Software Factory may apply changes itself.** Depending on its mode (write-through checkpoints, or a staging mode that applies non-destructive changes on its own), a run's changes can be on disk before you have read them; the run's result says which. Wherever this skill says to read diffs before applying, read them with `get_file_diffs` straight after the run instead. Undo any file you didn't want from git (`git checkout -- <file>`, or delete a new one) before going on. A change that drops code is still a gap in the model: fix the model (resolve-destructive-changes). If `run_software_factory` takes an `autoApply` parameter, pass `false` wherever this skill says to read diffs before applying.
- **Leaving part of a run unapplied has a cost.** Files from a run that you don't apply are still registered as Intent-managed, so if you later exclude their element, the SF proposes deleting them. Exclude with a `Registration Filter` before the run instead. If such a delete is proposed, don't apply it; resolve it with resolve-destructive-changes.
- **A designer ignores changed settings for a while.** After a settings change, a designer may keep the old values in memory. The Services designer still camelCased parameters after the naming conventions were set to `manual`, and the Domain designer kept `Key Creation Mode` explicit and added an `id: guid` to every new class. In a script, set each changed field on `application.getSettings("<group title>")` (for example `"Service Settings"`, `"Domain Settings"` or `"Database Settings"`) with `getField(name).value = "manual"`. Then check new elements against the code after every create, and delete anything the old value added.
- **Eventing package references can point at the wrong package.** On eShop, every script-created reference to a service's eventing package resolved to another service's eventing package after each save, so `Subscribe Integration Event` failed as "not reachable"; `addReference` then added a duplicate and `removeReference` threw. After each save, re-read the references with `getPackageReferences()`. If one points at the wrong package, stop and ask the user to repair it in the designer. Don't rename a package once other packages reference it: the SF resolves references by file path, and the file keeps its old name.
- **`Perform Invocation` creates a mapping you didn't ask for.** `createAssociation("Perform Invocation", …)` adds a partial `Invocation Mapping` (the call end only), which raises "required but not mapped" warnings. Delete it straight away: `assoc.getAdvancedMapping("Invocation Mapping")?.delete()`.
- **An `Update Entity Action` can change an operation's signature.** Creating one on a parameterless operation makes DomainInteractions add a key parameter. Delete the parameter, and check every operation's parameters still match the code.
- **A file that crashes the merge.** RoslynWeaver can fail on a file, e.g. "Stack empty" when types are declared in the global namespace after the namespace block. Exclude that element with the Template Output's `Registration Filter` (e.g. `Name.EndsWith("Controller") && Name != "TestController"`). Comment the element with the reason, and list it in step 8's message.
- **Saves.** `saveOnSuccess` can time out; retry the no-op save.
- **Diagrams.** The designer behaviours to know (names reverting on save, every line drawn, the snapshot reading only the open diagram) are in `references/diagrams.md`.
