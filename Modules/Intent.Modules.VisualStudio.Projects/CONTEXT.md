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
  - `SdkAttributeSync`: an existing `Sdk` whose name isn't one of the `.NET Settings` SDK options
    (`Uno.Sdk`, `Microsoft.Build.NoTargets/3.7.0`) is never rewritten. A representable one always
    follows the model, so `MSTest.Sdk` modelled as `Microsoft.NET.Sdk` *is* rewritten. Rejected: a
    free-text or `(unspecified)` SDK option (the user chose an explicit `MSTest.Sdk` option instead).
  - `TargetFrameworksSync`: see the invariant below. Rejected for now: modelling platform TFMs.
  - `AppSettingsTemplate.UpdateExistingFile`: same no-op/format-preserving contract as
    `LaunchSettingsMerger`, via the shared `JsonFileFormatting`.
  - `SlnxMerger.NormalizePath`: `./src/X/X.csproj` and `src/X/X.csproj` are the same entry.
    `CSharpProjectNETModel.GetRelativeLocation()` keeps its `.` seed, because it also feeds
    `ProjectConfig`'s output path, and the classic `.sln` template emits `.\` deliberately. Only the
    `.slnx` side normalises. An existing `./`-prefixed entry is matched and kept as written, not
    rewritten, since rewriting it would be exactly the churn being removed.

- **`Generate AppSettings File` / `Generate LaunchSettings File` registers a host project's template
  in one of two modes rather than not at all.** For Web, Worker and Blazor WASM projects, flag off
  means *hands-off*: the template is still registered, `CanRunTemplate()` is true only when the file
  exists, and `RunTemplate()` returns it unchanged. A registered template whose output equals the
  file on disk is what stops the Software Factory both rewriting it and proposing to delete it.
  Not registering at all would leave a previously generated file looking orphaned. Hands-off also keeps
  claiming `AppSettingRegistrationRequest`s (`MarkHandled`) and drops them, with one SF info line, so
  they can't fall through to another project's file. Hands-off covers the file's `.csproj` items
  too: the template sets the module-internal `FileAddedData.LeaveProjectItemsUnchangedKey` file
  metadata and the project sync then neither adds nor removes the file's items. Rejected: just
  omitting the item-type metadata in hands-off mode. The sync reads "no metadata" as "remove the
  item", so a Worker switched from managed to hands-off lost its `<Content Include>` and stopped
  copying `appsettings.json`. Keeping the managed metadata instead would add `Content` items to a
  brownfield Worker. A plain `Microsoft.NET.Sdk` project keeps the
  old rule (registered only when on), because a hands-off template there would swallow settings that
  class libraries rely on flowing to their host. The legacy `ASP.NET Core Web Application` element
  stays unconditional.

- **Existing host projects keep their behaviour through migrations; after that the flags are the
  user's.** `MigrationHelper.ApplyGenerateSettingsDefaults` sets both flags to true on every Web,
  Worker and Blazor WASM project, from the `4.1.11-pre.0` version migration (existing apps) and
  `OnInstallMigration` (apps installing the module, e.g. seeded from an architecture template).
  Nothing infers defaults in the designer afterwards: a host project added later starts with the
  stored default (off), and the user ticks the flags. Rejected: a `.NET Settings` On Changed script
  with an element-metadata marker to default the flags whenever the SDK became a host SDK. It was
  removed as an unconventional way to manage a setting's value, and it flipped marker-less projects
  (e.g. apps already on 4.1.11-pre.0) back to on at their first `.NET Settings` edit.

