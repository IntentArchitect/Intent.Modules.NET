# CONTEXT — Intent.Modules.Blazor

## Multiple hosts in one application (multi-host)

An Intent application may contain more than one ASP.NET Core host project (e.g. an `Api` and a
`Mobile.Api`, or a dedicated Blazor host alongside plain API hosts). Host-scoped templates — `App.Program`,
`AppRazorTemplate`, `ServerImportsRazorTemplate`, `ThemeServiceTemplate`, etc. — have one instance per host
project, and any factory extension or template that resolves them must loop (`FindTemplateInstances`) or
scope by `OutputTarget` rather than use the singular, application-wide lookup. See
`Intent.Modules.AspNetCore/CONTEXT.md` for the general pattern; this module's factory extensions
(`BlazorAspNetCoreStartupInstaller`, `ThemeServiceTemplatePartial`) were fixed to follow it (2026-08-12).

## Known latent issue: `.Web` launch profile can leak into sibling ASP.NET Core hosts

`ProgramTemplatePartial.cs` (`Templates/Templates/Client/Program`), in its `IsInteractiveServer` branch,
registers a `.Web` launch profile via `LaunchProfileRegistrationRequest` with `ForProjectWithRole = "Startup"`.

This correctly excludes non-ASP.NET-Core hosts (e.g. Worker SDK projects) from getting the profile, because
`IForProjectWithRoleRequest.IsApplicableTo` (in `Intent.Modules.VisualStudio.Projects`) checks
`template.OutputTarget.GetProject().HasRole(request.ForProjectWithRole)`, and Worker SDK projects don't carry
the `Startup` role.

**It does not fully scope in a genuinely multi-Web-host application.** `Startup` is the role every ASP.NET
Core executable host carries generically (see `Intent.AspNetCore.imodspec`: `Startup;App.Program`,
`Startup;App.Startup`; and this module's own `AppRazorTemplate`: `<role>Startup</role>`). There is no more
granular role available that identifies "this specific Blazor host and not its sibling API hosts" — so in an
app with e.g. `Api` + `Mobile.Api` + a Blazor host, the `.Web` profile still gets written into `Api`'s and
`Mobile.Api`'s `launchSettings.json`, not just the Blazor host's own.

Investigated alternatives (2026-08-12), all rejected:
- `this.ApplyLaunchProfile(...)` (`Intent.Modules.Common.CSharp.Templates.CSharpTemplateExtensions`) — worse:
  it sets no `ForProjectWithRole` at all, so it leaks into *every* project including Worker SDK hosts.
- `this.EmitOrPublish(request)` (`IntentFileTemplateBase.EmitOrPublish<T>`, the dependency-graph-aware
  sibling of `Publish` used elsewhere for e.g. `ServiceConfigurationRequest.HasDependency(this)`) — no
  different from plain `Publish` for this request type. Confirmed via decompilation:
  `LaunchProfileRegistrationRequest` implements only `IForProjectWithRoleRequest` — it has no
  `HasDependency`/dependency-carrying member for `EmitOrPublish` to route on, so it falls back to the exact
  same `ForProjectWithRole`/`HasRole` filtering as `Publish`. Verified empirically against the WebAndWorker
  repro: identical staged diff (Api/Mobile.Api still get the leaked profile) as plain `Publish`.
- A more specific Blazor-only role — none exists; `AppRazorTemplate`'s only declared role is the generic
  `Startup`.

**Root cause is outside this module.** `LaunchProfileRegistrationRequest.ForProjectWithRole` (defined in
`Intent.Modules.Common.CSharp.Configuration`, an external SDK package) can only filter by role name, and no
role exists that's unique per ASP.NET Core host in a multi-host app. Properly fixing this would mean adding
an `OutputTarget`-scoped variant of `LaunchProfileRegistrationRequest` (mirroring how
`DefaultLaunchUrlPathRequest` already has `outputTarget.EmitDefaultLaunchUrlPathRequest(...)` "scoped to
projects that reference your output target") to `Intent.Modules.Common.CSharp` — a change to the SDK, not to
this module.

**Do not try to "fix" this again with a role string** — every role available on the Blazor host project is
also present on plain API hosts in this codebase's ecosystem. The fix requires new SDK capability.
