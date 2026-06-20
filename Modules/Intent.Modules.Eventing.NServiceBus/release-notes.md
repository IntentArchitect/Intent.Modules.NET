### Version 1.0.0

- New Feature: Added SQL Server transport option using `NServiceBus.Transport.SqlServer`, with auto-generated `ConnectionStrings:NServiceBus` appsettings entry and queue table creation via `EnableInstallers()`.
- New Feature: NServiceBus integration module supporting transport configuration, message handler generation, recoverability policies, SQL Persistence transactional outbox, and multi-broker co-existence.