- **The Worker `Program.cs` adapts to an existing file's builder line and run call only.**
  `ExistingProgramShape` reads the file (through `ExecutionContext.FileSystem`, because
  `TryGetExistingFileContent` needs `FileMetadata`, which isn't configured until after the
  constructor). When it finds exactly one `builder` declaration from `WebApplication.CreateBuilder` or
  `Host.CreateApplicationBuilder`, one `var app = builder.Build();` and one `app.Run();` /
  `await app.RunAsync();`, the template re-emits those verbatim, so Merge mode adds nothing that
  duplicates them. `RunAsync` makes a generated `Main` `async Task`. Any other shape is
  `Unrecognised`: the file is generated `OnceOff` (left untouched) with a warning, because merging
  into an unknown shape is how duplicate `Run` calls happen. Statement style (top-level vs `Main`)
  is *never* detected: it stays with the modelled `Use top-level statements`, and brownfield setup
  sets that to match the file. Rejected: detecting it from the file, because on-disk detection must
  never override a modelled setting.

- **A top-level `Program.cs` only gets `using <project namespace>;` when a generated type in the same
  project is declared in that namespace or under it.** The using stands in for the enclosing
  namespace a `Main` method's class would have. A brownfield project's code can live under a
  different root than its name (eShop's `OrderProcessor` project uses `eShop.OrderProcessor.*`), and
  an unconditional `using OrderProcessor;` failed to compile with CS0246. The check runs in an
  `OnBuild` callback, once every template is registered. Hand-written types aren't visible to it,
  which only drops a using that no generated statement needs. Rejected: never adding it, which could
  break greenfield top-level Workers whose generated statements reference root-namespace types by
  name. `Intent.AspNetCore`'s `ProgramTemplate` still adds the using unconditionally.

- **The Worker template is in Merge mode (`IntentManagedMerge()`) by default.** Previously generated
  files get a one-line `Mode.Fully` → `Mode.Merge` change. Accepted consequences: uninstalling a module
  removes its generated *statements* (RoslynWeaver 4.7.0+) but can leave its `using` lines; Serilog's
  try/catch wrapper is added to a brownfield file on its first merge.

- **The Worker's `Logging:EventLog:*` settings go through `AppSettingsTemplate.AddSetting`, not
  `ApplyAppSetting`.** An `AppSettingRegistrationRequest` without a role is claimed by *every* host
  project's appsettings template, so those settings leaked into all of them. `AddSetting` targets the
  Worker project's own base (no runtime environment) instance, found with `FindTemplateInstances`
  from the `AddClass` callback, by which point every template is registered.

## Invariants & Constraints

- **`TargetFrameworksSync` only rewrites a single, unconditional, plain-moniker value whose set
  differs from the model.** Multiple `TargetFramework(s)` elements, a `Condition` on the element or
  its `PropertyGroup`, or a `$(` reference: untouched. Equal as a set, raw or after reducing
  `net10.0-android` to `net10.0`: untouched, so order (the first TFM is the IDE's default target) and
  platform suffixes survive. Platform-suffixed values with a different base set: untouched, with a
  warning, because rewriting them would mean guessing per-platform version suffixes
  (`windows10.0.19041.0`). So changing a MAUI project's framework in the model is a manual `.csproj`
  edit.

- **`AppSettingsTemplate` consults `.editorconfig` only for a brand-new file.** An existing file's
  indentation and newlines are detected from the file itself. Only the `[*]` section and sections
  whose glob matches the file apply; previously every `indent_size` in the file applied and the last
  one won, so eShop's `[*.{cs,...}] indent_size = 4` re-indented its JSON.

- **The project sync never leaves a file's `Remove` item behind when it drops that file's
  re-include.** `CoreProjectSyncProcessor` removes a file's item when its template requests no item
  settings. A same-type `Remove` + `Include` pair is the shape written for `RemoveItemType`, so both
  go together (and an emptied `ItemGroup` with them), restoring the SDK's default globbing.
  Previously only the `Include` went, which excluded the file entirely. A `Remove` with no matching
  re-include is a deliberate exclusion by the developer and is kept.

- **`HasLaunchSettings` now requires `Generate LaunchSettings File` for every SDK.** The
  `3.9.2-pre.0` migration's call to `ApplyLaunchSettingsStereotype` therefore skips host projects
  for an app upgrading from before it, since the flags aren't set yet at that point. The
  `4.1.11-pre.0` migration repeats the call after setting them. Note that `ApplyLaunchSettingsStereotype`
  only saves the package when `setValue` is true.

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
