# Context: Intent.VisualStudio.Projects

## Purpose

This module generates the Visual Studio solution and project layout (`.sln`/`.slnx`, `.csproj`,
`Directory.Packages.props`, `appsettings.json`, `launchSettings.json`, etc.) from the Codebase
Structure designer. It owns NuGet package installation into `.csproj` files and Central Package
Management (CPM) for a solution.

## Architectural Decisions

- **`Central Package Floating Versions Enabled` is modelled on the `Visual Studio Solution Options`
  stereotype (per-solution), not as a module setting.** `Manage Package Versions Centrally` already
  lives there, and a single application's Codebase Structure designer can hold more than one Visual
  Studio Solution. An application-level module setting could not express "floating on for this
  solution, off for that one" — it would apply uniformly across every solution in the application.

- **The property is written in `DirectoryPackagesPropsTemplatePartial`'s constructor, not in
  `SdkSchemeProcessor` (the NuGet factory extension).** The constructor already builds the
  `ProjectRootElement` that `RunTemplate()` returns, and it already reads
  `ManagePackageVersionsCentrally()` the same way. Writing the flag there means it always appears
  when the file is generated, regardless of whether any package happens to change that run — tying
  it to `SdkSchemeProcessor`'s per-package loop would leave it unset on a run where nothing about
  package versions actually changed.

- **Unchecking the property leaves an existing `true` value in place; it is never written back to
  `false`.** `Directory.Packages.props` is `UserControlledWeave` and is seeded from the file already
  on disk, so a hand-written `CentralPackageFloatingVersionsEnabled` can already be sitting there
  when this module runs. Writing `false` on an unchecked box would silently revert that on the very
  next Software Factory run after a developer upgrades to a version of this module carrying the
  property — so `ApplyFloatingVersionsProperty()` is only ever called when the box is checked, and
  it only ever sets the value to `"true"`.

- **An existing `launchSettings.json` is merged as a JSON tree (`JObject`), never round-tripped
  through the typed `LaunchSettings` classes** (see `Templates/LaunchSettings/LaunchSettingsMerger.cs`).
  The typed round-trip dropped unknown members (`$schema`), reordered properties into C# declaration
  order, normalised `commandName` casing and rewrote formatting and the trailing newline, so every
  run produced a diff. `CommandNameConverter` also throws on an unrecognised `commandName`, which
  crashed the Software Factory. Rejected: adding `[JsonExtensionData]` to the typed classes (keeps
  unknown members but still reorders properties and normalises enum values); patching the raw text
  (fragile).

- **Merging is add-only.** A registered profile is appended only when no profile of that name
  exists. A same-named profile is left untouched even if it lacks fields we would generate, because
  re-adding fields a developer deliberately removed is worse than an incomplete profile. Environment
  variables are only added, never overwritten. The HTTP URL for `LaunchProfileHttpPortRequired` is
  only appended to an `applicationUrl` that has an `https://` entry and no `http://` entry.

- **`launchUrl` on `*.Api` profiles is only kept in sync when it points at an Intent-managed default
  path** (`swagger` or `scalar/v1`, held in `IntentManagedLaunchUrlPaths`). Swapping Swashbuckle for
  Scalar therefore still updates it, but a custom `launchUrl` is never touched. Rejected: never
  updating an existing `launchUrl` (leaves `swagger` behind after switching to Scalar); always
  rewriting it, which was the previous behaviour and overwrote custom URLs (and threw on a null or
  relative value).

- **A new file still goes through the typed model**: defaults → `JObject` → the same add-only rules →
  `ToObject<LaunchSettings>()` → `ToJson()`. One set of rules serves both paths, while ending on the
  typed serializer keeps a new file byte-identical to what was generated before (e.g.
  `environmentVariables` stays ahead of `workingDirectory`). Existing `Tests/**/launchSettings.json`
  outputs therefore don't churn.

- **Sync steps read what is on disk first, and write only what the model can represent and has
  actually changed.** Driven by the dotnet/eShop brownfield adoption, where each defect was a sync step
  overwriting something the model can't express or didn't change. The model stays authoritative for
  everything it *can* represent: what's on disk only shapes details the model has no setting for
  (indentation, a builder line), and never overrides a modelled setting. Applied in:
  - `SlnxMerger.NormalizePath`: `./src/X/X.csproj` and `src/X/X.csproj` are the same entry.
    `CSharpProjectNETModel.GetRelativeLocation()` keeps its `.` seed, because it also feeds
    `ProjectConfig`'s output path, and the classic `.sln` template emits `.\` deliberately. Only the
    `.slnx` side normalises. An existing `./`-prefixed entry is matched and kept as written, not
    rewritten, since rewriting it would be exactly the churn being removed.

