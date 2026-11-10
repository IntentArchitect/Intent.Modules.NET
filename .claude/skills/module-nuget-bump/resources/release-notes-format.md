# Release-notes format for a NuGet package bump

`module-docs-chore` owns where and when a release-notes entry is written. This file only fixes the shape of the NuGet bump's entry, so every module's entry reads the same and a consumer can see exactly what moved.

## The entry

One bullet, with one nested row per package:

```markdown
### Version 3.11.0

- Improvement: Updated NuGet package versions:
  - `FluentValidation` 11.11.0 → 12.0.0
  - `FluentValidation.DependencyInjectionExtensions` 11.11.0 → 12.0.0
```

- **The heading** is the module's version without its `-pre.#` suffix, as `module-docs-chore` says.
- **One row per package**, ordered by package id, the id in backticks, then `old → new`. The old version is the one a consumer resolves today, the new one what they resolve after the bump.
- **Frameworks only when they differ.** When every framework moves the same way, the row has no frameworks. When they move differently, list each move with its frameworks, newest framework first, separated by `;`:

  ```markdown
  - `Microsoft.EntityFrameworkCore.SqlServer` 10.0.9 → 10.0.14 (net10.0); 9.0.17 → 9.0.22 (net9.0, net8.0)
  ```

- **A framework that gains its own row** reads `added <version> (<framework>)`.

The `releaseNoteRows` for each module in `.nuget-bump/candidates.json` are already in this shape. Adjust them for the lock and breaking-change decisions before using them:

- a package whose lock was kept unchanged, or which was locked because it broke a template, has no row;
- a locked row that was kept and patched within its line gets a row for that framework, such as `` `Microsoft.EntityFrameworkCore` 10.0.9 → 10.0.14 (net10.0); 8.0.28 → 8.0.33 (net8.0) ``;
- a package that was unlocked gets the row for its new version.

## When the version heading already exists

The module is already on an unpublished version (`module-version-increment` left it in flight), so its heading may already have entries.

- **No NuGet bullet yet**: add this entry's bullet alongside the others.
- **A detailed NuGet bullet from an earlier bump in the same version line**: merge into it. A package already listed keeps its original old version and takes the new one; a new package gets its own row.
- **A bare `- Improvement: Updated NuGet package versions.` bullet** (the old tool's wording): replace it with the detailed bullet. The packages that bullet covered are no longer known, so keep the replacement to what this run moved.

Never leave two NuGet bullets under one heading.

## When consumer code is impacted

When the breaking-change review found breaking changes on APIs consumers commonly use (the versioning matrix's **minor** row), add this block after the bullets, once per heading. Name the package and the major version, and link the vendor's upgrade or breaking-changes guide:

```markdown
> ⚠️ **BREAKING CHANGES**
>
> This module update may cause compilation breaks if you have written custom code which uses FluentValidation features not supported on v12. Any generated code will be compliant. For details see the [upgrade guide](https://docs.fluentvalidation.net/en/latest/upgrading-to-12.html).
```

When several packages in one entry are impacted, name each in the first line and give one link per package:

```markdown
> ⚠️ **BREAKING CHANGES**
>
> This module update may cause compilation breaks if you have written custom code which uses PackageA features not supported on v3, or PackageB features not supported on v5. Any generated code will be compliant. For details see the [PackageA migration guide](https://example.com/package-a/v3) and the [PackageB migration guide](https://example.com/package-b/v5).
```

"Any generated code will be compliant" is a claim the Tests build has proved. Leave it out when that build has not run.

## When a template changed too

When the user chose to fix a template for a package's breaking change, the template change is a separate observable change, so it gets its own bullet after the NuGet bullet. Describe what the generated code now does differently, not the template's internals:

```markdown
- Improvement: Updated NuGet package versions:
  - `PackageA` 2.4.1 → 3.0.0
- Improvement: Generated `PackageA` registrations use the v3 options-based `AddPackageA` overload.
```
