# Restore Missing Output Targets

Use this when the Software Factory says a template has no output target for its role: "Template X (module M) targets the anchor/role R, but the Codebase Structure has no output target (Template Output) for it", or the older "Unable to find output target for template [X] with role [R]". Use it too for the same problem caught earlier as Codebase Structure validation errors "This Template Output is unassigned…", and when the Output panel's **Try fix with AI** sends you here.

A template names a **role**; the Codebase Structure designer places its output through an **Output Anchor** element with that name (see **architecture-templates**). The error means the anchor, or the module's Template Output under it, was removed. Fix the structure, then reinstall — don't edit generated code. The script and the reinstalls do the deterministic fixes; you do the judgement, the ordering and the talking to the user.

## Facts that drive the fix

- **Roles match by longest dotted prefix**, and a role may list several values separated by `,` `;` or `|`. `AI.Context.Skills` is served by an anchor of that name, else `AI.Context`.
- **A forced reinstall places a module's missing Template Outputs under roles that exist at that moment.** Without the role it goes under `Default` if that role exists, else unassigned at the package root. A later reinstall moves an unassigned output once the role exists, but never one under `Default`. So create anchors BEFORE reinstalling the modules that target them.
- **A reinstall never re-runs a module's on-install migration**, so reinstalling Intent.Common does not recreate the `AI.Context*` anchors; the script in step 4 does.
- **Always reinstall at the installed version.** Read it with `run_solution_query` `listInstalledModules()` and pass it as `version` with `forceReinstall: true`. Omitted, the tool resolves the latest version and UPGRADES the module. If a result's Action is `Updated` rather than `Reinstalled`, or the installed version can't be found, stop and tell the user.
- **A reinstall reloads open designers and drops unsaved edits**, so every script here runs with `saveOnSuccess: true`; check the result reports it saved before reinstalling, and stop if it didn't. The Codebase Structure's `designerId` comes from `get_applications`.

## Procedure

1. **Get template, module and role.** The Software Factory error names all three. A validation error "This Template Output is unassigned…" names only the template, and the SF won't name the role (an unassigned output still counts as a target): if any `AI.Context*` anchor is missing, treat the role as `AI.Context`; otherwise ask the user which role the template needs. With no module named, use the installed module whose id is the longest prefix of the template id and say it is an inference.
2. **Ask once whether the removal was deliberate**, unless the task came from **Try fix with AI** (its prompt says so). If the removal was deliberate, offer to leave the anchors out and turn off the affected skill and instruction templates instead (describe how; no scripting property for it is documented), and stop.
3. **Confirm the reinstalls** (see **working-with-modules**), listing modules, versions and order from `listInstalledModules()` (leave out any that aren't installed). **Try fix with AI** already confirms the named module, plus Intent.Common and Intent.Common.CSharp for an `AI.Context*` role; confirm any further module.
4. **Restore the role** (for a multi-value role, ask the user which value to restore unless one is `AI.Context*`):
   - **An `AI.Context*` value**, once per conversation (it restores all three): run the script below via `run_designer_script` (load **scripting-the-model** first). Then reinstall Intent.Common, and Intent.Common.CSharp if it is installed — never add it.
   - **Any other role:** don't invent a location. Ask the user where it belongs, then create an element of that name there with `run_designer_script`, using the element type the app's other roles use (`Output Anchor` in Codebase Structure, `Role` in a Folder Structure or Visual Studio designer); stop if they don't know.
5. **Reinstall the named module**, unless it was already reinstalled after its role existed.
6. **Re-run `run_software_factory`.** A failure on a DIFFERENT template: back to step 1 for it (step 2 doesn't repeat; step 3 only for a module not yet confirmed; step 4 only for a role not yet restored). If a template still fails after its role was restored and its module reinstalled, stop and report; never loop on an unresolved error (see **generating-and-applying-code**).
7. **Done when** the run has no output-target errors and `await getDesignerValidationErrors()` (`run_designer_query`) on the Codebase Structure reports no unassigned Template Output, and each reinstalled module's Template Outputs sit under the anchors you restored. One under `Default` instead stays there through reinstalls: delete that Template Output with `run_designer_script` (saved), reinstall its module, and re-run the Software Factory. Report what was recreated.

The script mirrors Intent.Common's `AiOutputAnchorsHelper.EnsureExist`: an anchor found in any package is left alone; missing ones go in the first package's `.agents` Folder (`AI.Context`), with `instructions` and `skills` Folders beside `AI.Context` for the other two. An application with a Folder Structure designer instead uses `Role` elements; describe that to the user rather than scripting it.

```js
const packages = getPackages();
if (packages.length === 0) return "No package in this designer; ask the user to create one.";
const allAnchors = () => packages.flatMap(p => p.findChildren({ specialization: "Output Anchor" }));
const anchorExists = name => allAnchors().find(a => a.getName() === name) ?? null;
const folderUnder = (parent, name) =>
    parent.getChildren("Folder").find(f => f.getName() === name) ?? parent.addChild("Folder", name);

let context = anchorExists("AI.Context");
if (!context) context = folderUnder(packages[0], ".agents").addChild("Output Anchor", "AI.Context");
const home = context.getParent() ?? packages[0]; // siblings sit beside AI.Context, in its package
if (!anchorExists("AI.Context.Instructions")) folderUnder(home, "instructions").addChild("Output Anchor", "AI.Context.Instructions");
if (!anchorExists("AI.Context.Skills")) folderUnder(home, "skills").addChild("Output Anchor", "AI.Context.Skills");
return allAnchors().map(a => a.getName());
```
