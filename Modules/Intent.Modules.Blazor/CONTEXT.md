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

`AppRazorTemplate` emitted `<link href="app.css">` unconditionally, but `app.css` only ships from the
`NoSamplePages` / `WasmNoSamplePages` content groups, each gated on `!ComponentLibraryInstalled` **and**
a specific render mode. Any application with a component library installed — which is most of them,
since `Intent.Modelers.UI.Core` is the marker — and any Interactive Auto application, which neither
group covers, 404'd on `app.css` on every page load. The link had been that way long enough that the
404 read as normal.

`TemplateHelper.ShipsAppCss` is now the single source of truth for that condition, consulted by
`AppRazorTemplate`; the two content-group registrations carry a comment pointing at it. **If a content
group's own condition changes, change the helper in the same edit** — the two drifting apart is the
whole defect.

Note `content/WasmSamplePages` and its `app.css` have **no registration at all** and are dead content.
Left in place as out of scope, but do not treat their presence as evidence that combination is covered.

## Invariant: `Settings/ModuleSettingsExtensions.cs` is deliberately ignored — add new accessors by hand (2026-09-15)

`Settings/ModuleSettingsExtensions.cs` (the `GetBlazor()` / `Blazor` typed accessor for the `Blazor Settings` module settings group) is listed in `Intent.Modules.Blazor.application.output.config.xml` with `state="ignored"` (no `once-off-generated` suffix — this one is permanently protected, not a once-off seed). **This is deliberate, not an oversight.**

Un-ignoring it and re-running the Software Factory (tried while adding the `UseCustomStylesheets` setting) regenerates the file from the *currently installed* `Intent.ModuleBuilder` templates, which use a **different naming convention** than what is on disk: `GetBlazor()` → `GetBlazorSettings()`, class `Blazor` → `BlazorSettings`, `ServerPrerendering()` → `Prerendering()`. Applying that regeneration would rename every one of those members and break every template in this module (and `Intent.Blazor.Components.MudBlazor`) that calls `.GetBlazor()` / `.ServerPrerendering()` — a sweeping, unrelated rename bundled into what should be an additive accessor addition. That drift between the on-disk names and the installed template's current convention is exactly why this file was ignored in the first place.

**When adding a new Module Settings Field Configuration to this group, do NOT unignore this file.** Hand-add the new accessor method directly, copying the exact pattern of an existing one (e.g. `EnableThemeToggle()`) with the new field's own setting id:

```csharp
public bool UseCustomStylesheets() => bool.TryParse(_groupSettings.GetSetting("<field-guid>")?.Value.ToPascalCase(), out var result) && result;
```

If the file ever needs full regeneration (e.g. deliberately adopting the new `BlazorSettings` naming), that is a separate, explicit rename migration across this module and every dependent — not something to fall into while adding an unrelated setting.

## Superseded: the `Prerendering` setting hint lived in a hand-edited `.imodspec` (2026-08-31)

The longer two-sentence hint for `Prerendering` had been hand-edited into `Intent.Blazor.imodspec`,
which is generated from the model and therefore reverted on the next Software Factory run — it was
lost the first time anything else regenerated the file. The text now lives on the model
(`Prerendering` → `Field Configuration` → `Hint`) where it survives. **Never hand-edit `.imodspec`**;
see the `module-versioning` skill for the same trap on `<version>`.

## Invariant: no hand-written C# file may ship as static content — it bypasses the weaver (2026-09-16)

`StaticContentTemplate` emits its content **verbatim**; all it does is substitute `<#= Token #>`
placeholders. A `.cs` file shipped that way therefore never reaches the Roslyn Weaver, so **none of the
weaver's C# style settings apply to it** — `Namespace Declaration Style`, usings placement/sorting,
`.editorconfig` formatting. It is also the one `.cs` file in the output with no
`[assembly: IntentTemplate(...)]` header, which is how you spot one.

`Components/Layout/UserMenu.razor.cs` was the last such file, shipped byte-for-byte identically from
**two** places — this module's `content/ThemeToggle/` and MudBlazor's `content/Theme/`. It stayed
block-scoped in applications that had set `prefer-file-scoped`, failing their own StyleCop rule on a file
nobody wrote. It is now `UserMenuCodeBehindTemplate` (`Templates/Templates/Common/UserMenuCodeBehind/`) —
a Single File C# Template built with `CSharpFile` + `.WithFileExtension("razor.cs")` +
`.IntentManagedMerge()`, mirroring `RazorComponentCodeBehindTemplate`.

**When adding a `.razor` to a content folder, its code-behind goes in a template, not next to it.**

Decisions taken, and what was rejected:

- **A replacement token for the namespace declaration** — rejected. The block-scoped form needs a closing
  brace and an extra indent level on the body, which flat token substitution cannot express. Worse, there
  is no supported API in `Intent.Modules.Common.CSharp` for reading this setting at all — it belongs to
  `Intent.OutputManager.RoslynWeaver` (setting id `75da3fda-a64d-4161-a8a2-38614053fb1d`) — so the module
  would be doing a raw string lookup into another module's settings. Generating through `CSharpFile` gets
  this setting and every future weaver style setting for free and permanently.
- **`OverwriteBehaviour.OverwriteDisabled` (write-once)** — rejected. The two content copies disagreed:
  MudBlazor's `ThemeArtifacts` forced `OverwriteDisabled`, the base module's `ThemeToggle` inherited
  always-overwrite. `.IntentManagedMerge()` is strictly better than either — the three `[Parameter]`
  members stay correct on regeneration *and* anything the developer adds survives. The cost is a one-time
  diff on existing applications as the weaver takes ownership; that is what makes the setting apply
  retroactively rather than only to newly created applications.
- **One template in this module serving both cases**, rather than one per module. `Intent.Blazor.Components.MudBlazor`
  depends on `Intent.Blazor`, and the two content copies were identical — keeping them in sync by hand was
  a standing duplication hazard.

**Cross-module floor:** `UserMenuCodeBehindTemplate` registers when MudBlazor is installed **or** Theme
Toggle is enabled, exactly reproducing which applications received the file before. Because MudBlazor
2.0.5 stopped shipping its copy and relies on this template, `Intent.Blazor.Components.MudBlazor` pins
`Intent.Blazor` at `2.0.5-pre.0` in its `.imodspec`. **Do not lower that floor** — MudBlazor 2.0.5 paired
with Intent.Blazor 2.0.4 produces no `UserMenu.razor.cs` at all (2.0.4's `ThemeToggle` registration
returns early whenever MudBlazor is installed), and the consumer's build fails on `UserMenu.razor`'s
`Title` / `Trigger` / `ChildContent` bindings.

**Out of scope, deliberately:** `content/ComponentSkillSamples/**` in both modules still ships `.cs` /
`.razor.cs` files as static content with the same verbatim-emission property. They are reference samples
written into `.agents/skills/` for AI consumption — not compiled application code, not part of the build.
Left alone.
