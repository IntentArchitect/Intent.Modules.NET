# CONTEXT — Intent.Modules.Blazor

## Multiple hosts in one application (multi-host)

An Intent application may contain more than one ASP.NET Core host project (e.g. an `Api` and a `Mobile.Api`, or a dedicated Blazor host alongside plain API hosts). Host-scoped templates — `App.Program`, `AppRazorTemplate`, `ServerImportsRazorTemplate`, `ThemeServiceTemplate`, etc. — have one instance per host project, and any factory extension or template that resolves them must loop (`FindTemplateInstances`) or scope by `OutputTarget` rather than use the singular, application-wide lookup. See `Intent.Modules.AspNetCore/CONTEXT.md` for the general pattern; this module's factory extensions (`BlazorAspNetCoreStartupInstaller`, `ThemeServiceTemplatePartial`) were fixed to follow it (2026-08-12).

## Known latent issue: `.Web` launch profile can leak into sibling ASP.NET Core hosts

`ProgramTemplatePartial.cs` (`Templates/Templates/Client/Program`), in its `IsInteractiveServer` branch, registers a `.Web` launch profile via `LaunchProfileRegistrationRequest` with `ForProjectWithRole = "Startup"`.

This correctly excludes non-ASP.NET-Core hosts (e.g. Worker SDK projects) from getting the profile, because `IForProjectWithRoleRequest.IsApplicableTo` (in `Intent.Modules.VisualStudio.Projects`) checks `template.OutputTarget.GetProject().HasRole(request.ForProjectWithRole)`, and Worker SDK projects don't carry the `Startup` role.

