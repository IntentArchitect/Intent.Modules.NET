### Version 1.0.3

- Fixed: Now that `Intent.Application.Wolverine` generates its own message bus flush middleware, selecting NServiceBus's transactional outbox in a Wolverine-dispatched application would have double-flushed events (once via the middleware, once via the `DbContext.SaveChanges` splice). `NServiceBusMessageBusInteropExtension` now strips the Wolverine flush middleware when the outbox is enabled, matching the existing behaviour for MediatR and ServiceContract dispatch.

### Version 1.0.2

- Fixed: The generated `using NServiceBus;` was nondeterministically stripped or kept depending on whether `obj`/`bin` existed. NServiceBus now registers itself as an implicit using directly, so generation is consistent regardless of build state.


### Version 1.0.1

- Improvement: Shortened and clarified the Module Settings hints for `Recoverability Policy`, `Persistence`, `Enable Outbox`, `Enable Audit Queue`, and `Enable Instance Identification`.
- Improvement: Updated NuGet package versions.


### Version 1.0.0

- New Feature: License Path support — optional `NServiceBus:LicensePath` appsetting; calls `endpointConfiguration.LicensePath(path)` when present, allowing NServiceBus license files to be specified per environment without code changes.
- New Feature: NHibernate Persistence — new `NHibernate` option on the `Persistence` setting (previously `OutboxPattern`) using `NServiceBus.NHibernate`. Enables saga, subscription, and outbox storage via NHibernate.
- Improvement: `OutboxPattern` setting renamed to `Persistence` with a dedicated `Enable Outbox` checkbox. This separates persistence provider selection from outbox enablement, allowing NHibernate persistence without the outbox.
- New Feature: Audit Queue support — `Enable Audit Queue` setting; when enabled, generates `AuditProcessedMessagesTo` configuration driven by `NServiceBus:AuditQueue` (required) and `NServiceBus:AuditTimeToBeReceived` (optional TimeSpan).
- New Feature: Instance Identification support — `Enable Instance Identification` setting; when enabled, generates `UniquelyIdentifyRunningInstance().UsingNames(instanceId, endpointName)` for Particular Service Platform monitoring, driven by `NServiceBus:InstanceId`.
- New Feature: Added SQL Server transport option using `NServiceBus.Transport.SqlServer`, with auto-generated `ConnectionStrings:NServiceBus` appsettings entry and queue table creation via `EnableInstallers()`.
- New Feature: NServiceBus integration module supporting transport configuration, message handler generation, recoverability policies, SQL Persistence transactional outbox, and multi-broker co-existence.
- Fixed: NHibernate transactional session open options type renamed from `NHibernateSynchronizedStorageSessionOpenSessionOptions` to `NHibernateOpenSessionOptions` (breaking rename in `NServiceBus.NHibernate.TransactionalSession` v11.x).
- Fixed: Missing `using NServiceBus.Persistence;` in the generated `NServiceBusConfiguration` when NHibernate persistence is selected, causing `UseConfiguration` extension method not to be found.
- Fixed: NHibernate NuGet package versions corrected to `NServiceBus.NHibernate 11.1.0` and `NServiceBus.NHibernate.TransactionalSession 11.1.0` (targeting NServiceBus 10.x on net10.0).
- Fixed: NHibernate persistence now declares the `Microsoft.Data.SqlClient` NuGet dependency. The generated configuration hardcodes `NHibernate.Driver.MicrosoftDataSqlClientDriver`, which is loaded reflectively and was not brought in transitively by `NServiceBus.NHibernate`, causing a runtime `Could not create the driver from NHibernate.Driver.MicrosoftDataSqlClientDriver` startup failure.
