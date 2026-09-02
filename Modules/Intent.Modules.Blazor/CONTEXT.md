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

## Superseded: the `Prerendering` setting hint lived in a hand-edited `.imodspec` (2026-08-31)

The longer two-sentence hint for `Prerendering` had been hand-edited into `Intent.Blazor.imodspec`,
which is generated from the model and therefore reverted on the next Software Factory run — it was
lost the first time anything else regenerated the file. The text now lives on the model
(`Prerendering` → `Field Configuration` → `Hint`) where it survives. **Never hand-edit `.imodspec`**;
see the `module-versioning` skill for the same trap on `<version>`.