## Invariants & Constraints

- **This flag makes floating versions legal to NuGet; it does not change how this module resolves
  versions.** `SdkSchemeProcessor` (see `FactoryExtensions/NuGet/SchemeProcessors/SdkSchemeProcessor.cs`)
  still rewrites a `PackageVersion` entry to a pinned version whenever an installed module requests
  that same package, because `VersionInfo.Version` reads a floating range's `MinVersion` and compares
  it against the requested version like any other pin. A floating entry only survives untouched when
  no installed module requests that package. The one way to also protect a module-requested package's
  float is the *Dependency Version Overwrite Behavior* module setting (from `Intent.ModuleBuilder`)
  set to `Never` — its `Never` branch only writes a version when the `PackageVersion` entry is
  entirely absent.

- **`ApplyFloatingVersionsProperty()` must stay idempotent.** It looks for an existing
  `CentralPackageFloatingVersionsEnabled` property (case-insensitive) before adding one, and re-sets
  its value rather than appending a duplicate `<PropertyGroup>` element. A second Software Factory
  run against the same file must not accumulate elements.

- **Stereotype property definitions have no ordering API.** `IStereotypePropertyDefinitionApi` (see
  `core.context.types.d.ts`) exposes no `setOrder`/`moveTo` — a new property added via
  `def.addProperty(...)` is always appended after every existing property. When adding
  `Central Package Floating Versions Enabled`, it therefore landed after `Solution File Format`
  rather than immediately next to `Manage Package Versions Centrally` as originally intended; there
  was no way to reorder it without deleting and recreating the surrounding properties (which would
  change their ids and risk existing applications' persisted values). Accepted as a cosmetic
  deviation — the two CPM properties are still adjacent in the stereotype's own description text and
  in `docs/README.md`, just not in designer property-pane order.

- **When there is nothing to add, `LaunchSettingsMerger.Merge` must return the original string.**
  It compares the merged tree with the original (`JToken.DeepEquals`) and only re-serializes on a
  real change. The "no diff" guarantee depends on this; never re-serialize unconditionally. When a
  change does need writing, the indent character and width, newline style, and leading/trailing
  whitespace are taken from the existing file. JSON comments are lost on a write, since `JObject`
  can't hold them inside objects.

- **Every read of an existing `launchSettings.json` goes through `LaunchSettingsMerger.Parse`, never
  `LaunchSettings.FromJson`.** That includes `BeforeTemplateExecution()`, which reads the
  `applicationUrl` and `iisExpress` ports. `Parse` turns off date parsing and parses floats as
  decimals, so date-like strings and values such as `1.50` survive a rewrite exactly as written.

- **`GetDefaultLaunchSettings()` is only called when no file exists.** It is passed to the merger as
  a `Func`, because `ReplacePorts()` mutates the registered `Profile` instances.

## Module Interactions

- **`Intent.ModuleBuilder`** — its *Dependency Version Overwrite Behavior* module setting interacts
  with the limitation above: setting it to `Never` is the only way to also preserve a
  module-requested package's float, since this module's own NuGet resolution otherwise always wins
  when a module requests the same package.

- **`Intent.AspNetCore.Swashbuckle` / `Intent.AspNetCore.Scalar`** — publish
  `DefaultLaunchUrlPathRequest` with `swagger` / `scalar/v1`. A module that publishes a new default
  path must also have it added to `LaunchSettingsMerger.IntentManagedLaunchUrlPaths`, otherwise a
  later swap away from it won't update existing `launchUrl` values.

- **`Intent.AspNetCore` (when *Enable HTTPS Redirect* is off) and `Intent.Dapr.AspNetCore`** publish
  `LaunchProfileHttpPortRequired`. **`Intent.AspNetCore.Docker`, `Intent.Aws.Lambda.Functions` and
  `Intent.Blazor`** publish `LaunchProfileRegistrationRequest`. Their profiles are appended to an
  existing file only when it lacks a profile with that name.
