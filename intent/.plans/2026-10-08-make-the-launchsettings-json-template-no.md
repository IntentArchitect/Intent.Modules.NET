# Make the launchSettings.json template non-destructive (Intent.VisualStudio.Projects)

## Context

When `Properties/launchSettings.json` already exists, the template (`LaunchSettingsJsonTemplate`) rewrites the whole file, which creates noise:

- it drops `$schema`;
- it moves `dotnetRunMessages` after `applicationUrl`;
- it strips the trailing newline.

Root cause, in `TransformText()` (`Templates/LaunchSettings/LaunchSettingsJsonTemplate.cs:280-371`):

1. It deserializes the existing file into quicktype-generated classes (`LaunchSettings.cs`).
2. It adds what's missing.
3. It re-serializes with Newtonsoft.

That round-trip has these effects:

- unknown members are lost (`$schema`, any field the classes don't model);
- properties are reordered into C# declaration order;
- `commandName` casing is normalised;
- formatting and the trailing newline are rewritten.

On top of that, two rules override existing values rather than only filling gaps:

- an `http://` URL is appended to an HTTPS-only `applicationUrl` when `LaunchProfileHttpPortRequired` is raised;
- `launchUrl` is rewritten on `*.Api` profiles to match the Swashbuckle/Scalar path.

The second rule also throws when `launchUrl` is null or relative. `launchSettings.Profiles.Values` throws when the file has no `profiles`.

**Intended outcome:** the template only adds what's missing.

- If nothing is missing, the existing file is returned byte-for-byte, with no diff.
- When something is added, everything else in the file is left as it was: keys, order, unknown fields, casing, indentation, line endings and the trailing newline.

New files are generated exactly as today, so the ~224 sample `Tests/**/launchSettings.json` outputs don't churn.

## Approach

### 1. Merge on the JSON tree, not typed classes
- Extract the merge logic from `TransformText()` into a new internal, pure, testable class `LaunchSettingsMerger` (same folder). Its input is the existing file text (or null) plus the collected registrations:
  - profiles;
  - environment variables;
  - `httpPortRequired` and the port;
  - the default launch-url path.

  Its output is the file text.
- **Existing file:** parse with Newtonsoft `JObject.Parse` (already referenced, 13.0.3). This keeps property order and unknown members, and doesn't touch `commandName` values. Keep a `DeepClone()` of the original for comparison.
- **New profiles:** still built with the typed `Profile` class, as today, then converted with `JObject.FromObject(profile, JsonSerializer.Create(Converter.Settings))`, so their JSON shape matches today's output. They are appended to `profiles` without reordering existing entries, and only if the key is absent. Same-named existing profiles are untouched (decision B).
- **Environment variables:** for each target profile, create `environmentVariables` if missing, then add each missing key at the end. Existing keys and values are never touched.
- **No changes:** if `JToken.DeepEquals(original, merged)`, return the original content unchanged.
- **Changes:** serialize with formatting taken from the existing file:
  - indentation width, from the first indented line (default 2);
  - newline style, `\r\n` vs `\n`;
  - trailing newline, kept only if the original had one.

  Use `JsonTextWriter` with `Formatting.Indented`, `Indentation = n`.
- **No file:** keep today's typed path, `GetDefaultLaunchSettings()` + `ToJson()`, so new files are unchanged.

### 2. Make the "forcing" rules add-only
- **`applicationUrl` with `LaunchProfileHttpPortRequired`:** append `http://localhost:{port}` only when the profile has an `applicationUrl` and none of its entries is `http://`. That still gives today's result for an HTTPS-only URL. Profiles without `applicationUrl` are left alone.
- **`launchUrl` on `*.Api` profiles (decision A):**
  - If missing: leave it. Fresh files already get it from the defaults.
  - If present, update it only when its path is an Intent-managed default (`swagger` or `scalar/v1`, i.e. a previous `DefaultLaunchUrlPathRequest` value) and differs from the current default. Swapping Swashbuckle for Scalar keeps working; a custom `launchUrl` is never changed.
  - Handle relative values (e.g. `"swagger"`) by replacing the path text directly, and absolute values with `UriBuilder` as today, with no null or relative-URI exceptions.
- Guard a missing `profiles` object: create it only if there's something to add.

### 3. Read the existing file without the typed round-trip
`BeforeTemplateExecution()` (lines 188-226) calls `LaunchSettings.FromJson`. That throws on an unknown `commandName` (`CommandNameConverter`, `LaunchSettings.cs:412`) and would crash the run before the merge ever happens. Switch these reads to the same `JObject` parse: first profile `applicationUrl`, `iisSettings.iisExpress.applicationUrl` and `sslPort`. Behaviour is otherwise unchanged, including the Base Url stereotype write.

### Decisions to confirm when you review (no question tool was available this session)
- **A. `launchUrl`:** recommended as above, so only Intent-default paths (`swagger`, `scalar/v1`) are kept in sync and custom values stay. The alternative is to never change an existing `launchUrl`, which would leave `swagger` behind after switching to Scalar.
- **B. Same-named profile already in the file:** recommended to leave it untouched, as today (`TryAdd`). The alternative is to add any registered fields it lacks (e.g. `dotnetRunMessages`). That's more "complete" but re-adds fields a user deliberately removed.

## Files
- `Modules/Intent.Modules.VisualStudio.Projects/Templates/LaunchSettings/LaunchSettingsJsonTemplate.cs`:
  - `TransformText()` delegates to the merger;
  - `BeforeTemplateExecution()` reads via `JObject`.
- `Modules/Intent.Modules.VisualStudio.Projects/Templates/LaunchSettings/LaunchSettingsMerger.cs` (new): the merge, formatting detection and add-only rules.
- `LaunchSettings.cs` is unchanged; it's still used to build new profiles and fresh files.
- `Modules/Intent.Modules.VisualStudio.Projects.Tests/Templates/LaunchSettings/LaunchSettingsMergerTests.cs` (new): xunit + Shouldly, matching the existing test project. `InternalsVisibleTo` is already set in the csproj.
- **Close-out:**
  - `Intent.VisualStudio.Projects.imodspec` version 4.1.10 → **4.1.11**. 4.1.10 was promoted to release in `9ca441af24`; run the `module-version-increment` gate first. Keep the csproj version consistent.
  - `release-notes.md`: one "Fixed" entry under 4.1.11.
  - Module `CONTEXT.md`: record the add-only, order-preserving merge decision and why typed round-tripping was rejected.

## Verification
1. **Unit tests** (`dotnet test` on `Intent.Modules.VisualStudio.Projects.Tests`):
   - Your exact sample file (with `$schema`, `dotnetRunMessages` before `applicationUrl`, and a trailing newline) with nothing to add → output is byte-identical.
   - A registered profile not in the file → appended last, with all other text unchanged in order, `$schema` kept, and the trailing newline kept.
   - Missing env var → added at the end of that profile's `environmentVariables`; an existing different value is kept.
   - CRLF vs LF and 4-space indentation are both preserved after a change.
   - An unknown `commandName` (e.g. `"DockerCompose"`) and unknown properties survive.
   - `httpPortRequired`: an HTTPS-only URL gets `;http://localhost:N`. A URL already holding an `http://` entry is untouched.
   - `launchUrl`:
     - `.../swagger` with a Scalar default → `.../scalar/v1`;
     - a custom `.../health` is unchanged;
     - relative `"swagger"` is handled;
     - null doesn't throw.
   - No existing file → output equals today's `GetDefaultLaunchSettings().ToJson()`.
2. **Build:** `dotnet build --no-incremental` on `Intent.Modules.VisualStudio.Projects` must exit 0.
3. **SF check:** re-run the Software Factory on a test app that already has `launchSettings.json`, e.g. one under `Tests/` that installs from `Modules/Intent.Modules` via `Tests/intent.repositories.config`.
   - Its launchSettings must produce no change.
   - Then hand-add `$schema` and reorder a profile's keys; re-run, and there should still be no change.
   - Then remove a registered profile; re-run, and only that profile should be re-added.
