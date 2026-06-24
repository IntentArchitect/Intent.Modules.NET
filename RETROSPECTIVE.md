# Module Build Retrospective

> Append-only. Add new entries at the bottom. Never reformat existing entries.

---

## 2026-06-23 | Intent.Eventing.NServiceBus — v1.0.0-pre.3 NHibernate / EnableInstallers

### Intent Gaps

- `install_or_update_modules` fails consistently with `[object Object]` SignalR errors even when both solutions are confirmed open by `get_status`. Two consecutive retries both fail. → IA team should investigate SignalR instability for the module install path; the MCP should surface the inner error rather than `[object Object]`.
- `NuGet Package` designer only supports a fixed set of `Minimum Target Framework` values. `Any,Version=v0.0` generates `( >= 0, >= 0)` in the C# switch expression — not `( >= 2, >= 0)` as a developer might hand-edit. The mapping is undocumented in the designer UI. → Document the framework-to-generated-code mapping in the Module Builder designer schema or tooltip so the developer knows what each option generates.

### Process Gaps

- **`lookupById` not `getElementById`:** The designer script API exposes `lookupById(id)` to find an element by GUID. The first attempt used `getElementById` (from DOM familiarity) which threw `ReferenceError`. → Add a "Known wrong name" note in `intent-architect-mcp` skill under Known Gotchas: "Use `lookupById(id)`, NOT `getElementById(id)` — the latter is not defined in the designer script context."
- **`install_or_update_modules` MCP fallback is unclear:** When the tool fails twice, the skill says to manually edit `modules.config`. But the fallback was rejected by the developer mid-session (PowerShell read was denied). The correct fallback should be asking the developer to run the module update from the IA UI directly. → Update `intent-architect-mcp` skill gotcha for `install_or_update_modules`: add "If the SignalR error persists after two attempts and a solution re-open, STOP and ask the developer to update the module version in the Intent Architect UI (Modules panel). Do NOT attempt a `modules.config` hand-edit."
- **CONTEXT.md stated no NHibernate test apps exist, but they were built.** Two apps were created: `N_ServiceBus.Persistence.NHibernate.Publish` and `N_ServiceBus.Persistence.NHibernate.Subscribe`. → CONTEXT.md acceptance matrix updated in this session — ensure future sessions check CONTEXT.md currency before stating "no test app exists."
- **`NugetPackages.cs` hand-edits are silently overwritten.** The developer had to correct hand-edits made in a previous session because they differed from what the designer generates. → Reinforce in `module-increment-loop` that any NuGet package version edit MUST be made via the Module Builder designer, not by hand-editing `NugetPackages.cs`, since the file is `[DefaultIntentManaged(Mode.Fully)]`.

### PRD / User Gaps

- **NuGet version data in training knowledge is unreliable.** The agent incorrectly assumed `NServiceBus.NHibernate 11.1.0` did not exist (old training data said only `10.1.2` existed for NSB 9.x). The developer had to correct this. → Add to `tech-pattern-researcher` skill: "Always verify NuGet package versions against the live NuGet API (`https://api.nuget.org/v3/registration5-gz-semver2/<packageid>/index.json` or the catalog page). Never rely on training-data version knowledge — NuGet packages publish frequently and training cutoffs mean versions are outdated."

---

## 2026-06-23 | Intent.Eventing.NServiceBus — NHibernate runtime verification (end-to-end)

### Intent Gaps

- The "persistent" `install_or_update_modules`/SF failures from the earlier entry were actually **transient** — caused by other concurrent IA processes. Waiting ~1 minute and retrying the same call succeeds. → IA team: the MCP should surface a "busy/contended, retry" signal instead of opaque errors so agents retry rather than escalate to version/install changes.
- Upgrading an already-installed module is **destructive to application settings**: `installApplicationSettings: false` dropped the entire module settings group (later `GetGroup(...)` returned null → template `NullReferenceException`), and `installApplicationSettings: true` reset all setting values to their defaults. → IA team: module version upgrades should preserve existing application setting values rather than dropping or resetting them.
- NHibernate persistence requires the target SQL **database to pre-exist** — `EnableInstallers()` creates the NHibernate tables but not the database itself, so a first run fails with `Cannot open database "NServiceBus"`. → Consider documenting this prominently or auto-creating the database; it is a silent first-run blocker.

### Process Gaps

- A transient SF glitch produced a config missing the (correctly-modelled) `RegisterHandlers` block; this was misdiagnosed as a stale-DLL / model bug and triggered a long wrong-path investigation, including changing module versions and reinstalling. → Add to `module-increment-loop`: "If SF output is missing content you expect from the model, re-run SF once (failures are usually transient from concurrent IA work) before investigating. Never change module version numbers or reinstall different versions to 'fix' generated output — the install state is usually already correct."
- Runtime verification caught a real module bug that compile + code-review did not: `Microsoft.Data.SqlClient` was never declared for the NHibernate path even though the generated config hardcodes `MicrosoftDataSqlClientDriver` (loaded reflectively, fails only at startup). → Reinforce in `reference-app-builder`: "Each persistence/driver path must be exercised at runtime, not just compiled — reflectively-loaded providers (NHibernate drivers, etc.) fail at startup with no compile-time signal."