**It does not fully scope in a genuinely multi-Web-host application.** `Startup` is the role every ASP.NET Core executable host carries generically (see `Intent.AspNetCore.imodspec`: `Startup;App.Program`, `Startup;App.Startup`; and this module's own `AppRazorTemplate`: `<role>Startup</role>`). There is no more granular role available that identifies "this specific Blazor host and not its sibling API hosts" — so in an app with e.g. `Api` + `Mobile.Api` + a Blazor host, the `.Web` profile still gets written into `Api`'s and `Mobile.Api`'s `launchSettings.json`, not just the Blazor host's own.

Investigated alternatives (2026-08-12), all rejected:

- `this.ApplyLaunchProfile(...)` (`Intent.Modules.Common.CSharp.Templates.CSharpTemplateExtensions`) — worse: it sets no `ForProjectWithRole` at all, so it leaks into _every_ project including Worker SDK hosts.
- `this.EmitOrPublish(request)` (`IntentFileTemplateBase.EmitOrPublish<T>`, the dependency-graph-aware sibling of `Publish` used elsewhere for e.g. `ServiceConfigurationRequest.HasDependency(this)`) — no different from plain `Publish` for this request type. Confirmed via decompilation: `LaunchProfileRegistrationRequest` implements only `IForProjectWithRoleRequest` — it has no `HasDependency`/dependency-carrying member for `EmitOrPublish` to route on, so it falls back to the exact same `ForProjectWithRole`/`HasRole` filtering as `Publish`. Verified empirically against the WebAndWorker repro: identical staged diff (Api/Mobile.Api still get the leaked profile) as plain `Publish`.
- A more specific Blazor-only role — none exists; `AppRazorTemplate`'s only declared role is the generic `Startup`.

**Root cause is outside this module.** `LaunchProfileRegistrationRequest.ForProjectWithRole` (defined in `Intent.Modules.Common.CSharp.Configuration`, an external SDK package) can only filter by role name, and no role exists that's unique per ASP.NET Core host in a multi-host app. Properly fixing this would mean adding an `OutputTarget`-scoped variant of `LaunchProfileRegistrationRequest` (mirroring how `DefaultLaunchUrlPathRequest` already has `outputTarget.EmitDefaultLaunchUrlPathRequest(...)` "scoped to projects that reference your output target") to `Intent.Modules.Common.CSharp` — a change to the SDK, not to this module.

**Do not try to "fix" this again with a role string** — every role available on the Blazor host project is also present on plain API hosts in this codebase's ecosystem. The fix requires new SDK capability.

## Decision: `Prerendering` is a real setting in every render mode (2026-08-30)

The `Prerendering` setting (`d851b4d1-…`) existed from the start but was **hidden** for Interactive WebAssembly by an `Is Active Function`, and `AppRazorTemplate` ignored it for that mode anyway — WebAssembly applications always emitted a prerendering render mode. Both have been removed: the switch is offered in all three render modes and `GetRenderModeForPage()` honours it uniformly. The default is `false`, which it always was, so **existing Interactive WebAssembly applications stop prerendering on regeneration**. That is the intended uniform default and is the release note's main call-out.

The reason it matters beyond a blank first paint: **a prerendered page executes on the server**. Everything it does during `OnInitializedAsync` — including calls to authenticated APIs — is issued by the server process, not the browser. That is what surfaced the `AuthorizationMessageHandler` defect fixed in `Intent.Blazor.Authentication` at the same time; see that module's `CONTEXT.md`.

**This module has no knowledge of, and must not gain a dependency on, any authentication module.** `AppRazorTemplate` reads only `GetBlazor().ServerPrerendering()`. A Blazor application with no authentication module installed is a supported configuration, and this change behaves identically in one.

## Invariant: a `<link>` in `App.razor` must be gated on the same condition as the file it links (2026-08-31)

`AppRazorTemplate` emitted `<link href="app.css">` unconditionally, but `app.css` only ships from the `NoSamplePages` / `WasmNoSamplePages` content groups, each gated on `!ComponentLibraryInstalled` **and** a specific render mode. Any application with a component library installed — which is most of them, since `Intent.Modelers.UI.Core` is the marker — and any Interactive Auto application, which neither group covers, 404'd on `app.css` on every page load. The link had been that way long enough that the 404 read as normal.

`TemplateHelper.ShipsAppCss` is now the single source of truth for that condition, consulted by `AppRazorTemplate`; the two content-group registrations carry a comment pointing at it. **If a content group's own condition changes, change the helper in the same edit** — the two drifting apart is the whole defect.

Note `content/WasmSamplePages` and its `app.css` have **no registration at all** and are dead content. Left in place as out of scope, but do not treat their presence as evidence that combination is covered.

## Invariant: `Settings/ModuleSettingsExtensions.cs` is deliberately ignored — add new accessors by hand (2026-09-15)

`Settings/ModuleSettingsExtensions.cs` (the `GetBlazor()` / `Blazor` typed accessor for the `Blazor Settings` module settings group) is listed in `Intent.Modules.Blazor.application.output.config.xml` with `state="ignored"` (no `once-off-generated` suffix — this one is permanently protected, not a once-off seed). **This is deliberate, not an oversight.**

Un-ignoring it and re-running the Software Factory (tried while adding the `UseCustomStylesheets` setting) regenerates the file from the _currently installed_ `Intent.ModuleBuilder` templates, which use a **different naming convention** than what is on disk: `GetBlazor()` → `GetBlazorSettings()`, class `Blazor` → `BlazorSettings`, `ServerPrerendering()` → `Prerendering()`. Applying that regeneration would rename every one of those members and break every template in this module (and `Intent.Blazor.Components.MudBlazor`) that calls `.GetBlazor()` / `.ServerPrerendering()` — a sweeping, unrelated rename bundled into what should be an additive accessor addition. That drift between the on-disk names and the installed template's current convention is exactly why this file was ignored in the first place.

**When adding a new Module Settings Field Configuration to this group, do NOT unignore this file.** Hand-add the new accessor method directly, copying the exact pattern of an existing one (e.g. `EnableThemeToggle()`) with the new field's own setting id:

```csharp
public bool UseCustomStylesheets() => bool.TryParse(_groupSettings.GetSetting("<field-guid>")?.Value.ToPascalCase(), out var result) && result;
```

If the file ever needs full regeneration (e.g. deliberately adopting the new `BlazorSettings` naming), that is a separate, explicit rename migration across this module and every dependent — not something to fall into while adding an unrelated setting.

## Superseded: the `Prerendering` setting hint lived in a hand-edited `.imodspec` (2026-08-31)

The longer two-sentence hint for `Prerendering` had been hand-edited into `Intent.Blazor.imodspec`, which is generated from the model and therefore reverted on the next Software Factory run — it was lost the first time anything else regenerated the file. The text now lives on the model (`Prerendering` → `Field Configuration` → `Hint`) where it survives. **Never hand-edit `.imodspec`**; see the `module-versioning` skill for the same trap on `<version>`.

## Invariant: no hand-written C# file may ship as static content — it bypasses the weaver (2026-09-16)

`StaticContentTemplate` emits its content **verbatim**; all it does is substitute `<#= Token #>` placeholders. A `.cs` file shipped that way therefore never reaches the Roslyn Weaver, so **none of the weaver's C# style settings apply to it** — `Namespace Declaration Style`, usings placement/sorting, `.editorconfig` formatting. It is also the one `.cs` file in the output with no `[assembly: IntentTemplate(...)]` header, which is how you spot one.

`Components/Layout/UserMenu.razor.cs` was the last such file, shipped byte-for-byte identically from **two** places — this module's `content/ThemeToggle/` and MudBlazor's `content/Theme/`. It stayed block-scoped in applications that had set `prefer-file-scoped`, failing their own StyleCop rule on a file nobody wrote. It is now `UserMenuCodeBehindTemplate` (`Templates/Templates/Common/UserMenuCodeBehind/`) — a Single File C# Template built with `CSharpFile` + `.WithFileExtension("razor.cs")` + `.IntentManagedMerge()`, mirroring `RazorComponentCodeBehindTemplate`.

**When adding a `.razor` to a content folder, its code-behind goes in a template, not next to it.**

Decisions taken, and what was rejected:

- **A replacement token for the namespace declaration** — rejected. The block-scoped form needs a closing brace and an extra indent level on the body, which flat token substitution cannot express. Worse, there is no supported API in `Intent.Modules.Common.CSharp` for reading this setting at all — it belongs to `Intent.OutputManager.RoslynWeaver` (setting id `75da3fda-a64d-4161-a8a2-38614053fb1d`) — so the module would be doing a raw string lookup into another module's settings. Generating through `CSharpFile` gets this setting and every future weaver style setting for free and permanently.
- **`OverwriteBehaviour.OverwriteDisabled` (write-once)** — rejected. The two content copies disagreed: MudBlazor's `ThemeArtifacts` forced `OverwriteDisabled`, the base module's `ThemeToggle` inherited always-overwrite. `.IntentManagedMerge()` is strictly better than either — the three `[Parameter]` members stay correct on regeneration _and_ anything the developer adds survives. The cost is a one-time diff on existing applications as the weaver takes ownership; that is what makes the setting apply retroactively rather than only to newly created applications.
- **One template in this module serving both cases**, rather than one per module. `Intent.Blazor.Components.MudBlazor` depends on `Intent.Blazor`, and the two content copies were identical — keeping them in sync by hand was a standing duplication hazard.

**Cross-module floor:** `UserMenuCodeBehindTemplate` registers when MudBlazor is installed **or** Theme Toggle is enabled, exactly reproducing which applications received the file before. Because MudBlazor 2.0.5 stopped shipping its copy and relies on this template, `Intent.Blazor.Components.MudBlazor` pins `Intent.Blazor` at `2.0.5-pre.0` in its `.imodspec`. **Do not lower that floor** — MudBlazor 2.0.5 paired with Intent.Blazor 2.0.4 produces no `UserMenu.razor.cs` at all (2.0.4's `ThemeToggle` registration returns early whenever MudBlazor is installed), and the consumer's build fails on `UserMenu.razor`'s `Title` / `Trigger` / `ChildContent` bindings.

**Out of scope, deliberately:** `content/ComponentSkillSamples/**` in both modules still ships `.cs` / `.razor.cs` files as static content with the same verbatim-emission property. They are reference samples written into `.agents/skills/` for AI consumption — not compiled application code, not part of the build. Left alone.

## Invariant: removing `Secured` must remove `@attribute [Authorize]` on every component variant (2026-10-07)

`TransformTextCore()` (Client and Server `*ComponentRazorTemplateBase`) treats an existing `.razor` file as user-owned and only rebuilds the directives it manages: `@page`, `<PageTitle>` and the `Secured`-driven `@attribute [Authorize]`. It had an early return — when the model emits none of the three, return the file verbatim — and that early return skipped `RemoveManagedDirectives`. For a **Page** it never fires (`@page` is always emitted), which is why the bug did not reproduce on pages. For a plain **Component** or **Dialog**, removing `Secured` made all three empty, so the stale `@attribute [Authorize]` survived every regeneration.

The early return now strips only the `@attribute [Authorize]` directives and leaves everything else byte-for-byte. Rejected: dropping the early return and running the full rebuild path — that normalises line endings and strips `@page` / `<PageTitle>` on every non-page component, a one-time churn across every component file plus deleting any hand-added `@page` on a Component. Known residual: converting a Page to a Component still leaves its old `@page` line behind, by the same reasoning.

**One recogniser for both paths: `SecuredHelper.RemoveAuthorizeAttributeDirectives`.** The Page path (`RemoveManagedDirectives`) and the Component/Dialog early return both call it — never re-add a local `Authorize` check to either base, or the two paths drift apart again. It was introduced because the original `StartsWith("@attribute [") && Contains("Authorize")` test was simultaneously too narrow and too broad:

- _Missed_ (so a stale directive survived): `@attribute[Authorize]`, extra spaces/tabs, `[ Authorize ]`, `global::`-prefixed or `AuthorizeAttribute` names, and arguments wrapped across several lines (only the first line was dropped, leaving the rest as broken Razor).
- _Over-matched_ (deleting the developer's own code): any `@attribute` line containing the substring, e.g. `[MyAuthorize]` or `[Route("/Authorize")]`.

Deliberately left alone: combined lists such as `[Authorize, StreamRendering]` — the template never emits that form, and removing the line would also delete the other attribute. Commented-out directives (`@* ... *@`) are left too. The edge cases were verified against the compiled helper and both bases' `RemoveManagedDirectives` (via reflection) — the module has no test project.

Not covered by the template at all, by design: `[Authorize]` on the `.razor.cs` partial class (the code-behind is `Mode.Merge` and the template never emits that attribute, so it is never removed), and `@attribute [Authorize]` in a folder `_Imports.razor`. Both are hand/LLM-authored and outside what this template owns.

## Invariant: a change to any skill's text must be re-benchmarked before it ships (2026-11-24)

The agent skills this module emits (`blazor-page-*`, `blazor-dialog-*`, `blazor-app-menu`, `blazor-ux-theme-sync`, `updating-styling`) are benchmarked by the eval suite in `evals/`, which runs each case twice — once against the frozen baseline (`old_skill`, materialised from the git ref pinned in `evals/baseline/manifest.json`) and once against the working tree (`new_skill`) — then compares the two. `evals/README.md` holds the procedure; `evals/results/` holds the committed reports. The shared harness, `evals/tools/evaltool.py`, serves this module and `Intent.Blazor.Components.MudBlazor` both.

**Parity is the gate, not size.** Per case and model, over 3 runs per arm: no assertion may pass in more than one fewer run than baseline, the build must pass at least as often, and the new arm's mean judge score must be at least the baseline's lowest. Shortening a skill is only finished when that passes.

**A release that changes a sample owes its consumers a note.** The 2.1.1 reduction was planned as skill text only, on the reasoning that `SkillSampleOverwritePolicy` ties sample overwrites to the `SKILL.md` hash and so samples needed no thought. The samples then changed anyway (the ~13% sample cut, and the unrendered-`ErrorMessage` fix in the view page and edit dialog), which makes a consumer-facing consequence live: the policy keys the protection on the **skill**, never on the sample, so a consumer who edited a sample but not its `SKILL.md` loses that edit silently, while one who edited a `SKILL.md` receives nothing at all for that skill — neither the reduction nor the behaviour fixes — until they delete its `contentHash` line. State both halves in the release notes of any version that touches a sample. Reversing the policy to key on the sample's own hash would be more correct, but it changes a shared mechanism and must not ship in the same version as the content it protects.

**These cases do not run on a `Tests/` app.** Every Blazor test app has MudBlazor installed, so none of them produces the Bootstrap variants of the target files. The cases run on `evals/fixtures/bootstrap/`, a stripped Bootstrap app committed here and named by `app_dir` in each case's `run.json`. It does not track the `Tests/` apps: if these templates change the shape of what they generate, refresh the fixture by hand or the cases will measure a layout the module no longer produces.

Three things this suite taught the hard way, each of which cost a re-run:

- **A case must fail on an untouched fixture.** `boot-menu-rerun-idempotent` originally asserted only _at most once_, _still present_ and _unchanged_ — all true before any agent ran, so it scored full marks on an untouched app and measured nothing. Step 0 of the README is now that gate.
- **Editing a skill invalidates every case that loads it, not just the one that regressed.**
- **Repetition in the old text was sometimes load-bearing, and the fix is accuracy, not narration.** The old app-menu skill named only `Program.cs` for the auth registrations but repeated the check in four places, so agents kept searching until they found them in `Startup.cs`. Stated once, agents checked `Program.cs`, found nothing and concluded NoAuth — `01-auth-detected` fell to 1/3. The repair was to name both files and require the reply to say which was read.

Judge scores are the noisiest layer: re-judging the same runs against slightly reworded expectations moved scores by a full point. Treat a one-point difference as noise and look at the deterministic checks first.

**`compare` cannot read a judged arm against an unjudged one.** Running the batch without judging is a legitimate split — the runs are the slow part, judging is interactive the next day — but `compare` then reports every judge expectation as a full regression (`01-layout-matches-sample 3/3 → 0/3`), because it counts an expectation absent from the new arm's `grading.json` as zero passes. It is counting missing data as failure, and the `judge score` row is simultaneously _skipped_ (that comparison is guarded on both arms having scores), so nothing in the output says judging is what is missing. The tell is that every "regression" is a numbered grader filename rather than a check id. Either judge both arms before comparing, or compare only the expectations with `kind == "deterministic"`.

## Invariant: a CRUD skill reads `ux-tokens.css` and nothing else of the design system (2026-11-25)

The entity page and dialog skills' "Read first" list names only `ux-tokens.css`. It deliberately does **not** name `design.md`, `ux-base.css` or `ux-components.css`, and adding any of them back undoes the largest token saving these skills have.

The reasoning, in the order it has to be checked before anyone changes it:

- **Measured first.** In a generated app the old list cost ~42 KB per CRUD task (`design.md` 17.2, `ux-tokens.css` 8.9, `ux-base.css` 4.4, `ux-components.css` 11.9) against a skill text of 3.8–5.1 KB — about 60% of the whole payload, and far more than the samples. Cutting it takes a page task from ~69 KB to ~27 KB.
- **`design.md` is not a CRUD input.** It is the theme-sync skills' input — they apply it — and documents rationale a CRUD task never acts on. What a CRUD task needs is real token _names_, which live in `ux-tokens.css`.
- **The page and dialog skills differ, and the difference decides the stylesheets.** The 8 page skills carry §3's "grep each utility class name across `wwwroot/**/*.css`", so a grep already reaches `ux-base.css` and `ux-components.css` when a class is actually wanted. The 4 dialog skills carry no grep and name no utility class — their only styling rule is "inline `Style=` uses `var(--token)`" — so for them those two files supplied nothing but token names, which `ux-tokens.css` already does. That is why one identical replacement was correct for all 12 rather than two variants.
- **The theme-sync skills were not touched.** `blazor-ux-theme-sync` still reads `design.md` in full, correctly.

If a styling regression ever points here, the repair is to state the grep more firmly in §3 — not to restore a read. Restoring `design.md` to a CRUD skill puts 17 KB back on every page, dialog, view and search task to supply something the task does not use.

**The frontmatter `paths:` globs are a separate mechanism and cost nothing.** They remain `**/design.md`, `**/ux-base.css`, `**/ux-components.css`. `paths` (and its aliases `applyTo` / `appliesTo` / `globs`) is an _inclusion gate_ — the skill is offered when a chat attachment matches one of them — not a preload list. Narrowing them would change which skills are offered, not how many tokens one costs.

## A worktree cannot pick up a rebuilt module until its asset repository points inside the worktree (2026-11-25)

`Tests/intent.repositories.config` had one entry, `..\..\.intent-modules`. Relative to `Tests/` that resolves to `D:\Dev\.intent-modules` in the main checkout and is correct there — but in a git worktree it resolves to a path that does not exist, and **nothing reports it**. The symptom is a Software Factory run returning _0 changes_ after a module change that is genuinely present: the module build succeeds, the `.imod` is written, `install_or_update_modules` even answers "Reinstalled", and the DLLs under `Tests/.intent/modules/**/lib/` keep their old timestamp because there was no newer source to read.

Fixed by adding a second entry, `Worktree` → `..\Modules\Intent.Modules`, which is where this repository's packager drops and is therefore correct in a worktree and in the main checkout alike. `Modules/intent.repositories.config` already used exactly this pattern for the module solution; the Tests solution simply never got it.

Two things worth keeping in mind when this shape recurs:

- **The timestamp on the installed DLL is the quickest discriminator.** Compare it against the `.imod` the build just wrote. A stale DLL means the install found nothing newer, which is a repository-address problem, not a template problem — and no amount of re-installing or renumbering fixes it.
- **A module version bump makes the failure loud.** Re-installing the _same_ version silently reuses the cache, so it looks like success. Asking for a version the cache does not hold turns a silent no-op into a visible update, which is how this was finally confirmed.
