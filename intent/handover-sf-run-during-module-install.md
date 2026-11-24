# Handover: a Software Factory run can start while a module install is still running its migrations

**For:** the Intent Architect platform team **From:** the drift engine work (Intent.Common.Drift 2.0), 2026-11-24 **Source checked:** `D:\Dev\Intent`, branch `release/5.4.x` at `48e6002bed`. Paths below are relative to `Intent.IntentArchitect/`.

## Summary

Intent means no Software Factory run to happen during a module install. The Modules tab shuts down the application's Software Factory before installing. It relaunches the Software Factory only once the install, its migrations included, has finished. Nothing enforces that window, though: launching or restarting a run doesn't wait for a module operation to finish.

On a real update (Intent.Common.Drift 1.0.0-pre.2 to 2.0.0-pre.0, from the Modules tab), a run loaded its module list partway through the install, before the module's Version Migration had finished. Two things went wrong:

1. **The run failed.** The migration removes two modules, `Intent.Domain.Guardrails.Contracts` and `Intent.Services.Guardrails.Contracts`, along with their Template Outputs. The run still had both modules loaded, but read the Codebase Structure fresh, so it found no Template Outputs for their templates:

   > MissingOutputTargetException: Template `Intent.Domain.Guardrails.Contracts.CosmosDbRecipeGuide` (module `Intent.Domain.Guardrails.Contracts`) targets the anchor/role `AI.Context.Skills`, but the Codebase Structure has no output target (Template Output) for it. ... Execution interrupted after 00:00:00.1060016

   The message tells the user to restore the anchor and reinstall a module that the migration had deliberately removed, which is the wrong fix.

2. **The run undid part of the migration.** The migration had removed 223 entries from `.intent/output-log.xml` and from `<application>.application.managed-files.xml`. The run then wrote both files back from the copy it loaded before the migration, and the entries returned: `managed-files.xml` is byte-for-byte the committed version again.

The second problem applies to any migration, not just this module's. A run that overlaps a migration reads metadata that is half migrated, and can overwrite files the migration changed.

## Timeline (file write times, 2026-11-24, +02:00)

| Time              | Written                                                                                  | By                                                                                         |
| ----------------- | ---------------------------------------------------------------------------------------- | ------------------------------------------------------------------------------------------ |
| 17:43:34          | 30 Services metadata files                                                               | The installer's save before migrations (`ModuleInstallationManager.cs:470-493`)            |
| 17:44:06.9 - 07.8 | 223 `.cs` files (the Intent attributes removed)                                          | The Version Migration                                                                      |
| 17:44:08.2        | `modules.config`, without the two Guardrails modules                                     | The Version Migration                                                                      |
| 17:44:08 - 09     | Codebase Structure (their Template Outputs removed) and about 190 other metadata files   | The Version Migration                                                                      |
| 17:44:10.5        | `<application>.application.config`                                                       | Most likely the installer's save after migrations (`ModuleInstallationManager.cs:496-504`) |
| 17:44:10.78       | `.intent/output-log.xml`, with all 223 contract entries back                             | A Software Factory run                                                                     |
| 17:44:10.87       | `managed-files.xml`, with all 233 contract entries back (the same as the committed file) | A Software Factory run                                                                     |
| 17:44:10.88       | `.intent/output.cache`                                                                   | A Software Factory run                                                                     |
| 17:44:37          | `.intent/software-factory.lock`                                                          | A new Software Factory host taking the lock                                                |

**How we know when the run loaded its modules.** Every run replaces `output.cache`'s `installedModules` with the modules it loaded: `ApplicationOutputCachePersistableExtensions.Update` calls `InstalledModulesById.Clear()` and then adds `_application.InstalledModules` (`IntentArchitect.SoftwareFactory/Intent.SoftwareFactory/Helpers/ApplicationOutputCachePersistableExtensions.cs:29-43`, called from `SoftwareFactoryProcessOrchestrator.cs:389`). The file written at 17:44:10.88 lists three modules together:

- `Intent.Common.DriftEngine`, which the install added;
- both Guardrails modules, which the migration removed at 17:44:08.2.

`modules.config` held all three only between the install adding DriftEngine and the migration removing the Guardrails modules. So the run read its module list during the install, after Intent had shut the Software Factory down for it.

No Intent log on this machine covers the run (`%APPDATA%\Intent Architect\logs` is from August), so what launched it isn't known. See "Still to establish" below.

## Why the run failed

A Software Factory host builds its module list once and keeps it for every hot restart. The model and the Codebase Structure, however, are read fresh on each run.

