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

## Module Interactions

- **`Intent.ModuleBuilder`** — its *Dependency Version Overwrite Behavior* module setting interacts
  with the limitation above: setting it to `Never` is the only way to also preserve a
  module-requested package's float, since this module's own NuGet resolution otherwise always wins
  when a module requests the same package.
