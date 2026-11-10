# Bumping the modules' NuGet packages

Every module in this repository that generates .NET code installs NuGet packages into the applications that use it. Those packages are modelled in each module's Module Builder designer as `NuGet Package` elements, with one `Package Version` row per target framework, and the Software Factory turns them into the module's `NugetPackages.cs`. Periodically we move them all to their latest versions, so applications generated today start on current, supported packages.

The bump is run by a coding agent, through the [`module-nuget-bump`](../.claude/skills/module-nuget-bump/SKILL.md) skill. A script does the mechanical part: reading every module's packages and asking nuget.org for the latest version per target framework. The agent does the part that needs judgement:

- it reads why a package is locked and checks whether that still holds;
- it reads the release notes of every minor and major update, and checks the module's templates against them;
- it picks each module's version component from what it found;
- it writes release notes that say exactly which packages moved, and warns consumers when their own code may break.

Two decisions always come back to you: changing a lock, and what to do when an update would break the code a template generates.

This replaces the `Intent.NuGetReferenceUpdater` tool. That tool skipped locks without reading them, read no release notes, gave every module a patch bump, and wrote the same release note for every module.

## Before you start

- A clean branch off an up-to-date `development`.
- Intent Architect running, with this repository's solutions open and the agent connected to it (MCP).
- The GitHub CLI (`gh`) authenticated, for reading release notes.
- `INTENT_PACKAGER_USERNAME` and `INTENT_PACKAGER_PASSWORD` set, as for `apply-template-testing.ps1` and `run-pre-commit-checks.ps1`.
- PowerShell 7 (`pwsh`).

## Running it

Ask the agent to bump the NuGet packages, or type `/module-nuget-bump`. To limit the run to some modules, pass a filter on the module folder or id, for example `/module-nuget-bump Intent.EntityFrameworkCore*`. To carry on from an earlier run, pass `--resume`.

The agent keeps its working state in `.nuget-bump/` at the repository root, which git ignores:

| File              | What it is                                                                                                                                                                            |
| ----------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| `report.md`       | The run's report: progress, the inventory, the lock review, the breaking-change review, the version each module took, and what was deferred. It becomes the pull request description. |
| `candidates.json` | The inventory in full, as the script produced it                                                                                                                                      |
| `cache/`          | nuget.org lookups, reused for 12 hours                                                                                                                                                |

You can run the inventory on its own at any time. It only reads the repository:

```powershell
pwsh .claude/skills/module-nuget-bump/scripts/Get-NuGetBumpCandidates.ps1
pwsh .claude/skills/module-nuget-bump/scripts/Get-NuGetBumpCandidates.ps1 -ModuleFilter 'Intent.EntityFrameworkCore*'
```

## What happens

1. **Preflight.** The agent stops unless the tree is clean, the branch is current with `development`, and the modules have no outstanding Software Factory changes. Otherwise the generated `NugetPackages.cs` files the inventory reads might not match the models.
2. **Inventory.** The script finds every package with a newer version, using the same lookup rules as the old tool: the latest stable version, the highest version for each target framework from .NET Standard 2.0 to .NET 10, and no later framework given a lower version than an earlier one. It works out what a consumer on each framework gets before and after, and lists locked rows, hand-written (`IntentIgnore`) branches, deprecated packages and version drift between modules.
3. **Lock review.** For each locked package with a newer version, the agent reads the lock's reason from the element's comment in the model, researches whether it still holds, and recommends keeping it, keeping it but moving it to the newest patch in its locked line (EF Core 8.0.28 → 8.0.33, say), unlocking it, or an alternative. **It asks you before changing any lock.** A lock with no reason is flagged, so one can be added.
4. **Breaking-change review.** For every minor or major update, and any update of a 0.x package, the agent reads the release notes or migration guide, and searches the owning module's templates for the APIs each breaking change affects. It records a verdict for each package. **If a template would generate code that no longer compiles, it stops for that package and asks you** whether to fix the template or lock the package. Other packages carry on.
5. **Model.** The agent updates each module's `Package Version` rows and their dependencies in the Module Builder designer.
6. **Version.** Each changed module moves version according to the matrix below.
7. **Release notes.** Each changed module gets an entry listing every package that moved.
8. **Regenerate and build.** The Software Factory regenerates each module, and the agent checks the diff against what it intended, then builds the module.
9. **Tests.** The agent updates the Tests solution to the new module versions, applies the changes, and runs `run-pre-commit-checks.ps1`.
10. **Report.** The agent completes `.nuget-bump/report.md`.