- `SoftwareFactoryExecutionEnvironment.CreateSoftwareFactoryTask` calls `InitializeSoftwareFactory` only when `_manifestLoader == null`. Otherwise it does a "Hot Restarting Execution" with the same `_manifestLoader` (`IntentArchitect.SoftwareFactory/Intent.SoftwareFactory/SoftwareFactoryExecutionEnvironment.cs:159-183`).
- `ModuleConfigManager` is created inside `InitializeSoftwareFactory` (`:233`), and every later configuration reuses it (`:259`). It caches one `ApplicationModuleConfigManager` per application (`Configuration/ModuleConfigManager.cs:20-26`), which reads `modules.config` when it is created.
- `ApplicationManager.NoOutputTarget` throws `MissingOutputTargetException` when an installed module's template has no Template Output (`Engine/Runtime/ApplicationManager.cs:273-287`).

A host started during the install therefore kept the two removed modules for as long as it lived, including restarts after the install finished. The "Execution interrupted after 0.1 s" fits a hot restart, which reuses the loaded modules and fails while registering templates.

## Why nothing stopped it

- **Intent's design.**
  - `ModuleInstallationService` notes which applications have a Software Factory running and shuts every task for them down (`IntentArchitect.ElectronClient/Intent.IArchitect.Electron/wwwroot/App/Solution/Application/ModulesManager/ModuleInstallationService.ts:181-184`).
  - It then installs (`:207`), and on releasing the module lock relaunches those Software Factories (`ModuleOperationLock.ts:36-46`).
  - The agent also shuts down idle hosts for the applications (`Intent.IArchitect.Agent/Implementation/LocalPackageManagerServiceHandlers/InstallOrUpdateModulesSCH.cs:86-91`).
- **The gap.** That shutdown happens once, when the install starts. Neither lock covers Software Factory runs:
  - `ModuleOperationGate` is taken only by `InstallOrUpdateModulesSCH.cs:55` and `UninstallModuleSCH.cs:41`;
  - the renderer's `ModuleOperationLock` only orders module operations among themselves.

  A `LaunchSoftwareFactoryRequest` or `RestartAllHostedTasksForApplicationRequest` during the install starts a run regardless. The second one comes from, among others, saving a designer tab (`AgentsShell/AgentsShellViewModel.ts:1485`, `Solution/ShellViewModel.ts:5607`), the AI tools' run (`ShellViewModel.ts:4904`) and the application settings save (`Application/Configuration/ApplicationConfigurationViewModel.ts:72`).

## Suggested fix

1. **A Software Factory launch or restart for an application waits while a module operation for that application holds the lock.** The relaunch when the lock is released already runs the Software Factory once the install is done, so a request made during the install can be queued, or dropped in favour of that relaunch. With this in place, no run sees half-migrated metadata, and no run overwrites a migration's changes with its in-memory copy.
2. **Consider:** a hot restart whose application's `modules.config` no longer matches the host's module list cold-starts instead (the check would go in `CreateSoftwareFactoryTask`). That is a separate case from this bug: it covers a module task that changes `modules.config` outside an install. After a module task, Intent doesn't reload the application (Intent.Common.Drift's Migrate to Drift Engine task works around that by telling the developer to reopen the solution). It isn't needed for migrations if (1) is in place.

## Still to establish

- **What launched the run during the install.** To reproduce:
  - write a test module whose Version Migration sleeps for 30 seconds and then removes a module entry from `modules.config`;
  - update it from the Modules tab with the application's Software Factory open;
  - try each trigger during the sleep: saving a designer tab, the AI `run_software_factory` tool, and leaving it alone.

  The renderer's `[module-lock]` console lines and the agent's "SF hot restart triggered" lines (`Intent.IArchitect.Agent/SoftwareFactoryProcessManager.cs:552`) show which trigger starts the run.
- **Whether the run reported drift.** The user also saw drift errors from this update. Whether they came from the run with the old modules is unknown, because its report was staged and not applied.

## What the module side does meanwhile

Nothing in the migration can stop a run from overlapping it. The migration's own changes to the model, `modules.config` and the contract files were all correct on disk afterwards. For the records the run wrote back, Intent.Common.Drift already has a recovery path built for teammates' machines:

- **Stand-ins.** For each contract file the logs still list, it registers a stand-in template, so the next run proposes no delete.
- **Pruner.** It removes the contract entries from the records again at After Template Execution.

This recovery hasn't yet been checked on the affected application.
