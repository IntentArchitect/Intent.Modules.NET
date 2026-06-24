# Working State

## Open SF cycle — NServiceBus NHibernate `Microsoft.Data.SqlClient` fix

**Status:** module-side fix in source (built, compiles), but NOT yet verified through a full
Software Factory cycle. The module stays at **1.0.0-pre.2** — do NOT bump the version for this.

### What was done
- `NServiceBusConfigurationTemplatePartial.cs`: the `IsNhibernate()` block now declares
  `AddNugetDependency(NugetPackages.MicrosoftDataSqlClient(OutputTarget))`. The generated
  NHibernate config hardcodes `MicrosoftDataSqlClientDriver` (loaded reflectively), which was
  not brought in transitively by `NServiceBus.NHibernate` — causing a runtime
  `Could not create the driver from NHibernate.Driver.MicrosoftDataSqlClientDriver` startup crash.
- Module builds clean (0 errors). Release note added under the `Version 1.0.0` section.

### Why the cycle is open
- The NHibernate test apps (`Tests/N_ServiceBus.Persistence.NHibernate.{Publish,Subscribe}`)
  run against a pre.2 module build that predates this fix. To make them run, the
  `Microsoft.Data.SqlClient` package was added **manually** to both Infrastructure `.csproj`
  files. These manual edits are currently load-bearing.
- The fix was therefore never confirmed via SF.

### To close (next session) — no version bump, no reinstall
1. Rebuild the module `.csproj` (version stays pre.2). IA hot-reloads the rebuilt DLL
   automatically — the apps are already on pre.2, so NO `install_or_update_modules` is needed.
   (Reinstalling resets application settings; avoid it.)
2. Run SF on each NHibernate app → confirm the staged diff adds `Microsoft.Data.SqlClient` to
   the Infrastructure `.csproj` automatically. The manual csproj additions then become redundant.
3. Apply, build, and re-run the end-to-end test (Publish → RabbitMQ → Subscribe handler).

### Verified this session (runtime, end-to-end, RabbitMQ + NHibernate + outbox)
- `PUT /api/test-send-command` → 204 → `[HANDLER HIT] Subscribe.TestCommandHandler received TestCommand`, no errors.
- Runtime prerequisite: the `NServiceBus` SQL database must pre-exist (`EnableInstallers()`
  creates tables, not the database).