## Versioning

| Package update | Release notes say                                                      | Impact found          | Module bump                                                             |
| -------------- | ---------------------------------------------------------------------- | --------------------- | ----------------------------------------------------------------------- |
| patch          | not reviewed                                                           | none                  | patch                                                                   |
| minor or major | no breaking changes                                                    | builds and tests pass | patch                                                                   |
| any            | breaking, but not on APIs generated code or typical consumer code uses | none                  | patch, with the verdict in the report                                   |
| any            | breaking on APIs consumers commonly use                                | generated code fine   | **minor**, with a ⚠️ warning in the release notes                        |
| any            | breaking on APIs the templates emit                                    | generated code breaks | you decide: fix the template (minor), or lock the package with a reason |

A module already on an unpublished prerelease keeps its version; the component only rises if this bump's impact is larger.

## Release notes

Each changed module gets one bullet, with a row per package:

```markdown
### Version 3.11.0

- Improvement: Updated NuGet package versions:
  - `FluentValidation` 11.11.0 → 12.0.0
  - `FluentValidation.DependencyInjectionExtensions` 11.11.0 → 12.0.0

> ⚠️ **BREAKING CHANGES**
>
> This module update may cause compilation breaks if you have written custom code which uses FluentValidation features not supported on v12. Any generated code will be compliant. For details see the [upgrade guide](https://docs.fluentvalidation.net/en/latest/upgrading-to-12.html).
```

The warning appears only when consumers' own code may break. The full format, including how rows read when target frameworks move differently, is in the skill's [release-notes format](../.claude/skills/module-nuget-bump/resources/release-notes-format.md).

## Reviewing the result

Before you commit:

- Read `.nuget-bump/report.md` end to end. Check that every lock decision and every breaking-change verdict is one you agree with, and look at what was deferred.
- Look at the diff. Expect changes to Module Builder metadata, `NugetPackages.cs`, `.imodspec`, `release-notes.md` and some `.csproj` files under `Modules/`, plus the Tests solution's regenerated code and `modules.config` files. Nothing else should change.
- Spot-check a few `NugetPackages.cs` diffs against the report: the versions moved, and the locked rows did not.
- Confirm `run-pre-commit-checks.ps1` passed.

Use `report.md` as the pull request description.

## Troubleshooting

**`NU1605` (package downgrade) when building a module.** See _NuGet Package Downgrade Errors (NU1605)_ in `.claude/rules/known-build-gotchas.instructions.md`.

**The inventory looks wrong or stale.** It reads the generated `NugetPackages.cs` files, so it is only as current as the last Software Factory run. Run `./PipelineScripts/ensure-no-outstanding-sf-changes.ps1 -IslnPath Modules/Intent.Modules.NET.isln`, apply any outstanding changes, and run the inventory again. To ignore the nuget.org cache, pass `-CacheMaxAgeHours 0`.

**The run stopped part-way.** Start it again with `/module-nuget-bump --resume`. The agent picks up from the first unticked step in `report.md`'s progress list. Re-running the inventory rewrites only its own section of the report, so the review notes survive.

**EF Core's per-framework rows.** EF Core's net8.0 and net9.0 rows are locked, because not all the net8.0 providers (Oracle and MySQL) supported EF Core 9. The old process updated these by hand. They now come up in the lock review with their reason, and the agent researches whether the providers have caught up. Note that EF Core 9 targets net8.0, so nuget.org has no net9.0 match for EF Core packages: the net9.0 row appears as "locked for a framework nuget.org no longer resolves", and would be deleted if it were unlocked.

**A package nuget.org cannot resolve.** Usually it has been deprecated and every version unlisted, such as `IdentityModel` in favour of `Duende.IdentityModel`. Replacing a package changes what the templates generate, so the bump reports it rather than doing it. Deal with it as a separate module change.

**Hand-written branches.** Some modules register packages in hand-written `IntentIgnore` code, for example AutoMapper's and MassTransit's pre-commercial versions. The bump lists them and never changes them.

## Scope

The bump covers only the packages a module installs into consumer applications. It does not touch the modules' own `.csproj` references to `Intent.Modules.Common*` or `Intent.SoftwareFactory.SDK`: raising those raises the minimum Intent Architect version a module needs, which is a separate decision.
